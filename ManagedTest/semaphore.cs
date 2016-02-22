/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test35
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
        // A semaphore that simulates a limited resource pool.
        //
        private static Semaphore _pool;

        public static void Main()
        {
            FooBar();
        }

        public static void FooBar()
        {

            // Create a semaphore that can satisfy up to three
            // concurrent requests. Use an initial count of zero,
            // so that the entire semaphore count is initially
            // owned by the main program thread.
            //
            _pool = new Semaphore(0, 2);

            // Create and start five numbered threads. 
            //
            for (int i = 1; i <= 2; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(Worker));
                t.Start(i);
            }

            // Wait for half a second, to allow all the
            // threads to start and to block on the semaphore.
            //
            Thread.Sleep(0);

            // The main thread starts out holding the entire
            // semaphore count. Calling Release(2) brings the 
            // semaphore count back to its maximum value, and
            // allows the waiting threads to enter the semaphore,
            // up to three at a time.
            //
            // Console.WriteLine("Main thread calls Release(2).");
            _pool.Release(2);

            // Console.WriteLine("Main thread exits.");
        }

        private static void Worker(object num)
        {
            // Each worker thread begins by requesting the
            // semaphore.
            // Console.WriteLine("Thread {0} begins " +
            //    "and waits for the semaphore.", num);
            _pool.WaitOne();

            // A padding interval to make the output more orderly.
            // int padding = Interlocked.Add(ref _padding, 100);

            // Console.WriteLine("Thread {0} enters the semaphore.", num);

            // The thread's "work" consists of sleeping for 
            // about a second. Each thread "works" a little 
            // longer, just to make the output more orderly.
            //
            Thread.Sleep(0);

            // Console.WriteLine("Thread {0} releases the semaphore.", num);
            //Console.WriteLine("Thread {0} previous semaphore count: {1}",
            _pool.Release();
        }
    }
}