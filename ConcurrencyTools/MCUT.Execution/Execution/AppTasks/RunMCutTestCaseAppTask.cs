using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    /// <summary>
    /// Represents a task used to run a test case via mcut on a separate process.
    /// </summary>
    public abstract class RunMCutTestCaseAppTask : AppScriptProcessTask, IRunTestTask, IAcceptsRunTestOptions
    {

        public const string TestCaseFilename = "testCase.xml";
        public const string TestResultFilename = "testResult.xml";

        #region Constructors

        protected RunMCutTestCaseAppTask(MCutTestEntity test)
        {
            InitializeTask();
            Test = test;
        }

        protected RunMCutTestCaseAppTask(MCutTestCaseRunEntity run)
            : base(run.DataElement.Element(XSessionNames.TaskState))
        {
            InitializeTask();

            Test = run.OwningTest;
            RunType = run.RunType;
            Run = run;
            ParentEntity = run.Parent;
            PreviousRun = run.Ancestors().OfType<MCutTestCaseRunEntity>().FirstOrDefault();

            EnableRegressionTestAsserts = (bool?)run.DataElement.Attribute(XSessionNames.AEnableRegressionTestAsserts) ?? false;
            RunInteractively = (bool?)run.DataElement.Attribute(XSessionNames.ARunInteractively) ?? false;

            Debug.Assert(Status >= AppTaskStatus.Setup);
        }

        private void InitializeTask()
        {
            StartScriptFilename = "startMCut.cmd";
            RedirectOutputToFiles = true;
            OutputFilename = "mcut.out";
            ErrorFilename = "mcut.err";
        }

        #endregion

        public override string ExecutableFilePath
        {
            get { return MCutToolsUtil.MCutExecutableFilePath; }
        }

        /// <summary>
        /// When implemented in a derived class, gets the name of the type of test this is.
        /// This name is also used in the test case xml to identify what kind of test mcut should execute.
        /// </summary>
        protected abstract string TestTypeName { get; }

        public MCutTestEntity Test { get; private set; }
        public MCutTestRunType RunType { get; set; }
        public MCutTestCaseRunEntity Run { get; private set; }

        ///// <summary>The type of test run to execute. e.g. regular test run vs a re-run.</summary>
        //public TestRunType RunType { get; set; }

        public EntityBase ParentEntity { get; set; }
        public TestContextEntityBase TestContext { get; set; }
        public TestArgs TestArgs { get; set; }

        // Options
        public MCutTestCaseRunEntity PreviousRun { get; set; }
        public bool EnableRegressionTestAsserts { get; set; }
        public bool RunInteractively { get; set; }

        #region Task Validation & Setup

        protected override void OnValidate()
        {
            base.OnValidate();
            if (Test == null)
                throw new InvalidOperationException("The Test property has not been set yet.");
        }

        protected override void OnPerformSetup()
        {
            base.OnPerformSetup();

            // Create the run entity
            XElement xrun = CreateXRun();
            Run = (MCutTestCaseRunEntity)Controller.Model.EntityBuilder.CreateEntityAndBindToElement(xrun);
            Run.LoadChildren(true);
            // Need to also set our self so the Run has access to it before it adds itself to the xml tree.
            Run.Task = this;

            // Write the test case to the file
            using (XmlWriter xwriter = XmlWriter.Create(Path.Combine(TaskFolderPath, TestCaseFilename)))
                Run.TestCase.DataElement.WriteTo(xwriter);

            // Add the run to the parent entity.
            if (ParentEntity != null)
                ParentEntity.DataElement.Add(Run.DataElement);

            PublishTestSourceFiles();
        }

        public override IEnumerable<string> GetExecutableArguments()
        {
            yield return "runTestCase";
            yield return TestCaseFilename;
            yield return "/resultFile";
            yield return TestResultFilename;

            //// Indicate whether to enable regression test asserts
            //if (EnableRegressionTestAsserts)
            //    yield return "/enableRegressionTestAsserts";
        }

        protected virtual void PublishTestSourceFiles()
        {
            // TODO: copy dlls over to the task folder
        }

        #endregion

        protected override void OnCompleting()
        {
            base.OnCompleting();

            // TODO: Grab the result file and insert the xml into the RunEntity
            var xresult = RetrieveXResult();
            var result = Controller.Model.EntityBuilder.CreateEntityAndBindToElement(xresult);
            Run.DataElement.Add(xresult);
        }

        protected virtual XElement RetrieveXResult()
        {
            string resultFilePath = Path.Combine(TaskFolderPath, TestResultFilename);
            return TestResultUtil.CheckForResultFile(resultFilePath)
                // Since we're completed, if the result file doesn't exist, than something went wrong
                // Here, we're just gonna save it as a result rather than a task execution failure (via an ex).
                ?? TestResultUtil.CreateErrorXTestResult("Result file does not exist.");
        }

        #region Xml Methods

        protected virtual XElement CreateXRun()
        {
            return new XElement(XSessionNames.MCutTestRun
                , new XAttribute(XSessionNames.AMCutTestRunType, RunType)
                , new XAttribute(XSessionNames.ARunInteractively, RunInteractively)
                , new XAttribute(XSessionNames.AEnableRegressionTestAsserts, EnableRegressionTestAsserts)
                , base.XTaskState   // Add on the task's state
                , PreviousRun == null ? CreateXTestCase() : CreateXTestCaseFromPreviousRun()
                );
        }

        private XElement CreateXTestCaseFromPreviousRun()
        {
            // Start with a direct clone of the previous run
            XElement xtestCase = new XElement(PreviousRun.TestCase.DataElement);
            UpdateXTestCaseFromPreviousRun(xtestCase);
            return xtestCase;
        }

        protected virtual void UpdateXTestCaseFromPreviousRun(XElement xtestCase)
        {
        }

        protected virtual XElement CreateXTestCase()
        {
            Debug.Assert(TestContext != null, "Can't create a test case w/o a context. Use a default context instance.");

            // Create the base 
            string testCaseName = UnitTestsUtil.GetUnitTestCaseDisplayName(Test.OwningTestMethod, TestArgs);
            XElement xtestCase = new XElement(XTestCaseNames.TestCase
                , new XAttribute(XTestCaseNames.ATestTypeName, TestTypeName)
                , new XAttribute(XNames.AName, testCaseName)
                , new XAttribute(XTestCaseNames.AContextName, TestContext.Name)
                );

            // Add the test source info
            xtestCase.Add(Test.TestSource.ToXTestCaseSource());

            // Add the test arguments
            if (TestArgs != null)
                xtestCase.Add(new XElement(TestArgs.DataElement));

            // ** Add expected result element
            string expResultKey = TestContext.ExpectedResultKey;
            ExpectedTestResultEntity expResult = Test.OwningTestMethod.GetExpectedResult(expResultKey);
            if (expResult != null)
                xtestCase.Add(new XElement(expResult.DataElement));

            // Add expectedRegressionTestResult
            if (EnableRegressionTestAsserts)
            {
                ExpectedRegressionTestResultEntity expRegressionResult = Test.TestSource.GetExpectedRegressionTestResult();
                if (expRegressionResult != null)
                    xtestCase.Add(new XElement(expRegressionResult.DataElement));
            }

            return xtestCase;
        }

        #endregion

        protected abstract void AcceptRunOptions(IRunTestOptions runOptions);

        #region IAcceptsRunTestOptions Members

        void IAcceptsRunTestOptions.Accept(IRunTestOptions runOptions)
        {
            AcceptRunOptions(runOptions);
        }

        #endregion

        #region ITaskDefinesARun Members

        TaskRunEntity ITaskDefinesARun.Run { get { return this.Run; } }

        #endregion

    }
}
