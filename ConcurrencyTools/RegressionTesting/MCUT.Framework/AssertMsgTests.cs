using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.Concurrency.TestTools.UnitTesting.RegressionTests
{

    /// <summary>
    /// Tests the functionality of Chess test methods.
    /// </summary>
    public class AssertMsgTests
    {

        private const string MsgPlaceholder = "[msg]";

        [UnitTestMethod]
        [ExpectedException(typeof(AssertFailedException), ExpectedMessage = "Assert.Fail failed.")]
        public void Assert_Fail_NoMsg()
        {
            Assert.Fail();
        }
        [UnitTestMethod]
        [ExpectedException(typeof(AssertFailedException), ExpectedMessage = "Assert.Fail failed. " + MsgPlaceholder)]
        public void Assert_Fail_WMsg()
        {
            Assert.Fail(MsgPlaceholder);
        }

        [UnitTestMethod]
        [ExpectedException(typeof(AssertFailedException), ExpectedMessage = "Assert.IsTrue failed.")]
        public void Assert_IsTrue_NoMsg()
        {
            Assert.IsTrue(false);
        }
        [UnitTestMethod]
        [ExpectedException(typeof(AssertFailedException), ExpectedMessage = "Assert.IsTrue failed. " + MsgPlaceholder)]
        public void Assert_IsTrue_WMsg()
        {
            Assert.IsTrue(false, MsgPlaceholder);
        }

        [UnitTestMethod]
        [ExpectedException(typeof(AssertFailedException), ExpectedMessage = "Assert.IsFalse failed.")]
        public void Assert_IsFalse_NoMsg()
        {
            Assert.IsFalse(true);
        }
        [UnitTestMethod]
        [ExpectedException(typeof(AssertFailedException), ExpectedMessage = "Assert.IsFalse failed. " + MsgPlaceholder)]
        public void Assert_IsFalse_WMsg()
        {
            Assert.IsFalse(true, MsgPlaceholder);
        }

        [UnitTestMethod]
        [ExpectedException(typeof(AssertFailedException), ExpectedMessage = "Assert.IsNull failed.")]
        public void Assert_IsNull_NoMsg()
        {
            Assert.IsNull(3);
        }
        [UnitTestMethod]
        [ExpectedException(typeof(AssertFailedException), ExpectedMessage = "Assert.IsNull failed. " + MsgPlaceholder)]
        public void Assert_IsNull_WMsg()
        {
            Assert.IsNull(3, MsgPlaceholder);
        }

        [UnitTestMethod]
        [ExpectedException(typeof(AssertFailedException), ExpectedMessage = "Assert.AreEqual failed. Expected:<0>. Actual:<1>.")]
        public void Assert_AreEqual_NoMsg()
        {
            Assert.AreEqual(0, 1);
        }
        [UnitTestMethod]
        [ExpectedException(typeof(AssertFailedException), ExpectedMessage = "Assert.AreEqual failed. Expected:<0>. Actual:<1>. " + MsgPlaceholder)]
        public void Assert_AreEqual_WMsg()
        {
            Assert.AreEqual(0, 1, MsgPlaceholder);
        }

        [UnitTestMethod]
        [ExpectedException(typeof(AssertInconclusiveException), ExpectedMessage = "Assert.Inconclusive.")]  // Note: This is a little different than the VS Testing.
        public void Assert_Inconclusive_NoMsg()
        {
            Assert.Inconclusive();
        }
        [UnitTestMethod]
        [ExpectedException(typeof(AssertInconclusiveException), ExpectedMessage = "Assert.Inconclusive. " + MsgPlaceholder)]  // Note: This is a little different than the VS Testing.
        public void Assert_Inconclusive_WMsg()
        {
            Assert.Inconclusive(MsgPlaceholder);
        }

        [UnitTestMethod]
        [ExpectedResult(TestResultType.Exception
            , Message="Unit test threw exception InvalidOperationException."// Exception message: " + MsgPlaceholder
            )]
        public void Assert_UnexpectedThrown()
        {
            throw new InvalidOperationException(MsgPlaceholder);
        }

        [UnitTestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [ExpectedResult(TestResultType.AssertFailure
            , Message = "Unit test did not throw expected exception InvalidOperationException."
            )]
        public void Assert_ExExpected_NoExThrown()
        {
        }

        [UnitTestMethod]
        [ExpectedException(typeof(InvalidOperationException), MsgPlaceholder)]
        [ExpectedResult(TestResultType.AssertFailure
            // Note, since no ex thrown, it's not appended to the end of this ErrorMessage
            , Message = "Unit test did not throw expected exception InvalidOperationException."
            )]
        public void Assert_ExExpected_NoExThrown_WMsg()
        {
        }

        [UnitTestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [ExpectedResult(TestResultType.AssertFailure
            , Message = "Unit test threw exception ArgumentException, but exception InvalidOperationException was expected."// Exception message: [ArgExMsg]"
            )]
        public void Assert_ExExpected_WrongExThrown()
        {
            throw new ArgumentException("[ArgExMsg]");
        }

        [UnitTestMethod]
        [ExpectedException(typeof(InvalidOperationException), MsgPlaceholder)]
        [ExpectedResult(TestResultType.AssertFailure
            , Message = "Unit test threw exception ArgumentException, but exception InvalidOperationException was expected."// Exception message: [ArgExMsg]"
            )]
        public void Assert_ExExpected_WrongExThrown_WMsg()
        {
            throw new ArgumentException("[ArgExMsg]");
        }

    }
}
