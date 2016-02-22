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
    /// Summary description for UnitTest1
    /// </summary>
    public class BasicUsageTests : MpiTestBase
    {

        [ScheduleTestMethod]
        public void NoCodeInProcessTest()
        {
            // This just verifies that the runtime will exit even if there are no 
            // MPI calls made to the runtime at all.
            ExecuteMpiProgram(2, mpiRuntime => {
            });
        }

        [ScheduleTestMethod]
        public void InitFinalizeOnlyTest()
        {
            // Does basic correct usage of the mpi runtime.
            ExecuteMpiProgram(2, mpi => {
                mpi.MpiInit();
                mpi.MpiFinalize();
            });
        }

        [ScheduleTestMethod]
        public void GetRankTest1()
        {
            List<int> ranks = new List<int>();
            int n = 3;

            ExecuteMpiProgram(n, mpi => {
                mpi.MpiInit();
                int rank = mpi.GetRank();
                //Assert.IsTrue(rank >= 0 && rank < n, "Invalid rank assigned to process.");

                mpi.MpiFinalize();

                // Add it after we're done w/all the Mpi stuff.
                lock (ranks)
                    ranks.Add(rank);
            });

            //System.Diagnostics.Debug.Write("Sorting ranks...");
            ranks.Sort();
            CollectionAssert.AreEqual(Enumerable.Range(0, n).ToList(), ranks);
        }

        [ScheduleTestMethod]
        public void AssertFinalizeDoesImplicitBarrier()
        {
            int n = 3;
            int atFinalizeCnt = 0;
            ExecuteMpiProgram(n, mpi => {
                mpi.MpiInit();
                int rank = mpi.GetRank();

                // Should all get here and then should all continue
                Interlocked.Increment(ref atFinalizeCnt);
                mpi.MpiFinalize();
                Assert.AreEqual(n, Thread.VolatileRead(ref atFinalizeCnt), "After mpi Finalize finishes (and unblocks) atFinalizeCnt should be equal to n if they all blocked.");
            });
        }

        #region Catching process exception tests

        [ScheduleTestMethod]
        public void AnyProcessesThrowsExTest_NoMpiCalls()
        {
            int n = 3;
            try
            {
                ExecuteMpiProgram(n, mpiRuntime => {
                    throw new TestUserException("Test exception for process.");
                });
                Assert.Fail("No exception thrown.");
            }
            catch (MiniMPIExecutionException execEx)
            {
                // Need to make sure that all processes threw an exception
                Assert.IsNotNull(execEx.FirstException);

                // Make sure they're all wrapped with ranks and that all ranks exist
                for (int i = 0; i < n; i++)
                {
                    var pEx = execEx.InnerExceptions.SingleOrDefault(inex => inex.Rank == i);
                    Assert.IsNotNull(pEx);
                    Assert.IsNotNull(pEx.InnerException);

                    // Because since we don't make any Mpi API calls, there's no chance of a collective abort
                    Assert.IsTrue(pEx.InnerException is TestUserException);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "ExecuteMpiProcessWithExpectedException - Unexpected Exception");
                Assert.Fail("Only the MiniMPIExecutionException exception is expected to be returned from ExecuteMpiProgram.");
            }
        }

        [ScheduleTestMethod]
        public void EachProcessThrowsExTest()
        {
            int n = 3;
            try
            {
                ExecuteMpiProgram(n, mpiRuntime => {
                    mpiRuntime.MpiInit();
                    throw new TestUserException(mpiRuntime.GetRank().ToString());
                });
                Assert.Inconclusive("No exception thrown. Check the AnyProcessesThrowsExTest_NoMpiCalls test.");
            }
            catch (MiniMPIExecutionException execEx)
            {
                // Verify the first one that occurred is the expected exception
                Assert.IsTrue(execEx.FirstException.InnerException is TestUserException);

                // Need to make sure that all processes threw an exception
                for (int i = 0; i < n; i++)
                {
                    var pEx = execEx.InnerExceptions.SingleOrDefault(inex => inex.Rank == i);
                    Assert.IsNotNull(pEx);
                    Assert.IsNotNull(pEx.InnerException);

                    var innerEx = pEx.InnerException;
                    if (!(innerEx is MiniMPICollectiveAbortException))
                    {
                        Assert.IsTrue(innerEx is TestUserException);
                        Assert.AreEqual(i.ToString(), innerEx.Message);
                    }
                    // else, the collective abort means this process threw the exception before any mpi calls after the first one threw
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "ExecuteMpiProgram - Unexpected Exception");
                Assert.Fail("Only the MiniMPIExecutionException exception is expected to be returned from ExecuteMpiProgram.");
            }
        }

        [ScheduleTestMethod]
        public void SingleProcessThrowsExTest()
        {
            int n = 2;
            int rankToThrowEx = 1;
            try
            {
                ExecuteMpiProgram(n, mpiRuntime => {
                    mpiRuntime.MpiInit();
                    int rank = mpiRuntime.GetRank();
                    if (rank == rankToThrowEx)
                        throw new TestUserException(rank.ToString());
                    mpiRuntime.MpiFinalize();
                });
                Assert.Inconclusive("No exception thrown. Check the AnyProcessesThrowsExTest_NoMpiCalls test.");
            }
            catch (MiniMPIExecutionException execEx)
            {
                // Verify the first one that occurred is the expected exception
                var firstEx = execEx.FirstException;
                Assert.AreEqual(rankToThrowEx, firstEx.Rank);
                Assert.IsTrue(firstEx.InnerException is TestUserException);

                // Ensure that all other exceptions thrown were collective aborts
                foreach (var pEx in execEx.InnerExceptions)
                {
                    if (pEx.Rank != rankToThrowEx)
                        Assert.IsTrue(pEx.InnerException is MiniMPICollectiveAbortException);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "ExecuteMpiProgram - Unexpected Exception");
                Assert.Fail("Only the MiniMPIExecutionException exception is expected to be returned from ExecuteMpiProgram.");
            }
        }

        #endregion

        #region Invalid programs - init/finalize

        /// <summary>
        /// The way the runtime works on the raven cluster, if a process exits w/o calling
        /// Finalize, then a 'collective abort' is thrown with all the threads.
        /// </summary>
        [ScheduleTestMethod]
        [ExpectedException(typeof(InvalidMiniMPIProgramException))]
        public void InitNoFinalizeTest_throwsEx()
        {
            ExecuteMpiProgramWithExpectedException(2, null, mpi => {
                mpi.MpiInit();
            });
        }

        [ScheduleTestMethod]
        [ExpectedException(typeof(InvalidMiniMPIOperationException))]
        public void FinalizeNoInitTest_throwsEx()
        {
            ExecuteMpiProgramWithExpectedException(2, null, mpi => {
                mpi.MpiFinalize();// Should raise an exception
            });
        }

        [ScheduleTestMethod]
        [ExpectedException(typeof(InvalidMiniMPIOperationException))]
        public void MakeMpiApiCallBeforeInitTest_throwsEx()
        {
            ExecuteMpiProgramWithExpectedException(2, null, mpi => {
                // Make any call into the MPI API
                mpi.GetRank();// Should raise an exception

                // Shouldn't ever get called
                mpi.MpiInit();
                mpi.MpiFinalize();
            });
        }

        [ScheduleTestMethod]
        [ExpectedException(typeof(InvalidMiniMPIOperationException))]
        public void MakeMpiApiCallAfterFinalizeTest_throwsEx()
        {
            ExecuteMpiProgramWithExpectedException(2, null, mpi => {
                mpi.MpiInit();
                mpi.MpiFinalize();

                // Make any call into the MPI API
                mpi.GetRank();// Should raise an exception
            });
        }

        #endregion

    }
}
