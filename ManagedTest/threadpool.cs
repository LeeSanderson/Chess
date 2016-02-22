/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// This example shows how to create an object containing task
// information, and pass that object to a task queued for
// execution by the thread pool.
using System;
using System.Threading;

namespace Test38
{
    public class ChessTest
    {
        public static bool Run()
        {
            Example.Main();
            return true;
        }
    }

    // TaskInfo holds state information for a task that will be
    // executed by a ThreadPool thread.
    public class TaskInfo
    {
        // State information for the task.  These members
        // can be implemented as read-only properties, read/write
        // properties with validation, and so on, as required.
        public string Boilerplate;
        public int Value;
        public Semaphore s;

        // Public constructor provides an easy way to supply all
        // the information needed for the task.
        public TaskInfo(string text, int number)
        {
            Boilerplate = text;
            Value = number;
            s = new Semaphore(0, 1);
        }
    }

    public class Example
    {
        public static void Main()
        {
            // Create an object containing the information needed
            // for the task.
            TaskInfo ti = new TaskInfo("This report displays the number {0}.", 42);

            // Queue the task and data.
            if (ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadProc), ti))
            {
                // Console.WriteLine("Main thread does some work, then sleeps.");

                // If you comment out the Sleep, the main thread exits before
                // the ThreadPool task has a chance to run.  ThreadPool uses 
                // background threads, which do not keep the application 
                // running.  (This is a simple example of a race condition.)
                ti.s.WaitOne();

                // Console.WriteLine("Main thread exits.");
            }
            else
            {
                Console.WriteLine("Unable to queue ThreadPool request.");
            }
        }

        // The thread procedure performs the independent task, in this case(
        // formatting and printing a very simple report.
        //
        static void ThreadProc(Object stateInfo)
        {
            TaskInfo ti = (TaskInfo)stateInfo;
            // Console.WriteLine(ti.Boilerplate, ti.Value);
            ti.s.Release();
        }
    }
}