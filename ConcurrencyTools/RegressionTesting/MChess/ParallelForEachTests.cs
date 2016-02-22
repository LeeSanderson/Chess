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
    public class ParallelForEachTests : MChessRegressionTestBase
    {

        volatile int x;

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 1, LastThreadCount = 1, LastExecSteps = 2)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 1, LastThreadCount = 1, LastExecSteps = 2)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 1, LastThreadCount = 1, LastExecSteps = 2)]
        public void TestEmptySource()
        {
            int[] rg = new int[0];
            Parallel.ForEach(rg, (int i) => { System.Console.WriteLine("asdk"); });
        }

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 121, LastThreadCount = 2)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 406, LastThreadCount = 2)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 616, LastThreadCount = 2)]
        public void TestSum_GlobalAccumulator()
        {
            int[] rg = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Object lockobj = new Object();
            int sum = 0;
            Parallel.ForEach(rg, (int i) => { lock (lockobj) { sum++; } });
            Assert.AreEqual(10, sum);
        }

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.UnitTestAssertFailure)]
        [ExpectedChessResult("csb2", ChessExitCode.UnitTestAssertFailure)]
        [ExpectedChessResult("csb3", ChessExitCode.UnitTestAssertFailure)]
        public void TestNonatomicForeach()
        {
            int[] rg = new int[] { 0, 1 };
            x = 0;
            Parallel.ForEach(rg, (int i) => { x++; });
            Assert.AreEqual(2, x);
        }

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success)]
        [ExpectedChessResult("csb2", ChessExitCode.Success)]
        [ExpectedChessResult("csb3", ChessExitCode.Success)]
        public void TestNonatomicButNonconcurrentForeach()
        {
            int[] rg = new int[] { 0, 1 };
            x = 0;
            ParallelOptions o = new ParallelOptions();
            o.MaxDegreeOfParallelism = 1;
            Parallel.ForEach(rg, o, (int i) => { x++; });
            Assert.AreEqual(2, x);
        }


        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.ChessDeadlock)]
        [ExpectedChessResult("csb2", ChessExitCode.ChessDeadlock)]
        [ExpectedChessResult("csb3", ChessExitCode.ChessDeadlock)]
        public void TestDeadlockOneForeach()
        {
            int[] rg = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            ManualResetEvent e = new ManualResetEvent(false);
            Parallel.ForEach(rg, (int i) => {
                if (i == 0)
                    e.Set();
                else if (i == 9)
                    e.WaitOne();
            });
        }

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.ChessDeadlock)]
        [ExpectedChessResult("csb2", ChessExitCode.ChessDeadlock)]
        [ExpectedChessResult("csb3", ChessExitCode.ChessDeadlock)]
        public void TestDeadlockTwoForeach()
        {
            int[] rg = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            ManualResetEvent e = new ManualResetEvent(false);
            Parallel.ForEach(rg, (int i) => {
                if (i == 7)
                    e.Set();
                else if (i == 6)
                    e.WaitOne();
            });
        }

        /// <summary>
        /// NOTE: When the local state gets instrumented, then we'll need to
        /// update or remove this test.
        /// </summary>
        [UnitTestMethod]    // To verify our test is correct w/o instrumentation
        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.UnitTestException, SchedulesRan = 0)]
        [ExpectedChessResult("csb2", ChessExitCode.UnitTestException, SchedulesRan = 0)]
        [ExpectedChessResult("csb3", ChessExitCode.UnitTestException, SchedulesRan = 0)]
        public void ForEachOverloadsUsingLocalState_NotImplemented()
        {
            var rg = Enumerable.Range(0, 5);
            int totalSum = 0;
            Parallel.ForEach<int, int>(rg
                , () => 0
                , (i, _, localSum) => localSum + i
                , localSum => Interlocked.Add(ref totalSum, localSum)
                );
            Assert.AreEqual(10, totalSum);
        }

    }
}
