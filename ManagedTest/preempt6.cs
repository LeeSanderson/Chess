/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;
using Microsoft.ManagedChessAPI;

namespace Preemptions
{
    public class ChessTest
    {
        static public volatile int a, b, c, d;
        static public volatile int x;  // use this to make sure there are schedule points between

        static public void Main(string[] args)
        {
            Console.WriteLine(Run());
        }

        // needs to print all interleavings

        static public bool Run()
        {
            b = c = 0;
            var child = new Thread(() => { childthread(); });
            child.Start();
            int read_b = b; // we should never read 1 here
            child.Join();
            return read_b != 1;
        }

        static public void childthread()
        {
            Chess.PreemptionDisable();
            b = 1;
            b = 2;
            Chess.PreemptionEnable();
            c = 1;
            Chess.PreemptionDisable();
            c = 1;
            Chess.PreemptionEnable();
        }

    }
}
