using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using System.Xml.Linq;
using System.Reflection;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Chess;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using System.IO;

namespace Microsoft.Concurrency.TestTools.UnitTesting.MCutConsole
{
    public class MChessTestController : TestTypeControllerBase
    {

        public override ITestCaseRunner CreateRunner()
        {
            return new MChessTestCaseRunner(this);
        }

    }
}
