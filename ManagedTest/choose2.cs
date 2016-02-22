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
        public static void Main()
        {
	   Thread t1 = new Thread(delegate (object o) { int choice = Chess.Choose(5); });
	   Thread t2 = new Thread(delegate (object o) { int choice = Chess.Choose(5); });

	   CalculateTest c = new CalculateTest();
	   t1.Start(c);
	   t2.Start(c);
	   t1.Join();
	   t2.Join();
        }
    }

}