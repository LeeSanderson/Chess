using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    public class DupHandle : MChessRegressionTestBase
    {
        // Constants for native methods
        const int DUPLICATE_SAME_ACCESS = 0x00000002;

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 1, LastThreadCount = 2, LastExecSteps = 11, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 1, LastThreadCount = 2, LastExecSteps = 11, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 1, LastThreadCount = 2, LastExecSteps = 11, LastHBExecSteps = 1)]
        public void Test()
        {
            AutoResetEvent e1 = new AutoResetEvent(false);
            AutoResetEvent e2 = new AutoResetEvent(false);
            System.IntPtr childHandle = System.IntPtr.Zero;

            Thread t = new Thread(delegate()
            {
                // put child thread code here
                IntPtr currentThread = NativeMethods.GetCurrentThread();
                IntPtr currentProcess = NativeMethods.GetCurrentProcess();

                NativeMethods.DuplicateHandle(currentProcess,
                                                            currentThread,
                                                            currentProcess,
                                                            out childHandle,
                                                            0,
                                                            false,
                                                            DUPLICATE_SAME_ACCESS);
                e1.Set();

                e2.WaitOne();

            });
            t.Start();
            // put parent thread code here

            e1.WaitOne();
            e2.Set();
            // childHandle is valid. Wait on it, instead of join
            // There was a CHESS bug, by which CHESS didnt associate the right SyncVar to a DuplicateHandle(...,currentThread,...)
            // This test is to check the fix
            IntPtr[] h = new IntPtr[1];
            h[0] = childHandle;
            unsafe
            {
                fixed (IntPtr* handlePtr = h)
                {
                    NativeMethods.WaitForMultipleObjects(1, handlePtr, true, Timeout.Infinite);
                }
            }
        }
    }
}