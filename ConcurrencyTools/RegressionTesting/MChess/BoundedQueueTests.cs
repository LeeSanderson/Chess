using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    public class BoundedQueueTests : MChessRegressionTestBase
    {
        // ORIG: boundedqueue.cs
        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.ChessDeadlock, SchedulesRan = 1, LastThreadCount = 4, LastExecSteps = 21, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb2", ChessExitCode.ChessDeadlock, SchedulesRan = 1, LastThreadCount = 4, LastExecSteps = 21, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb3", ChessExitCode.ChessDeadlock, SchedulesRan = 1, LastThreadCount = 4, LastExecSteps = 21, LastHBExecSteps = 1)]
        public void BoundedQueueTest()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(2);

            ParallelTasks ptasks = new ParallelTasks();

            // PRODUCER thread 1 inserts 3 elements
            ptasks.Add("Producer1", () => {
                for (int i = 0; i < 3; i++)
                    queue.Enqueue(i);
            });

            // PRODUCER thread 2 inserts 3 elements
            ptasks.Add("Producer2", () => {
                for (int i = 0; i < 3; i++)
                    queue.Enqueue(i);
            });

            // CONSUMER thread removes 4 elements
            ptasks.Add("Consumer", () => {
                for (int i = 0; i < 4; i++)
                {
                    System.Diagnostics.Debug.Assert(i >= queue.Dequeue());
                }
            });

            ptasks.Execute();

            // check size: should be two
            Assert.AreEqual(2, queue.Size(), "The final size of the queue.");
        }

        // ORIG: boundedqueue2.cs
        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.ChessDeadlock, SchedulesRan = 1, LastThreadCount = 4, LastExecSteps = 48, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb2", ChessExitCode.ChessDeadlock, SchedulesRan = 1, LastThreadCount = 4, LastExecSteps = 48, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb3", ChessExitCode.ChessDeadlock, SchedulesRan = 1, LastThreadCount = 4, LastExecSteps = 48, LastHBExecSteps = 1)]
        public void BoundedQueueTest2()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(2);

            ParallelTasks ptasks = new ParallelTasks();

            // PRODUCER thread inserts 6 elements
            ptasks.Add("Producer", () => {
                for (int i = 0; i < 6; i++)
                    queue.Enqueue(i);
            });

            // CONSUMER thread 1 removes 3 elements
            ptasks.Add("Consumer1", () => {
                for (int i = 0; i < 3; i++)
                {
                    int r = queue.Dequeue();
                    System.Diagnostics.Debug.Assert(i <= r);
                }
            });

            // CONSUMER thread 2 removes 3 elements
            ptasks.Add("Consumer2", () => {
                for (int i = 0; i < 3; i++)
                {
                    int r = queue.Dequeue();
                    System.Diagnostics.Debug.Assert(i <= r);
                }
            });

            ptasks.Execute();

            // check size: should be zero
            Assert.AreEqual(0, queue.Size(), "The final size of the queue.");
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
}
