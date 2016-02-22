using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    public class Example : MChessRegressionTestBase
    {
        public class State
        {
            public State(int n) { N = n; }
            public int signal = 0;
            public int changes = 0;
            public int N;
        }

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 5, LastThreadCount = 2, LastExecSteps = 15, LastHBExecSteps = 3)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 10, LastThreadCount = 2, LastExecSteps = 15, LastHBExecSteps = 5)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 11, LastThreadCount = 2, LastExecSteps = 15, LastHBExecSteps = 6)]
        [TestArgs(2)]
        public void Test(int n)
        {
            State s = new State(n);
            Thread t = new Thread(WriteY);
            t.Start(s);
            for (int i = 0; i < s.N; i++)
            {
                lock (s)
                {
                    if (s.signal == 1)
                        s.changes++;
                    s.signal = 0;
                };
            }
            t.Join();
            Assert.IsTrue(s.changes <= 6);
        }

        public static void WriteY(object o)
        {
            State s = (State)o;
            for (int j = 0; j < s.N; j++)
            {
                lock (s)
                {
                    if (s.signal == 0)
                        s.changes++;
                    s.signal = 1;
                };
            }
        }
    }
}
