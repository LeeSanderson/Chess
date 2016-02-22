/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;
using Microsoft.ManagedChessAPI;

namespace Race
{
    public class ChessTest
    {
        static public volatile int x = 0;
        static public volatile int a = 0;
        static public void Main(string[] args)
        {
            Console.WriteLine(Run());
        }

        // expect the following to return false
        // for some interleaving
        static public bool Run() {
            x = a = 0;
            var child = new Thread(() =>
            {
                Chess.PreemptionDisable();
                x = 1;
                Chess.PreemptionEnable();
                a = 1;
            });
            child.Start();
            int read_x = x;
            int read_a = a;
            child.Join();
            return !(read_x == 1 && read_a == 0);
        }
    }
}
