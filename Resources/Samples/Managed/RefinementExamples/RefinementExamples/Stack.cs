/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RefinementExamples
{
    public interface IStack<T>
    {
        void Push(T elt);
        bool Pop(out T elt);
        void Clear();
        bool Contains(T elt);
    }

    public class LockfreeStack<T> : IStack<T>
    {
        protected class Node 
        {
            public T elt;
            public volatile Node next;
        }
        
        volatile Node head = null;

        public void Push(T elt)
        {
            Node newhead = new Node();
            newhead.elt = elt;
            Node oldhead;
            do
            {
                oldhead = head;
                newhead.next = oldhead;
            }
            while (Interlocked.CompareExchange<Node>(ref head, newhead, oldhead) != oldhead);
        }

        public bool Pop(out T elt)
        {
            Node oldhead, newhead;
            do
            {
                oldhead = head;
                if (oldhead == null)
                {
                    elt = default(T);
                    return false;
                }
                newhead = oldhead.next;
            }
            while (Interlocked.CompareExchange<Node>(ref head, newhead, oldhead) != oldhead);
            elt = oldhead.elt;
            return true;
       }

        
        public void Clear()
        {
            head = null;
        }

        public bool Contains(T elt)
        {
            Node cur = head;
            while (cur != null)
            {
                if (cur.elt.Equals(elt))
                    return true;
                cur = cur.next;
            }
            return false;
        }
    }
}
