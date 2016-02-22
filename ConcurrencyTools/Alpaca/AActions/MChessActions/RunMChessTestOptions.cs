using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Chess;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Alpaca.AActions;
using Microsoft.Concurrency.TestTools.Alpaca.AActions.MChessActions;
using Microsoft.Concurrency.TestTools.Execution.AppTasks;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    class MChessTestRunOptions : RunMCutTestOptions
    {

        public MChessOptions MChessOptions { get; set; }

        protected override void OnBindToContext()
        {
            base.OnBindToContext();

            var prevRun = Context.MCutTestRun;
            if (prevRun != null)
            {
                // Incorporate the prev run's options in, while being overridden by the current options
                MChessOptions opts = new MChessOptions(prevRun.TestCase.XMChessOptions);
                opts.MergeWith(MChessOptions);
                MChessOptions = opts;
            }
        }

        public override bool AcceptsModifier(AActionModifier modifier)
        {
            return modifier is AMChessOptionsModifier;
        }

        public override void ApplyModifier(AActionModifier modifier)
        {
            base.ApplyModifier(modifier);

            if (modifier is AMChessOptionsModifier)
            {
                var opts = MChessOptions ?? new MChessOptions();
                opts.MergeWith(((AMChessOptionsModifier)modifier).MChessOptions);
                MChessOptions = opts;
            }
        }

        #region VisitTask()

        protected override void VisitTask(RunMChessTestTask runTestTask)
        {
            base.VisitTask(runTestTask);
            DoVisitTask(runTestTask);
        }

        protected override void VisitTask(RunMChessBasedTestTask runTestTask)
        {
            base.VisitTask(runTestTask);
            DoVisitTask(runTestTask);
        }

        private void DoVisitTask<TTask>(TTask runTestTask)
            where TTask : RunMCutTestCaseAppTask, IHasMChessOptions
        {
            if (this.MChessOptions != null)
            {
                var opt = runTestTask.MChessOptions ?? new MChessOptions();
                opt.MergeWith(this.MChessOptions);
                runTestTask.MChessOptions = opt;  // Reassign just incase it was originally null
            }
        }

        #endregion

    }

}
