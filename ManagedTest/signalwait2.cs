/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test37
{
    public class ChessTest
    {

        // the regular test entry point
        public static void Main(string[] s)
        {
            if (Startup(s))
            {
                bool ret = Run();
                Console.WriteLine(ret);
            }
        }

        // validate input 
        public static bool Startup(string[] s)
        {
            return true;
        }

        static AutoResetEvent a;
        static AutoResetEvent b;

        public static bool Run()
        {
            a = new AutoResetEvent(false);
            b = new AutoResetEvent(false);
            Thread t = new Thread(delegate()
            {
                WaitHandle.SignalAndWait(a, b);
            });
            t.Start();
            // put parent thread code here
            b.Set();
            a.WaitOne();
            bool ret = b.WaitOne(0, false);
            t.Join();
            // check consistency of state
            return (ret == false);
        }
    }
}
  