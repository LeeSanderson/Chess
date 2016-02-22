/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test16 {
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
            t.Start();
            Interlocked.Increment(ref n.counter);
            t.Join();
            return (n.counter == 2);
        }
    }
}