using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Chess;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Alpaca.AActions;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    class UnitTestController : TestTypeController
    {

        public UnitTestController() : base(typeof(UnitTestEntity)) { }

        internal override IEnumerable<AAction> CreateTestActions(AActionContext context)
        {
            yield return new ARunAllMCutTestCasesAction("Run Unit Test");
            yield return new ARerunTestAction("Repeat Test");
        }

    }
}
