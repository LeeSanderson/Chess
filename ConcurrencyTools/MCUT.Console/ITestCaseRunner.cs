using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Execution;

namespace Microsoft.Concurrency.TestTools.UnitTesting.MCutConsole
{
    /// <summary>
    /// Runs a test case given an xml description of the test case.
    /// </summary>
    public interface ITestCaseRunner
    {

        /// <summary>
        /// Runs the test and returns the result xml.
        /// </summary>
        /// <param name="metadata">The metadata for the test case to run.</param>
        /// <returns>The testResult xml element representing the result of running the test.</returns>
        XElement RunTestCase(TestCaseMetadata metadata);

        /// <summary>
        /// Performs any additional processing once the initial results have been received.
        /// </summary>
        /// <param name="metadata">The metadata for the test case that was run.</param>
        /// <param name="testResult">The result from the test run.</param>
        /// <returns></returns>
        TestResultEntity PreProcessResults(TestCaseMetadata metadata, TestResultEntity testResult);


    }
}
