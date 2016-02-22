using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>
    /// This is the base class for all of the Concurrency Unit Testing exceptions.
    /// </summary>
    [Serializable]
    public class ConcurrencyUnitTestException : Exception
    {
        public ConcurrencyUnitTestException() { }
        public ConcurrencyUnitTestException(string message) : base(message) { }
        public ConcurrencyUnitTestException(string message, Exception inner) : base(message, inner) { }
        protected ConcurrencyUnitTestException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}