/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// from MSDN

using System;
using System.Collections.Generic;
using System.Threading;

namespace Test19 {
    public class ChessTest
    {
        public static bool Run()
        {
            return MonitorSample.doit();
        }
    }

    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    class MonitorSample
    {
        //Define the queue to safe thread access.
        private Queue<int> m_inputQueue;
        private int MAXSIZE = 2;

        public MonitorSample()
        {
            m_inputQueue = new Queue<int>(); 
        }

        public bool IsEmpty() 
        {
            return (m_inputQueue.Count == 0);
        }

        public bool IsFull()
        {
            return (m_inputQueue.Count == MAXSIZE);
        }

        //Add an element to the queue and obtain the monitor lock for the queue object.
        public void AddElement(int qValue)
        {
            //Lock the queue.
            Monitor.Enter(m_inputQueue);
            while (IsFull())
                Monitor.Wait(m_inputQueue);
            //Add element
            m_inputQueue.Enqueue(qValue);
            Monitor.Pulse(m_inputQueue);
            //Unlock the queue.
            Monitor.Exit(m_inputQueue);
        }
        
        //Delete all elements that equal the given object and obtain the monitor lock for the queue object.
        public int DeleteElement()
        {
            int qValue;
            //Lock the queue.
            Monitor.Enter(m_inputQueue);
            while (IsEmpty())
                Monitor.Wait(m_inputQueue);
            //Delete element
            qValue = m_inputQueue.Dequeue();
            Monitor.Pulse(m_inputQueue);
            //Unlock the queue.
            Monitor.Exit(m_inputQueue);
            return qValue;
        }

        public static bool doit()
        {
            MonitorSample sample = new MonitorSample();
            Thread child = new Thread(delegate(object o)
            {
                MonitorSample s = (MonitorSample)o;
                for (int i = 0; i < 3; i++)
                    s.AddElement(i);

            });
            Thread child2 = new Thread(delegate(object o)
            {
                MonitorSample s = (MonitorSample)o;
                for (int i = 0; i < 3; i++)
                {
                    bool ok = (s.DeleteElement() == i);
                    if (!ok)
                        throw new System.Exception();
                    // System.Diagnostics.Debug.Assert(ok);
                }
            });
            child.Start(sample);
            child2.Start(sample);
            child.Join();
            child2.Join();
            return sample.IsEmpty();
        }
        static void Main(string[] args)
        {
            doit();
        }
    }
}
