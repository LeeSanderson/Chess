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

namespace Microsoft.ManagedChess.ChessTaskScheduler
{
    public class MyTask : Task
    {
        int f;
        public void Execute()
        {
            Monitor.Enter(this);
            f++;
            Monitor.Exit(this);
        }
    }

    class ChessTest
    {
        public static bool Startup(string[] args)
        {
            return true;
        }

        public static bool Run()
        {
            ChessTaskScheduler scheduler = new ChessTaskScheduler(2);
            scheduler.QueueTask(new MyTask());
            scheduler.QueueTask(new MyTask());
            scheduler.TryExecuteTaskInline(new MyTask(), false);
            scheduler.Shutdown();
            return true;
        }

        public static bool Shutdown()
        {
            return true;
        }
    }
}
