using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MiniMPI
{
    /// <summary>
    /// The exception thrown when one or more MPI threads threw an exception.
    /// </summary>
    [global::System.Serializable]
    public class MiniMPIExecutionException : AggregateException
    {

        private const string DefaultMessageFormatString = "The MiniMPI program failed due to one or more of the processes throwing an exception. First Exception Thrown (rank = {0}): {1}";

        /// <summary>Initializes a new instance.</summary>
        /// <param name="firstError">The first exception raised from a MiniMPI process thread.</param>
        /// <param name="errors">All errors that occurred (up to one per process).</param>
        public MiniMPIExecutionException(MiniMPIProcessException firstError, IList<MiniMPIProcessException> errors)
            : base(String.Format(DefaultMessageFormatString
                , firstError.Rank
                , firstError.Message
                )
            , errors
            )
        {
            System.Diagnostics.Debug.Assert(errors.Contains(firstError));

            FirstException = firstError;
            InnerExceptions = new ReadOnlyCollection<MiniMPIProcessException>(errors);
        }

        protected MiniMPIExecutionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        /// <summary>The first exception thrown from a child process.</summary>
        public MiniMPIProcessException FirstException { get; private set; }

        new public ReadOnlyCollection<MiniMPIProcessException> InnerExceptions { get; private set; }

        /// <summary>Returns a string representing this exception.</summary>
        /// <returns>A string representing this exception.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(base.Message + Environment.NewLine);

            // Report the error states of all the processes
            sb.AppendLine("Thread termination errors:");
            foreach (MiniMPIProcessException procEx in InnerExceptions)
            {
                int rank = procEx.Rank;
                var ex = procEx.InnerException;
                if (ex is MiniMPICollectiveAbortException)
                    sb.AppendFormat("   [{0}] Collectively aborted.", rank);
                else
                    sb.AppendFormat("   [{0}] {1}: {2}", rank, ex.GetType().Name, ex.Message);
                sb.AppendLine();
            }

            return sb.ToString();
        }

    }
}
