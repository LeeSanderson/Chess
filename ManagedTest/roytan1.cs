/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;
using System.Diagnostics;

namespace Test32 {
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
                lock (o)
                {
                    Math.Min(1, 2); //simulate work;
                }
            });
            t.Start();

            lock (o)
            {
                Math.Min(1, 2); //simulate work;
            }
            t.Join();
        }
    }
} 