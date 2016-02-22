/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test13 {
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

        public static bool ChessOnErrorCallback(int exitCode, string details)
        {
            System.Console.WriteLine("Chess is exiting with {0} : {1}", exitCode, details);
            return false;
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
            lock (lock1)
            {
                lock (lock2)
                {
                    x++;
                }
            }
        }

        public static void Child()
        {
            lock (lock2)
            {
                lock (lock1)
                {
                    x++;
                }
            }
        }
    }
}