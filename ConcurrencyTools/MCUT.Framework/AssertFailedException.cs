using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>
    /// The exception that is thrown to indicate failure of a Concurrency Unit Test.
    /// </summary>
    [Serializable]
    public class AssertFailedException : ConcurrencyUnitTestException
    {
        public AssertFailedException() { }
        public AssertFailedException(string message) : base(message) { }
        public AssertFailedException(string message, Exception inner) : base(message, inner) { }
        protected AssertFailedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}