/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;
using System.Collections.Generic;

namespace Test7b
{
    public class ChessTest
    {
        // the regular test entry point
        public static void Main(string[] s)
        {
            if (Startup(s))
            {
                bool r = Run();
                Console.WriteLine(r);
                Shutdown();
            }
        }

        // validate input 
        public static bool Startup(string[] s)
        {
            return true;
        }

        public static bool Run()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(2);

            ParallelTasks ptasks = new ParallelTasks();

            // PRODUCER thread 1 inserts 3 elements
            ptasks.Add("Producer1", () =>
            {
                for (int i = 0; i < 3; i++)
                    queue.Enqueue(i);
            });

            // PRODUCER thread 2 inserts 3 elements
            ptasks.Add("Producer2", () =>
            {
                for (int i = 0; i < 3; i++)
                    queue.Enqueue(i);
            });

            // CONSUMER thread removes 4 elements
            ptasks.Add("Consumer", () =>
            {
                for (int i = 0; i < 4; i++)
                    System.Diagnostics.Debug.Assert(i >= queue.Dequeue());
            });

            ptasks.Execute();

            // check size: should be two
            return queue.Size() == 2;
        }
    
        public static bool Shutdown()
        {
            return true;
        }
    }


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

    class BoundedQueue<ElementType>
    {
        private LinkedList<ElementType> queue = new LinkedList<ElementType>();
        private int bound;

        //
        // Create a Queue object, with capacity bounded by bound.
        //
        public BoundedQueue(int bound)
        {
            while (bound <= 0)
                throw new ArgumentException("Bound must be a positive number");
            this.bound = bound;
            queue = new LinkedList<ElementType>();
        }

        //
        // Remove all elements from the queue.
        //
        public void Clear()
        {
            lock (this)
            {
                queue.Clear();
            }
        }

        //
        //  Add an element to the queue (may block if there is no room).
        //
        public void Enqueue(ElementType element)
        {
            lock (this)
            {
                // block until there is room in the queue
                if (queue.Count == bound)
                    System.Threading.Monitor.Wait(this);

                // add the element to the queue
                queue.AddLast(element);

                // wake up potentially waiting consumers
                if (queue.Count == 1)
                    System.Threading.Monitor.Pulse(this);
            }
        }

        //
        //  Remove the oldest element from the queue (may block if queue is empty) .
        //
        public ElementType Dequeue()
        {
            ElementType element;
            lock (this)
            {
                // block until there is at least one element in the queue
                if (queue.Count == 0)
                    System.Threading.Monitor.Wait(this);

                // remove an element from the queue
                element = queue.First.Value;
                queue.RemoveFirst();

                // wake up potentially waiting producers
                if (queue.Count == (bound - 1))
                    System.Threading.Monitor.Pulse(this);
            }
            // return the dequeued element
            return element;
        }

        // 
        // return the number of elements currently in the queue.
        //
        public int Size()
        {
            lock (queue)
                return queue.Count;
        }


    }

}
