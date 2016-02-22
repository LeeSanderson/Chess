using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;


namespace Microsoft.Concurrency.TestTools.UnitTesting.RegressionTests
{
    public class PreemptionControlTests
    {


        [ChessTestMethod]
        public void SimpleAtomicityTest1()
        {
            racyvariable = 0;

            Thread t = new Thread((Object irrelevant) =>
            {
                ChessAPI.TraceEvent("doing child ops");
                ChessAPI.PreemptionDisable();
                racyvariable++;
                ChessAPI.PreemptionEnable();

            });
            t.Start();
            ChessAPI.TraceEvent("doing parent ops");
            ChessAPI.PreemptionDisable();
            ChessAPI.PreemptionDisable();
            ChessAPI.PreemptionDisable();
            ChessAPI.PreemptionEnable();
            ChessAPI.PreemptionEnable();
            racyvariable++;
            ChessAPI.PreemptionEnable();
            t.Join();

            Assert.AreEqual(2, racyvariable);
        }

        volatile int racyvariable;

        [ChessTestMethod]
        [ExpectedResult(TestResultType.AssertFailure)]
        public void SimpleAtomicityTest2()
        {
            racyvariable = 0;

            Thread t = new Thread((Object irrelevant) =>
            {
                ChessAPI.TraceEvent("doing child ops");
                ChessAPI.PreemptionDisable();
                ChessAPI.PreemptionDisable();
                ChessAPI.PreemptionEnable();
                ChessAPI.PreemptionEnable();
                racyvariable++;
            });
            t.Start();

            ChessAPI.TraceEvent("doing parent ops");
            ChessAPI.PreemptionDisable();
            racyvariable++;
            ChessAPI.PreemptionEnable();

            t.Join();

            Assert.AreEqual(2, racyvariable);
        }

        [ChessTestMethod]
        public void SimpleAtomicityTest3a()
        {
            racyvariable = 0;

            Thread t = new Thread((Object irrelevant) =>
            {
                Baschi.Whatever.RunWithoutPreemptions(() =>
                    {
                        racyvariable++;
                    });
            });
            t.Start();


            Baschi.Whatever.RunWithoutPreemptions(() =>
                  {
                      racyvariable++;
                  });

            t.Join();

            Assert.AreEqual(2, racyvariable);
        }


        [ChessTestMethod]
        [ExpectedResult(TestResultType.AssertFailure)]
        public void SimpleAtomicityTest3b()
        {
            racyvariable = 0;

            Thread t = new Thread((Object irrelevant) =>
            {
                ChessAPI.RunWithoutPreemptions(() =>
                    {
                        racyvariable++;
                    });
            });
            t.Start();

            try
            {
                ChessAPI.RunWithoutPreemptions(() => ((Object)(null)).GetHashCode());
            }
            catch (NullReferenceException)
            {
            }

            racyvariable++;

            t.Join();

            Assert.AreEqual(2, racyvariable);
        }
    }

}
