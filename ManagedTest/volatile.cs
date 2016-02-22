/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test43
{
    public class ChessTest
    {

        volatile static int x;
        volatile static int y;

        public static void Main(string[] s)
        {
            Run();
        }

        public static bool Run()
        {
            x = 0;     // initialize
            y = 0;
            Thread t = new Thread(delegate()
            {  // create child thread
                x = 2;
                y = 2;
            });
            t.Start(); // start child thread
            x = 1;     // concurrently update
            y = 1;     // shared variables
            t.Join();  // wait for child thread to finish

            Console.WriteLine("{0} {1}", x, y);
            return ((x == 1 && y == 1) || (x == 2 && y == 2)
                  || (x == 1 && y == 2) || (x == 2 && y == 1));
        }
    }
}