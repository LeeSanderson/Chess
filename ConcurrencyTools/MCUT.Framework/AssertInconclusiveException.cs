using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>
    /// The exception that is thrown to indicate a Concurrency Unit Test is not yet implemented or some other test should pass before this test will pass.
    /// </summary>
    [Serializable]
    public class AssertInconclusiveException : ConcurrencyUnitTestException
    {
        public AssertInconclusiveException() { }
        public AssertInconclusiveException(string message) : base(message) { }
        public AssertInconclusiveException(string message, Exception inner) : base(message, inner) { }
        protected AssertInconclusiveException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}