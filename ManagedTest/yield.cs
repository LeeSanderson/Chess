/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Yield
{
    public class ChessTest
    {
        static public void Main(string[] args)
        {
            Console.WriteLine(Run());
        }

	static volatile int a;

         static public bool Run() {
            a = 0;
            var child = new Thread(() =>
            {
		Thread.Sleep(0);
		Thread.Sleep(0);
                a = 1;
            });
            child.Start();
            int read_a = a;
            child.Join();
            return (read_a != 1);
        }
    }
}
