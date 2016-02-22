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
using System.Diagnostics;

namespace Microsoft.ManagedChess.ChessTaskScheduler
{
    public interface Task
    {
        void Execute();
    }

    public class ChessTaskScheduler
    {
        private int NumThreads;
        private Thread[] Threads;

        private class QueueEntry
        {
            internal Task task;
            internal bool canceled;

            internal QueueEntry(Task t, bool b)
            {
                task = t;
                canceled = b;
            }
        }

        private Queue<QueueEntry> TaskQueue;
        private bool shutdown;

        public ChessTaskScheduler(int numThreads)
        {
            NumThreads = numThreads;
            Threads = new Thread[numThreads];
            TaskQueue = new Queue<QueueEntry>();
            shutdown = false;
            for (int i = 0; i < NumThreads; i++)
            {
                Threads[i] = new Thread(new ThreadStart(Run));
                Threads[i].Start();
            }
        }

        private void Run()
        {
            QueueEntry qe = null;
            while (true)
            {
                Monitor.Enter(this);
                while (TaskQueue.Count == 0 && !shutdown)
                    Monitor.Wait(this);
                if (TaskQueue.Count != 0)
                {
                    qe = TaskQueue.Dequeue();
                    Monitor.Exit(this);
                }
                else
                {
                    Debug.Assert(shutdown);
                    Monitor.Exit(this);
                    break;
                }
                Debug.Assert(qe != null);
                if (!qe.canceled)
                    qe.task.Execute();
            }
        }

        public void Shutdown()
        {
            Monitor.Enter(this);
            Debug.Assert(!shutdown);
            shutdown = true;
            Monitor.PulseAll(this);
            Monitor.Exit(this);
            for (int i = 0; i < NumThreads; i++)
            {
                Threads[i].Join();
            }
        }

        public void QueueTask(Task task)
        {
            Monitor.Enter(this);
            Debug.Assert(!shutdown);
            foreach (QueueEntry qe in TaskQueue)
            {
                Debug.Assert(qe.task != task);
            }
            TaskQueue.Enqueue(new QueueEntry(task, false));
            Monitor.Pulse(this);
            Monitor.Exit(this);
        }

        public bool TryDequeue(Task task)
        {
            bool canceled = false;
            Monitor.Enter(this);
            Debug.Assert(!shutdown);
            foreach (QueueEntry qe in TaskQueue)
            {
                if (qe.task == task)
                {
                    qe.canceled = true;
                    canceled = true;
                }
            } 
            Monitor.Exit(this);
            return canceled;
        }

        public bool TryExecuteTaskInline(Task task, bool wasQueuedBefore)
        {
            int choice = MChessChess.Choose(2);
            if (choice == 0)
                return false;
            else
            {
                if (!wasQueuedBefore)
                {
                    task.Execute();
                    return true;
                }
                else
                {
                    bool canceled = TryDequeue(task);
                    if (canceled)
                    {
                        task.Execute();
                        return true;
                    }
                    else
                    {
                        // task has already been executed
                        return false;
                    }
                }
            }
        }
    }  
}
