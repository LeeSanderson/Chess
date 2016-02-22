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
using Microsoft.Concurrency.TestTools.UnitTesting;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    public class RunMChessBasedTestTask : RunMCutTestCaseAppTask, IHasMChessOptions, IUsesInputObservationFile
    {

        public RunMChessBasedTestTask(MChessBasedTestEntity test)
            : base(test)
        {
        }

        public RunMChessBasedTestTask(MCutTestCaseRunEntity run)
            : base(run)
        {
            UseGoldenObservationFile = (bool?)run.DataElement.Attribute(XSessionNames.AUseGoldenObservationFile) ?? false;
        }

        protected override string TestTypeName { get { return TestTypeNames.MChess; } }

        new public MChessBasedTestEntity Test { get { return (MChessBasedTestEntity)base.Test; } }

        /// <summary>
        /// Gets or sets the options to save in the test case for this test.
        /// The initial value (before Setup) will override any settings set from
        /// a previous run or the defaults for a test.
        /// </summary>
        public MChessOptions MChessOptions { get; set; }

        public bool UseGoldenObservationFile { get; set; }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (Test is DeterminismTestEntity && !RunInteractively)
                throw new TestExecutionException("Determinism tests aren't implemented in mchess yet.", true);
        }

        protected override void OnPerformSetup()
        {
            SetupMChessOptions();
            base.OnPerformSetup();

            if (Test is ObservationTestEntity)
            {
                var mcopts = MChessOptions;

                // Get the obs file's expected name for the current test case
                Debug.Assert(!String.IsNullOrWhiteSpace(mcopts.ObservationFile));
                var optsFilename = Path.GetFileName(mcopts.ObservationFile);

                // Make sure the observation files input folder exists
                var testAssy = Test.OwningTestMethod.OwningClass.OwningAssembly;
                string inputObsFldrPath = testAssy.ObservationFilesFullFolderPath;
                if (UseGoldenObservationFile)
                {
                    // The GoldenObservationFilesFolderPath could be declared at the assembly or project level
                    if (!String.IsNullOrEmpty(testAssy.GoldenObservationFilesFolderPath))
                        inputObsFldrPath = testAssy.GoldenObservationFilesFullFolderPath;
                    else if (testAssy.OwningProject != null && !String.IsNullOrEmpty(testAssy.OwningProject.GoldenObservationFilesFolderPath))
                        inputObsFldrPath = testAssy.OwningProject.GoldenObservationFilesFullFolderPath;
                    // else, no golden obs files setup for the assembly, so just use the temp ones.
                }
                else if (String.IsNullOrEmpty(inputObsFldrPath))
                    throw new InvalidOperationException("The owning test assembly doesn't have an observation folder specified for the current session.");

                // If the file doesn't exist, then there'll be a test result failure, rather than a task error
                string srcFilePath = Path.Combine(inputObsFldrPath, optsFilename);
                if (File.Exists(srcFilePath))
                    File.Copy(srcFilePath, Path.Combine(TaskFolderPath, optsFilename), true);
            }
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

            // Add the options that make the MChessBasedTestEntity unique
            Test.SetBaseMChessOptionsForTestExecution(this, opts);

            // ** Add options that come from the context
            var context = TestContext as IMChessTestContext;
            if (context != null)
                opts.MergeWith(context.ToMChessOptions());

            return opts;
        }

        protected override XElement CreateXRun()
        {
            var xrun = base.CreateXRun();

            xrun.Add(new XAttribute(XSessionNames.AUseGoldenObservationFile, UseGoldenObservationFile));

            return xrun;
        }

        protected override XElement CreateXTestCase()
        {
            XElement xtestCase = base.CreateXTestCase();

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

        protected override void OnCompleting()
        {
            base.OnCompleting();

            if (!Run.HasResult || Run.Result.ResultType != TestResultType.Passed)
                return;

            if (Test is ObservationGeneratorEntity)
            {
                try
                {
                    var mcopts = new MChessOptions(Run.TestCase.XMChessOptions);

                    Debug.Assert(!String.IsNullOrWhiteSpace(mcopts.ObservationFile));
                    var optsFilename = Path.GetFileName(mcopts.ObservationFile);
                    string optsFilePath = Path.Combine(TaskFolderPath, optsFilename);

                    // Make sure the assy's folder exists
                    string assyFldrPath = Test.OwningTestMethod.OwningClass.OwningAssembly.ObservationFilesFullFolderPath;
                    if (String.IsNullOrEmpty(assyFldrPath))
                        throw new InvalidOperationException("The owning test assembly doesn't have an observation folder specified for the current session.");
                    Directory.CreateDirectory(assyFldrPath); // If already exists, noop

                    // Copy over to assy folder path
                    File.Copy(optsFilePath, Path.Combine(assyFldrPath, optsFilename), true);
                }
                catch (Exception ex)
                {
                    // JAM (1/10/2011): I believe the UI may not see this change because it would be after the model would have notified
                    // the UI that the entity tree had changed.
                    var errResult = TestResultUtil.CreateErrorXTestResult("Error copying observation file from task folder to the observation folder for the test assembly.", ex);
                    Run.Result.DataElement.ReplaceWith(errResult);
                }
            }
        }

        protected override void AcceptRunOptions(IRunTestOptions runOptions)
        {
            runOptions.VisitTask(this);
        }

    }
}
