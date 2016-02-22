using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using System.Xml.Linq;
using System.Reflection;
using Microsoft.Concurrency.TestTools.Execution;

namespace Microsoft.Concurrency.TestTools.UnitTesting.MCutConsole
{
    public abstract class TestTypeControllerBase
    {

        public abstract ITestCaseRunner CreateRunner();

    }
}
