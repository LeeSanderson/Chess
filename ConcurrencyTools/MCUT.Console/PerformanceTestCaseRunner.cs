using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.TaskoMeter;

namespace Microsoft.Concurrency.TestTools.UnitTesting.MCutConsole
{
    /// <summary>
    /// Runs a test case given an xml description of the test case.
    /// </summary>
    class PerformanceTestCaseRunner : UnitTestCaseRunner
    {

        public PerformanceTestCaseRunner(PerformanceTestController controller)
            : base(controller)
        {
        }

        protected override void Invoke(ManagedTestCase testCase, object testobject, MethodInfo method, object[] args, string name)
        {
            TaskoMeterTestContext ctx = (TaskoMeterTestContext)testCase.Context;

            Action invoke = delegate() { method.Invoke(testobject, args); };

            var t = Metering.MeasureInteractively(
                invoke,
                ctx.Repetitions,
                invoke,
                ctx.WarmupRepetitions,
                name);
            t.Join();
        }

    }
}
