using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Execution.AppTasks;

namespace Microsoft.Concurrency.TestTools.Alpaca.AActions
{
    /// <summary>
    /// Minimum implementation details for integration of test run options
    /// into the Actions framework.
    /// </summary>
    class RunMCutTestOptions : TestRunOptions
    {

        public bool RunInteractively { get; set; }
        public bool EnableRegressionTestAsserts { get; set; }
        public bool UseGoldenObservationFile { get; set; }

        protected override void OnBindToContext()
        {
            base.OnBindToContext();
        }

        #region VisitTask()

        protected override void VisitTask(RunMCutTestCaseAppTask runTestTask)
        {
            base.VisitTask(runTestTask);
            DoVisitTask(runTestTask);
        }

        protected override void VisitTask(RunMChessTestTask runTestTask)
        {
            base.VisitTask(runTestTask);
            DoVisitTask(runTestTask);
            //runTestTask.UseGoldenObservationFile = UseGoldenObservationFile;
        }

        protected override void VisitTask(RunMChessBasedTestTask runTestTask)
        {
            base.VisitTask(runTestTask);
            DoVisitTask(runTestTask);
            runTestTask.UseGoldenObservationFile = UseGoldenObservationFile;
        }

        private void DoVisitTask(RunMCutTestCaseAppTask runTestTask)
        {
            runTestTask.RunInteractively = RunInteractively;
            runTestTask.EnableRegressionTestAsserts = EnableRegressionTestAsserts;
        }

        #endregion

    }
}
