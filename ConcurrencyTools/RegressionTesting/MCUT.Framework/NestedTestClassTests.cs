using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.UnitTesting.RegressionTests
{
    // This is in response to a bug found where test methods on a sub-class are
    // detected by MCUT, but running them causes an error.
    // See issue #8205.
    public class ClassWithNestedTestClassesTests
    {

        public class ANestedClassWithTests
        {
            [UnitTestMethod]
            public void UnitTestInSubClass() { }

            [ScheduleTestMethod]
            public void ScheduleTestInSubClass() { }

            [DataRaceTestMethod]
            public void DataRaceTestInSubClass() { }
        }

        /// <summary>
        /// The tests in here should produce an Error saying that the nested class isn't public.
        /// </summary>
        private class APrivateNestedClassWithTests
        {
            [UnitTestMethod]
            [RegressionTestExpectedResult(TestResultType.Error)]
            public void UnitTestInSubClass() { }

            [ScheduleTestMethod]
            [RegressionTestExpectedResult(TestResultType.Error)]
            public void ScheduleTestInSubClass() { }

            [DataRaceTestMethod]
            [RegressionTestExpectedResult(TestResultType.Error)]
            public void DataRaceTestInSubClass() { }
        }

    }

    /// <summary>
    /// It appears that public nested classes within an internal parent class
    /// are actually instantiable.
    /// </summary>
    class InternalClassWithNestedTestClassesTests
    {

        public class ANestedClassWithTests
        {
            [UnitTestMethod]
            public void UnitTestInSubClass() { }

            [ScheduleTestMethod]
            public void ScheduleTestInSubClass() { }

            [DataRaceTestMethod]
            public void DataRaceTestInSubClass() { }
        }

    }
}
