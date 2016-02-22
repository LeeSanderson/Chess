using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    public class ExceptionTest : MChessRegressionTestBase
    {
        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.TestFailure, SchedulesRan = -1, LastThreadCount = -1, LastExecSteps = -1, LastHBExecSteps = -1)]
        [ExpectedChessResult("csb2", ChessExitCode.TestFailure, SchedulesRan = -1, LastThreadCount = -1, LastExecSteps = -1, LastHBExecSteps = -1)]
        [ExpectedChessResult("csb3", ChessExitCode.TestFailure, SchedulesRan = -1, LastThreadCount = -1, LastExecSteps = -1, LastHBExecSteps = -1)]

        public void Test1()
        {
            throw new Exception();
        }

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.TestFailure, SchedulesRan = -1, LastThreadCount = -1, LastExecSteps = -1, LastHBExecSteps = -1)]
        [ExpectedChessResult("csb2", ChessExitCode.TestFailure, SchedulesRan = -1, LastThreadCount = -1, LastExecSteps = -1, LastHBExecSteps = -1)]
        [ExpectedChessResult("csb3", ChessExitCode.TestFailure, SchedulesRan = -1, LastThreadCount = -1, LastExecSteps = -1, LastHBExecSteps = -1)]
        public void Test2()
        {
            Thread t = new Thread(Begin);
            t.Start();
            t.Join();
        }

        public static void Begin()
        {
            throw new Exception();
        }
    }
}
