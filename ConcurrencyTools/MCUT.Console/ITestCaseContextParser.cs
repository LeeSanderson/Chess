using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using System.Reflection;

namespace Microsoft.Concurrency.TestTools.UnitTesting.MCutConsole
{
    /// <summary>
    /// Parser for reading an xml test case and extracting the test context form it.
    /// </summary>
    public interface ITestCaseContextParser
    {

        /// <summary>Error message when reading failed.</summary>
        string Error { get; }

        /// <summary>The successfully parsed context.</summary>
        ITestContext Context { get; }

        /// <summary>
        /// When implemented in a derived class, should parse the xml
        /// and set the <see cref="UnitTestContextParser.Context"/> property.
        /// </summary>
        /// <param name="xtestCase"></param>
        /// <returns>True if parsing was successful; otherwise, false.</returns>
        bool ParseFromTestCase(XElement xtestCase);

    }
}
