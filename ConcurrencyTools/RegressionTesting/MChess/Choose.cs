using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    public class Choose : MChessRegressionTestBase
    {
        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 6, LastThreadCount = 1, LastExecSteps = 3, LastHBExecSteps = 6)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 6, LastThreadCount = 1, LastExecSteps = 3, LastHBExecSteps = 6)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 6, LastThreadCount = 1, LastExecSteps = 3, LastHBExecSteps = 6)]
        public void Test()
        {
            int choice = ChessAPI.Choose(6);
            Assert.IsTrue(0 <= choice && choice <= 5);
        }
    }

}