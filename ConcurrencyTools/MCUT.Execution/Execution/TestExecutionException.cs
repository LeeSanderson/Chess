using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// The exception that occurs during the running of a test (or test task) to
    /// indicate that the execution failed for some reason. This exception will
    /// often translate into a failed test result for the test.
    /// </summary>
    [Serializable]
    public class TestExecutionException : Exception
    {
        public TestExecutionException(string message, bool resultInconclusive = false)
            : base(message)
        {
            ResultInconclusive = resultInconclusive;
        }

        public TestExecutionException(string message, Exception inner) : base(message, inner) { }

        protected TestExecutionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public bool ResultInconclusive { get; private set; }

    }
}
