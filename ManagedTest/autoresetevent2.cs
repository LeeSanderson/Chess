/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test5 {
    public class ChessTest
    {

        // the regular test entry point
        public static void Main(string[] s)
        {
            Run();
        }

        public static bool Run()
        {
            AutoResetEvent e1 = new AutoResetEvent(false);
            AutoResetEvent e2 = new AutoResetEvent(false);
            Thread t = new Thread(delegate()
            {
                // put child thread code here
                e1.WaitOne();
                // Console.WriteLine("child thread");
                e2.Set();
            });
            t.Start();
            // put parent thread code here
            e1.Set();
            // Console.WriteLine("parent thread");
            e2.WaitOne();
            t.Join();
            // check consistency of state
            return true;
        }
    }
}