using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    /// <summary>a little utility class for executing parallel tasks</summary>
    internal class ParallelTasks
    {
        private List<Thread> threads = new List<Thread>();

        // specify a task (by name and delegate) 
        public void Add(string name, ThreadStart task)
        {
            Thread t = new Thread(task);
            t.Name = name;
            threads.Add(t);
        }

        /// <summary>execute all tasks in parallel, and wait for them to complete</summary>
        public void Execute()
        {
            foreach (Thread t in threads)
                t.Start();
            foreach (Thread t in threads)
                t.Join();
        }
    }
}
