using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Diagnostics;

namespace MiniMPI.Testing
{
    /// <summary>
    /// Provides common implementation for MPI tests.
    /// </summary>
    public abstract class MpiTestBase
    {

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        protected static void ExecuteMpiProgram(int processCnt, Action<IMiniMPIStringAPI> processWork)
        {
            MiniMPIProgram.Execute(processCnt, processWork);
        }

        /// <summary>
        /// Executes the processes and re-throws the exception from the first process that threw an exception.
        /// </summary>
        /// <param name="processCnt"></param>
        /// <param name="expectedFirstErroredProcess"></param>
        /// <param name="processWork"></param>
        protected static void ExecuteMpiProgramWithExpectedException(int processCnt
            , int? expectedFirstErroredProcess
            , Action<IMiniMPIStringAPI> processWork)
        {
            try
            {
                ExecuteMpiProgram(processCnt, processWork);
            }
            catch (MiniMPIExecutionException ex)
            {
                if (expectedFirstErroredProcess.HasValue)
                    Assert.AreEqual(expectedFirstErroredProcess.Value, ex.FirstException.Rank, "ex.FirstException.Rank was not the value expected.");

                // Just pass up the error that caused everything to quit
                Debug.WriteLine(ex.FirstException.Rank, "ex.FirstException.Rank");
                throw ex.FirstException.InnerException;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "ExecuteMpiProcessWithExpectedException - Unexpected Exception");
                Assert.Fail("Only MiniMPIExecutionException exceptions are expected to be thrown from ExecuteMpiProcess.");
            }
        }

    }
}
