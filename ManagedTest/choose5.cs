/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

// need this to access managed chess API
using Microsoft.ManagedChessAPI;

namespace Test3 {

    public class ChessTest {
        public static bool Run()
        {
	    CalculateTest.Main();
            return true;
        }
    }

    public class CalculateTest
    {
	public static int x;

        public static void Main()
        {
	   x = 0;
	   Thread t1 = new Thread(delegate (object o) { 
		  Interlocked.Increment(ref x);
		  });
	   Thread t2 = new Thread(delegate (object o) { 
	     	  Interlocked.Increment(ref x);
		  Interlocked.Increment(ref x);
		  });

	   t1.Start(null);
	   t2.Start(null);
	   t1.Join();
	   t2.Join();
        }
    }

}