using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace MiniMPI
{
    /// <summary>
    /// Represents a process in this MiniMPI simulation
    /// </summary>
    public class MpiProcess
    {

        private Thread _thread;
        private List<Instruction> _instructions;

        /// <summary>
        ///	Creates an MpiProcess object with the following rank and
        ///	the thread it represents.
        /// </summary>
        /// <param name="rank">The rank of the MpiProcess.</param>
        /// <param name="thread">The thread that this object represents.</param>
        internal MpiProcess(int rank, Thread thread)
        {
            _thread = thread;
            _instructions = new List<Instruction>();

            Rank = rank;
            SendQueue = new List<AsyncSendInstruction>();
            ReceiveQueue = new List<AsyncReceiveInstruction>();
        }

        /// <summary>
        /// The rank of this process.
        /// </summary>
        /// <remarks>
        /// Rank starts at zero and increases incrementally.  Each MpiProcess
        /// has a unique rank.
        /// </remarks>
        public int Rank { get; private set; }
        internal Thread Thread { get { return _thread; } }

        /// <summary>Gets or sets the state of the process.</summary>
        /// <remarks>
        /// This should only be set from within the MiniMPIProgram class and from within the process' home thread.
        /// If we want to allow multi-threaded process threads, then the thread safety may or may not become
        /// an issue.
        /// Otherwise, we'll need to make this thread-safe. And I don't think just locking the API
        /// instance on the client side will suffice.
        /// </remarks>
        internal MpiProcessState State;

        /// <summary>
        ///	Represents the current blocking instruction on this MpiProcess. 
        /// </summary>
        /// <remarks>
        ///	A null value means that there is no blocking instruction currently.
        /// </remarks>
        internal volatile IBlockingInstruction BlockingInstruction;

        internal bool IsBlocked { get { return BlockingInstruction != null; } }

        // Set in the processWorker. Read in the runtime's Execute thread
        /// <summary>Gets the exception caught while running the process.</summary>
        internal Exception Error;

        /// <summary>
        /// The id of the next MiniMPI instruction that gets issued.
        /// This is only accessed from the process thread.
        /// </summary>
        internal int ProgramCounter { get; private set; }

        internal int GetNextInstructionID()
        {
            return ProgramCounter++;
        }

        /// <summary>
        /// This is only accessed from the runtime thread.
        /// </summary>
        internal List<AsyncSendInstruction> SendQueue { get; private set; }

        /// <summary>
        /// This is only accessed from the runtime thread.
        /// </summary>
        internal List<AsyncReceiveInstruction> ReceiveQueue { get; private set; }

        /// <summary>
        /// Adds the instructions to the process' history.
        /// This is only accessed from the process thread.
        /// </summary>
        internal void AddInstructionToHistory(Instruction instr)
        {
            _instructions.Add(instr);
        }

    }
}
