using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    public class ParallelInvokeTests : MChessRegressionTestBase
    {
        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 1, LastThreadCount = 1, LastExecSteps = 2)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 1, LastThreadCount = 1, LastExecSteps = 2)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 1, LastThreadCount = 1, LastExecSteps = 2)]
        public void TestEmpty()
        {
            Parallel.Invoke();
        }
        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 2, LastThreadCount = 2, LastExecSteps = 8)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 2, LastThreadCount = 2, LastExecSteps = 8)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 2, LastThreadCount = 2, LastExecSteps = 8)]
        public void TestOne()
        {
            int x = 0;
            Parallel.Invoke(() => x = 5);
            Assert.AreEqual<int>(5, x);
        }
        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 2, LastThreadCount = 2, LastExecSteps = 8)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 2, LastThreadCount = 2, LastExecSteps = 8)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 2, LastThreadCount = 2, LastExecSteps = 8)]
        public void TestOneWithRestrictedConcurrency()
        {
            int x = 0;
            ParallelOptions o = new ParallelOptions();
            o.MaxDegreeOfParallelism = 1;
            Parallel.Invoke(o, () => x = 5);
            Assert.AreEqual<int>(5, x);
        }
        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 2, LastThreadCount = 2, LastExecSteps = 8)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 2, LastThreadCount = 2, LastExecSteps = 8)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 2, LastThreadCount = 2, LastExecSteps = 8)]
        public void TestTwo()
        {
            int[] x = new int[2];
            Parallel.Invoke(() => x[0] = 5, () => x[1] = 10);
            Assert.AreEqual<int>(5, x[0]);
            Assert.AreEqual<int>(10, x[1]);
        }
    }
}
