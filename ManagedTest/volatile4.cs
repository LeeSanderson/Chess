/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test46
{
    public class ChessTest
    {

        volatile static int x;

        public static void Main(string[] s)
        {
            Run();
        }

        public static bool Run()
        {
            x = 0;     // initialize
            Thread t = new Thread(delegate()
            {  // create child thread
                for (int i = 0; i < 10; i++)
                {
                    x = x + 1;
                }
            });
            t.Start(); // start child thread
            x = x + 1;     // concurrently update
            t.Join();  // wait for child thread to finish

            Console.WriteLine(x);
            return true;
        }
    }
}