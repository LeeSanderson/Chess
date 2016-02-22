using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMPI
{
    class MiniMPIStringProcessAPI : MiniMPIProcessAPI, IMiniMPIStringAPI
    {

        internal MiniMPIStringProcessAPI(MiniMPIStringRuntime runtime, MpiProcess process)
            : base(runtime, process)
        {
            this.Runtime = (MiniMPIStringRuntime)base.Runtime;
        }

        new protected readonly MiniMPIStringRuntime Runtime;

        #region IMiniMPIStringProcessAPI Members

        /// <summary>
        ///	Sends the payload message from the current thread to the thread
        ///	with the destination rank.  See MPI_Isend in the MPI2 API.
        /// </summary>
        /// <param name="destRank">The destination</param>
        /// <param name="payload">The message being sent</param>
        /// <returns>
        ///	A handle on the send instruction that can be used with a function call
        ///	to <see cref="Wait(SendHandle)" />.
        /// </returns>
        /// <remarks>The eager value of this send is false.</remarks>
        public SendHandle SendAsync(int destRank, string payload)
        {
            return SendAsync(destRank, payload, false);
        }

        /// <summary>
        ///	Sends the payload message from the current thread to the thread
        ///	with the destination rank.  See MPI_Isend in the MPI2 API.
        /// </summary>
        /// <param name="destRank">The destination</param>
        /// <param name="payload">The message being sent</param>
        /// <param name="eager">
        /// When true, the instructions is automatically marked as complete when
        /// issued.  Thus an MpiWait on the handle returned will be a no-op.
        /// </param>
        /// <returns>
        ///	A handle on the send instruction that can be used with a function call
        ///	to <see cref="Wait(SendHandle)" />.
        /// </returns>
        public SendHandle SendAsync(int destRank, string payload, bool eager)
        {
            ValidateRankArgument("destRank", destRank);

            AssertProcessCanAccessMpiApi();
            Runtime.DetectCollectiveAbort();

            // Rule: P_S
            var instr = new AsyncSendInstruction(Process, Process.GetNextInstructionID(), destRank, payload, eager);
            Process.AddInstructionToHistory(instr);

            // Rule: R_SB - Eager (buffered) sends should automatically complete
            // as soon as the runtime has had an opportunity to do the buffering.
            // In this case, no actual buffering is needed for string data 
            // in .net so just set the instructions to being completed
            //   Note: Setting these sends to being completed is different than
            // setting it as matched.  This instruction is not yet matched.  The
            // runtime is responsible for deciding which receive to match this
            // send with.
            if (eager)
                instr.IsCompleted = true;

            AddInstructionToPendingQueue(instr);
            Runtime.NotifyStateChanged();

            return new SendHandle(instr);
        }

        /// <summary>
        ///	Receives a message from the source process.
        ///	See MPI_Irecv in the MPI2 API.
        /// </summary>
        /// <param name="srcRank">The source of the message.  Null specifies a "wildcard receive.</param>
        /// <returns>
        ///	A handle on the receive instruction that can be used with a function call
        ///	to <see cref="Wait(ReceiveHandle)" />.
        ///	</returns>
        public ReceiveHandle ReceiveAsync(int? srcRank)
        {
            if (srcRank.HasValue)
                ValidateRankArgument("srcRank", srcRank.Value);

            AssertProcessCanAccessMpiApi();
            Runtime.DetectCollectiveAbort();

            var instr = new AsyncReceiveInstruction(Process, Process.GetNextInstructionID(), srcRank);
            Process.AddInstructionToHistory(instr);

            AddInstructionToPendingQueue(instr);
            Runtime.NotifyStateChanged();

            return new ReceiveHandle(instr);
        }

        /// <summary>
        ///	Waits for the send instruction associated to this handle to 
        ///	complete.  See MPI_Wait in the MPI2 API.
        /// </summary>
        /// <param name="handle">The handle to the send instruction.</param>
        /// <remarks>
        ///	This method will not return until the associated send 
        ///	instruction to the SendHandle completes.
        /// </remarks>
        public void Wait(SendHandle handle)
        {
            Wait_impl(handle);
        }

        /// <summary>
        ///	Waits for the receive instruction associated to this handle to 
        ///	complete.  See MPI_Wait in the MPI2 API.
        /// </summary>
        /// <param name="handle">The handle to the receive instruction</param>
        /// <remarks>
        ///	This method will not return until the associated receive 
        ///	instruction to the SendHandle completes.
        /// </remarks>
        public string Wait(ReceiveHandle handle)
        {
            Wait_impl(handle);
            return (string)handle.Instruction.Payload;
        }

        /// <summary>Performs an MPI wait for a matching MpiReceiveAsync call.</summary>
        private void Wait_impl(Handle handle)
        {
            if (handle == null)
                throw new ArgumentNullException("handle");

            // Only if they're passing data between processes should this be
            // possible
            if (handle.Instruction.Owner != Process)
                throw new ArgumentException("handle", "Handle owned by a different process.");

            AssertProcessCanAccessMpiApi();
            Runtime.DetectCollectiveAbort();

            // Rule: P_W
            var instr = new WaitInstruction(Process, Process.GetNextInstructionID(), handle);
            Process.AddInstructionToHistory(instr);

            AddInstructionToPendingQueue(instr);

#if SHOW_STORE_BUFFER_VULNERABILITY_BUG
            // JAM: I originally had this condition here to possibly speed things up
            // but it ended up causing a Store Buffer Vulnerability race condition.
            // It's here so that it can be shown as an example

            // If already finished, lets not bother waiting
            // Checking this here, even though it's volatile
            if (!handle.Instruction.IsCompleted)
#endif
            BlockProcessOnInstruction(instr);
            Runtime.DetectCollectiveAbort();    // Check again after we've been unblocked

            //Runtime.NotifyStateChanged();
        }

        #endregion

    }
}
