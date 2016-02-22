/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

// need this to access managed chess API
using Microsoft.ManagedChessAPI;

namespace Test49 {

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
	    int choice = Chess.Choose(6);
        }
    }

}