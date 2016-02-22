using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace MiniMPI
{
    /// <summary>
    ///	Represents the base implementation of the MiniMPI runtime.
    ///	Each runtime instance is considered it's own "communicator" 
    ///	as far as MPI terms are concerned.
    /// </summary>
    /// <remarks>
    ///	By implementing IDisposable, I am announcing that 
    ///	instances of this type allocate scarce resources.  Hence
    ///	it is a "Disposable" object.
    /// </remarks>
    internal abstract class MiniMPIRuntime : IDisposable
    {

        private int _isAborting;

        /// <summary>
        ///	This is an event handle that is triggered when the state of the
        /// MPIRuntime changes.
        /// </summary>
        private AutoResetEvent _runtimeStateChangedEvent;

        /// <summary>
        /// The wait handle used for blocking all processes on the MpiFinalize call.
        /// This is accordance with MPI2 specification that says MpiFinalize is
        /// also an implicit barrier.
        /// </summary>
        public readonly ManualResetEvent FinalizeWaitHandle;

        /// <summary>
        /// Never null, but a unique wait handle is used per cooperative barrier instruction.
        /// i.e. Once all processes have blocked on the barrier, a new handle is created for the
        /// next barrier instruction.
        /// </summary>
        private ManualResetEvent _currentBarrierWaitHandle;

        // Process management
        private MpiProcess[] _processes;
        protected MpiProcess[] Processes { get { return _processes; } }

        // Instructions
        private Queue<Instruction> _pendingInstructions;
        private object _pendingInstructionsLock = new object();
        private int _blockedProcessesCount;

        #region Properties

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
        internal MiniMPIRuntime(int processCount)
        {
            _runtimeStateChangedEvent = new AutoResetEvent(false);
            _currentBarrierWaitHandle = new ManualResetEvent(false);
            FinalizeWaitHandle = new ManualResetEvent(false);

            // Create all the processes
            ProcessCount = processCount;

            _pendingInstructions = new Queue<Instruction>();
        }

        #endregion

        #region Thread-safe methods when accessed in Runtime or Process threads

        internal MpiProcess GetProcessByRank(int rank)
        {
            MpiProcess foundP = Processes.FirstOrDefault(p => p.Rank == rank);
            if (foundP == null)
                throw new ArgumentOutOfRangeException("rank", rank, "No process with the specified rank exists.");
            return foundP;
        }

        /// <summary>
        /// Notifies the runtime thread that the state has changed. This allows
        /// it to process any new runtime rules that needs to be run.
        /// </summary>
        internal void NotifyStateChanged()
        {
            _runtimeStateChangedEvent.Set();
        }

        #endregion

        #region Runtime Thread Methods

        internal void InitializeProgram(MpiProcess[] processes)
        {
            // Since we're only setting this once, lets use Interlocked so the cache will be invalidated
            Interlocked.Exchange(ref _processes, processes);
        }

        internal void ProcessRules()
        {
            if (Thread.VolatileRead(ref _isAborting) == 1)
            {
                TryAborting();
            }
            else
            {
                // Process rules until none could be done
                bool didRuleGetFired;
                do
                {
                    didRuleGetFired = OnProcessRuntimeRules();
                } while (didRuleGetFired);
            }
        }

        /// <summary>
        /// When implemented in a derived class, this method should apply any
        /// rules for processing instructions.
        /// This method is called over and over again until it returns false.
        /// </summary>
        /// <returns>true, if rules got fired; Otherwise, false.</returns>
        protected abstract bool OnProcessRuntimeRules();

        internal void TryAborting()
        {
            // Release any blocked threads so they can abort
            foreach (var p in Processes)
                UnblockProcessForCollectiveAbort(p);

            OnTryAborting();
        }

        /// <summary>
        ///	A method intended to be called when the runtime has been
        ///	aborted for any reason.  It will perform necessary cleanup.
        /// </summary>
        /// <remarks>
        ///	Inside of this method, any thread that is still waiting on
        ///	the runtime must be released from their block, otherwise a
        ///	deadlock may occur.
        /// </remarks>
        protected abstract void OnTryAborting();

        /// <summary>Marks this process to abort.</summary>
        internal void UnblockProcessForCollectiveAbort(MpiProcess p)
        {
            // Try to unblock the process if it's blocked
            var blockingInstr = p.BlockingInstruction;
            if (blockingInstr != null)
                blockingInstr.WaitHandle.Set();
        }

        internal void BlockTillStateChanged()
        {
            _runtimeStateChangedEvent.WaitOne();
        }

        /// <summary>
        /// Adds any instructions added to the pending list from processes to the runtime state.
        /// </summary>
        internal void AddInstructionsFromPendingQueue()
        {
            var pendingInstrs = new List<Instruction>();

            // Dequeue all the instructions first while we have the lock open
            lock (_pendingInstructionsLock)
            {
                while (_pendingInstructions.Count > 0)
                    pendingInstrs.Add(_pendingInstructions.Dequeue());
            }

            // Now, lets add the pending instructions that we just dequeued
            foreach (var instr in pendingInstrs)
                AddInstructionFromPendingQueue(instr);
        }

        /// <summary>
        /// Adds an instruction that has just been dequeued from the pending instruction queue.
        /// </summary>
        protected abstract void AddInstructionFromPendingQueue(Instruction instr);

        /// <summary>
        /// Sets a new wait handle for the next barrier.
        /// This should be called just before calling Set on the current wait handle.
        /// </summary>
        protected void SetNextBarrierWaitHandle()
        {
            Interlocked.Exchange<ManualResetEvent>(ref _currentBarrierWaitHandle, new ManualResetEvent(false));
        }

        /// <summary>
        /// Gets a value indicating whether all processes are blocked on blocking instructions.
        /// </summary>
        /// <returns></returns>
        internal bool AreAllProcessesBlocked()
        {
            return Thread.VolatileRead(ref _blockedProcessesCount) == ProcessCount;
        }

        #endregion

        #region Process Thread Methods

        /// <summary>
        /// Starts a collective abort for all the processes.
        /// </summary>
        internal void StartCollectiveAbort()
        {
            Interlocked.Exchange(ref _isAborting, 1);
        }

        /// <summary>Allows the runtime to detect a collective abort or termination.</summary>
        internal void DetectCollectiveAbort()
        {
            if (Thread.VolatileRead(ref _isAborting) == 1)
                throw new MiniMPICollectiveAbortException();
        }

        /// <summary>Adds the instruction to the pending queue and notifies the runtime thread.</summary>
        internal void AddInstructionToPendingQueue(Instruction instr)
        {
            lock (_pendingInstructionsLock)
            {
                _pendingInstructions.Enqueue(instr);
            }
        }

        internal EventWaitHandle GetWaitHandleForCurrentBarrier()
        {
            return _currentBarrierWaitHandle;
        }

        /// <summary>
        /// Blocks the process and notifies the runtime that the state has changed just before blocking.
        /// </summary>
        internal void BlockProcessOnInstruction(MpiProcess p, IBlockingInstruction instr)
        {
            Debug.Assert(instr != null);

            // Setup state and notify the runtime we've changed the state of the runtime
            p.BlockingInstruction = instr;
            Interlocked.Increment(ref _blockedProcessesCount);
            _runtimeStateChangedEvent.Set();

            // And block
            BlockOnInstructionWaitHandle(instr);

            // Finished blocking, cleanup state
            Interlocked.Decrement(ref _blockedProcessesCount);
            p.BlockingInstruction = null;
        }

        internal void BlockOnInstructionWaitHandle(IBlockingInstruction instr)
        {
            // Runtime code
            instr.WaitHandle.WaitOne();

            /* Chess wrapper code
            do
            {
                SyncVarAccess;
                if (!instr.IsCompleted)
                    LocalBacktrack();
            }while(true);
            */
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

            _runtimeStateChangedEvent.Dispose();
            _currentBarrierWaitHandle.Dispose();
            FinalizeWaitHandle.Dispose();
        }

        #endregion

    }
}
