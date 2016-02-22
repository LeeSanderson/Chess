using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>The base implementation for a context in which to run a test.</summary>
    public class TestContext : ITestContext
    {

        public TestContext(string name, string expResultKey)
        {
            Name = name;
            ExpectedResultKey = expResultKey;
        }

        public string Name { get; private set; }

        /// <summary>The key of the expected result to use.</summary>
        public string ExpectedResultKey { get; private set; }

    }
}
