/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test33 {
    public class ChessTest
    {
        public static bool Run()
        {
            Program.Main(null);
            return true;
        }

    }

    public class Program
    {
        public static void Main(string[] args)
        {
            object o = new object();
            Thread t = new Thread(() => {
                Monitor.TryEnter(o, Timeout.Infinite);
            });
            t.Start();
            Monitor.Enter(o);
        }
    }
} 