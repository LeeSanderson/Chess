using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    public class ChoiZeller : MChessRegressionTestBase
    {
        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.UnitTestAssertFailure, SchedulesRan = 6, LastThreadCount = 3, LastExecSteps = 28, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb2", ChessExitCode.UnitTestAssertFailure, SchedulesRan = 9, LastThreadCount = 3, LastExecSteps = 28, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb3", ChessExitCode.UnitTestAssertFailure, SchedulesRan = 9, LastThreadCount = 3, LastExecSteps = 28, LastHBExecSteps = 1)]
        public void Test()
        {
            IntQueue iq = new IntQueue();
            Thread t = new Thread(delegate(object o)
            {
                // put child thread code here
                IntQueue miq = (IntQueue)o;
                miq.dequeue();
            });
            Thread t2 = new Thread(delegate(object o)
            {
                IntQueue miq = (IntQueue)o;
                iq.enqueue(2);
            });

            t.Start(iq);
            t2.Start(iq);
            iq.enqueue(1);
            t.Join();
            t2.Join();
            // check consistency of state
            Assert.IsFalse(iq.empty());
        }

        public class IntQueue
        {
            // The queue holds integers in the range
            // of [1..numberOfElements - 1]

            // link[N] is N’s successor in the queue
            int[] link;

            volatile int head; // First element of queue
            volatile int tail; // Last element of queue

            // Constructor
            public IntQueue()
            {
                head = 0;
                tail = 0;
                link = new int[10];
                for (int i = 0; i < 10; i++)
                {
                    link[i] = 0;
                }
            }

            public bool empty()
            {
                return (head == 0);
            }

            // Enqueue ELEM. 
            public void enqueue(int elem)
            {
                link[elem] = 0;
                if (head == 0)
                    head = elem;
                else
                {
                    lock (this)
                    {
                        link[tail] = elem;
                    }
                }
                tail = elem;
            }

            // Return first element of queue.
            // No error checking.
            public int dequeue()
            {
                int elem = head;
                if (elem == tail)
                    tail = 0;
                lock (this)
                {
                    head = link[head];
                }
                return elem;
            }
        }
    }
}