using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMPI
{
    /// <summary>
    /// The exception thrown when a MiniMPI call is invalid for the runtime or
    /// calling process' current state.
    /// </summary>
    [global::System.Serializable]
    public class InvalidMiniMPIOperationException : MiniMPIException
    {
        /// <summary>Initializes a new instance.</summary>
        public InvalidMiniMPIOperationException() { }

        /// <summary>Initializes a new instance with a specified error message.</summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidMiniMPIOperationException(string message) : base(message) { }

        /// <summary>Initializes a new instance with a specified error message.</summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public InvalidMiniMPIOperationException(string message, Exception inner) : base(message, inner) { }

        protected InvalidMiniMPIOperationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

    }
}
