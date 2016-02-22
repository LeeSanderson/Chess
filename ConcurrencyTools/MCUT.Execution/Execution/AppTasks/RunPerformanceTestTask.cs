using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using System.IO;
using System.Diagnostics;
using Microsoft.Concurrency.TestTools.Execution.Chess;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    public class RunPerformanceTestTask : RunMCutTestCaseAppTask
    {

        public RunPerformanceTestTask(PerformanceTestEntity test)
            : base(test)
        {
        }

        public RunPerformanceTestTask(MCutTestCaseRunEntity run) : base(run) { }

        protected override string TestTypeName { get { return TestTypeNames.PerformanceTest; } }

        new public PerformanceTestEntity Test { get { return (PerformanceTestEntity)base.Test; } }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (!RunInteractively)
                throw new TestExecutionException("Performance tests may only be run interactively.", true);
        }

        protected override XElement CreateXTestCase()
        {
            XElement xtestCase = base.CreateXTestCase();

            xtestCase.Add(new XElement(XNames.TaskoMeter
                , new XAttribute("WarmupRepetitions", Test.WarmupRepetitions)
                , new XAttribute("Repetitions", Test.Repetitions)
                ));

            return xtestCase;
        }

        protected override void AcceptRunOptions(IRunTestOptions runOptions)
        {
            runOptions.VisitTask(this);
        }

    }
}
