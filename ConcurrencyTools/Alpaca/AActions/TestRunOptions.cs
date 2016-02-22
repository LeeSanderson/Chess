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
    class TestRunOptions : IRunTestOptions
    {

        protected AActionContext Context { get; private set; }

        public void BindToContext(AActionContext context)
        {
            Context = context;
            OnBindToContext();
        }

        protected virtual void OnBindToContext() { }

        public virtual bool AcceptsModifier(AActionModifier modifier)
        {
            return false;
        }

        public virtual void ApplyModifier(AActionModifier modifier)
        {
        }

        protected virtual void VisitTask(RunMCutTestCaseAppTask runTestTask) { }
        protected virtual void VisitTask(RunMChessTestTask runTestTask) { }
        protected virtual void VisitTask(RunMChessBasedTestTask runTestTask) { }

        #region IRunTestOptions Members

        void IRunTestOptions.VisitTask(RunMCutTestCaseAppTask runTestTask) { VisitTask(runTestTask); }
        void IRunTestOptions.VisitTask(RunMChessTestTask runTestTask) { VisitTask(runTestTask); }
        void IRunTestOptions.VisitTask(RunMChessBasedTestTask runTestTask) { VisitTask(runTestTask); }

        #endregion
    }
}
