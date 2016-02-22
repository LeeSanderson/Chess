/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
using System;
using System.Collections.Generic;
using System.Text;
using Original = global::System.Threading;
using Microsoft.ExtendedReflection.Monitoring;
using Microsoft.ExtendedReflection.Collections;
using System.Runtime.InteropServices;
using System.Security;
using ClrThread = System.Int32;
using ChessTask = System.Int32;
using System.Threading;
using System.Diagnostics;

using MChess;
using Microsoft.ManagedChess;
using Microsoft.ManagedChess.EREngine;

namespace __Substitutions.System.Threading
{
    [DebuggerNonUserCode]
    public static partial class Monitor
    {
        #region .Net 2.0 Enter documentation
        // Use Enter to acquire the Monitor on the object passed as the parameter.
        // If another thread has executed an Enter on the object, but has not yet 
        // executed the corresponding Exit, the current thread will block until the 
        // other thread releases the object. It is legal for the same thread to invoke
        // Enter more than once without it blocking; however, an equal number of Exit calls 
        // must be invoked before other threads waiting on the object will unblock.

        // Use Monitor to lock objects (that is, reference types), not value types. 
        // When you pass a value type variable to Enter, it is boxed as an object. 
        // If you pass the same variable to Enter again, it is boxed as a separate object, 
        // and the thread does not block. The code that Monitor is supposedly protecting is not protected. 
        // Furthermore, when you pass the variable to Exit, still another separate object is created. 
        // Because the object passed to Exit is different from the object passed to Enter, Monitor throws 
        // SynchronizationLockException. For details, see the conceptual topic Monitors.

        // Interrupt can interrupt threads waiting to enter a Monitor on an object. 
        // A ThreadInterruptedException will be thrown.
        #endregion
        public static void Enter(object @lock)
        {
            TryEnterRaw(@lock, Timeout.Infinite, MSyncVarOp.LOCK_ACQUIRE, "Monitor.Enter");
        }

        public static void Enter(object obj, ref bool tookLock)
        {
            global::System.Diagnostics.Debug.Assert(!tookLock);
            TryEnterRaw(obj, Timeout.Infinite, MSyncVarOp.LOCK_ACQUIRE, "Monitor.Enter");
            tookLock = true;
        }

        public static bool TryEnter(object @lock) {
            return TryEnterRaw(@lock, 0, MSyncVarOp.LOCK_TRYACQUIRE, "Monitor.TryEnter");    
        }

        public static void TryEnter(object @lock, ref bool tookLock)
        {
            global::System.Diagnostics.Debug.Assert(!tookLock);
            tookLock = TryEnterRaw(@lock, 0, MSyncVarOp.LOCK_TRYACQUIRE, "Monitor.TryEnter");
        }

        // Monitor.TryEnter (Object, Int32)  Attempts, for the specified number of milliseconds, to 
        //   acquire an exclusive lock on the specified object.  
        public static bool TryEnter(object @lock, int timeOut)
        {
            MSyncVarOp mop = (timeOut == Timeout.Infinite) ? MSyncVarOp.LOCK_ACQUIRE : MSyncVarOp.LOCK_TRYACQUIRE;
            return TryEnterRaw(@lock, timeOut, mop, "Monitor.TryEnter");    
        }

        public static void TryEnter(object obj, int millisecondsTimeout, ref bool tookLock)
        {
            global::System.Diagnostics.Debug.Assert(!tookLock);
            MSyncVarOp mop = (millisecondsTimeout == Timeout.Infinite) ? MSyncVarOp.LOCK_ACQUIRE : MSyncVarOp.LOCK_TRYACQUIRE;
            tookLock = TryEnterRaw(obj, millisecondsTimeout, mop, "Monitor.TryEnter");
        }

        // Monitor.TryEnter (Object, TimeSpan)  Attempts, for the specified amount of time, to acquire 
        //   an exclusive lock on the specified object.
        public static bool TryEnter(object @lock, TimeSpan timeOut)
        {
            MSyncVarOp mop = (timeOut.TotalMilliseconds == Timeout.Infinite) ? MSyncVarOp.LOCK_ACQUIRE : MSyncVarOp.LOCK_TRYACQUIRE;
            return TryEnterRaw(@lock, (int) timeOut.TotalMilliseconds, mop, "Monitor.TryEnter");
        }

        public static void TryEnter(object obj, TimeSpan millisecondsTimeout, ref bool tookLock)
        {
            global::System.Diagnostics.Debug.Assert(!tookLock);
            MSyncVarOp mop = (millisecondsTimeout.TotalMilliseconds == Timeout.Infinite) ? MSyncVarOp.LOCK_ACQUIRE : MSyncVarOp.LOCK_TRYACQUIRE;
            tookLock = TryEnterRaw(obj, (int) millisecondsTimeout.TotalMilliseconds, mop, "Monitor.TryEnter");
        }	

        // Monitor.TryEnter (Object)  Attempts to acquire an exclusive lock on the specified object. 
        public static bool TryEnterRaw(object @lock, int timeOut, MSyncVarOp operation, string name)
        {
            if (@lock == null)
                throw new ArgumentNullException();
            if (timeOut < 0 && timeOut != Timeout.Infinite)
                throw new ArgumentOutOfRangeException();
            return Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    bool flag;
                    while (true)
                    {
                        try
                        {
                            manager.SetMethodInfo(name);
                            manager.SyncVarAccess(@lock, operation);
                            flag = Original::Monitor.TryEnter(@lock);
                            if (!flag && timeOut == Timeout.Infinite)
                            {
                                manager.LocalBacktrack();
                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                            manager.CommitSyncVarAccess();
                            throw e;
                        }
                        if (flag)
                        {
                            MonitorHelper.IncrementReentrancyCount(@lock);
                        }
                        if (!flag)
                            manager.MarkTimeout();
                        manager.CommitSyncVarAccess();
                        break;
                    }
                    return flag;
                },
                delegate()
                {
                    return Original::Monitor.TryEnter(@lock, timeOut);
                });
        }


        #region .Net 2.0 Exit documentation
        // The calling thread must own the lock on the obj parameter. If the calling thread owns the lock on 
        // the specified object, and has made an equal number of Exit and Enter calls for the object, then the 
        // lock is released. If the calling thread has not invoked Exit as many times as Enter, the lock is not released.

        // If the lock is released and other threads are in the ready queue for the object, 
        // one of the threads acquires the lock. If other threads are in the waiting queue waiting 
        // to acquire the lock, they are not automatically moved to the ready queue when the owner of 
        // the lock calls Exit. To move one or more waiting threads into the ready queue, call Pulse or 
        // PulseAll before invoking Exit.
        #endregion
        public static void Exit(object @lock)
        {
            if (@lock == null)
                throw new ArgumentNullException();
            Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    manager.SetMethodInfo("Monitor.Exit");
                    manager.SyncVarAccess(@lock, MSyncVarOp.LOCK_RELEASE);
                    try
                    {
                        Original::Monitor.Exit(@lock);
                    }
                    catch (Exception e)
                    {
                        manager.CommitSyncVarAccess();
                        throw e;
                    }
                    MonitorHelper.DecrementReentrancyCount(@lock);
                    manager.CommitSyncVarAccess();
                    return false;
                },
                delegate()
                {
                    Original::Monitor.Exit(@lock);
                    return false;
                });
        }

        // TODO: other Monitor methods

        #region .Net 2.0 Pulse documentation
        // Only the current owner of the lock can signal a waiting object using Pulse.
        // The thread that currently owns the lock on the specified object invokes this method 
        // to signal the next thread in line for the lock. Upon receiving the pulse, the waiting 
        // thread is moved to the ready queue. When the thread that invoked Pulse releases the lock,
        // the next thread in the ready queue (which is not necessarily the thread that was pulsed) 
        // acquires the lock.

        // Important:  
        // The Monitor class does not maintain state indicating that the Pulse method has been called. 
        // Thus, if you call Pulse when no threads are waiting, the next thread that calls Wait blocks as 
        // if Pulse had never been called. If two threads are using Pulse and Wait to interact, this could 
        // result in a deadlock. Contrast this with the behavior of the AutoResetEvent class: If you signal
        // an AutoResetEvent by calling its Set method, and there are no threads waiting, the AutoResetEvent 
        // remains in a signaled state until a thread calls WaitOne, WaitAny, or WaitAll. The AutoResetEvent 
        // releases that thread and returns to the unsignaled state.

        // Note that a synchronized object holds several references, including a reference to the thread that 
        // currently holds the lock, a reference to the ready queue, which contains the threads that are ready 
        // to obtain the lock, and a reference to the waiting queue, which contains the threads that are waiting 
        // for notification of a change in the object's state. 

        // The Pulse, PulseAll, and Wait methods must be invoked from within a synchronized block of code. 
        #endregion
        public static void Pulse(object @lock)
        {
            if (@lock == null)
                throw new ArgumentNullException();
            Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    manager.SetMethodInfo("Monitor.Pulse");
                    manager.SyncVarAccess(@lock, MSyncVarOp.LOCK_RELEASE);
                    MonitorHelper.RemoveTaskFromWaitQueue(@lock);
                    manager.CommitSyncVarAccess();
                    return false;
                },
                delegate()
                {
                    Original::Monitor.Pulse(@lock);
                    return false;
                });
        }

        #region .Net 2.0 PulseAll documentation
        // The thread that currently owns the lock on the specified object invokes this method to signal 
        // all threads waiting to acquire the lock on the object. After the signal is sent, the waiting 
        // threads are moved to the ready queue. When the thread that invoked PulseAll releases the lock, 
        // the next thread in the ready queue acquires the lock.
        #endregion
        public static void PulseAll(object @lock)
        {
            if (@lock == null)
                throw new ArgumentNullException();
            Helper.SimpleWrap<bool>(
            delegate(ClrSyncManager manager)
            {
                manager.SetMethodInfo("Monitor.PulseAll");
                manager.SyncVarAccess(@lock, MSyncVarOp.LOCK_RELEASE);
                MonitorHelper.RemoveAllTasksFromWaitQueue(@lock);
                manager.CommitSyncVarAccess();
                return false;
            },
            delegate()
            {
                Original::Monitor.PulseAll(@lock);
                return false;
            });
        }

        // Monitor.Wait (Object)  
        // - Releases the lock on an object and blocks the current thread until it reacquires the lock.
        public static bool Wait(object @lock)
        {
            return WaitRaw(@lock, Timeout.Infinite, false);
        }

        // Monitor.Wait (Object, Int32)
        // - Releases the lock on an object and blocks the current thread until it reacquires the lock.
        // - If the specified time-out interval elapses, the thread enters the ready queue.
        public static bool Wait(object @lock, int timeOut)
        {
            return WaitRaw(@lock, timeOut, false);
         }

        // Monitor.Wait (Object, TimeSpan)
        // - Releases the lock on an object and blocks the current thread until it reacquires the lock. 
        // - If the specified time-out interval elapses, the thread enters the ready queue.
        public static bool Wait(object @lock, TimeSpan timeOut)
        {
            return WaitRaw(@lock, (int) timeOut.TotalMilliseconds, false);
         }

        // Monitor.Wait (Object, Int32, Boolean)
        // - Releases the lock on an object and blocks the current thread until it reacquires the lock. 
        // - If the specified time-out interval elapses, the thread enters the ready queue. 
        // - This method also specifies whether the synchronization domain for the context (if in a synchronized context) 
        // - is exited before the wait and reacquired afterward.
        public static bool Wait(object @lock, int timeOut, bool domain)
        {
            return WaitRaw(@lock, timeOut, domain);
        }

        // Monitor.Wait (Object, TimeSpan, Boolean)
        // - Releases the lock on an object and blocks the current thread until it reacquires the lock. 
        // - If the specified time-out interval elapses, the thread enters the ready queue. 
        // - Optionally exits the synchronization domain for the synchronized context before the wait and 
        // - reacquires the domain afterward. 
        public static bool Wait(object @lock, TimeSpan timeOut, bool domain)
        {
            return WaitRaw(@lock, (int) timeOut.TotalMilliseconds, domain);
         }

        private static bool WaitRaw(object @lock, int timeOut, bool domain)
        {
            if (@lock == null)
                throw new ArgumentNullException();
            if (timeOut < 0 && timeOut != Timeout.Infinite)
                throw new ArgumentOutOfRangeException();
            return Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    if (timeOut == Timeout.Infinite)
                        return MonitorHelper.WaitDelegate(manager, @lock, "Monitor.Wait");
                    else
                        return MonitorHelper.WaitDelegateWithTimeout(manager, @lock, "Monitor.Wait");
                },
                delegate()
                {
                    return Original::Monitor.Wait(@lock, timeOut, domain);
                });
        }
    }
}