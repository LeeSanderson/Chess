using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading;

namespace MiniMPI.Testing
{
    /// <summary>
    /// Tests basic Barrier functionality of the runtime.
    /// </summary>
    
    public class BarrierTests : MpiTestBase
    {

        [ScheduleTestMethod]
        public void BarrierTest_BarrierOnlyProg()
        {
            int n = 3;
            int atBarrierCnt = 0;
            ExecuteMpiProgram(n, mpi =>
            {
                mpi.MpiInit();
                int rank = mpi.GetRank();

                // Should all get here and then should all continue
                Interlocked.Increment(ref atBarrierCnt);
                mpi.Barrier();
                Assert.AreEqual(n, Thread.VolatileRead(ref atBarrierCnt), "atBarrierCnt should equal the # of processes.");

                mpi.MpiFinalize();
            });
        }

        [ScheduleTestMethod]
        public void BarrierTest_ConsecutiveBarriers()
        {
            int n = 3;
            int atBarrier1Cnt = 0;
            int atBarrier2Cnt = 0;
            ExecuteMpiProgram(n, mpi =>
            {
                mpi.MpiInit();

                // Should all get here and then should all continue
                Interlocked.Increment(ref atBarrier1Cnt);
                mpi.Barrier();
                Assert.AreEqual(n, Thread.VolatileRead(ref atBarrier1Cnt), "After the 1st barrier, atBarrier1Cnt should equal n.");
                Assert.AreEqual(0, Thread.VolatileRead(ref atBarrier2Cnt), "After the 1st barrier, atBarrier2Cnt should equal zero.");

                // Should all get here and then should all continue
                Interlocked.Increment(ref atBarrier2Cnt);
                mpi.Barrier();
                Assert.AreEqual(n, Thread.VolatileRead(ref atBarrier1Cnt), "After the 2nd barrier, atBarrier1Cnt should equal n.");
                Assert.AreEqual(n, Thread.VolatileRead(ref atBarrier2Cnt), "After the 2nd barrier, atBarrier2Cnt should equal n.");

                mpi.MpiFinalize();
            });
        }

        [ScheduleTestMethod]
        public void BarriersInDifferentBlocks()
        {
            int[] values = new int[3];
            object valuesSync = new object();

            ExecuteMpiProgram(3, mpi =>
            {
                mpi.MpiInit();
                int rank = mpi.GetRank();

                if (rank == 0)
                {
                    Thread.Sleep(200);
                    lock (valuesSync)
                        values[rank] = 1;
                    mpi.Barrier();
                }
                else if (rank == 1)
                {
                    lock (valuesSync)
                        values[rank] = 2;
                    mpi.Barrier();
                }
                else
                {
                    lock (valuesSync)
                        values[rank] = 3;
                    mpi.Barrier();
                }

                // Just have the one do the assertions
                if (rank == 0)
                {
                    lock (valuesSync)
                    {
                        for (int i = 0; i < 3; i++)
                            Assert.AreEqual(i + 1, values[i]);
                    }
                }

                mpi.MpiFinalize();
            });
        }

        //[ScheduleTestMethod]
        //public void MessageConversationTest()
        //{
        //	ExecuteMpiProcess(2, mpi => {
        //		mpi.MpiInit();
        //		int rank = mpi.MpiGetRank();

        //		mpi.MpiFinalize();
        //	});
        //}

        [ScheduleTestMethod]
        public void BarrierBetweenSendRecv_Test1()
        {
            bool eagerVal = true;
            ExecuteMpiProgram(2, mpi =>
            {
                mpi.MpiInit();
                int rank = mpi.GetRank();

                if (rank == 0)
                {
                    // P0
                    var h0 = mpi.SendAsync(1, "msg", eagerVal);
                    mpi.Barrier();
                    mpi.Wait(h0);
                }
                else
                {
                    // P1
                    mpi.Barrier();
                    var h1 = mpi.ReceiveAsync(0);
                    String msg = mpi.Wait(h1);
                    Debug.Assert(msg.Equals("msg"));
                }

                mpi.MpiFinalize();
            });
        }

        [ScheduleTestMethod]
        public void BarrierBetweenSendRecv_Test2()
        {
            bool eagerVal = true;
            ExecuteMpiProgram(2, mpi =>
            {
                mpi.MpiInit();
                int rank = mpi.GetRank();

                if (rank == 0)
                {
                    // P0
                    mpi.Barrier();
                    var h0 = mpi.SendAsync(1, "msg", eagerVal);
                    mpi.Wait(h0);
                }
                else
                {
                    // P1
                    var h1 = mpi.ReceiveAsync(0);
                    mpi.Barrier();
                    String msg = mpi.Wait(h1);
                    Debug.Assert(msg.Equals("msg"));
                }

                mpi.MpiFinalize();
            });
        }
    }
}
