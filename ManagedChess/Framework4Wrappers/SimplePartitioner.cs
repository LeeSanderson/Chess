using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using OThreading = global::System.Threading;
using OTasks = global::System.Threading.Tasks;
using ClrThread = System.Int32;
using ChessTask = System.Int32;
using Microsoft.ManagedChess.EREngine;
using Microsoft.ManagedChess;
using Microsoft.ExtendedReflection.Monitoring;


namespace __Substitutions.System.Threading.Tasks
{
    class SimplePartitioner<T> : OrderablePartitioner<T>, IEnumerable<KeyValuePair<long, T>>
    {
        ClrSyncManager manager;
        public IDisposable protectingcontext; // set by caller

        private long min;
        private long max;
        private long split;
        private Func<long, T> get;

        private long forwardpos;
        private long backwardpos;

        ChessTask taskid;
        private OThreading.Thread mainthread;
        private OThreading.Thread childthread;
        private OThreading.Semaphore semaphore;
        OThreading.Barrier barrier;
        bool childdone;

        public SimplePartitioner(ClrSyncManager manager, 
                                 long start, long end, long split, Func<long, T> get, bool keysNormalized) :
            base(false, false, keysNormalized)
        {
            this.manager = manager;
            this.min = start;
            this.max = end - 1;
            this.split = split;
            this.get = get;
            this.forwardpos = min;
            this.backwardpos = max;
            barrier = new OThreading.Barrier(2);
            semaphore = new OThreading.Semaphore(0, 1);
            mainthread = OThreading.Thread.CurrentThread;
            //global::System.Console.WriteLine("SimplePartitioner constructed by " + OThreading.Thread.CurrentThread.ManagedThreadId);
        }

        private IEnumerator<KeyValuePair<long, T>> Enumerate()
        {
            if (OThreading.Thread.CurrentThread == mainthread)
                return EnumerateForward();
            else
                return EnumerateBackward();
        }

        // called by the main thread
        private IEnumerator<KeyValuePair<long, T>> EnumerateForward()
        {
            global::System.Diagnostics.Debug.Assert(OThreading.Thread.CurrentThread == mainthread);

            // synchronize with child thread (first handshake)
            barrier.SignalAndWait();

            // let chess think we are forking a new thread here
            taskid = manager.TaskFork();
            manager.RegisterTaskSemaphore(taskid, semaphore, false);
            manager.AddChildHandle(taskid, childthread); // thread is now known because it was set before barrier
            manager.TaskResume(taskid);

            // synchronize with child thread (second handshake)
            barrier.SignalAndWait();

            for (long i = min; i < split; i++)
            {
                T t = get(i);
                //MChessChess.TraceEvent("forward enum:" + i + ":" + t.ToString());
                yield return new KeyValuePair<long, T>(i, t);
            }

            // we're done... wait for child thread
            EndMainThread();
        }

        // called by the child thread
        private IEnumerator<KeyValuePair<long, T>> EnumerateBackward()
        {
            childthread = OThreading.Thread.CurrentThread; // set this before barrier so main thread can read it after barrier
            global::System.Diagnostics.Debug.Assert(childthread != mainthread);

            // synchronize with main thread (first handshake)
            barrier.SignalAndWait();

            // synchronize with main thread (second handshake)
            barrier.SignalAndWait();

            manager.ThreadBegin(childthread);

            // capture thread
            for (long i = max; i >= split; i--)
            {
                T t = get(i);
                //MChessChess.TraceEvent("backward enum:" + i + ":" + t.ToString());
                yield return new KeyValuePair<long, T>(i, get(i));
            }

            // we're done ... terminate chess thread
            EndChildThread();
        }

        public Action<T, OTasks.ParallelLoopState> WrapBody(Action<T> body)
        {
            return (T t, OTasks.ParallelLoopState ls) => 
                {
                    Exception exception = null;
                    MChessChess.TraceEvent("body(" + t.ToString() + ")");
                    MChessChess.LeaveChess();
                    if (OThreading.Thread.CurrentThread == mainthread)
                        protectingcontext.Dispose();
                    try
                    {
                        body(t);
                    }
                    catch (Exception e) // catch recoverable exception in user code
                    {
                        exception = e;
                    }
                    if (OThreading.Thread.CurrentThread == mainthread)
                         protectingcontext = _ProtectingThreadContext.Acquire();
                    MChessChess.EnterChess();
                    if (exception != null)
                    {
                        global::System.Diagnostics.Debug.Fail("Not implemented: exceptions in Parallel loops");
                        throw exception; // once we implement this we'll rethrow and let TPL catch & aggregate it
                    }
                };
        }

        private void EndMainThread()
        {
            // join child thread
            while (true)
            {
                manager.SyncVarAccess(taskid, MSyncVarOp.TASK_JOIN);
                if (childdone)
                    break; // done
                manager.LocalBacktrack();
            }
            manager.CommitSyncVarAccess();
        }

        
        private void EndChildThread()
        {

            if (manager.BreakDeadlockMode)
                MChessChess.WakeNextDeadlockedThread(false, true);
            else
            {
                childdone = true;
                manager.ThreadEnd(childthread);
            }
         }

        #region interface to TPL

        // Produces a list of "numPartitions" IEnumerators that can each be
        // used to traverse the underlying collection in a thread-safe manner.
        // This will return a static number of enumerators, as opposed to
        // GetOrderableDynamicPartitions(), the result of which can be used to produce
        // any number of enumerators.
        public override IList<IEnumerator<KeyValuePair<long, T>>> GetOrderablePartitions(int numPartitions)
        {
            if (numPartitions == 1)
                throw new Exception("Internal Error: must use exactly two partitions");
            else if (numPartitions == 2)
                return new IEnumerator<KeyValuePair<long, T>>[2] { Enumerate(), Enumerate() };
            else
                throw new ArgumentOutOfRangeException("NumPartitions");
        }

        // Returns an instance of our internal Enumerable class.  GetEnumerator()
        // can then be called on that (up to two times) to produce enumerators.
        public override IEnumerable<KeyValuePair<long, T>> GetOrderableDynamicPartitions()
        {
            return this;
        }

        // Must be set to true if GetDynamicPartitions() is supported.
        public override bool SupportsDynamicPartitions
        {
            get { return true; }
        }


        int enumeratorcount;

        public IEnumerator<KeyValuePair<long, T>> GetEnumerator()
        {
            enumeratorcount++;
            //global::System.Console.WriteLine("GetEnumerator() call " + enumeratorcount + " by " + OThreading.Thread.CurrentThread.ManagedThreadId);

            if (enumeratorcount <= 2)
                return Enumerate();
            else
                throw new Exception("May not use more than 2 enumerators");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<long, T>>)this).GetEnumerator();
        }

        #endregion

    }

}
