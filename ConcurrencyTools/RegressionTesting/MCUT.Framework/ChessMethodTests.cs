using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.UnitTesting.RegressionTests
{
    /// <summary>
    /// Tests the functionality of Chess test methods.
    /// </summary>
    public class ChessMethodTests
    {

        public ChessTestContext ChessTestContext { get; set; }

        [ChessTestMethod]
        public void NoOpTest()
        {
        }

        [ChessTestMethod]
        public void Pass()
        {
        }

        [ChessTestMethod]
        [ExpectedException(typeof(Microsoft.Concurrency.TestTools.UnitTesting.AssertFailedException))]
        public void AssertFail()
        {
            Assert.Fail("This test is expected to throw the AssertFailedException.");
        }

        [ChessTestMethod]
        [ExpectedException(typeof(Microsoft.Concurrency.TestTools.UnitTesting.AssertInconclusiveException))]
        public void AssertInconclusive()
        {
            Assert.Inconclusive("This test is expected to throw the AssertInconclusiveException.");
        }

        [ChessTestMethod]
        public void ChessContext_NotSpecified()
        {
            Assert.IsNotNull(ChessTestContext);
        }

        [ChessTestMethod]
        [ChessTestContext()]
        public void ChessContext_EmptySpecified()
        {
            Assert.IsNotNull(ChessTestContext);
            // Unfortunately, these aren't actually able to be passed thru.
            //Assert.AreEqual(false, ChessTestContext.Options.EnableRaceDetection, "ChessTestContext.Options.EnableRaceDetection");
            //Assert.AreEqual(false, ChessTestContext.Options.EnableAtomicityChecking, "ChessTestContext.Options.EnableAtomicityChecking");
            Assert.AreEqual(false, ChessTestContext.Options.PreemptAllAccesses, "ChessTestContext.Options.PreemptAllAccesses");
        }

        [ChessTestMethod]
        [ChessTestContext(ProcessorCount = 6,
            // Currently, there isn't any way to pass these thru to the context
            //EnableRaceDetection = true,
            //EnableAtomicityChecking = true,
            PreemptAllAccesses = true,
            MaxPreemptions = 5,
            MaxExecs = 23,
            MaxExecSteps = 94,
            MaxExecTime = 12345
            )]
        public void ChessContext_Specified()
        {
            Assert.IsNotNull(ChessTestContext);
            Assert.AreEqual(true, ChessTestContext.Options.PreemptAllAccesses, "ChessTestContext.Options.PreemptAllAccesses");
            Assert.AreEqual(6, ChessTestContext.Options.ProcessorCount, "ChessTestContext.Options.ProcessorCount");
            Assert.AreEqual(5, ChessTestContext.Options.MaxPreemptions, "ChessTestContext.Options.MaxPreemptions");
            Assert.AreEqual(23, ChessTestContext.Options.MaxExecs, "ChessTestContext.Options.MaxExecs");
            Assert.AreEqual(94, ChessTestContext.Options.MaxExecSteps, "ChessTestContext.Options.MaxExecSteps");
            Assert.AreEqual(12345, ChessTestContext.Options.MaxExecTime, "ChessTestContext.Options.MaxExecTime");
        }

        [ChessTestMethod()]
        [ChessTestContext(ExtraCommandLineArgs = new[] { "/processorcount:16" })]
        public void ChessContext_ExtraCommandLineArgs()
        {
            Assert.IsNotNull(ChessTestContext);
            // The cmd line is actually just the ProcessorCount, so when it get to this test being run
            // the mchess context should get this set.
            Assert.AreEqual(16, ChessTestContext.Options.ProcessorCount, "ChessTestContext.Options.ExtraCommandLineArgs - Cmd line args weren't passed thru.");
        }

        [ChessTestMethod()]
        [ChessTestContext(PreRunScript = "SET MY_ENV_VAR=[mytestvalue]")]
        public void ChessContext_PreRunScript()
        {
            Assert.IsNotNull(ChessTestContext);
            Assert.AreEqual("[mytestvalue]", Environment.GetEnvironmentVariable("MY_ENV_VAR"), "PreRunScript didn't run.");
        }

        [ChessTestMethod]
        [TestArgs('c', "abccbacatdog", 123)]
        public void TestWithArgs(char c, string input, int p)
        {
            // Not bothering with an actual implementation here.
            Assert.AreEqual('c', c);
            Assert.AreEqual("abccbacatdog", input);
            Assert.AreEqual(123, p);
        }

        #region Expected Chess Result tests

        [ChessTestMethod]
        [ExpectedChessResult(ChessExitCode.Success)]
        public void ExpChessResult_ExitCode_Success()
        {
            // Purposely empty.
        }

        [ChessTestMethod]
        [ExpectedChessResult(ChessExitCode.ChessDeadlock)]
        public void ExpChessResult_ExitCode_ChessDeadlock()
        {
            ParallelTasks ptasks = new ParallelTasks();
            object sync1 = new object();
            object sync2 = new object();

            ptasks.Add("t1", () => {
                lock (sync1)
                    lock (sync2)
                    { }
            });
            ptasks.Add("t2", () => {
                lock (sync2)
                    lock (sync1)
                    { }
            });

            ptasks.Execute();
        }

        [ChessTestMethod]
        [ChessTestContext(EnableRaceDetection = true)]
        [ExpectedChessResult(ChessExitCode.ChessRace)]
        public void ExpChessResult_ExitCode_ChessRace()
        {
            ParallelTasks ptasks = new ParallelTasks();
            int balance = 0;

            ptasks.Add("t1", () => {
                balance++;
                int dud = balance;
            });
            ptasks.Add("t2", () => {
                balance--;
                int dud = balance;
            });

            ptasks.Execute();
        }

        [ChessTestMethod]
        [ExpectedChessResult(ChessExitCode.UnitTestAssertFailure)]
        public void ExpChessResult_ExitCode_UnitTestAssertFailure()
        {
            Assert.Fail("Expected failure.");
        }

        [ChessTestMethod]
        [ExpectedChessResult(ChessExitCode.UnitTestAssertInconclusive)]
        public void ExpChessResult_ExitCode_UnitTestAssertInconclusive()
        {
            Assert.Inconclusive("Expected inconclusive.");
        }

        [ChessTestMethod]
        [ExpectedChessResult(ChessExitCode.UnitTestException)]
        public void ExpChessResult_ExitCode_UnitTestException()
        {
            throw new InvalidOperationException("Expected exception.");
        }

        #endregion

    }
}
