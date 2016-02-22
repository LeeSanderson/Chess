using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using System.Collections;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    public class MonitorTest : MChessRegressionTestBase
    {

            [ChessTestMethod]
            [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 37, LastThreadCount = 3, LastExecSteps = 52, LastHBExecSteps = 10)]
            [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 143, LastThreadCount = 3, LastExecSteps = 52, LastHBExecSteps = 42)]
            [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 311, LastThreadCount = 3, LastExecSteps = 52, LastHBExecSteps = 90)]
            public void Test()
            {
                m_inputQueue = new Queue();
                Thread child = new Thread(delegate()
                {
                    for (int i = 5; i < 10; i++)
                        AddElement(i);

                });
                Thread child2 = new Thread(delegate()
                {
                    for (int i = 0; i < 5; i++)
                        AddElement(i);

                });
                child.Start();
                child2.Start();
                child.Join();
                child2.Join();
                for (int i = 0; i < 10; i++)
                    DeleteElement(i);
                Assert.IsTrue(IsEmpty());
            }

            //Define the queue to safe thread access.
            private Queue m_inputQueue;
         
            public bool IsEmpty()
            {
                return (m_inputQueue.Count == 0);
            }

            //Add an element to the queue and obtain the monitor lock for the queue object.
            public void AddElement(object qValue)
            {
                //Lock the queue.
                Monitor.Enter(m_inputQueue);
                //Add element
                m_inputQueue.Enqueue(qValue);
                //Unlock the queue.
                Monitor.Exit(m_inputQueue);
            }

            //Try to add an element to the queue.
            //Add the element to the queue only if the queue object is unlocked.
            public bool AddElementWithoutWait(object qValue)
            {
                //Determine whether the queue is locked 
                if (!Monitor.TryEnter(m_inputQueue))
                    return false;
                m_inputQueue.Enqueue(qValue);

                Monitor.Exit(m_inputQueue);
                return true;
            }

            //Try to add an element to the queue. 
            //Add the element to the queue only if during the specified time the queue object will be unlocked.
            public bool WaitToAddElement(object qValue, int waitTime)
            {
                //Wait while the queue is locked.
                if (!Monitor.TryEnter(m_inputQueue, waitTime))
                    return false;
                m_inputQueue.Enqueue(qValue);
                Monitor.Exit(m_inputQueue);

                return true;
            }

            //Delete all elements that equal the given object and obtain the monitor lock for the queue object.
            public void DeleteElement(object qValue)
            {
                //Lock the queue.
                Monitor.Enter(m_inputQueue);
                int counter = m_inputQueue.Count;
                while (counter > 0)
                {
                    //Check each element.
                    object elm = m_inputQueue.Dequeue();
                    if (!elm.Equals(qValue))
                    {
                        m_inputQueue.Enqueue(elm);
                    }
                    --counter;
                }
                //Unlock the queue.
                Monitor.Exit(m_inputQueue);
            }

            //Print all queue elements.
            public void PrintAllElements()
            {
                //Lock the queue.
                Monitor.Enter(m_inputQueue);
                IEnumerator elmEnum = m_inputQueue.GetEnumerator();
                while (elmEnum.MoveNext())
                {
                    //Print the next element.
                    Console.WriteLine(elmEnum.Current.ToString());
                }
                //Unlock the queue.
                Monitor.Exit(m_inputQueue);
            }
        }
}