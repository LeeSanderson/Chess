/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Race
{
    public class ChessTest
    {
        static public int x = 0;
        static public void Main(string[] args)
        {
            Console.WriteLine(Run());
        }

        static public bool Run() {
          x = 10;
          var child = new Thread(() => { x = x - 2; });
          child.Start();
          x = x + 1;
          child.Join();
          return (x==9);
        }
    }
}
