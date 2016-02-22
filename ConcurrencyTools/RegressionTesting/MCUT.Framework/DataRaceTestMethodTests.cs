using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.UnitTesting.RegressionTests
{
    /// <summary>
    /// Tests the functionality of the <see cref="DataRaceTestMethodAttribute"/> attribute.
    /// </summary>
    public class DataRaceTestMethodTests
    {

        [DataRaceTestMethod]
        public void NoOpTest_AssertNoRaces()
        {
        }

        [DataRaceTestMethod]
        [ExpectedResult(TestResultType.DataRace)]
        [RegressionTestExpectedResult(TestResultType.ResultAssertFailure)]
        public void NoOpTest_ExpectRacesWhenNoneExist()
        {
            // NOTE: This test tests the HasDataRaces attribute by indicating there should be races found
            // in this test, but the result will indicate none were found. In that case, this test should
            // fail. The RegressionTestExpectedResult attribute allows this test to be valid when regression
            // testing is enabled.
        }

        [DataRaceTestMethod]
        [ExpectedResult(TestResultType.DataRace)]
        public void SimpleRace()
        {
            ParallelTasks ptasks = new ParallelTasks();
            int balance = 0;

            ptasks.Add("t1", () => {
                balance++;
                int dud = balance;
            });
            ptasks.Add("t2", () => {
                balance--;
                int dud = balance;
            });

            ptasks.Execute();
        }

    }
}
