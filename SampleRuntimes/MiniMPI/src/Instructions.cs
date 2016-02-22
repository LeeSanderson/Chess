using System.Threading;

namespace MiniMPI
{
    /// <summary>
    ///   An enum used for determining different types of Instructions.
    /// </summary>
    public enum InstructionType
    {
        AsyncSend,
        AsyncReceive,
        Wait,
        Barrier,
        FinalizeBarrier,
    }

    public interface IBlockingInstruction
    {
        /// <summary>Gets the wait handle used to block this instruction.</summary>
        EventWaitHandle WaitHandle { get; }

        bool IsCompleted { get; }
    }

    public abstract class Instruction
    {
        public readonly MpiProcess Owner;
        public readonly int ID;

        protected Instruction(MpiProcess owner, int id, InstructionType type)
        {
            Owner = owner;
            ID = id;
            Type = type;
            //HasWaitInstruction = false;
        }

        // JAM: I removed this, not knowing why it's here. It's set by the runtime but not read anywhere.
        //public volatile bool HasWaitInstruction;

        /// <summary>Describes the type of Instruction that this is.</summary>
        public InstructionType Type { get; private set; }

        private volatile bool _isCompleted;
        /// <summary>Indicates whether this instruction has completed or not.</summary>
        public bool IsCompleted
        {
            get { return _isCompleted; }
            set { _isCompleted = value; }
        }

    }

    public class AsyncSendInstruction : Instruction
    {
        public AsyncSendInstruction(MpiProcess owner, int id, int destRank, string payload, bool eager)
            : base(owner, id, InstructionType.AsyncSend)
        {
            ReceiverRank = destRank;
            Payload = payload;
            IsEager = eager;
        }

        public bool IsEager { get; private set; }
        public string Payload { get; private set; }

        public int SenderRank { get { return Owner.Rank; } }
        public int ReceiverRank { get; private set; }

        public AsyncReceiveInstruction MatchedReceive { get; set; }

    }

    public class AsyncReceiveInstruction : Instruction
    {
        public AsyncReceiveInstruction(MpiProcess owner, int id, int? senderRank)
            : base(owner, id, InstructionType.AsyncReceive)
        {
            SenderRank = senderRank;
        }

        public int? SenderRank { get; private set; }
        public int ReceiverRank { get { return Owner.Rank; } }

        /// <summary>Indicates whether this receive instruction is a wildcard receive.</summary>
        public bool IsWildcardReceive { get { return !SenderRank.HasValue; } }

        public AsyncSendInstruction MatchedSend { get; set; }

        public volatile string Payload;

    }

    public class WaitInstruction : Instruction, IBlockingInstruction
    {

        public WaitInstruction(MpiProcess owner, int id, Handle handle)
            : base(owner, id, InstructionType.Wait)
        {
            WaitingOnInstruction = handle.Instruction;
            WaitHandle = handle.WaitHandle;
        }

        /// <summary>The instruction that this wait is waiting on.</summary>
        public Instruction WaitingOnInstruction { get; private set; }

        public EventWaitHandle WaitHandle { get; private set; }

    }

    public class BarrierInstruction : Instruction, IBlockingInstruction
    {
        public BarrierInstruction(MpiProcess owner, int id, EventWaitHandle waitHandle)
            : base(owner, id, InstructionType.Barrier)
        {
            WaitHandle = waitHandle;
        }

        /// <summary>The shared wait handle for this barrier across all processes.</summary>
        public EventWaitHandle WaitHandle { get; private set; }

        bool IBlockingInstruction.IsCompleted { get { return this.IsCompleted; } }

    }

    public class FinalizeBarrierInstruction : Instruction, IBlockingInstruction
    {
        public FinalizeBarrierInstruction(MpiProcess owner, int id, EventWaitHandle waitHandle)
            : base(owner, id, InstructionType.FinalizeBarrier)
        {
            WaitHandle = waitHandle;
        }

        /// <summary>The shared wait handle for all processes executing the MpiFinalize instruction.</summary>
        public EventWaitHandle WaitHandle { get; private set; }

        bool IBlockingInstruction.IsCompleted { get { return this.IsCompleted; } }

    }
}
