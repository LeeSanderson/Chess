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
    public class PerformanceTestController : UnitTestController
    {

        public override ITestCaseRunner CreateRunner()
        {
            return new PerformanceTestCaseRunner(this);
        }

        protected override ITestContext CreateTestContext(TestCaseMetadata runningTestCase)
        {
            var ctx = new TaskoMeterTestContext(runningTestCase.TestCase.ContextName);

            TaskoMeterEntity taskometer = runningTestCase.TestCase.EntityOfType<TaskoMeterEntity>();
            if (taskometer != null)
            {
                ctx.WarmupRepetitions = taskometer.WarmupRepetitions;
                ctx.Repetitions = taskometer.Repetitions;
            }

            return ctx;
        }

    }
}
