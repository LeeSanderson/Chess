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
    class PerformanceTestController : TestTypeController
    {

        public PerformanceTestController() : base(typeof(PerformanceTestEntity)) { }

        internal override IEnumerable<AAction> CreateTestActions(AActionContext context)
        {
            yield return new ARunAllMCutTestCasesAction("Run Performance Test Interactively");
            yield return new ARerunTestAction("Repeat Test");
        }

    }
}
