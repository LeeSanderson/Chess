using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Chess;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution.AppTasks;

namespace Microsoft.Concurrency.TestTools.UnitTesting.MCutConsole
{
    /// <summary>
    /// Runs a test case given an xml description of the test case.
    /// </summary>
    class MChessTestCaseRunner : ITestCaseRunner
    {

        public MChessTestCaseRunner(MChessTestController controller)
        {
            Controller = controller;
        }

        public MChessTestController Controller { get; private set; }

        /// <summary>
        /// Runs the test and returns the result xml.
        /// </summary>
        /// <returns></returns>
        public XElement RunTestCase(TestCaseMetadata metadata)
        {
            using (ExecuteMChessTask task = new ExecuteMChessTask())
            {
                task.TestCase = metadata.TestCase;
                AppTaskController.ExecuteTaskInline(task);

                if (task.Status == AppTaskStatus.Complete)
                    return task.XTestResult;
                else if (task.Status == AppTaskStatus.Error)
                    return TestResultUtil.CreateErrorXTestResult(task.XError);
                else
                    return TestResultUtil.CreateErrorXTestResult("The ExecuteMChessTask completed without producing results.");
            }
        }

        public TestResultEntity PreProcessResults(TestCaseMetadata metadata, TestResultEntity testResult)
        {
            var expChessResult = metadata.TestCase.EntityOfType<ExpectedChessTestResultEntity>();
            if (expChessResult != null)
            {
                XElement xtestResults = testResult.DataElement;

                try
                {
                    if (expChessResult.ExitCode.HasValue)
                        Assert.AreEqual(expChessResult.ExitCode, testResult.ChessExitCode, "Expected Result: ChessExitCode");

                    XElement xfinalStats = testResult.DataElement
                        .Elements(XChessNames.ChessResults)
                        .Elements(XChessNames.FinalStats)
                        .SingleOrDefault();
                    AssertFinalStatValue(xfinalStats, expChessResult.SchedulesRan, XChessNames.ASchedulesRan, "Schedules Ran");
                    AssertFinalStatValue(xfinalStats, expChessResult.LastThreadCount, XChessNames.ALastThreadCount, "Last Thread Count");
                    AssertFinalStatValue(xfinalStats, expChessResult.LastExecSteps, XChessNames.ALastExecSteps, "LastExecSteps");
                    AssertFinalStatValue(xfinalStats, expChessResult.LastHBExecSteps, XChessNames.ALastHBExecSteps, "Last HB Exec Steps");

                    // If we got here, than all the assertions passed
                    xtestResults.SetAttributeValue(XTestResultNames.ATestResultType, TestResultType.Passed);
                    xtestResults.SetElementValue(XTestResultNames.ResultMessage, "Passed expected MChess test result assertions.");
                }
                catch (AssertFailedException ex)
                {
                    xtestResults.SetAttributeValue(XTestResultNames.ATestResultType, TestResultType.ResultAssertFailure);
                    xtestResults.SetElementValue(XTestResultNames.ResultMessage, ex.Message);
                }
                catch (AssertInconclusiveException ex)
                {
                    xtestResults.SetAttributeValue(XTestResultNames.ATestResultType, TestResultType.ResultInconclusive);
                    xtestResults.SetElementValue(XTestResultNames.ResultMessage, ex.Message);
                }
                catch (Exception ex)
                {
                    xtestResults.SetAttributeValue(XTestResultNames.ATestResultType, TestResultType.Error);
                    xtestResults.SetElementValue(XTestResultNames.ResultMessage, "Expected Chess Result Assertion: " + ex.Message);
                }
            }

            return testResult;
        }

        private void AssertFinalStatValue(XElement xfinalStats, int? expValue, XName xstatName, string statName)
        {
            if (expValue.HasValue)
            {
                Assert.IsNotNull(xfinalStats, "Final chess stats missing from results xml file.");
                Assert.AreEqual(expValue, (int?)xfinalStats.Attribute(xstatName), "Final chess statistic '{0}' is not incorrect.", statName);
            }
        }

    }
}
