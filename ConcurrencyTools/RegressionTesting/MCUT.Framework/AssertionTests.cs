using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.Concurrency.TestTools.UnitTesting.RegressionTests
{

    /// <summary>
    /// Tests the functionality of the <see cref="Assert"/> class, ExpectedResultAttribute 
    /// and ExpectedExceptionAttribute.
    /// </summary>
    public class AssertionTests
    {

        [UnitTestMethod]
        [ExpectedResult(TestResultType.Passed)]
        public void Noop_Pass()
        {
        }

        [UnitTestMethod]
        [ExpectedResult(TestResultType.Passed)]
        public void NoAssertions_Pass()
        {
        }

        [UnitTestMethod]
        [ExpectedResult(TestResultType.Passed)]
        public void Assert_Pass()
        {
            Assert.Pass();
        }

        [UnitTestMethod]
        [ExpectedResult(TestResultType.AssertFailure)]
        public void Assert_Fail()
        {
            Assert.Fail();
        }

        [UnitTestMethod]
        [ExpectedResult(TestResultType.Inconclusive)]
        public void Assert_Inconclusive()
        {
            Assert.Inconclusive();
        }

        // TODO: Write up unit tests for remaining methods on the Assert class.

    }
}
