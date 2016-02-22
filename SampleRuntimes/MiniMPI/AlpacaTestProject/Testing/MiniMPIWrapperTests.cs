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

    public class MiniMPIWrapperTests : MpiTestBase
    {

        [ObservationTestMethod]
        [ObservationGenerator]
        public void SchedulerTest_WildcardRecvCanMatchMany()
        {
            Assert.Inconclusive("This Observation test needs to be finished. It needs all the observation recordings etc.");
            ExecuteMpiProgram(3, mpi => {
                mpi.MpiInit();

                var rank = mpi.GetRank();
                if (rank == 0)
                {
                    var h1 = mpi.ReceiveAsync(null);
                    var h2 = mpi.ReceiveAsync(null);
                    string msg1 = mpi.Wait(h1);
                    string msg2 = mpi.Wait(h2);

                    Assert.Inconclusive("TODO: Need to record outputs using the ObservationsUtil.");
                    //Console.WriteLine("msg1 = {0}", msg1);
                    //Console.WriteLine("msg2 = {0}", msg2);
                }
                else
                {
                    var h1 = mpi.SendAsync(0, "p" + rank);
                    mpi.Wait(h1);
                }

                mpi.MpiFinalize();
            });
        }

        [ObservationTestMethod]
        [ObservationGenerator]
        public void SchedulerTest_WildcardRecvCanMatchMany_WithBarrier()
        {
            Assert.Inconclusive("This Observation test needs to be finished. It needs all the observation recordings etc.");
            ExecuteMpiProgram(3, mpi => {
                mpi.MpiInit();

                var rank = mpi.GetRank();
                if (rank == 0)
                {
                    // Here, we want to show that the wildcard recv can recieve any of the
                    // sends even though the barrier for P1 was before it did its Isend
                    var h1 = mpi.ReceiveAsync(null);
                    mpi.Barrier();
                    string msg1 = mpi.Wait(h1);
                    Assert.Inconclusive("TODO: Need to record the msg1 using the ObservationsUtil.");

                    // Do the other one now, but we don't care for this example
                    // It's only here for completeness
                    var h2 = mpi.ReceiveAsync(null);
                    string msg2 = mpi.Wait(h2);
                    
                    //Console.WriteLine("msg1 = {0}", msg1);
                    //Console.WriteLine("msg2 = {0}", msg2);
                }
                else if(rank == 1)
                {
                    mpi.Barrier();
                    // Even though the barrier happens before this Isend, there should still be a schedule
                    // in which the first wildcard recv matches this Isend
                    var h1 = mpi.SendAsync(0, "p" + rank);
                    mpi.Wait(h1);
                }
                else // rank == 2
                {
                    var h1 = mpi.SendAsync(0, "p" + rank);
                    mpi.Barrier();
                    mpi.Wait(h1);
                }

                mpi.MpiFinalize();
            });
        }

    }
}
