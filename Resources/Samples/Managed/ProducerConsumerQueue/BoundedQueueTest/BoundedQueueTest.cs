/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using ProducerConsumerQueue;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace BoundedQueueTest
{
    /// <summary>
    ///This is a test class for BoundedQueueTest and is intended
    ///to contain all BoundedQueueTest Unit Tests
    ///</summary>
    public class BoundedQueueTest
    {

        //[ExpectedException(typeof(System.ArgumentException), "Bound must be a positive number")]
        public void IllegalBound()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(0);
        }

        public void TwoInTwoOut()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(2);
            queue.Enqueue(1);
            queue.Enqueue(2);
            Assert.AreEqual(queue.Dequeue(), 1);
            Assert.AreEqual(queue.Dequeue(), 2);
        }


        //[TestProperty("ChessExpectedResult", "Deadlock")]
        public void NoConsumer()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(10);
            int i = 0;
            while (true)
                queue.Enqueue(i++);
        }

        //[TestProperty("ChessExpectedResult", "Deadlock")]
        public void NoProducer()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(10);
            while (true)
                queue.Dequeue();
        }

        public void OneProdOneCons()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(2);

            ParallelTasks ptasks = new ParallelTasks();

            // PRODUCER thread inserts 4 elements
            ptasks.Add("Producer", () =>
                {
                    for (int i = 0; i < 4; i++)
                        queue.Enqueue(i);
                });

            // CONSUMER thread removes 4 elements
            ptasks.Add("Consumer", () =>
                 {
                      for (int i = 0; i < 4; i++)
                        Assert.AreEqual(i, queue.Dequeue());
                 });

            ptasks.Execute();

            // check size: should be zero
            Assert.AreEqual(queue.Size(), 0);
        }

        public void OneProdTwoCons()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(2);

            ParallelTasks ptasks = new ParallelTasks();

            // PRODUCER thread inserts 6 elements
            ptasks.Add("Producer", () =>
            {
                for (int i = 0; i < 6; i++)
                    queue.Enqueue(i);
            });

            // CONSUMER thread 1 removes 3 elements
            ptasks.Add("Consumer1", () =>
            {
                for (int i = 0; i < 3; i++)
                    Assert.IsTrue(i <= queue.Dequeue());
            });

            // CONSUMER thread 2 removes 3 elements
            ptasks.Add("Consumer2", () =>
            {
                for (int i = 0; i < 3; i++)
                    Assert.IsTrue(i <= queue.Dequeue());
            });

            ptasks.Execute();

            // check size: should be zero
            Assert.AreEqual(queue.Size(), 0);
        }

        public void TwoProdOneCons()
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
                    Assert.IsTrue(i >= queue.Dequeue());
            });

            ptasks.Execute();

            // check size: should be two
            Assert.AreEqual(queue.Size(), 2);
        }

        public void OneProdOneClear()
        {
            BoundedQueue<int> queue = new BoundedQueue<int>(2);

            ParallelTasks ptasks = new ParallelTasks();

            // PRODUCER thread inserts 4 elements
            ptasks.Add("Producer", () =>
            {
                for (int i = 0; i < 4; i++)
                    queue.Enqueue(i);
            });

            // CLEAR thread clears queue once it reaches size 2
            ptasks.Add("Clear", () =>
            {
                while (queue.Size() < 2)
                   Thread.Sleep(0);
                queue.Clear();
            });

            ptasks.Execute();
        }

    }
}
