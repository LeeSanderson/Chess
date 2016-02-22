/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.IO;
using System.Threading;

namespace TestWaitInit
{
    public class ChessTest
    {
        public static bool Run()
        {
            Main();
            return true;
        }

        static void Main()
        {

        ManualResetEvent ev1 = new ManualResetEvent(false);
        ManualResetEvent ev2 = new ManualResetEvent(true) ;

        Thread t1 = new Thread(() =>
            {
                ev1.Set();
            });
	    t1.Start();
        Thread.Sleep(0);
        WaitHandle.WaitAll(new WaitHandle[] { ev1, ev2 });
        t1.Join();
        }
    }

 
}


