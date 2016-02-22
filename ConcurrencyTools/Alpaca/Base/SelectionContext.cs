using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Chess;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using System.Xml.Linq;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    /// <summary>
    /// Represents context information about a selected entity.
    /// </summary>
    class SelectionContext
    {

        /// <param name="target">The selected entity.</param>
        public SelectionContext(EntityBase target)
        {
            Target = target;

            FindFullContext();
        }

        public EntityBase Target { get; private set; }
        public Model Model { get { return (Model)Target.Model; } }

        public TestProjectEntity TestProject { get; private set; }
        public TestAssemblyEntity TestAssembly { get; private set; }
        public TaskRunEntity TaskRun { get; private set; }

        public TestEntity Test { get; private set; }
        public TestRunEntity TestRun { get; private set; }
        public MCutTestCaseRunEntity MCutTestRun { get; private set; }
        public TestResultEntity TestResult { get; private set; }

        public ChessResultEntity ChessResult { get; private set; }

        public XElement LastXSchedule { get; private set; }

        private void FindFullContext()
        {
            // First, try setting the properties just off of the Target
            TaskRun = Target as TaskRunEntity;
            TestProject = Target as TestProjectEntity;
            TestAssembly = Target as TestAssemblyEntity;

            if (Target is ChessResultEntity)
            {
                ChessResult = (ChessResultEntity)Target;

                // NOTE: The old actions would only set the TestRun/TestResult contexts if the result is an error/notification, but not for warnings.
                TestResult = ChessResult.OwningTestResult;
                TestRun = ChessResult.OwningTestRun;
                Test = TestRun.OwningTest;
            }
            else if (Target is TestResultEntity)
            {
                TestResult = (TestResultEntity)Target;
                TestRun = TestResult.OwningTestRun;
                Test = TestRun.OwningTest;

                // Find children we know about
                // None
            }
            else if (Target is TestRunEntity)
            {
                TestRun = (TestRunEntity)Target;
                Test = TestRun.OwningTest;

                // Find children we know about
                TestResult = TestRun.Result;
            }
            else if (Target is TestEntity)
            {
                Test = (TestEntity)Target;
            }

            if (TaskRun == null)
                TaskRun = TestRun;
            if (TestProject != null && TestProject.TestAssembly != null)
                TestAssembly = TestProject.TestAssembly;

            LastXSchedule = TestRun == null ? null : TestRun.GetLastXSchedule();
            MCutTestRun = TestRun as MCutTestCaseRunEntity;

            // If there's only one bad chess result, then allow the UI to expose it's functionality
            // w/o the user needing to first select the chess result.
            if (ChessResult == null && TestResult != null)
            {
                var chessResults = Views.EntityViewUtil.GetNonInformationalChessResults(TestResult);
                if (chessResults.Count() == 1)
                    ChessResult = chessResults.First(); ;
            }
        }

    }
}
