using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution;

namespace Microsoft.Concurrency.TestTools.UnitTesting.MCutConsole
{
    /// <summary>
    /// Engine for running test cases using runners.
    /// </summary>
    class TestCaseRunnerEngine
    {

        TestCaseMetadata metadata = null;
        TestTypeControllerBase testController = null;
        ITestCaseRunner runner = null;

        public string Error { get; private set; }
        public Exception ErrorEx { get; private set; }

        /// <summary>
        /// Reads the test case xml from the file. If not specified, reads the xml from Console.In.
        /// </summary>
        /// <param name="testCaseFilePath"></param>
        /// <returns></returns>
        internal bool LoadTestCase(string testCaseFilePath)
        {
            // Read the test case xml
            XDocument testCaseXDoc = null;
            if (!String.IsNullOrEmpty(testCaseFilePath))
            {
                try
                {
                    testCaseXDoc = XDocument.Load(testCaseFilePath);
                }
                catch (Exception ex)
                {
                    Error = "Could not load test case description file.";
                    ErrorEx = ex;
                    return false;
                }
            }
            else
            {
                try
                {
                    testCaseXDoc = XDocument.Load(Console.In);
                }
                catch (Exception ex)
                {
                    Error = "Could not load test case description from the standard input stream.";
                    ErrorEx = ex;
                    return false;
                }
            }

            //Validate the xml
            try
            {
                UnitTestingSchemaUtil.ValidateTestCaseXml(testCaseXDoc);
            }
            catch (Exception ex)
            {
                Error = "Invalid test case xml: " + ex.Message;
                return false;
            }

            // Create the RunningTestCase instance
            try
            {
                metadata = new TestCaseMetadata(testCaseXDoc.Root);
                testController = CreateTestController(metadata.TestTypeName);
                if (testController == null)
                    return false;

                runner = testController.CreateRunner();
                if (runner == null)
                {
                    Error = testController.GetType().Name + ".CreateRunner didn't return a runner instance.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Error = "Error with test case.";
                ErrorEx = ex;
                return false;
            }

            return Error == null;
        }

        private TestTypeControllerBase CreateTestController(string testTypeName)
        {
            switch (testTypeName)
            {
                case TestTypeNames.UnitTest:
                    return new UnitTestController();

                case TestTypeNames.MChess:
                    return new MChessTestController();

                case TestTypeNames.PerformanceTest:
                    return new PerformanceTestController();

                default:
                    Error = "Unrecognized test type: " + testTypeName;
                    return null;
            }
        }


        internal XElement RunTestCase()
        {
            // Run the test case
            try
            {
                return runner.RunTestCase(metadata);
            }
            catch (Exception ex)
            {
                return TestResultUtil.CreateErrorXTestResult("Error running test case.", ex);
            }
        }

        internal TestResultEntity PreProcessResults(TestResultEntity testResult)
        {
            return runner.PreProcessResults(metadata, testResult);
        }

        internal TestResultEntity ProcessExpectedResultAssertions(TestResultEntity testResult)
        {
            ExpectedTestResultEntity expResult = metadata.TestCase.ExpectedTestResult;
            if (expResult != null)
            {
                XElement xtestResults = testResult.DataElement;

                try
                {
                    // First, do the common result assertions
                    // If the expResult is specified, the ResultType is required by the xml schema
                    Assert.AreEqual(expResult.ResultType, testResult.ResultType, "Result.ResultType");
                    if (expResult.Message != null)
                        Assert.AreEqual(expResult.Message, testResult.Message, "Result.Message");

                    // If we got here, than all the assertions passed
                    xtestResults.SetAttributeValue(XTestResultNames.ATestResultType, TestResultType.Passed);
                    xtestResults.SetElementValue(XTestResultNames.ResultMessage, "Passed expected result assertions.");
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
                    xtestResults.SetElementValue(XTestResultNames.ResultMessage, "Expected Result Assertion: " + ex.Message);
                }
            }

            return testResult;
        }

        internal TestResultEntity ProcessRegressionTestAsserts(TestResultEntity testResult)
        {
            // See if there's any regression test info defined
            var expResult = metadata.TestCase.ExpectedRegressionTestResult;
            if (expResult != null)
            {
                XElement xtestResults = testResult.DataElement;

                TestResultType[] expResultTypes = expResult.ResultTypes;
                if (expResultTypes != null && expResultTypes.Length != 0)
                {
                    // Need to hold the temporary value because the xtestResults.SetAttributeValue
                    // will modify this and the error message won't make sense.
                    var resultType = testResult.ResultType;
                    if (expResultTypes.Contains(resultType))
                    {
                        xtestResults.SetAttributeValue(XTestResultNames.ATestResultType, TestResultType.Passed);
                        xtestResults.SetElementValue(XTestResultNames.ResultMessage, String.Format("Expected regression result type assertion passed."));
                    }
                    else
                    {
                        xtestResults.SetAttributeValue(XTestResultNames.ATestResultType, TestResultType.RegressionAssertFailure);
                        xtestResults.SetElementValue(XTestResultNames.ResultMessage, String.Format("Expected result type of any of the following [{0}] but got [{1}]."
                            , String.Join(", ", expResult.ResultTypes), resultType));
                    }
                }
            }

            return testResult;
        }

    }
}
