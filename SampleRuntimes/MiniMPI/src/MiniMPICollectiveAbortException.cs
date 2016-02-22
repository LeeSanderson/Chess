using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMPI
{
    /// <summary>
    /// The exception thrown when a MiniMPI process is collectively aborted.
    /// </summary>
    [global::System.Serializable]
    public class MiniMPICollectiveAbortException : MiniMPIException
    {
        private const string DefaultMessage = "Terminated via collective abort.";

        /// <summary>Initializes a new instance.</summary>
        public MiniMPICollectiveAbortException() : base(DefaultMessage) { }

        /// <summary>Initializes a new instance with a specified error message.</summary>
        /// <param name="message">The message that describes the error.</param>
        public MiniMPICollectiveAbortException(string message) : base(message) { }

        /// <summary>Initializes a new instance with a specified error message.</summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public MiniMPICollectiveAbortException(string message, Exception inner) : base(message, inner) { }

        protected MiniMPICollectiveAbortException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

    }
}
