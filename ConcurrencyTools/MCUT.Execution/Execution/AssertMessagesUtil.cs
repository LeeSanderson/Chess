using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Microsoft.Concurrency.TestTools.Execution
{
    public static class AssertMessagesUtil
    {

        /// <summary>
        /// The message to use when a test has thrown an exception when none was expected.
        /// </summary>
        public static string FormatAssertionMessage_UnExpectedExceptionThrown(ManagedTestCase testCase, Exception ex)
        {
            return String.Format("Unit test threw exception {1}."
                , testCase.UnitTestName
                , ex.GetType().Name
                );
        }

        /// <summary>
        /// The message to use when a test has thrown an exception that is of a different type than expected.
        /// </summary>
        public static string FormatAssertionMessage_ExceptionOfWrongTypeThrown(ManagedTestCase testCase, Exception ex)
        {
            bool sameName = testCase.ExpectedExceptionType.Name.Equals(ex.GetType().Name);
            return String.Format("Unit test threw exception {2}, but exception {1} was expected."
                , testCase.UnitTestName
                , sameName ? testCase.ExpectedExceptionType.FullName : testCase.ExpectedExceptionType.Name
                , sameName ? ex.GetType().FullName : ex.GetType().Name
                );
        }

        /// <summary>
        /// The message to use when a test has not thrown an exception when a the test expect one to be thrown.
        /// </summary>
        public static string FormatAssertionMessage_ExpectedExceptionNotThrown(ManagedTestCase testCase)
        {
            string errMsg = String.Format("Unit test did not throw expected exception {1}."
                , testCase.UnitTestName
                , testCase.ExpectedExceptionType.Name
                );
            if (!String.IsNullOrEmpty(testCase.ExpectedExceptionMessage))
                errMsg += " " + testCase.ExpectedExceptionMessage;
            return errMsg;
        }

    }
}
