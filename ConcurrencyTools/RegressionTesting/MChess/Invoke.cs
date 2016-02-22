using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    public class Invoke : MChessRegressionTestBase
    {
        public class AsyncDemo
        {
            // The method to be executed asynchronously.
            public string TestMethod(int callDuration, out int threadId)
            {
                Thread.Sleep(1);
                threadId = Thread.CurrentThread.ManagedThreadId;
                return String.Format("{0}", threadId);
            }
        }
        // The delegate must have the same signature as the method
        // it will call asynchronously.
        public delegate string AsyncMethodCaller(int callDuration, out int threadId);

        // invoke.cs
        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 2, LastThreadCount = 2, LastExecSteps = 12, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 2, LastThreadCount = 2, LastExecSteps = 12, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 2, LastThreadCount = 2, LastExecSteps = 12, LastHBExecSteps = 1)]
        public void Test()
        {
            // The asynchronous method puts the thread id here.
            int threadId;

            // Create an instance of the test class.
            AsyncDemo ad = new AsyncDemo();

            // Create the delegate.
            AsyncMethodCaller caller = new AsyncMethodCaller(ad.TestMethod);

            // Initiate the asychronous call.
            IAsyncResult result = funnyhelper(caller, out threadId);

            Thread.Sleep(0);

            // Call EndInvoke to wait for the asynchronous call to complete,
            // and to retrieve the results.
            string returnValue = caller.EndInvoke(out threadId, result);

            bool flag = (threadId == Convert.ToInt32(returnValue));
            Assert.IsTrue(flag);
        }

        public static IAsyncResult funnyhelper(AsyncMethodCaller caller, out int tid)
        {
            try
            {
                return caller.BeginInvoke(3000, out tid, null, null);
            }
            catch (Exception)
            {
                tid = 0;
                return null;
            }
        }
    }
}