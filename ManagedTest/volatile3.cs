/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test45
{
    public class ChessTest
    {

        volatile static int x;
        volatile static int y;
        static object o;

        public static void Main(string[] s)
        {
            Run();
        }

        public static bool Run()
        {
            o = new Object();
            x = 0;     // initialize
            y = 0;
            Thread t = new Thread(delegate()
            {  // create child thread
                lock (o)
                {
                    x = 2;
                    y = 2;
                }
            });
            t.Start(); // start child thread
            lock (o)
            {
                x = 1;     // concurrently update
                y = 1;     // shared variables
            }
            t.Join();  // wait for child thread to finish

            Console.WriteLine("{0} {1}", x, y);
            return (x == y);
        }
    }
}