/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Threading.ConcurrencyScheduler;

namespace Test14 {
    public class ChessTest
    {
        // Constants for native methods
        const int DUPLICATE_SAME_ACCESS = 0x00000002;

        // the regular test entry point
        public static void Main(string[] s)
        {
            if (Startup(s))
            {
                Run();
            }
        }

        // validate input 
        public static bool Startup(string[] s)
        {
            return true;
        }

        public static bool Run()
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



            // check consistency of state
            return true;
        }
    }
}