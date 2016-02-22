using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    public class NonDeterminismTests
    {
        // TODO: Finish implementing Issue #7902 that allows Alpaca to handle the "non-determinism detected" chess result

        [ChessTestMethod]
        // Due to having non-determinism, this test could result in two different result types.
        [RegressionTestExpectedResultAttribute(TestResultType.Deadlock, TestResultType.Passed)]
        public void UsingRandomToControlLockOrder()
        {
            object lockA = new object();
            object lockB = new object();

            Random rng = new Random();
            Thread t1 = new Thread(() => {
                if (rng.NextDouble() < 0.5)
                {
                    lock (lockA)
                        lock (lockB)
                        {
                        }
                }
            });
            Thread t2 = new Thread(() => {
                if (rng.NextDouble() < 0.5)
                {
                    lock (lockB)
                        lock (lockA)
                        {
                        }
                }
            });

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            //Assert.Inconclusive("TODO: This test will cause MChess to to either detect a DeadLock or pass, with sometimes issuing a warning that non-determinism was detected.");
        }

    }
}
