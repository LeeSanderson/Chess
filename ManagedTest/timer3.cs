/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test41
{
    class ChessTest
    {
        public static void Main(string[] args)
        {
            Run();
        }

        public static bool Run()
        {
            Timer t = new Timer(new TimerCallback(delegate(object o)
            {
                Console.WriteLine("writing one line");
            }), null, 0, Timeout.Infinite);

            Thread.Sleep(2000);

            // when we comment this out, we currently get a liveness violation
            // which is expected (until quiescence is updated)
            // t.Dispose();

            return true;
        }
    }
}