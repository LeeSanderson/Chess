using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>
    /// The exception that is thrown to indicate a call to <see cref="System.Diagnostics.Debug.Assert(bool)"/> failed.
    /// </summary>
    [Serializable]
    public class DebugAssertFailedException : ConcurrencyUnitTestException
    {
        public DebugAssertFailedException() { }
        public DebugAssertFailedException(string message) : base(message) { }
        public DebugAssertFailedException(string message, Exception inner) : base(message, inner) { }

        public DebugAssertFailedException(string message, string detailedMessage, Exception inner)
            : base(message, inner)
        {
            DetailedMessage = detailedMessage;
        }

        protected DebugAssertFailedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public override string Message
        {
            get
            {
                string finalMsg = "Debug.Assert Failed";
                if (base.Message == null)
                    finalMsg += ".";
                else
                    finalMsg += ": " + base.Message;

                if (DetailedMessage != null)
                    finalMsg += Environment.NewLine + "Detail Message: " + DetailedMessage;

                return finalMsg;
            }
        }

        public string DetailedMessage { get; private set; }

    }
}