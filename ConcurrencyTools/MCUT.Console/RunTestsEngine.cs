using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.AppTasks;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using System.Xml;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.UnitTesting.MCutConsole
{
    public class RunTestCaseInfo
    {
        public MCutTestEntity Test;
        public int TestIdx;
        public TestContextEntityBase Context;
        public TestArgs Args;
        public RunMCutTestCaseAppTask RunTestTask;

        //
        public TestResultType? ResultType;
        public string ResultMessage;
    }

    /// <summary>
    /// Engine for running a series of tests using a custom tests runner.
    /// </summary>
    class RunTestsEngine
    {

        private const ConsoleColor ErrorColor = ConsoleColor.Red;
        private const ConsoleColor InconclusiveColor = ConsoleColor.Yellow;

        private string _containerFilePath;
        TestGroupingEntity _testsContainerEntity;
        ConsoleColor _normalConsoleColor;

        private IAppTaskController _taskController;
        private Dictionary<AppTask, RunTestCaseInfo> _taskToInfo = new Dictionary<AppTask, RunTestCaseInfo>();
        private List<RunTestCaseInfo> _testsNotPassed = new List<RunTestCaseInfo>();

        public Regex ClassNameFilter { get; set; }
        public Regex TestMethodNameFilter { get; set; }
        public Regex TestTypeNameFilter { get; set; }

        public int MaxConcurrentTests
        {
            get { return _taskController.MaxConcurrentTasks; }
            set { _taskController.MaxConcurrentTasks = value; }
        }
        public bool UseGoldenObservationFiles { get; set; }

        public int TotalTestsCount { get; private set; }
        public int TotalTestCasesCount { get; private set; }

        public int PassedCount { get; private set; }
        public int InconclusiveCount { get; private set; }
        public int FailedCount { get; private set; }

        public string ResultsFilePath { get; set; }

        public RunTestsEngine(string containerFilePath)
        {
            _containerFilePath = containerFilePath;
            _testsContainerEntity = LoadTestContainer(_containerFilePath);

            // Need to setup any test assemblies
            foreach (var testAssy in _testsContainerEntity.DescendantsAndSelf<TestAssemblyEntity>())
                Model.Instance.Session.RegisterTestAssembly(testAssy);

            _taskController = new AppTaskController(Model.Instance, Model.Instance.Session.BaseTasksFolderPath) {
                MaxConcurrentTasks = 1,
            };
            _taskController.TaskStarted += new AppTaskEventHandler(TaskController_TaskStarted);
            _taskController.TaskCompleted += new AppTaskEventHandler(TaskController_TaskCompleted);

            _normalConsoleColor = Console.ForegroundColor;
        }

        private static TestGroupingEntity LoadTestContainer(string containerPath)
        {
            TestContainerLoader loader = new TestContainerLoader(containerPath) {
                Model = Model.Instance
            };

            if (!loader.Load())
                throw new Exception(loader.LoadError.Message);

            // TODO: If the loader failed, then the TestContainer will be an error entity, not a test group
            return (TestGroupingEntity)loader.TestContainer;
        }

        private bool PassesTestFilters(MCutTestEntity t)
        {
            if (TestTypeNameFilter != null && !TestTypeNameFilter.IsMatch(t.GetType().Name))
                return false;
            if (TestMethodNameFilter != null && !TestMethodNameFilter.IsMatch(t.OwningTestMethod.MethodName))
                return false;
            if (ClassNameFilter != null && !ClassNameFilter.IsMatch(t.OwningTestMethod.OwningClass.ClassFullName))
                return false;
            return true;
        }

        public void RunContainer()
        {
            // Get the non-observation generator tests
            var tests = _testsContainerEntity.TestsAndSelf()
                .OfType<MCutTestEntity>() // We only know how to spawn runs for mcut tests
                .Where(PassesTestFilters)
                .ToArray();
            TotalTestsCount = tests.Length;

            var runTestTasks = GetRunTestTaskInfos(tests).ToArray();
            TotalTestCasesCount = runTestTasks.Length;

            if (TotalTestCasesCount == 0)
                Console.Error.WriteLine("No tests found in test container. Be sure to build any assemblies first.");
            else
            {
                Console.WriteLine("Running {0} tests with {1} test cases...", TotalTestsCount, TotalTestCasesCount);
                foreach (var runInfo in runTestTasks)
                {
                    // Create the task
                    var task = runInfo.Test.CreateRunTestTask(MCutTestRunType.RunTest);
                    task.TestContext = runInfo.Context;
                    task.TestArgs = runInfo.Args;
                    task.ParentEntity = runInfo.Test; // Since we aren't pulling the tests from session, we'll just save to our temp output file
                    task.EnableRegressionTestAsserts = true;    // When running on the console, we enable this for all tests

                    var reqObsFile = task as IUsesInputObservationFile;
                    if (reqObsFile != null)
                        reqObsFile.UseGoldenObservationFile = UseGoldenObservationFiles;

                    // Add the task to be executed
                    runInfo.RunTestTask = task;
                    _taskToInfo.Add(task, runInfo);
                    _taskController.AddTask(task);

                    // Since we may have a lot of tasks, lets keep them going as soon as possible
                    _taskController.ProcessTasks();
                }

                SaveResultsFile();
                _taskController.WaitForAllTasksToComplete();
            }

            // Save the final state of the results file
            SaveResultsFile();
            ReportFinalStatistics();
        }

        private IEnumerable<RunTestCaseInfo> GetRunTestTaskInfos(IEnumerable<MCutTestEntity> tests)
        {
            return tests
                .SelectMany((test, tidx) => {
                    // Get the contexts
                    var contexts = test.GetContexts().ToArray();
                    // This isn't needed yet because the GetContexts method should always return at least the default context instance
                    //if (contexts.Length == 0)
                    //    contexts = new TestContextEntityBase[] { null };

                    // Get the argument sets
                    TestArgs[] argsSets = test.GetTestArgsSets().ToArray();
                    // Need to make sure there is at least one arg instance for tests that don't have parameters
                    if (argsSets.Length == 0 && !test.TestSource.HasParameters)
                        argsSets = new TestArgs[] { null };

                    // Get the cross product of contexts with arg sets
                    return from ctx in contexts
                           from args in argsSets
                           select new RunTestCaseInfo {
                               Test = test,
                               TestIdx = tidx,
                               Context = ctx,
                               Args = args
                           };

                });
        }

        void TaskController_TaskStarted(AppTask task, EventArgs e)
        {
            var info = _taskToInfo[task];
            Console.WriteLine("   {0}> Running test case {1}.", info.RunTestTask.ID, info.RunTestTask.Run.TestCase.DisplayName);
        }

        void TaskController_TaskCompleted(AppTask task, EventArgs e)
        {
            var info = _taskToInfo[task];
            TestResultType resultType;

            // Get the result type
            if (task.Status == AppTaskStatus.Error)
            {
                Debug.Assert(task.ErrorEx != null, "The ErrorEx should still be available because we haven't re-constituted the task from xml, in which case we wouldn't have the ErrorEx.");
                var testExecEx = task.ErrorEx as TestExecutionException;
                if (testExecEx != null)
                    resultType = testExecEx.ResultInconclusive ? TestResultType.Inconclusive : TestResultType.Error;
                else
                    resultType = TestResultType.Error;

                info.ResultMessage = "Task execution failed: " + task.ErrorEx.Message;
            }
            else
            {
                resultType = info.RunTestTask.Run.Result.ResultType;
                info.ResultMessage = info.RunTestTask.Run.Result.Message;
            }
            info.ResultType = resultType;

            // Report the result type only
            Console.WriteLine("   {0}> Test result: {1}", task.ID, resultType);
            if (resultType == TestResultType.Passed)
                PassedCount++;
            else
            {
                _testsNotPassed.Add(info);

                if (resultType == TestResultType.Inconclusive || resultType == TestResultType.ResultInconclusive)
                    InconclusiveCount++;
                else
                    FailedCount++;

                // Report to console
                ReportTestCaseResult(info);
            }

            SaveResultsFile();
        }

        private void SaveResultsFile()
        {
            try
            {
                using (XmlWriter xwriter = XmlWriter.Create(ResultsFilePath, new XmlWriterSettings() { Indent = true }))
                {
                    _testsContainerEntity.DataElement.WriteTo(xwriter);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error saving results file: " + ex.Message);
            }
        }

        public void ReportFinalStatistics()
        {
            if (_testsNotPassed.Count != 0)
            {
                // Write out the failures
                Console.WriteLine();
                Console.WriteLine("Tests not passed:");
                foreach (var testInfo in _testsNotPassed)
                    ReportTestCaseResult(testInfo);
            }

            Console.WriteLine();
            Console.WriteLine("Test Cases Run: {0}", TotalTestCasesCount);
            Console.ForegroundColor = PassedCount == 0 ? _normalConsoleColor : ResultTypeToConsoleColor(TestResultType.Passed);
            Console.WriteLine("   Passed Count: {0}", PassedCount);
            Console.ForegroundColor = InconclusiveCount == 0 ? _normalConsoleColor : ResultTypeToConsoleColor(TestResultType.Inconclusive);
            Console.WriteLine("   Inconclusive Count: {0}", InconclusiveCount);
            Console.ForegroundColor = FailedCount == 0 ? _normalConsoleColor : ResultTypeToConsoleColor(TestResultType.Error);
            Console.WriteLine("   Failed Count: {0}", FailedCount);
            Console.ForegroundColor = _normalConsoleColor;
            Console.WriteLine();

            Console.WriteLine("Finished running tests.");
            Console.WriteLine();
        }

        private void ReportTestCaseResult(RunTestCaseInfo info)
        {
            Console.ForegroundColor = ResultTypeToConsoleColor(info.ResultType.Value);

            // Write the test identification text
            if (info.RunTestTask.IsSetup)
            {
                var testCase = info.RunTestTask.Run.TestCase;
                Console.Write("  {0}> {1} (Context={2}, TestType={3})"
                    , info.RunTestTask.ID
                    , testCase.DisplayName
                    , testCase.ContextName
                    , testCase.TestTypeName
                    );
            }
            else
            {
                Console.Write("  {0}> {1}(???) (Context={2}, TestType={3})"
                    , info.RunTestTask.ID
                    , info.RunTestTask.Test.OwningTestMethod.MethodName
                    , info.RunTestTask.TestContext.Name
                    , info.RunTestTask.Test.GetType().Name
                    );

            }

            // And then the result information
            Console.WriteLine(": [{0}]{1}", info.ResultType, info.ResultMessage);
            Console.ForegroundColor = _normalConsoleColor;
        }

        private static ConsoleColor ResultTypeToConsoleColor(TestResultType resultType)
        {
            switch (resultType)
            {
                // Success
                case TestResultType.Passed:
                    return ConsoleColor.Green;

                // Inconclusive
                case TestResultType.Inconclusive:
                case TestResultType.ResultInconclusive:
                    return InconclusiveColor;

                // Soft errors
                case TestResultType.DataRace:
                case TestResultType.Deadlock:
                case TestResultType.Livelock:
                    return ConsoleColor.DarkRed;

                // Errors
                case TestResultType.Error:
                case TestResultType.Exception:
                case TestResultType.AssertFailure:
                case TestResultType.ResultAssertFailure:
                case TestResultType.RegressionAssertFailure:
                    return ErrorColor;

                default:
                    throw new NotImplementedException("Result type not implemented: " + resultType);
            }
        }

    }
}
