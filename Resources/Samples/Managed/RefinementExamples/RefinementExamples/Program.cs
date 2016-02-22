/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RefinementExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            if (ChessTest.Startup(args))
                ChessTest.Run();
        }
    }

    class ChessTest
    {
        public static bool Startup(string[] args)
        {
            if (args.Length != 1)
                return false;
            testname = args[0];
            return true;
        }

        private static string testname;

        public static bool Run()
        {
            Tests suite = new Tests();
            suite.GetType().GetMethod(testname).Invoke(suite, new object[0]);
            return true;
        }
    }


}
