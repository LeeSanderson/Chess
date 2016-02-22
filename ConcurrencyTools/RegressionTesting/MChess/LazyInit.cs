using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    public class LazyInit : MChessRegressionTestBase
    {
        static public volatile int count;

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.TestFailure, SchedulesRan = 1, LastThreadCount = 2, LastExecSteps = 9, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb2", ChessExitCode.TestFailure, SchedulesRan = 1, LastThreadCount = 2, LastExecSteps = 9, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb3", ChessExitCode.TestFailure, SchedulesRan = 1, LastThreadCount = 2, LastExecSteps = 9, LastHBExecSteps = 1)]
        public void Test()
        {
            // side thread: read count, ignore value
            ThreadPool.QueueUserWorkItem((x) => { int dummy = count; });

            // main thread: purposefully not idempotent

            if (count == 0)
            {
                Interlocked.Increment(ref count);
                Assert.IsTrue(true);
            }
            else
            {
                count = count + 1;
                Assert.IsTrue(false);
            }
        }
    }

}