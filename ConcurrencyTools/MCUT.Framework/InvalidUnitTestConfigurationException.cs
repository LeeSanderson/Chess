using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>Exception thrown when a MCUT assembly has an invalid configuration.</summary>
    [Serializable]
    public class InvalidUnitTestConfigurationException : Exception
    {
        public InvalidUnitTestConfigurationException() { }
        public InvalidUnitTestConfigurationException(string message) : base(message) { }
        public InvalidUnitTestConfigurationException(string message, Exception inner) : base(message, inner) { }
        protected InvalidUnitTestConfigurationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
