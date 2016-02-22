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

namespace PFXWrappers
{
    internal class ChessEnumerable<T> : IParallelEnumerable<T>
    {
        IParallelEnumerable<T> e;
        Func<T, bool> predicate;
        public ChessEnumerable(IParallelEnumerable<T> e, Func<T, bool> predicate)
        {
            this.e = e;
            this.predicate = predicate;
        }

        #region IParallelEnumerable<T> Members

        public IEnumerator<T> GetEnumerator(bool usePipelining)
        {
            return new Enumerator(e, predicate);
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(e, predicate);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerator(e, predicate);
        }

        #endregion

        internal class Enumerator : IEnumerator<T>, System.Collections.IEnumerator
        {
            internal class Item
            {
                public Thread t;
                public T val;
                public bool valid;

                public Item(Thread t, T s)
                {
                    this.t = t;
                    this.val = s;
                    this.valid = false;
                }
            }

            List<Item> items;
            IEnumerator<Item> itemEnumerator;

            public Enumerator(IParallelEnumerable<T> source, Func<T, bool> predicate)
            {
                items = new List<Item>();
                ParameterizedThreadStart threadStart =
                    (object o) =>
                    {
                        Item item = (Item)o;
                        if (predicate(item.val))
                            item.valid = true;
                    };
                foreach (var s in source)
                {
                    var t = new Thread(threadStart);
                    Item item = new Item(t, s);
                    items.Add(item);
                    t.Start(item);
                }
                itemEnumerator = items.GetEnumerator();
            }

            #region IEnumerator Members

            public T Current
            {
                get
                {
                    while (true)
                    {
                        Item item = itemEnumerator.Current;
                        item.t.Join();
                        if (item.valid)
                            return item.val;
                        if (!itemEnumerator.MoveNext())
                            throw new InvalidOperationException();
                    }
                }
            }

            public bool MoveNext()
            {
                while (true)
                {
                    if (!itemEnumerator.MoveNext())
                        return false;
                    Item item = itemEnumerator.Current;
                    item.t.Join();
                    if (item.valid)
                        return true;
                }
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IEnumerator<T> Members

            T IEnumerator<T>.Current
            {
                get { return this.Current; }
            }
            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                foreach (Item item in items)
                {
                    item.t.Join();
                }
                itemEnumerator.Dispose();
            }

            #endregion
        }
    }
}
