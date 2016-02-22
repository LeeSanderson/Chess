using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>Provides information about a context in which unit tests run in.</summary>
    public interface ITestContext
    {

        /// <summary>The friendly name for the context.</summary>
        string Name { get; }

        /// <summary>The key of the expected result to use.</summary>
        string ExpectedResultKey { get; }

    }
}
