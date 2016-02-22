/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// This example shows how a Mutex is used to synchronize access
// to a protected resource. Unlike Monitor, Mutex can be used with
// WaitHandle.WaitAll and WaitAny, and can be passed across
// AppDomain boundaries.

using System;
using System.Threading;

namespace Test21
{
    public class ChessTest
    {
        public static bool Run()
        {
            MutexTest.Main();
            return true;
        }
    }

    class MutexTest
    {
        // Create a new Mutex. The creating thread does not own the
        // Mutex.
        private const int numIterations = 1;
        private const int numThreads = 3;

        public static void Main()
        {
            Mutex mutex = new Mutex();

            // Create the threads that will use the protected resource.

            Thread myThread = new Thread(MyThreadProc);
            myThread.Name = String.Format("Thread{0}", 1);
            myThread.Start(mutex);

            Thread myThread2 = new Thread(MyThreadProc);
            myThread2.Name = String.Format("Thread{0}", 2);
            myThread2.Start(mutex);

            Thread myThread3 = new Thread(MyThreadProc);
            myThread3.Name = String.Format("Thread{0}", 3);
            myThread3.Start(mutex);


            myThread.Join();
            myThread2.Join();
            myThread3.Join();

        }

        private static void MyThreadProc(object o)
        {
            for (int i = 0; i < numIterations; i++)
            {
                UseResource((Mutex)o);
            }
        }

        // This method represents a resource that must be synchronized
        // so that only one thread at a time can enter.
        private static void UseResource(Mutex mutex)
        {
            // Wait until it is safe to enter.
            mutex.WaitOne();

            Console.WriteLine("{0} has entered the protected area",
                Thread.CurrentThread.Name);

            // Place code to access non-reentrant resources here.

            // Simulate some work.
            Thread.Sleep(500);

            Console.WriteLine("{0} is leaving the protected area\r\n",
                Thread.CurrentThread.Name);

            // Release the Mutex.
            mutex.ReleaseMutex();
        }
    }
}