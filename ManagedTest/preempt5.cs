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
        static public volatile int a, b, c;
        static public volatile int x;  // use this to make sure there are schedule points between

        static public void Main(string[] args)
        {
            Console.WriteLine(Run());
        }

        // needs to print all interleavings

        static public bool Run()
        {
            a = b = c = x = 0;
            var child = new Thread(() => { thread2(); });
            child.Start();
            thread1();
            child.Join();
            return true;
        }

        static public void thread1()
        {
            x = 1;
            {
                Chess.PreemptionDisable();
                a = 1;   Console.Error.Write("a=1 ");
                Chess.PreemptionEnable();

            }
            x = 1;
            {
                Chess.PreemptionDisable();
                b = 1;   Console.Error.Write("b=1 ");
                Chess.PreemptionEnable();
            }
        }

        static public void thread2()
        {
            x = 1;
            {
                Chess.PreemptionDisable();
                c = 1; Console.Error.Write("c=1 ");
                Chess.PreemptionEnable();

            }
        }

    }
}
