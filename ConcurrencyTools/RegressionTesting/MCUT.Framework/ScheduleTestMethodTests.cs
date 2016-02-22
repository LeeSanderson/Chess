using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.UnitTesting.RegressionTests
{
    /// <summary>
    /// Tests the functionality of the <see cref="ScheduleTestMethodAttribute"/> attribute.
    /// </summary>
    public class ScheduleTestMethodTests
    {

        [ScheduleTestMethod]
        public void ExpectedResult_Passed()
        {
        }


        private void Test_that_takes_way_too_long()
        {
            for (int i = 1; i < 10; i++)
                System.Threading.ThreadPool.QueueUserWorkItem((Object o) =>
                    {
                        int dummy = 0;
                        for (int j = 0; j < 10; j++)
                            lock (this)
                            {
                                dummy += 1;
                            }
                    });
        }

        [ChessTestMethod]
        [ChessTestContext(MaxExecs = 100)]
        public void MaxExecsParamForChessTest()
        {
            Test_that_takes_way_too_long();
        }
        [ScheduleTestMethod(MaxSchedules=100)]
        public void MaxExecsParamForScheduleTest()
        {
            Test_that_takes_way_too_long();
        }

        [ScheduleTestMethod]
        [ExpectedResult(TestResultType.Deadlock)]
        [RegressionTestExpectedResult(TestResultType.ResultAssertFailure)]
        public void ExpectedResult_Deadlock_NotFound()
        {
        }

        [ScheduleTestMethod]
        [ExpectedResult(TestResultType.Deadlock)]
        public void ExpectedResult_Deadlock()
        {
            ParallelTasks ptasks = new ParallelTasks();
            object sync1 = new object();
            object sync2 = new object();

            ptasks.Add("t1", () => {
                lock (sync1)
                    lock (sync2)
                    { }
            });
            ptasks.Add("t2", () => {
                lock (sync2)
                    lock (sync1)
                    { }
            });

            ptasks.Execute();
        }

    }
}
