/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace JoinWaitNamespace
{
    public class ChessTest
    {

        static public void Main() { Run(); }

        static Object o = new Object();

        static public bool Run()
        {
            Thread t1 = new Thread(
               () => { lock (o) { } }
            );

            Thread t2 = new Thread(
              () => { lock (o) { } }
           );

            t1.Start();
            t2.Start();
            while (!t1.Join(1) || !t2.Join(1)) { }
            return true;
        }

    }
}