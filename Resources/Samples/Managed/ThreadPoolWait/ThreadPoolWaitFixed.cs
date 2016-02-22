/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Threading;
using System.Collections;

namespace NetMatters
{
    /// <summary>ThreadPool utility class that allows for easily waiting on queued delegates.</summary>
    public class ThreadPoolWaitFixed : IDisposable
    {
        /// <summary>Number of items remaining to be executed.</summary>
        private int _remainingWorkItems = 1;
        /// <summary>
        /// Event that signals whether all items have been executed.
        /// Also used as a monitor to protected _remainingWorkItems and set/reset of itself.
        /// </summary>
        private ManualResetEvent _done = new ManualResetEvent(false);

        /// <summary>Queues a user work item to the ThreadPool.</summary>
        /// <param name="callback">The work item to be queued.</param>
        public void QueueUserWorkItem(WaitCallback callback)
        {
            QueueUserWorkItem(callback, null);
        }

        /// <summary>Queues a user work item to the ThreadPool.</summary>
        /// <param name="callback">The work item to be queued.</param>
        /// <param name="state">State to be passed to the queued work item</param>
        public void QueueUserWorkItem(WaitCallback callback, object state)
        {
            ThrowIfDisposed();
            QueuedCallback qc = new QueuedCallback();
            qc.Callback = callback;
            qc.State = state;
            lock (_done) _remainingWorkItems++;
            ThreadPool.QueueUserWorkItem(new WaitCallback(HandleWorkItem), qc);
        }

        /// <summary>Wait for all items to finish executing.</summary>
        public bool WaitOne() { return WaitOne(-1, false); }

        /// <summary>Wait for all items to finish executing.</summary>
        public bool WaitOne(TimeSpan timeout, bool exitContext)
        {
            return WaitOne((int)timeout.TotalMilliseconds, exitContext);
        }

        /// <summary>Wait for all items to finish executing.</summary>
        public bool WaitOne(int millisecondsTimeout, bool exitContext)
        {
            ThrowIfDisposed();
            DoneWorkItem();
            bool rv = _done.WaitOne(millisecondsTimeout, exitContext);
            lock (_done)
            {
                // If the event is set, then we'll return true, but first
                // reset so that this instance can be used again. If the 
                // event isn't set, put back the 1 unit removed by calling 
                // DoneWorkItem.  The next time WaitOne is called,
                // this unit will be removed by the call to DoneWorkItem.
                if (rv)
                {
                    _remainingWorkItems = 1;
                    _done.Reset();
                }
                else _remainingWorkItems++;
            }
            return rv;
        }

        /// <summary>Executes the callback passed as state and signals its completion when it has finished executing.</summary>
        /// <param name="state"></param>
        private void HandleWorkItem(object state)
        {
            QueuedCallback qc = (QueuedCallback)state;
            try { qc.Callback(qc.State); }
            finally { DoneWorkItem(); }
        }

        /// <summary>Decrements the number of remaining items, signaling the ManualResetEvent if 0 are left.</summary>
        private void DoneWorkItem()
        {
            lock (_done)
            {
                --_remainingWorkItems;
                if (_remainingWorkItems == 0) _done.Set();
            }
        }

        /// <summary>State class that wraps a WaitCallback and the state object to be passed to it.</summary>
        private class QueuedCallback
        {
            /// <summary>A callback delegate queued to the ThreadPool.</summary>
            public WaitCallback Callback;
            /// <summary>The state to be passed to the callback.</summary>
            public object State;
        }

        /// <summary>Throws an exception if this instance has already been disposed.</summary>
        private void ThrowIfDisposed()
        {
            if (_done == null) throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>Disposes this ThreadPoolWait.</summary>
        public void Dispose()
        {
            if (_done != null)
            {
                ((IDisposable)_done).Dispose();
                _done = null;
            }
        }
    }
}