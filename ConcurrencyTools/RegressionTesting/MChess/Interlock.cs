using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    public class Interlock : MChessRegressionTestBase
    {
        public class Counter {
            public int counter;
        }

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 2, LastThreadCount = 2, LastExecSteps = 9, LastHBExecSteps = 2)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 3, LastThreadCount = 2, LastExecSteps = 9, LastHBExecSteps = 2)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 3, LastThreadCount = 2, LastExecSteps = 9, LastHBExecSteps = 2)]
        public void Test()
        {
            var n = new Counter();
            Thread t = new Thread(delegate()
            {
                Interlocked.Increment(ref n.counter);
            });
            t.Start();
            Interlocked.Increment(ref n.counter);
            t.Join();
            Assert.Equals(n.counter,2);
        }

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 10, LastThreadCount = 3, LastExecSteps = 15, LastHBExecSteps = 6)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 20, LastThreadCount = 3, LastExecSteps = 15, LastHBExecSteps = 6)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 26, LastThreadCount = 3, LastExecSteps = 15, LastHBExecSteps = 6)]
        public void Test2()
        {
            var n = new Counter();
            Thread t = new Thread(delegate()
            {
                Interlocked.Increment(ref n.counter);
            });
            Thread t2 = new Thread(delegate()
            {
                Interlocked.Increment(ref n.counter);
            });
            t.Start();
            t2.Start();
            Interlocked.Increment(ref n.counter);
            t.Join();
            t2.Join();
            Assert.Equals(n.counter, 2);
        }
    }
}