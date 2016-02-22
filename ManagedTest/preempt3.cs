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

        // expect the following NOT to return false
        // which is a bit counterintuitive
        static public bool Run()
        {
            x = a = 0;
            var child = new Thread(() =>
            {
                x = 1;
                Chess.PreemptionDisable();
                a = 1;
                Chess.PreemptionEnable();
            });
            child.Start();
            int read_x = x;
            int read_a = a;
            child.Join();
            return !(read_x == 1 && read_a == 0);
        }
    }
}
