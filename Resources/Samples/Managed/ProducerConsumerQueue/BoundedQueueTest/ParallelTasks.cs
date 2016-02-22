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

namespace BoundedQueueTest
{
    // a little utility class for executing parallel tasks

    public class ParallelTasks
    {
        private List<Thread> threads = new List<Thread>();

        // specify a task (by name and delegate) 
        public void Add(string name, ThreadStart task)
        {
            Thread t = new Thread(task);
            t.Name = name;
            threads.Add(t);
        }

        // execute all tasks in parallel, and wait for them to complete
        public void Execute()
        {
            foreach (Thread t in threads)
                t.Start();
            foreach (Thread t in threads)
                t.Join();
        }
    }
}
