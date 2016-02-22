/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test11 {
    public class ChessTest
    {
        static Object lock1;
	    static Object lock2;
        static int x;

        public static void Main(string[] s)
        {
            if (Startup(s))
            {
                Run();
            }
        }

        public static bool Startup(string[] s)
        {
            lock1 = new Object();
            lock2 = new Object();
            x = 0;
            return true;
        }

        public static bool Run()
        {
            Thread t = new Thread(Child);
            t.Start();
	        Parent();
            t.Join();
            return true;
        }

	    public static void Parent()
        {
            bool done = false;
            while (!done)
            {
                lock (lock1)
                {
                    if (Monitor.TryEnter(lock2))
                    {
                        x++;
                        done = true;
                        Monitor.Exit(lock2);
                    }
                    if(!done)
                    {
                        Thread.Sleep(1);
                    }
                }
            }
        }

        public static void Child()
        {
            bool done = false;
            while (!done)
            {
                lock (lock2)
                {
                    if (Monitor.TryEnter(lock1))
                    {
                        x++;
                        done = true;
                        Monitor.Exit(lock1);
                    }
                    if(!done)
                    {
                        Thread.Sleep(1);
                    }
                }
            }
        }

        public static void Child2()
        {
            while (x != 2)
            {
                Thread.Sleep(2);
                ;
            }
        }
    }
}
