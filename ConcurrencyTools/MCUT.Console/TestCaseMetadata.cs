using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.Execution;
using System.Xml.Linq;
using System.Reflection;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.UnitTesting.MCutConsole
{
    /// <summary>
    /// Information about a test case to be run by a runner. Provides access and helper methods
    /// to common metadata.
    /// </summary>
    public class TestCaseMetadata
    {

        public TestCaseMetadata(XElement xtestCase)
        {
            TestCase = (TestCaseEntity)Model.Instance.EntityBuilder.CreateEntityAndBindToElement(xtestCase);
            TestArgs = TestCase.GetTestArgs();
        }

        public TestCaseEntity TestCase { get; private set; }

        public TestArgs TestArgs { get; private set; }

        public string TestTypeName { get { return TestCase.TestTypeName; } }

    }
}
