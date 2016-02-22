using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMPI
{
    /// <summary>
    /// The exception encapsulating the exception thrown by an MiniMPI process.
    /// </summary>
    [global::System.Serializable]
    public class MiniMPIProcessException : MiniMPIException
    {

        private const string DefaultMessageFormatString = "The MiniMPI process (rank={0}) threw exception {1}: {2}";

        /// <summary>Initializes a new instance.</summary>
        /// <param name="rank">The rank of the process that threw the error.</param>
        /// <param name="ex">The exception that was thrown.</param>
        public MiniMPIProcessException(int rank, Exception ex)
            : base(String.Format(DefaultMessageFormatString
                , rank
                , ex.GetType().Name
                , ex.Message
                )
            , ex)
        {
            Rank = rank;
        }

        protected MiniMPIProcessException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        /// <summary>The rank of the process that threw the exception.</summary>
        public int Rank { get; private set; }

    }
}
