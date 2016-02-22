/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Dekker
{
    public class Test
    {
        public static volatile bool thread_1_is_entering;
        public static volatile bool thread_2_is_entering;

        public static void thread1()
        {
            thread_1_is_entering = true;
            if (!thread_2_is_entering)
            {
                // (mutually exclusive section would go here)
            }
        }

        public static void thread2()
        {
            thread_2_is_entering = true;
            if (!thread_1_is_entering)
            {
                // (mutually exclusive section would go here)
            }
        }

        public static bool Run()
        {
            // initalize variables
            thread_1_is_entering = false;
            thread_2_is_entering = false;

            // set up two threads
            Thread t = new Thread(() => thread1());
            Thread s = new Thread(() => thread2());

            t.Start();
            s.Start();
            t.Join();
            s.Join();

            return true;
        }
    }
}
