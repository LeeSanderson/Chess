using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Diagnostics;

namespace MiniMPI.Testing
{
    /// <summary>
    /// Common mpi scenarios that test the functionality of the runtime.
    /// </summary>
    
    public class ScenarioTests : MpiTestBase
    {

        // From "Verifying MPI for Slack Invariance" paper - Ganesh
        [ScheduleTestMethod]
        public void Scenario_Fig1_OO_ProcessingInMPI_UsesAllCoreMpiCalls()
        {
            ExecuteMpiProgram(3, mpi =>
            {
                mpi.MpiInit();
                int rank = mpi.GetRank();

                if (rank == 0)
                {
                    // P0
                    var h00 = mpi.SendAsync(1, "s00");
                    var h01 = mpi.SendAsync(2, "s01");
                    mpi.Barrier();
                    var h03 = mpi.ReceiveAsync(1);
                    mpi.Wait(h01);
                    mpi.Wait(h00);
                    string msg1 = mpi.Wait(h03);
                    Assert.AreEqual("s11", msg1);
                    var h07 = mpi.SendAsync(1, "s07");
                    mpi.Wait(h07);
                }
                else if (rank == 1)
                {
                    // P1
                    var h10 = mpi.ReceiveAsync(0);
                    var h11 = mpi.SendAsync(0, "s11");
                    mpi.Barrier();
                    mpi.Wait(h11);
                    string msg1 = mpi.Wait(h10);
                    Assert.AreEqual("s00", msg1);
                    var h15 = mpi.ReceiveAsync(0);
                    string msg2 = mpi.Wait(h15);
                    Assert.AreEqual("s07", msg2);
                }
                else//if (rank == 2)
                {
                    // P2
                    var h20 = mpi.ReceiveAsync(0);
                    mpi.Barrier();
                    string msg1 = mpi.Wait(h20);
                    Assert.AreEqual("s01", msg1);
                }

                mpi.MpiFinalize();
            });
        }

        [ScheduleTestMethod]
        [RegressionTestExpectedResult(TestResultType.Deadlock)]
        public void OneProcessNoneagerDeadlock()
        {
            bool eagerVal = false;
            ExecuteMpiProgram(1, mpi =>
            {
                mpi.MpiInit();

                var h1 = mpi.SendAsync(0, "message", eagerVal);
                mpi.Wait(h1);
                mpi.Barrier();
                var h2 = mpi.ReceiveAsync(0);
                mpi.Wait(h2);

                mpi.MpiFinalize();
            });
        }

        [ScheduleTestMethod]
        public void OneProcessEagerNoDeadlock()
        {
            bool eagerVal = true;
            ExecuteMpiProgram(1, mpi =>
            {
                mpi.MpiInit();

                var h1 = mpi.SendAsync(0, "message", eagerVal);
                mpi.Wait(h1);
                mpi.Barrier();
                var h2 = mpi.ReceiveAsync(0);
                mpi.Wait(h2);

                mpi.MpiFinalize();
            });
        }

        [ScheduleTestMethod]
        public void OneProcessAutoSend()
        {
            bool eagerVal = false;
            ExecuteMpiProgram(1, mpi =>
            {
                mpi.MpiInit();

                var h1 = mpi.ReceiveAsync(0);
                mpi.Barrier();
                var h2 = mpi.SendAsync(0, "message", eagerVal);
                String msg = mpi.Wait(h1);
                mpi.Wait(h2);
                Console.WriteLine("h1 = {0}", h1);
                Console.WriteLine("h2 = {0}", h2);
                Console.WriteLine("msg = {0}", msg);

                mpi.MpiFinalize();
            });
        }

        [ScheduleTestMethod]
        public void OneProcessDoesSomeStuff()
        {
            bool eagerVal = true;
            ExecuteMpiProgram(1, mpi =>
            {
                mpi.MpiInit();

                var h1 = mpi.SendAsync(0, "message1", eagerVal);
                mpi.Wait(h1);
                var h2 = mpi.SendAsync(0, "message2", eagerVal);
                mpi.Wait(h2);
                var h3 = mpi.ReceiveAsync(0);
                String msg1 = mpi.Wait(h3);
                var h4 = mpi.ReceiveAsync(0);
                String msg2 = mpi.Wait(h4);

                Console.WriteLine("msg1 = {0}", msg1);
                Console.WriteLine("msg2 = {0}", msg2);

                mpi.Barrier();

                mpi.MpiFinalize();
            });
        }

        [ScheduleTestMethod]
        public void ThreeProcessesInconsistentMatchingWithoutBarrier()
        {
            bool eagerVal = false;
            ExecuteMpiProgram(3, mpi =>
            {
                mpi.MpiInit();
                int rank = mpi.GetRank();

                if (rank == 0)
                {
                    // P0
                    mpi.SendAsync(2, "from p0", eagerVal);
                }
                else if (rank == 1)
                {
                    // P1
                    mpi.SendAsync(2, "from p1", eagerVal);
                }
                else
                {
                    // P2
                    // null means a wildcard receive
                    mpi.ReceiveAsync(null);
                    mpi.ReceiveAsync(1);
                }


                mpi.MpiFinalize();
            });
        }

        [ScheduleTestMethod]
        public void ThreeProcessesInconsistentMatchingWithBarrier()
        {
            bool eagerVal = false;
            ExecuteMpiProgram(3, mpi =>
            {
                mpi.MpiInit();
                int rank = mpi.GetRank();

                if (rank == 0)
                {
                    // P0
                    mpi.SendAsync(2, "from p0", eagerVal);
                    mpi.Barrier();
                }
                else if (rank == 1)
                {
                    // P1
                    mpi.Barrier();
                    mpi.SendAsync(2, "from p1", eagerVal);
                }
                else
                {
                    // P2
                    // null means a wildcard receive
                    mpi.ReceiveAsync(null);
                    mpi.Barrier();
                    mpi.ReceiveAsync(1);
                }


                mpi.MpiFinalize();
            });
        }

        [ScheduleTestMethod]
        public void TwoProcessesInconsistentMatchingWithBarrier()
        {
            bool eagerVal = false;
            ExecuteMpiProgram(2, mpi =>
            {
                mpi.MpiInit();
                int rank = mpi.GetRank();

                if (rank == 0)
                {
                    // P0
                    mpi.SendAsync(1, "from p0", eagerVal);
                    mpi.Barrier();
                }
                else
                {
                    // P1
                    // null means a wildcard receive
                    mpi.ReceiveAsync(null);
                    mpi.Barrier();
                    mpi.SendAsync(1,"from p1");
                    mpi.ReceiveAsync(1);
                }
                mpi.MpiFinalize();
            });
        }

        [ScheduleTestMethod]
        public void TwoProcessesInconsistentMatchingWithoutBarrier()
        {
            bool eagerVal = false;
            ExecuteMpiProgram(2, mpi =>
            {
                mpi.MpiInit();
                int rank = mpi.GetRank();

                if (rank == 0)
                {
                    // P0
                    mpi.SendAsync(1, "from p0", eagerVal);
                }
                else
                {
                    // P1
                    // null means a wildcard receive
                    mpi.ReceiveAsync(null);
                    mpi.SendAsync(1, "from p1");
                    mpi.ReceiveAsync(1);
                }
                mpi.MpiFinalize();
            });
        }
    }
}
