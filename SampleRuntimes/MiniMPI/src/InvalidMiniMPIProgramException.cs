using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMPI
{
    /// <summary>
    /// The exception thrown when a MiniMPI program or process is incorrect, as
    /// opposed to just making an invalid MPI operation. e.g. Forgetting to call
    /// <see cref="IMiniMPICoreAPI.MpiFinalize"/> after calling 
    /// <see cref="IMiniMPICoreAPI.MpiInit"/>.
    /// </summary>
    [global::System.Serializable]
    public class InvalidMiniMPIProgramException : MiniMPIException
    {
        /// <summary>Initializes a new instance.</summary>
        public InvalidMiniMPIProgramException() { }

        /// <summary>Initializes a new instance with a specified error message.</summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidMiniMPIProgramException(string message) : base(message) { }

        /// <summary>Initializes a new instance with a specified error message.</summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public InvalidMiniMPIProgramException(string message, Exception inner) : base(message, inner) { }

        protected InvalidMiniMPIProgramException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

    }
}
