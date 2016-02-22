/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test29
{
    public class ChessTest
    {
        public static bool Run()
        {
            Example.Main();
            return true;
        }
    }

    public class Example
    {
        public static void Main()
        {
            AutoResetEvent ev1 = new AutoResetEvent(false);
            // Queue the task.
            RegisteredWaitHandle rwh =
                ThreadPool.RegisterWaitForSingleObject(
                    ev1,
                    new WaitOrTimerCallback(ThreadProc),
                    null,
                    10,
                    true);

            Console.WriteLine("Main thread does some work, then sleeps.");
            // If you comment out the Sleep, the main thread exits before
            // the thread pool task runs.  The thread pool uses background
            // threads, which do not keep the application running.  (This
            // is a simple example of a race condition.)
            //Thread.Sleep(10000);

            AutoResetEvent ev2 = new AutoResetEvent(false);
            bool retVal = rwh.Unregister(ev2);
            Console.WriteLine(retVal);

            ev1.Set();

            ev2.WaitOne();
            Console.WriteLine("Main thread exits.");
        }

        // This thread procedure performs the task.
        static void ThreadProc(Object stateInfo, bool timedOut)
        {
            // No state object was passed to QueueUserWorkItem, so 
            // stateInfo is null.
            Console.WriteLine("Hello from the thread pool.");
        }
    }
}