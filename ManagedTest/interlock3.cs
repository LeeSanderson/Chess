/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test17 {
    public class ChessTest
    {
        public int counter;

        ChessTest()
        {
            counter = 0;
        }
        // the regular test entry point
        public static void Main(string[] s)
        {
            Run();
        }

        public static bool Run()
        {
            ChessTest n = new ChessTest();
            Thread t = new Thread(delegate()
            {
                Interlocked.Increment(ref n.counter);
            });
            Thread t2 = new Thread(delegate()
            {
                Interlocked.Increment(ref n.counter);
            });
            t.Start();
            t2.Start();
            Interlocked.Increment(ref n.counter);
            t.Join();
            t2.Join();
            return (n.counter == 3);
        }
    }
}