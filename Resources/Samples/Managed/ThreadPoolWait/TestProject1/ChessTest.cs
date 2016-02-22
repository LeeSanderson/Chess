/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMatters;

namespace TestProject1
{
    // define this class for use with mchess and ChessBoard
    // so we can run any test method of the class BoundedQueueTest  
    class ChessTest
    {
        public static bool Startup(string[] args)
        {
            try
            {
                // find the test method by name
                testmethod = typeof(TestHarness).GetMethod(args[0], new Type[0]);
                return (testmethod != null);
            }
            catch
            {
                return false;
            }
        }

        private static System.Reflection.MethodInfo testmethod;

        public static bool Run()
        {
            TestHarness t = new TestHarness();
            testmethod.Invoke(t, new object[0]);
            return true;
        }
    }
}
