/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// from MSDN

using System;
using System.Collections;
using System.Threading;

namespace Test20 {
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
        private Queue m_inputQueue;

        public MonitorSample()
        {
            m_inputQueue = new Queue(); 
        }

        public bool IsEmpty() 
        {
            return (m_inputQueue.Count == 0);
        }
       
        //Add an element to the queue and obtain the monitor lock for the queue object.
        public void AddElement(object qValue)
        {
            //Lock the queue.
            Monitor.TryEnter(m_inputQueue, Timeout.Infinite);
            //Add element
            m_inputQueue.Enqueue(qValue);
            //Unlock the queue.
            Monitor.Exit(m_inputQueue);
        }
        
        //Delete all elements that equal the given object and obtain the monitor lock for the queue object.
        public void DeleteElement(object qValue)
        {
            //Lock the queue.
            Monitor.TryEnter(m_inputQueue, Timeout.Infinite);
            int counter = m_inputQueue.Count;
            while(counter > 0)
            {    
                //Check each element.
                object elm = m_inputQueue.Dequeue();
                if(!elm.Equals(qValue))
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
            Monitor.TryEnter(m_inputQueue, Timeout.Infinite);            
            IEnumerator elmEnum = m_inputQueue.GetEnumerator();
            while(elmEnum.MoveNext())
            {
                //Print the next element.
                Console.WriteLine(elmEnum.Current.ToString());
            }
            //Unlock the queue.
            Monitor.Exit(m_inputQueue);    
        }

        public static bool doit() {
            MonitorSample sample = new MonitorSample();
            Thread child = new Thread(delegate(object o) {
                MonitorSample s = (MonitorSample)o;
                for(int i = 5; i < 10; i++)
                   s.AddElement(i);
                
            });
            Thread child2 = new Thread(delegate(object o) {
                MonitorSample s = (MonitorSample)o;
                for(int i = 0; i < 5; i++)
                   s.AddElement(i);
                
            });
            child.Start(sample);
            child2.Start(sample);
    	    child.Join();
	        child2.Join();
            for(int i = 0; i < 10; i++)
                sample.DeleteElement(i);
            return sample.IsEmpty();
        }
        static void Main(string[] args)
        {
            doit();
        }
    }
}
