using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Diagnostics;

namespace MiniMPI.Testing
{
    /// <summary>
    /// Tests basic Send-Recv-Wait functionality of the runtime.
    /// </summary>
    
    public class EagerTests : MpiTestBase
    {

        /// <summary>
        /// This test is an example of when without the Eager flag, the program will deadlock.
        /// </summary>
        [ScheduleTestMethod]
        [RegressionTestExpectedResult(TestResultType.Deadlock)]
        public void Scenario1_DeadlocksWithoutEager()
        {
            ExecuteMpiProgram(2, mpi =>
            {
                mpi.MpiInit();
                int rank = mpi.GetRank();

                if (rank == 0)
                {
                    // P0
                    SendHandle h00 = mpi.SendAsync(1, "s00", false);
                    mpi.Wait(h00); // DEADLOCKS on wait because the barrier's can't be satisfied
                    mpi.Barrier();
                }
                else
                {
                    // P1
                    mpi.Barrier(); // Never gets past here because P0 is waiting for us to recv first.
                    ReceiveHandle h11 = mpi.ReceiveAsync(0);
                    string msg1 = mpi.Wait(h11);
                    Assert.AreEqual("s00", msg1);
                }

                mpi.MpiFinalize();
            });
        }

        /// <summary>
        /// This test is an example of when with the Eager flag, the program will NOT deadlock.
        /// </summary>
        [ScheduleTestMethod]
        public void Scenario1_DoesNotDeadlockWhenEager()
        {
            ExecuteMpiProgram(2, mpi =>
            {
                mpi.MpiInit();
                int rank = mpi.GetRank();

                if (rank == 0)
                {
                    // P0
                    SendHandle h00 = mpi.SendAsync(1, "s00", true);
                    // Does Not DEADLOCK on wait because it will return when the string is copied
                    // to a local buffer.
                    mpi.Wait(h00);
                    mpi.Barrier();
                }
                else
                {
                    // P1
                    mpi.Barrier(); // Never gets past here because P0 is waiting for us to recv first.
                    ReceiveHandle h11 = mpi.ReceiveAsync(0);
                    string msg1 = mpi.Wait(h11);
                    Assert.AreEqual("s00", msg1);
                }

                mpi.MpiFinalize();
            });
        }

        /// <summary>
        /// This test is an example of when without the Eager flag, the program will deadlock.
        /// </summary>
        [ScheduleTestMethod]
        [RegressionTestExpectedResult(TestResultType.Deadlock)]
        public void Scenario2_SendToSelf_DeadlocksWithoutEager()
        {
            ExecuteMpiProgram(1, mpi =>
            {
                mpi.MpiInit();
                int rank = mpi.GetRank();

                if (rank == 0)
                {
                    // P0
                    var h00 = mpi.SendAsync(0, "s00", false);
                    mpi.Wait(h00); // DEADLOCKS on wait because the send isn't eager and it can't be matched because we're blocked here
                    var h02 = mpi.ReceiveAsync(0);
                    string msg1 = mpi.Wait(h02);
                    Assert.AreEqual("s00", msg1);
                }

                mpi.MpiFinalize();
            });
        }

        /// <summary>
        /// This test is an example of with the Eager flag, the program will NOT deadlock.
        /// </summary>
        [ScheduleTestMethod]
        public void Scenario2_SendToSelf_DoesNotDeadlockWithEager()
        {
            ExecuteMpiProgram(1, mpi =>
            {
                mpi.MpiInit();
                int rank = mpi.GetRank();

                if (rank == 0)
                {
                    // P0
                    var h00 = mpi.SendAsync(0, "s00", true);
                    mpi.Wait(h00); // no-op
                    var h02 = mpi.ReceiveAsync(0);
                    string msg1 = mpi.Wait(h02);
                    Assert.AreEqual("s00", msg1);
                }

                mpi.MpiFinalize();
            });
        }

    }
}
