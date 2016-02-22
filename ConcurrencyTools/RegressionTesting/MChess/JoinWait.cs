using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    public class JoinWait : MChessRegressionTestBase
    {
        static Object o = new Object();

        // joinwait.cs
        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 86, LastThreadCount = 3, LastExecSteps = 30, LastHBExecSteps = 22)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 276, LastThreadCount = 3, LastExecSteps = 36, LastHBExecSteps = 37)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 364, LastThreadCount = 3, LastExecSteps = 36, LastHBExecSteps = 37)]
        public void Test()
        {
            Thread t1 = new Thread(
               () => { lock (o) { } }
            );

            Thread t2 = new Thread(
              () => { lock (o) { } }
           );

            t1.Start();
            t2.Start();
            while (!t1.Join(1) || !t2.Join(1)) { }
        }

    }

}