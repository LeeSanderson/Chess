using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMPI
{
    /// <summary>
    /// 	The base exception for all exceptions in the MiniMPI namespace.
    /// </summary>
    [global::System.Serializable]
    public class MiniMPIException : Exception
    {

        /// <summary>Initializes a new instance.</summary>
        public MiniMPIException() { }
        
        /// <summary>Initializes a new instance with a specified error message.</summary>
        /// <param name="message">The message that describes the error.</param>
        public MiniMPIException(string message) : base(message) { }
        
        /// <summary>Initializes a new instance with a specified error message.</summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public MiniMPIException(string message, Exception inner) : base(message, inner) { }

        protected MiniMPIException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

    }
}
