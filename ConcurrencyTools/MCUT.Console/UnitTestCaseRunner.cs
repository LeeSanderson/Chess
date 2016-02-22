using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution;

namespace Microsoft.Concurrency.TestTools.UnitTesting.MCutConsole
{
    /// <summary>
    /// Runs a test case given an xml description of the test case.
    /// </summary>
    class UnitTestCaseRunner : ITestCaseRunner
    {

        public UnitTestCaseRunner(UnitTestController controller)
        {
            Controller = controller;
        }

        public UnitTestController Controller { get; private set; }

        /// <summary>
        /// Runs the test and returns the result xml.
        /// </summary>
        /// <returns></returns>
        public XElement RunTestCase(TestCaseMetadata metadata)
        {
            ManagedTestCase testCase = null;
            object testObject = null;
            try
            {
                testCase = Controller.CreateManagedTestCase(metadata);
                testObject = testCase.CreateNewTestObject();
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException)
                    ex = ((TargetInvocationException)ex).InnerException;

                throw new Exception("Unit test threw exception while creating test class.", ex);
            }

            // Exec the unit test
            MethodInfo unitTestMethod = testCase.Method;
            object[] unitTestArgs = testCase.Arguments;
            try
            {
                Invoke(testCase, testObject, unitTestMethod, unitTestArgs, testCase.DisplayNameWithArgs);
                if (testCase.ExpectedExceptionType != null)
                {
                    string errMsg = AssertMessagesUtil.FormatAssertionMessage_ExpectedExceptionNotThrown(testCase);
                    return TestResultUtil.CreateXTestResult(TestResultType.AssertFailure, errMsg, null);
                }
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException)
                    ex = ((TargetInvocationException)ex).InnerException;
                Type exType = ex.GetType();

                // Detect expected exceptions and handle Assert exceptions
                if (testCase.ExpectedExceptionType == null || !testCase.ExpectedExceptionType.IsAssignableFrom(exType))
                {
                    // Handle special UnitTest exceptions
                    if (ex is ConcurrencyUnitTestException)
                    {
                        // Pre-defined exceptions can just pass up their messages
                        if (ex is AssertFailedException)
                            return TestResultUtil.CreateXTestResult(TestResultType.AssertFailure, ex.Message, ex);
                        else if (ex is AssertInconclusiveException)
                            return TestResultUtil.CreateXTestResult(TestResultType.Inconclusive, ex.Message, ex);

#if DEBUG
                        // If there's another ex type that we've defined in the framework but haven't handled
                        // by the prev conditions lets warn the developer.
                        Type cutExType = typeof(ConcurrencyUnitTestException);
                        if (exType != cutExType && exType.Assembly.FullName == cutExType.Assembly.FullName)
                            System.Diagnostics.Trace.TraceWarning("Unhandled ConcurrencyUnitTestException derived type in the CUT assembly: " + exType.Name + ". Custom handling should be added here.");
#endif

                        // If not a built in exception, then use the regular handling below.
                    }

                    // If not an assert, then it's an unexpected exception
                    if (testCase.ExpectedExceptionType == null)
                    {
                        string errMsg = AssertMessagesUtil.FormatAssertionMessage_UnExpectedExceptionThrown(testCase, ex);
                        // Write the exception to the trace log so a debugger can see it.
                        System.Diagnostics.Trace.TraceError(errMsg + Environment.NewLine + "Stack Trace: " + ex.StackTrace);
                        return TestResultUtil.CreateXTestResult(TestResultType.Exception, errMsg, ex);
                    }
                    else // Then the exception isn't what's expected
                    {
                        string errMsg = AssertMessagesUtil.FormatAssertionMessage_ExceptionOfWrongTypeThrown(testCase, ex);
                        // Write the exception to the trace log so a debugger can see it.
                        System.Diagnostics.Trace.TraceError(errMsg + Environment.NewLine + "Stack Trace: " + ex.StackTrace);
                        return TestResultUtil.CreateXTestResult(TestResultType.AssertFailure, errMsg, ex);
                    }
                }
                // else - The exception was expected, so the test succedded, keep going
            }

            return TestResultUtil.CreateXTestResult(TestResultType.Passed, null, null);
        }

        protected virtual void Invoke(ManagedTestCase testCase, object testobject, MethodInfo method, object[] args, string name)
        {
            method.Invoke(testobject, args);
        }

        public TestResultEntity PreProcessResults(TestCaseMetadata metadata, TestResultEntity testResult)
        {
            // Nothing to do
            return testResult;
        }

    }
}
