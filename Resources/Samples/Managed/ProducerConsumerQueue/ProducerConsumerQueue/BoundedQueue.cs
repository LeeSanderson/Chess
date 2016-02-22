/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProducerConsumerQueue
{

#if ! BUGSFIXED

    public class BoundedQueue<ElementType>
    {
        private LinkedList<ElementType> queue = new LinkedList<ElementType>();
        private int bound;

        //
        // Create a Queue object, with capacity bounded by bound.
        //
        public BoundedQueue(int bound)
        {
            if (bound <= 0)
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

#else

    public class BoundedQueue<ElementType>
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
                System.Threading.Monitor.PulseAll(this); // FIX : need this line
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
                while (queue.Count == bound) // FIX : need "while", not "if"
                    System.Threading.Monitor.Wait(this);

                // add the element to the queue
                queue.AddLast(element);

                // wake up potentially waiting consumers
                if (queue.Count == 1)
                    System.Threading.Monitor.PulseAll(this); // FIX : need "PulseAll", not "Pulse"
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
                while (queue.Count == 0) // FIX : need "while", not "if"
                    System.Threading.Monitor.Wait(this);

                // remove an element from the queue
                element = queue.First.Value;
                queue.RemoveFirst();

                // wake up potentially waiting producers
                if (queue.Count == (bound - 1))
                    System.Threading.Monitor.PulseAll(this); // FIX : need "PulseAll", not "Pulse"
            }
            // return the dequeued element
            return element;
        }

        // 
        // return the number of elements currently in the queue.
        //
        public int Size()
        {
            lock (this) // FIX: need to lock "this", not "queue"
                        // NOTE: Chess did not find this bug
                return queue.Count;
        }


    }

}
#endif