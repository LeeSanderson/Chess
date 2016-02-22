using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MiniMPI
{
    // TODO: Need to add logic to assert whether the current thread can access the API. Only the process' thread should be able to access it.
    // I think this should be only a DEBUG thing, like how Windows Forms will only throw an exception if trying to access the UI state from
    // a thread that isn't the UI thread. Possibly implement via contracts???
    // Note: This is only an issue if the user code thread doesn't catch exceptions and bubble them up to the process' thread, otherwise, it would be fine.

    abstract class MiniMPIProcessAPI : IMiniMPICoreAPI
    {

        /// <summary>The underlying runtime for this instance.</summary>
        protected readonly MiniMPIRuntime Runtime;

        /// <summary>The process instance for this API instance.</summary>
        protected readonly MpiProcess Process;

        // Thread-local variable so we don't have to read the volatile process.state variables
        private bool _isMpiInited;
        private bool _isMpiFinalized;

        /// <summary>Gets the number of processes managed by this runtime instance.</summary>
        /// <remarks>
        /// This is equivalent to a call to MPI_Comm_size(grpID, value) where
        /// value is what's returned and grpID would be associated with this
        /// instance.
        /// </remarks>
        public int ProcessCount { get { return Runtime.ProcessCount; } }

        internal MiniMPIProcessAPI(MiniMPIRuntime runtime, MpiProcess process)
        {
            Runtime = runtime;
            Process = process;
        }

        /// <summary>Asserts that the <see cref="MpiProcess" /> can access MPI calls.</summary>
        /// <remarks>
        /// This method does NOT need to be called from within a lock block on the p.SyncRoot object.
        /// </remarks>
        /// <exception cref="InvalidMiniMPIOperationException">
        /// The process can not access the MPI API because it's not in the initialized state.
        /// </exception>
        protected void AssertProcessCanAccessMpiApi()
        {
            if (!_isMpiInited || _isMpiFinalized)
                throw new InvalidMiniMPIOperationException("MPI calls must be between calls to Initialize and Finalize.");
        }

        protected void ValidateRankArgument(string paramName, int rank)
        {
            if (rank < 0 || rank >= ProcessCount)
                throw new ArgumentOutOfRangeException(paramName, rank, "No process with the specified rank exists. Must be in the range [0, ProcessCount)");
        }

        protected void AddInstructionToPendingQueue(Instruction instr)
        {
            Runtime.AddInstructionToPendingQueue(instr);
        }

        /// <summary>
        /// Blocks the process and notifies the runtime that the state has changed just before blocking.
        /// </summary>
        protected void BlockProcessOnInstruction(IBlockingInstruction instr)
        {
            Runtime.BlockProcessOnInstruction(Process, instr);
        }

        #region IMiniMPIProcessAPI Members

        /// <summary>Registers the current process as being ready to use the MPI runtime.</summary>
        /// <remarks>
        /// This is modeled after the MPI_Init method, where MPI 2 doesn't
        /// require any args to be passed in.
        /// </remarks>
        public void MpiInit()
        {
            // Need to allow this to occur before checking the state
            Runtime.DetectCollectiveAbort();

            if (_isMpiInited)
                throw new InvalidMiniMPIOperationException("The current process has already been initialized.");

            Debug.Assert(Process.State == MpiProcessState.Started);
            Process.State = MpiProcessState.MpiInitialized;
            _isMpiInited = true;
        }

        /// <summary>Gets the rank within the runtime's group of threads for the calling thread.</summary>
        /// <returns>A value in the range [0,ProcessCount).</returns>
        public int GetRank()
        {
            AssertProcessCanAccessMpiApi();
            Runtime.DetectCollectiveAbort();

            return Process.Rank;
        }

        public void MpiFinalize()
        {
            AssertProcessCanAccessMpiApi();
            Runtime.DetectCollectiveAbort();

            if (!_isMpiInited)
                throw new InvalidMiniMPIOperationException("The process has not yet been initialized.");
            if (_isMpiFinalized)
                throw new InvalidMiniMPIOperationException("The process has already been finalized.");

            Debug.Assert(Process.State == MpiProcessState.MpiInitialized);
            Process.State = MpiProcessState.MpiFinalized;
            _isMpiFinalized = true;

            // Execute a barrier because in MPI2 API, MPI_Finalize is a collective blocking instruction just like a barrier.
            var instr = new FinalizeBarrierInstruction(Process, Process.GetNextInstructionID(), Runtime.FinalizeWaitHandle);
            Process.AddInstructionToHistory(instr);

            AddInstructionToPendingQueue(instr);
            BlockProcessOnInstruction(instr);
            Runtime.DetectCollectiveAbort();    // Check again after we've been unblocked
        }

        /// <summary>
        ///	Waits until all of the processes belonging to this runtime
        ///	have all called this Barrier method.  Execution will not
        ///	return to the caller until this condition has been met.
        ///	See MPI_Barrier in the MPI2 API.
        /// </summary>
        public void Barrier()
        {
            AssertProcessCanAccessMpiApi();
            Runtime.DetectCollectiveAbort();

            var instr = new BarrierInstruction(Process, Process.GetNextInstructionID(), Runtime.GetWaitHandleForCurrentBarrier());
            Process.AddInstructionToHistory(instr);

            AddInstructionToPendingQueue(instr);
            BlockProcessOnInstruction(instr);
            Runtime.DetectCollectiveAbort();    // Check again after we've been unblocked

            //Runtime.NotifyStateChanged();
        }

        #endregion

    }
}
