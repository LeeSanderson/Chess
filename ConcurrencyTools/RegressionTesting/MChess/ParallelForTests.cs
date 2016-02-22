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
    public class ParallelForTests : MChessRegressionTestBase
    {

        volatile int x;

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 1, LastThreadCount = 1, LastExecSteps = 2)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 1, LastThreadCount = 1, LastExecSteps = 2)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 1, LastThreadCount = 1, LastExecSteps = 2)]
        public void TestEmptyFor()
        {
            Parallel.For(33, 22, (int i) => { System.Console.WriteLine("asdk"); });
        }

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 121, LastThreadCount = 2)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 406, LastThreadCount = 2)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 616, LastThreadCount = 2)]
        public void TestSum()
        {
            Object lockobj = new Object();
            int sum = 0;
            Parallel.For(0, 10, (int i) => { lock (lockobj) { sum++; } });
            Assert.AreEqual(10, sum);
        }

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.UnitTestAssertFailure)]
        [ExpectedChessResult("csb2", ChessExitCode.UnitTestAssertFailure)]
        [ExpectedChessResult("csb3", ChessExitCode.UnitTestAssertFailure)]
        public void TestNonatomic()
        {
            x = 0;
            Parallel.For(0, 2, (int i) => { x++; });
            Assert.AreEqual(2, x);
        }

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success)]
        [ExpectedChessResult("csb2", ChessExitCode.Success)]
        [ExpectedChessResult("csb3", ChessExitCode.Success)]
        public void TestNonatomicButNonconcurrent()
        {
            x = 0;
            ParallelOptions o = new ParallelOptions();
            o.MaxDegreeOfParallelism = 1;
            Parallel.For(0, 2, o, (int i) => { x++; });
            Assert.AreEqual(2, x);
        }


        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.ChessDeadlock)]
        [ExpectedChessResult("csb2", ChessExitCode.ChessDeadlock)]
        [ExpectedChessResult("csb3", ChessExitCode.ChessDeadlock)]
        public void TestDeadlockOne()
        {
            ManualResetEvent e = new ManualResetEvent(false);
            Parallel.For(0, 10, (int i) => { if (i == 0) e.Set(); else if (i == 9) e.WaitOne(); });
        }

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.ChessDeadlock)]
        [ExpectedChessResult("csb2", ChessExitCode.ChessDeadlock)]
        [ExpectedChessResult("csb3", ChessExitCode.ChessDeadlock)]
        public void TestDeadlockTwo()
        {
            ManualResetEvent e = new ManualResetEvent(false);
            Parallel.For(0, 10, (int i) => { if (i == 7) e.Set(); else if (i == 6) e.WaitOne(); });
        }

    }
}
