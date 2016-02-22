using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MiniMPI
{
    /// <summary>
    ///		This represents the handle to a specific <see cref="Instruction" />
    /// </summary>
    public class Handle : IDisposable
    {

        internal readonly Instruction Instruction;

        internal Handle(Instruction instr)
        {
            Instruction = instr;
            // Keep set because once it's matched it shouldn't block anything
            WaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        }

        /// <summary>
        /// Notifies when this instruction has completed.
        /// For the send/recv instructions, this is set to true whenever the
        /// data has been copied to either an internal buffer (not
        /// necessarily matched yet) or, when not buffered, transmitted to
        /// its match.
        /// </summary>
        internal readonly EventWaitHandle WaitHandle;

        #region IDisposable Members

        public void Dispose()
        {
            WaitHandle.Dispose();
        }

        #endregion

    }

    /// <summary>
    ///	This represents the handle to a specific <see cref="AsyncSendInstruction" />.
    /// </summary>
    public class SendHandle : Handle
    {

        internal SendHandle(AsyncSendInstruction instr) : base(instr) { }

    }

    /// <summary>
    ///	This represents the handle to a specific <see cref="AsyncReceiveInstruction" />.
    /// </summary>
    public class ReceiveHandle : Handle
    {

        internal ReceiveHandle(AsyncReceiveInstruction instr) : base(instr) { }

        new internal AsyncReceiveInstruction Instruction { get { return (AsyncReceiveInstruction)base.Instruction; } }

    }
}
