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
    public class RunMChessTestTask : RunMCutTestCaseAppTask, IHasMChessOptions
    {

        public RunMChessTestTask(MChessTestEntity test)
            : base(test)
        {
        }

        public RunMChessTestTask(MCutTestCaseRunEntity run) : base(run) { }

        protected override string TestTypeName { get { return TestTypeNames.MChess; } }

        new public MChessTestEntity Test { get { return (MChessTestEntity)base.Test; } }

        /// <summary>
        /// Gets or sets the options to save in the test case for this test.
        /// The initial value (before Setup) will override any settings set from
        /// a previous run or the defaults for a test.
        /// </summary>
        public MChessOptions MChessOptions { get; set; }

        protected override void OnPerformSetup()
        {
            SetupMChessOptions();
            base.OnPerformSetup();
        }

        private void SetupMChessOptions()
        {
            // Start with the options from a previous run OR just use the defaults
            MChessOptions opts;
            if (PreviousRun == null)
                opts = CreateBaseMChessOptions();
            else
                opts = new MChessOptions(PreviousRun.TestCase.XMChessOptions);

            // Merge in the new options explicitly specified for this test (previously known via the xprototype)
            opts.MergeWith(MChessOptions);

            MChessOptions = opts;
        }

        private MChessOptions CreateBaseMChessOptions()
        {
            var opts = new MChessOptions();

            // Add the options derived from the test method hierarchy. (these aren't specified by the context)
            opts.IncludedAssemblies = ChessTestUtil.GetAssembliesToInstrument(Test.OwningTestMethod).ToArray();
            ChessTestUtil.SetPreemptionOptions(opts, Test.OwningTestMethod);

            // ** Add options that come from the context
            var context = TestContext as IMChessTestContext;
            if (context != null)
                opts.MergeWith(context.ToMChessOptions());

            return opts;
        }

        protected override XElement CreateXTestCase()
        {
            XElement xtestCase = base.CreateXTestCase();

            // Add stuff from the context (that's not located in the MChessOptions)
            var context = (ChessTestContextEntity)TestContext;
            if (context != null)
            {
                // Add preRunScript
                if (!String.IsNullOrWhiteSpace(context.PreRunScript))
                    xtestCase.Add(new XElement(XNames.MChessPreRunScript, context.PreRunScript));

                // ** Add expected MChess result element
                string expChessResultKey = context.ExpectedChessResultKey;
                var expChessResult = Test.GetExpectedChessResult(expChessResultKey);
                if (expChessResult != null)
                    xtestCase.Add(new XElement(expChessResult.DataElement));
            }

            // Add the MChessOptions
            if (MChessOptions != null)
            {
                var xopts = MChessOptions.ToXElement();
                if (xopts.HasAttributes || xopts.HasElements)
                    xtestCase.Add(xopts);
            }

            return xtestCase;
        }

        protected override void UpdateXTestCaseFromPreviousRun(XElement xtestCase)
        {
            base.UpdateXTestCaseFromPreviousRun(xtestCase);

            // The MChessOptions has already been merged from the prev run in SetupMChessOptions()
            // Just remove the old options element so it can be replaced with the new settings
            var oldxopts = xtestCase.Element(XChessNames.MChessOptions);
            if (oldxopts != null)
                oldxopts.Remove();

            // Add the new settings
            if (MChessOptions != null)
            {
                var xopts = MChessOptions.ToXElement();
                if (xopts.HasAttributes || xopts.HasElements)
                    xtestCase.Add(xopts);
            }
        }

        protected override void AcceptRunOptions(IRunTestOptions runOptions)
        {
            runOptions.VisitTask(this);
        }

    }
}
