using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;


namespace MiniMPI
{
    /// <summary>
    /// Represents an MPI runtime for managing string data.
    /// </summary>
    /// <remarks>
    /// Each runtime instance is considered it's own "communicator" as far as
    /// MPI terms are concerned.
    /// </remarks>
    internal class MiniMPIStringRuntime : MiniMPIRuntime
    {

        // We want to keep the sends separated by who issued them.  This is
        // used so that we can allow wildcard receives to match with eligible
        // sends based on the priority of the processes.
        private List<AsyncSendInstruction>[] _unmatchedSends;
        private List<AsyncReceiveInstruction> _unmatchedReceives;
        private List<WaitInstruction> _unmatchedWaits;
        private List<BarrierInstruction> _unmatchedBarriers;
        private List<FinalizeBarrierInstruction> _unmatchedFinalizeBarriers;
        private List<Instruction> _matched;

        #region Constructors

        /// <summary>Creates a new instance.</summary>
        /// <param name="processCount">The number of process threads to create to execute the MPI program.</param>
        public MiniMPIStringRuntime(int processCount)
            : base(processCount)
        {
            _unmatchedSends = new List<AsyncSendInstruction>[processCount];
            for (int processIdx = 0; processIdx < processCount; processIdx++)
            {
                _unmatchedSends[processIdx] = new List<AsyncSendInstruction>();
            }

            _unmatchedReceives = new List<AsyncReceiveInstruction>();
            _unmatchedWaits = new List<WaitInstruction>();
            _unmatchedBarriers = new List<BarrierInstruction>();
            _unmatchedFinalizeBarriers = new List<FinalizeBarrierInstruction>();
            _matched = new List<Instruction>();
        }

        #endregion

        /// <summary>
        ///	A method intended to be called when the runtime has been
        ///	aborted for any reason.  It will perform necessary cleanup.
        /// </summary>
        /// <remarks>
        ///	Inside of this method, any thread that is still waiting on
        ///	the runtime must be released from their block, otherwise a
        ///	deadlock may occur.
        /// </remarks>
        protected override void OnTryAborting()
        {
            // JAM: I believe the unblocking done in the base runtime should be sufficient.
            // If it's not, then we can see what of the following may be pertinent.

            //// Here we will release all of the blocking instructions
            //// that are in the list of _unmatchedWaits, 
            //// _unmatchedBarriers, and in the _pendingInstructions.
            ////   The problem that can arise if we don't is that
            //// the runtime may acquire the lock before a process
            //// starts waiting on a blocking instruction.  Before
            //// the runtime is able to process the wait instruction,
            //// it may have been told to abort or terminate.  If
            //// only the processes that are currently waiting are
            //// released, what about the ones who will begin waiting
            //// as soon as they acquire processing time?

            //lock (_pendingInstructions)
            //{
            //    foreach (Instruction i in _pendingInstructions)
            //        if (i is IBlockingInstruction)
            //            ((IBlockingInstruction)i).ReleaseBlock();
            //}

            //foreach (WaitInstruction w in _unmatchedWaits)
            //    w.ReleaseBlock();

            //foreach (BarrierInstruction b in _unmatchedBarriers)
            //    b.ReleaseBlock();
        }

        protected override void AddInstructionFromPendingQueue(Instruction instr)
        {
            // NOTE: This is run on the runtime thread

            var ownerProcess = instr.Owner;

            switch (instr.Type)
            {
                // We want to keep track of which process sent the message.  For that
                // reason, we have an array of lists.  One list for each process.
                case InstructionType.AsyncSend:
                    var sendInstr = (AsyncSendInstruction)instr;
                    ownerProcess.SendQueue.Add(sendInstr);
                    _unmatchedSends[instr.Owner.Rank].Add(sendInstr);
                    break;

                case InstructionType.AsyncReceive:
                    var recvInstr = (AsyncReceiveInstruction)instr;
                    ownerProcess.ReceiveQueue.Add(recvInstr);
                    _unmatchedReceives.Add(recvInstr);
                    break;

                case InstructionType.Wait:
                    _unmatchedWaits.Add((WaitInstruction)instr);
                    break;

                case InstructionType.Barrier:
                    _unmatchedBarriers.Add((BarrierInstruction)instr);
                    break;

                case InstructionType.FinalizeBarrier:
                    _unmatchedFinalizeBarriers.Add((FinalizeBarrierInstruction)instr);
                    break;

                default:
                    throw new NotImplementedException("The instruction type is not implemented yet: " + instr.Type.ToString());
            }
        }

        /// <summary>
        /// Applies the rules of this runtime and returns false only when no rules were fired.
        ///	See the MPI2 specifications for the rules that must be run inside of this method.
        /// </summary>
        /// <remarks>
        ///	This method is called by the runtime loop over and over until it returns false.
        /// </remarks>
        /// <returns>true, if rules got fired; Otherwise, false.</returns>
        protected override bool OnProcessRuntimeRules()
        {
            bool didRuleGetFired = false;

            didRuleGetFired |= RunRule_MatchDeterministicReceives();

            didRuleGetFired |= RunRule_ReleaseWaits();

            didRuleGetFired |= RunRule_ReleaseBarrier();

            didRuleGetFired |= RunRule_ReleaseFinalizeBarrier();

            // Rule: R_SR* - Match wildcard receives only if no other rule was
            //       fired.  We only want one of the wildcard receives to match per loop
            if (!didRuleGetFired)
                didRuleGetFired |= RunRule_MatchWildcardReceives();

            return didRuleGetFired;
        }

        private bool RunRule_ReleaseBarrier()
        {
            // Rule: R_B - Match barriers
            if (_unmatchedBarriers.Count != ProcessCount)
                return false;

            // Its possible that the instruction has been removed from the pending queue before the p.IsBlocked property is set
            if (!AreAllProcessesBlocked())
                return false;

            Debug.Assert(_unmatchedBarriers.Select(instr => instr.Owner.Rank).Distinct().Count() == ProcessCount
                , "Somehow, a process got added to the unmatched barrier queue more than once.");
            Debug.Assert(Processes.All(p => p.IsBlocked), "All the processes should be blocked.");

            var waitHandle = GetWaitHandleForCurrentBarrier();
            Debug.Assert(_unmatchedBarriers.All(instr => Object.ReferenceEquals(instr.WaitHandle, waitHandle))
                , "All unmatched barrier instructions should be using the same WaitHandle instance.");

            // Mark them as being matched and complete
            foreach (var barrInstr in _unmatchedBarriers)
            {
                _matched.Add(barrInstr);
                barrInstr.IsCompleted = true;
            }

            // Clear the instructions list
            _unmatchedBarriers.Clear();

            // Release the processes all at once using the single EventWaitHandle
            // But before releasing it, lets setup for the next barrier using a new handle since once Set, we won't ever Reset them.
            SetNextBarrierWaitHandle();
            waitHandle.Set();

            return true;
        }

        private bool RunRule_ReleaseFinalizeBarrier()
        {
            // Rule: not specified in the paper - Match barrier for the MpiFinalize command
            if (_unmatchedFinalizeBarriers.Count != ProcessCount)
                return false;

            // Its possible that the instruction has been removed from the pending queue before the p.IsBlocked property is set
            if (!AreAllProcessesBlocked())
                return false;

            Debug.Assert(_unmatchedFinalizeBarriers.Select(instr => instr.Owner.Rank).Distinct().Count() == ProcessCount
                , "Somehow, a process got added to the unmatched finalize barrier queue more than once.");
            Debug.Assert(Processes.All(p => p.IsBlocked), "All the processes should be blocked waiting for the finalize barrier to complete.");

            var waitHandle = FinalizeWaitHandle;
            Debug.Assert(_unmatchedFinalizeBarriers.All(instr => Object.ReferenceEquals(instr.WaitHandle, waitHandle))
                , "All unmatched FinalizeBarrier instructions should be using the WaitHandle instance as the Runtim.FinalizeWaitHandle.");

            // Mark them as being matched and complete
            foreach (var barrInstr in _unmatchedFinalizeBarriers)
            {
                _matched.Add(barrInstr);
                barrInstr.IsCompleted = true;
            }

            // Clear the instructions list
            _unmatchedFinalizeBarriers.Clear();

            // Release the processes all at once using the single EventWaitHandle
            waitHandle.Set();

            return true;
        }

        private bool RunRule_ReleaseWaits()
        {
            bool didRuleGetFired = false;

            // Rule: R_W - Waits finish as soon as the send/receive it refers to is 'complete'
            // - i.e. have copied out/in their data (whether they have matched or not)
            for (int wIdx = 0; wIdx < _unmatchedWaits.Count; wIdx++)
            {
                var waitInstr = _unmatchedWaits[wIdx];

                // Mark the instruction being waited upon as having a wait.
                //waitInstr.WaitingOnInstruction.HasWaitInstruction = true;

                if (waitInstr.WaitingOnInstruction.IsCompleted)
                {
                    didRuleGetFired = true;

                    _unmatchedWaits.RemoveAt(wIdx);
                    _matched.Add(waitInstr);

                    waitInstr.IsCompleted = true;

                    // Fires the R_Wret rule
                    waitInstr.WaitHandle.Set();

                    // Because we removed one from the list
                    wIdx--;
                }
            }

            return didRuleGetFired;
        }

        private bool RunRule_MatchDeterministicReceives()
        {
            bool didRuleGetFired = false;

            // Rule: R_SR - match between sends and deterministic receives
            for (int recvIdx = 0; recvIdx < _unmatchedReceives.Count; recvIdx++)
            {
                AsyncReceiveInstruction recvInstr = _unmatchedReceives[recvIdx];

                // Skip any non-deterministic receives (i.e. wildcard receives)
                if (recvInstr.IsWildcardReceive)
                    continue;

                int recverRank = recvInstr.ReceiverRank;
                int senderRank = recvInstr.SenderRank.Value;

                // Make sure that the recvInstr does not have an eligible receive
                // that was issued before it. (e.g. a wildcard receive before it
                // issued from the same MpiProcess)
                //   This check is only important if this recvInstr is not a wildcard receive.
                bool recvInstrHasUnmatchedPredecessor = _unmatchedReceives
                    .Take(recvIdx) // Look at all recvInstr before the current one
                    .Any(unmatchedRecv => unmatchedRecv.ReceiverRank == recverRank && unmatchedRecv.IsWildcardReceive);

                // Handle the deterministic receives first
                if (!recvInstrHasUnmatchedPredecessor)
                {
                    // before we look at the process and use it's lock, lets see if
                    // we can find it in the ready list first. 
                    //   If it's in the list, then we know it's been added to the
                    // process' queues due to the logic in the async methods.
                    int sendIdx = _unmatchedSends[senderRank]
                        .FindIndex(instr => instr.ReceiverRank == recverRank);
                    if (sendIdx != -1)
                    {
                        // Then we have a match
                        // Rules: R_SR
                        didRuleGetFired = true;

                        ProcessRule_Match_SendRecv(recvIdx, recvInstr, senderRank, sendIdx);

                        // Since we removed an element from the list we are iterating
                        // thru
                        recvIdx--;
                    }
                }
            }

            return didRuleGetFired;
        }

        private bool RunRule_MatchWildcardReceives()
        {
            // Rule: R_SR* - Match wildcard receives

            // Get the list of wildcard receive instruction indices that we could possibly match
            var wildRecvIdxs = _unmatchedReceives
                .Select((instr, idx) => new { idx, instr })
                .Where(x => x.instr.IsWildcardReceive)
                // Further filter by only the first per process
                // Since we want to fulfill the first ones first
                .GroupBy(x => x.instr.ReceiverRank)
                .Select(x => x.First())
                // Only return the wildcard receives that have at least
                // one send available to match with
                .Where(wildRecv => _unmatchedSends
                    .SelectMany(ary => ary)   // Flatten the list
                    .Any(sendInstr => sendInstr.ReceiverRank == wildRecv.instr.ReceiverRank)
                    )
                .Select(wildRecv => wildRecv.idx)
                .ToArray(); // Actualize to prevent re-querying the above

            int wildRecvIdx = ChooseWildcardReceiveToMatch(wildRecvIdxs);
            if (wildRecvIdx < 0)
                return false;

            Debug.Assert(wildRecvIdxs.Contains(wildRecvIdx));

            var wildRecvInstr = _unmatchedReceives[wildRecvIdx];
            int receiverRank = wildRecvInstr.ReceiverRank;

            // Get the list of sends available to match with the wildcard receive
            // The first send per process
            var sendInstrIdxs = _unmatchedSends
                .Select(sends => sends.FindIndex(instr => instr.ReceiverRank == receiverRank))
                .ToList();
            Debug.Assert(sendInstrIdxs.Count == ProcessCount);
            int senderRank = ChooseSendToMatchWithWildcardReceive(sendInstrIdxs);
            int sendIdx = sendInstrIdxs[senderRank];

            ProcessRule_Match_SendRecv(wildRecvIdx, wildRecvInstr, senderRank, sendIdx);

            return true;
        }

        /// <summary>
        /// Determines which wildcard receive instruction to match.
        /// </summary>
        /// <param name="wildRecvIdxs">The indices (within _unmatchedReceives) of the wildcard receives available to match.</param>
        /// <returns>
        /// -1 is no wildcard receive should be matched; Otherwise, an
        /// index contained within <paramref name="wildRecvIdxs"/>.
        /// </returns>
        /// <remarks>
        /// NOTE: This method should be private, but it's internal so extended reflection can intercept it
        /// </remarks>
        internal int ChooseWildcardReceiveToMatch(int[] wildRecvIdxs)
        {
            // We're just going to always return the first one (being the oldest)
            return wildRecvIdxs.Length == 0 ? -1 : wildRecvIdxs[0];
        }

        /// <summary>
        /// Chooses the rank of the a single sender rank and send instruction to match a wildcard receive.
        /// </summary>
        /// <param name="sendInstrIdxs">
        /// The indices (one per process rank) identifying the unmatched send instruction that could be chosen.
        /// Each element in this array (sendInstrIdxs[rank]) represents the first valid instruction
        /// within the _unmatchedReceives[rank] list.
        /// If a process doesn't have any available sends, then it's value sendInstrIdxs[rank]
        /// will equal -1.
        /// </param>
        /// <returns>
        /// The 'rank' of the unmatched send instruction to match with the wildcard receive.
        /// i.e. The instruction identified by _unmatchedSends[rank][sendInstrIdxs[rank]] will chosen.
        /// </returns>
        /// <remarks>
        /// NOTE: This method should be private, but it's internal so extended reflection can intercept it
        /// </remarks>
        internal int ChooseSendToMatchWithWildcardReceive(List<int> sendInstrIdxs)
        {
            // We'll just choose the first one available.
            return sendInstrIdxs.FindIndex(sendIdx => sendIdx != -1);
        }

        /// <summary>
        /// This method does the modification of the state for the following
        /// rules: R_SR, R_SR*
        /// </summary>
        /// <param name="recvIdx">The index in the _unmatchedRecieves list.</param>
        /// <param name="recvInstr"></param>
        /// <param name="senderRank"></param>
        /// <param name="sendIdx">The index in the _unmatchedSends list.</param>
        private void ProcessRule_Match_SendRecv(int recvIdx, AsyncReceiveInstruction recvInstr, int senderRank, int sendIdx)
        {
            // Get the send instruction
            MpiProcess senderP = GetProcessByRank(senderRank);

            // Look thru the sender's queue to get the match by looking for the
            // first one sent to the receiver.  This way, we ensure the 
            // NonOvertaking requirement
            var sendInstr = senderP.SendQueue.FirstOrDefault(instr => instr.ReceiverRank == recvInstr.ReceiverRank);
            Debug.Assert(sendInstr != null, "If an instruction is in the ready queue, it should also be in the process's queue of instructions too.");

            // Since we got the lock open, lets remove it now
            senderP.SendQueue.Remove(sendInstr);

            // Now remove it from the receiver's queue
            recvInstr.Owner.ReceiveQueue.Remove(recvInstr);

            // Match up on our end
            recvInstr.MatchedSend = sendInstr;
            _unmatchedReceives.RemoveAt(recvIdx);
            _matched.Add(recvInstr);

            sendInstr.MatchedReceive = recvInstr;
            _unmatchedSends[senderRank].RemoveAt(sendIdx);
            _matched.Add(sendInstr);

            // And finally, Simulate the completion of the instructions by copying
            // their data
            recvInstr.Payload = sendInstr.Payload;
            recvInstr.IsCompleted = true;
            // Even if the SendInstr was eager, just set it completed again
            sendInstr.IsCompleted = true;
        }

    }
}
