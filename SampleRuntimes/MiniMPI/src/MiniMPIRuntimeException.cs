using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMPI
{
    /// <summary>
    /// The exception thrown by a MiniMPIRuntime instance when an internal error occurs but not within a process.
    /// </summary>
    [Serializable]
    public class MiniMPIRuntimeException : MiniMPIException
    {
        /// <summary>Initializes a new instance.</summary>
        public MiniMPIRuntimeException() { }
        
        /// <summary>Initializes a new instance with a specified error message.</summary>
        /// <param name="message">The message that describes the error.</param>
        public MiniMPIRuntimeException(string message) : base(message) { }
        
        /// <summary>Initializes a new instance with a specified error message.</summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public MiniMPIRuntimeException(string message, Exception inner) : base(message, inner) { }

        protected MiniMPIRuntimeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
