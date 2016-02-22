using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace MiniMPI
{
    /// <summary>
    ///	Provides logic to execute a MiniMPI program.
    /// </summary>
    public class MiniMPIProgram : IDisposable
    {

        #region Static Members

        private static int _lastMpiRuntimeID = -1;

        /// <summary>
        ///	Gets the next id available for a new runtime instance.
        ///	</summary>
        private static int GetNextMpiRuntimeID()
        {
            return Interlocked.Increment(ref _lastMpiRuntimeID);
        }

        /// <summary>
        /// Starts the runtime thread and all process threads and waits for the
        /// runtime to finish.
        /// </summary>
        /// <exception cref="MiniMPIExecutionException">At least one spawned process thread threw an exception.</exception>
        public static void Execute(int processCount, Action<IMiniMPIStringAPI> processWork)
        {
            if (processCount < 1)
                throw new ArgumentOutOfRangeException("processCount", processCount, "Must be greater than zero.");
            if (processWork == null)
                throw new ArgumentNullException("processWork");

            using (var executor = new MiniMPIProgram(processCount))
            using (var runtime = new MiniMPIStringRuntime(processCount))
            {
                executor.Runtime = runtime;
                executor.CreateProcessAPI = (p) => new MiniMPIStringProcessAPI(runtime, p);
                executor.Execute(processWork);
            }
        }

        #endregion

        private readonly int _id;
        /// <summary>
        ///	The id of the thread that created this instance.
        ///	This is the only thread allowed to run some API calls. e.g. <see cref="Execute"/>.
        ///	</summary>
        private readonly int _owningThreadID;

        /// <summary>The runtime being used for the current program.</summary>
        private MiniMPIRuntime Runtime;

        private Func<MpiProcess, IMiniMPICoreAPI> CreateProcessAPI;

        /// <summary>
        ///	An event wait handle that will signal the MpiProcesses to 
        ///	start at the same time.
        /// </summary>
        private ManualResetEvent _startProcessesTogether;

        // Runtime state
        private bool _hasExecuteBeenCalled;
        //private int _stateID;
        private MiniMPIProgramState _state;
        private int _firstErroredProcessRank;

        // Process management
        private int _processesFinishedCount;
        protected MpiProcess[] Processes { get; private set; }

        #region Properties

        /// <summary>The unique ID of this runtime instance.</summary>
        public int ID { get { return _id; } }

        /// <summary>Gets the number of processes managed by this runtime instance.</summary>
        /// <remarks>
        /// This is equivalent to a call to MPI_Comm_size(grpID, value) where
        /// value is what's returned and grpID would be associated with this
        /// instance.
        /// </remarks>
        public readonly int ProcessCount;

        #endregion

        #region Constructors

        /// <summary>Creates a new instance.</summary>
        /// <param name="processCount">The number of process threads to create to execute the MPI program.</param>
        private MiniMPIProgram(int processCount)
        {
            _id = GetNextMpiRuntimeID();
            _owningThreadID = Thread.CurrentThread.ManagedThreadId;

            _startProcessesTogether = new ManualResetEvent(false);

            // Create all the processes
            ProcessCount = processCount;

            _firstErroredProcessRank = -1;
            _state = MiniMPIProgramState.Initialized;
        }

        #endregion

        #region Execute()

        /// <summary>
        /// Starts the runtime thread and all process threads and waits for the
        /// runtime to finish.
        /// </summary>
        /// <exception cref="MiniMPIExecutionException">At least one spawned process thread threw an exception.</exception>
        private void Execute<TIMpiApi>(Action<TIMpiApi> processWork)
            where TIMpiApi : IMiniMPICoreAPI
        {
            // ** Make sure we've only been called once **
            if (_hasExecuteBeenCalled)
                throw new InvalidOperationException("Execute has already been called for this runtime instance.");
            _hasExecuteBeenCalled = true;

            // Spawn and start the processes, but they'll wait until the next line to be
            // allowed to start all at the same time.
            SpawnWorkerProcesses(processWork);
            Runtime.InitializeProgram(this.Processes);

            _state = MiniMPIProgramState.Executing;

            // Signal to the processes that they may start now at the same time.
            //   Note: The reason this is done is so that all processes start at 
            // same time.  This will allow for no favoritism to the lower-ranked
            // processes which are started first.
            _startProcessesTogether.Set();

            // Execute the runtime loop
            ExecuteRuntimeLoop();

            // Make sure that the state of the MiniMPIRuntime is set to Finished,
            // otherwise something went wrong.
            Debug.Assert(_state == MiniMPIProgramState.Finished);

            // Make sure each process's thread has fully completed too
            // TODO: Think about making the Process threads background threads. Then they'll always get aborted once the program exits.
            foreach (var p in Processes)
                p.Thread.Join();

            // Make sure that the number of processes that finished is equal to
            // the number of processes created.
            Debug.Assert(Thread.VolatileRead(ref _processesFinishedCount) == ProcessCount);

            // Detect any errors
            // JAM: Do I need these to be a Thread.VolatileRead (p.Error, _firstErroredProcessRank)?
            //      Or is the Interlocked.ExchangeCompare in the process threads enough?
            int firstErroredProcess = _firstErroredProcessRank;
            if (firstErroredProcess != -1)
            {
                var processErrors = Processes
                    .Select(p => {
                        var ex = p.Error;
                        return ex == null ? null : new MiniMPIProcessException(p.Rank, ex);
                    })
                    .Where(pex => pex != null)
                    .ToArray();
                throw new MiniMPIExecutionException(
                    processErrors.Single(pex=>pex.Rank == firstErroredProcess)
                    , processErrors
                    );
            }
        }

        #endregion

        #region Runtime loop

        /// <summary>
        /// Every time the runtime state changes, this method determines the
        /// change and performs the processing of runtime rules.
        /// </summary>
        /// <remarks>
        ///	This method runs a continuous loop.  Inside of the continuous loop,
        /// it waits for the runtime state to change.  If all of the processes
        /// have finished, then it will set the state to finished and
        /// subsequently will wait until all of the processes join.  
        ///	A subclass of MiniMPIRuntime may define the function OnProcessRuntimeRules to describe
        ///	what this method should do as well each time the runtime state changes.
        /// </remarks>
        private void ExecuteRuntimeLoop()
        {
            do
            {
                // TODO: Use different handles for instructions versus _stateID changes.
                //WaitHandle.WaitAny

                // Wait for the state to change before trying to do anything
                Runtime.BlockTillStateChanged();

                // Determine the current state
                MiniMPIProgramState state = _state;
                if (state != MiniMPIProgramState.Finished)
                {
                    // The only way we can detect when to finish is whether all processes have
                    // finished.
                    if (Thread.VolatileRead(ref _processesFinishedCount) == ProcessCount)
                    {
                        // The most common way the state will get set to Finished is when
                        // all the process threads have finished.
                        _state = state = MiniMPIProgramState.Finished;
                    }
                }

                // Give highest priority to terminating this thread
                if (state == MiniMPIProgramState.Finished)
                    return;

                // First, move any new pending instructions to the ready state
                // Do this just once per iteration
                Runtime.AddInstructionsFromPendingQueue();

                // Only process rules if we are executing
                if (state == MiniMPIProgramState.Executing)
                {
                    // The ProcessRules method should handle performing the collective abort
                    Runtime.ProcessRules();
                }
            } while (true);
        }

        #endregion

        #region Process Management

        /// <summary>Creates the MpiProcesses and starts their threads.</summary>
        /// <typeparam name="TIMpiApi">The type of process API that will be passed to the <paramref name="processWork"/> delegate.</typeparam>
        /// <param name="processWork">The method that will be run by each MpiProcess.</param>
        private void SpawnWorkerProcesses<TIMpiApi>(Action<TIMpiApi> processWork)
            where TIMpiApi : IMiniMPICoreAPI
        {
            //
            ParameterizedThreadStart processThreadStart = obj => {
                MpiProcess p = (MpiProcess)obj;
                var processAPI = (TIMpiApi)CreateProcessAPI(p);
                ProcessWorker(p, processAPI, processWork);
            };

            // Creates the amount of MpiProcesses specified in the constructor
            // and has all of them run the same method.
            String processThreadNameFormatString = String.Format("P{{0}} (RuntimeID={0})", this.ID);
            Processes = (from i in Enumerable.Range(0, ProcessCount)
                         let t = new Thread(new ParameterizedThreadStart(processThreadStart)) {
                             // Setting the name helps when viewing the thread in a
                             // debugger or in Chess' ConcurrencyExplorer
                             Name = String.Format(processThreadNameFormatString, i)
                         }
                         select new MpiProcess(i, t))
                         .ToArray();

            // Start all the processes
            foreach (var p in Processes)
                p.Thread.Start(p);
        }

        protected void ProcessWorker<TIMpiApi>(MpiProcess process, TIMpiApi processAPI, Action<TIMpiApi> processWork)
            where TIMpiApi : IMiniMPICoreAPI
        {
            // Wait for the runtime to signal that all of the processes have been
            // started before I continue.
            _startProcessesTogether.WaitOne();

            process.State = MpiProcessState.Started;
            try
            {
                processWork(processAPI);

                // Detect if an MpiInitialize was matched with an MpiFinalize
                if (process.State == MpiProcessState.MpiInitialized)
                    throw new InvalidMiniMPIProgramException("MpiFinalized must be called.");
            }
            catch (MiniMPICollectiveAbortException ex)
            {
                // Record the error for the process
                Interlocked.Exchange(ref process.Error, ex);
            }
            catch (Exception ex)
            {
                Runtime.StartCollectiveAbort();

                // Only set this if not set yet
                Interlocked.CompareExchange(ref _firstErroredProcessRank, process.Rank, -1);

                // Record the error for the process
                Interlocked.Exchange(ref process.Error, ex);
            }
            finally
            {
                process.State = MpiProcessState.Finished;

                // Notify the runtime that the process has finished
                Interlocked.Increment(ref _processesFinishedCount);
                Runtime.NotifyStateChanged();
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///	Performs application-defined tasks associated with freeing, 
        ///	releasing, or resetting unmanaged resources. 
        /// </summary>
        /// <remarks>
        ///	This method will terminate the runtime and will cause it
        ///	to abort no matter what the state of the runtime is.
        /// </remarks>
        public virtual void Dispose()
        {
            // Note, since the runtime loop is executed in the same thread that calls Execute
            // we already know the runtime thread will have finished running

            _startProcessesTogether.Dispose();
        }

        #endregion

    }
}
