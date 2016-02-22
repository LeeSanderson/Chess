/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
using System;
using System.Collections.Generic;
using System.Text;
using Original = global::System.Threading;
using Microsoft.ExtendedReflection.Monitoring;
using System.Runtime.InteropServices;
using System.Security;
using ClrThread = System.Int32;
using ChessTask = System.Int32;
using System.Threading;
using MChess;
using System.Diagnostics;
using Microsoft.ExtendedReflection.Collections;
using Microsoft.ExtendedReflection.Utilities.Safe.Diagnostics;

namespace Microsoft.ManagedChess.EREngine
{
    [DebuggerNonUserCode]
    internal static class Helper
    {
        // this method encapsulates the logic we need in order to properly make sure
        // instrumentation is correct. All instrumentation should use this.
        public delegate T SimpleDel<T>();
        public delegate T SimpleDelManager<T>(ClrSyncManager manager);
        public static T SimpleWrap<T>(SimpleDelManager<T> instrumentedCode, SimpleDel<T> originalCode)
        {
            using (_ProtectingThreadContext.Acquire())
            {
                if(WrapperSentry.Wrap())
                {
                    using (new WrapperSentry())
                    {
                        ClrSyncManager manager = ClrSyncManager.SyncManager;
                        return instrumentedCode(manager);
                    }
                }
                else
                {
                    return originalCode();
                }
            }
        }
        
        public delegate void SimpleDelRef<T>(ref T r);
        public delegate void SimpleDelManagerRef<T>(ClrSyncManager manager, ref T r);
        public static void SimpleWrap<T>(ref T r, SimpleDelManagerRef<T> instrumentedCode, SimpleDelRef<T> originalCode)
        {
            using (_ProtectingThreadContext.Acquire())
            {
                if (WrapperSentry.Wrap())
                {
                    using (new WrapperSentry())
                    {
                        ClrSyncManager manager = ClrSyncManager.SyncManager;
                        instrumentedCode(manager, ref r);
                    }
                }
                else
                {
                    originalCode(ref r);
                }
            }
        }

        // need this for wrappers with callbacks on the same thread
        // instrumentation is correct. All instrumentation should use this.
        public delegate T SimpleDelManagerContext<T>(ClrSyncManager manager, ref IDisposable protectingcontext);
        public static T SimpleContextExposingWrap<T>(SimpleDelManagerContext<T> instrumentedCode, SimpleDel<T> originalCode)
        {
            IDisposable protectingcontext = _ProtectingThreadContext.Acquire();
            try
            {
                if (WrapperSentry.Wrap())
                {
                    using (new WrapperSentry())
                    {
                        ClrSyncManager manager = ClrSyncManager.SyncManager;
                        return instrumentedCode(manager, ref protectingcontext);
                    }
                }
                else
                {
                    return originalCode();
                }
            }
            finally
            {
                protectingcontext.Dispose();
            }
        }

        public delegate T SimpleRefDel<T>(ref T tgt);
        public static T SimpleRefWrap<T>(ref T tgt, SimpleRefDel<T> originalCode, String instrMethod)
        {
            using (_ProtectingThreadContext.Acquire())
            {
                if (WrapperSentry.Wrap())
                {
                    ClrSyncManager manager = ClrSyncManager.SyncManager;
                    manager.SetMethodInfo(instrMethod);
                    if (manager.TrackGC)
                    {
                       return ref_wrap_gcaddr(manager, ref tgt, originalCode);
                    }
                    else if (manager.TrackVolatile)
                    {
                        return ref_wrap_uintptr(manager, ref tgt, originalCode);
                    }
                    else
                    {
                        return originalCode(ref tgt);
                    }
                }
                else
                {
                    return originalCode(ref tgt);
                }
            }
        }

        private static T ref_wrap_gcaddr<T>(ClrSyncManager manager, ref T tgt, SimpleRefDel<T> originalCode)
        {
            GCAddress gca;
            if (GCAddress.FromByRef(ref tgt, out gca))
            {
                try
                {
                    manager.SyncVarAccess(gca, MSyncVarOp.RWVAR_READWRITE);
                    T ret;
                    try
                    {
                        ret = originalCode(ref tgt);
                    }
                    catch (Exception e)
                    {
                        manager.CommitSyncVarAccess();
                        throw e;
                    }
                    manager.CommitSyncVarAccess();
                    return ret;
                }
                finally
                {
                    gca.Release();
                }
            }
            else
            {
                // GC tracking returns false when addr is
                // -	On the stack, or
                // -	Allocated by native code, or
                // -	When it points to certain static fields, and you have only volatile-tracking enabled, but not all read/write accesses.
                return ref_wrap_uintptr<T>(manager, ref tgt, originalCode);
            }
        }

        private static T ref_wrap_uintptr<T>(ClrSyncManager manager, ref T tgt, SimpleRefDel<T> originalCode)
        {
            T ret;
            var ptr = ObjectTracking.GetCurrentRawAddress(ref tgt);
            manager.SyncVarAccess(ptr, MSyncVarOp.RWVAR_READWRITE);
            try
            {
                ret = originalCode(ref tgt);
            }
            catch (Exception e)
            {
                manager.CommitSyncVarAccess();
                throw e;
            }
            manager.CommitSyncVarAccess();
            return ret;
        }

        public delegate bool ReleaseOp(object o);
        public static bool WrapRelease(object o, MSyncVarOp mop, ReleaseOp ro, String instrMethod) {
            return Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    manager.SetMethodInfo(instrMethod);
                    manager.SyncVarAccess(o, mop);
                    bool ret;
                    try { 
                        ret = ro(o); 
                    } catch (Exception e) {
                        manager.CommitSyncVarAccess();
                        throw e;
                    };
                    manager.CommitSyncVarAccess();
                    return ret;
                },
                delegate()
                {
                    return ro(o);
                });
        }

        // specialized WrapRelease for NativeEvents
        public static bool WrapRelease(System.IntPtr o, MSyncVarOp mop, ReleaseOp ro, String instrMethod)
        {
            return Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    manager.SetMethodInfo(instrMethod);
                    manager.SyncVarAccess(manager.GetSyncVarFromNativeHandle((System.IntPtr)o), mop); 
                    bool ret;
                    try
                    {
                        ret = ro(o);
                    }
                    catch (Exception e)
                    {
                        manager.CommitSyncVarAccess();
                        throw e;
                    };
                    manager.CommitSyncVarAccess();
                    return ret;
                },
                delegate()
                {
                    return ro(o);
                });
        }

        public delegate bool BlockingAcquire();
        public delegate bool NonBlockingAcquire();
        public static bool WrapAcquire(BlockingAcquire b, NonBlockingAcquire nb, object o, MSyncVarOp mop, String instrMethod)
        {
            return SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    while (true)
                    {
                        manager.SetMethodInfo(instrMethod);
                        manager.SyncVarAccess(o, mop);
                        try
                        {
                            if (!nb())
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
                        manager.CommitSyncVarAccess();
                        break;
                    };
                    return true;
                },
                delegate()
                {
                    return b();
                });
        }


        public delegate bool QueueEmpty();
        public delegate T BlockingReceive<T>();
        public static T WrapReceive<T>(BlockingReceive<T> b, QueueEmpty qe, object o, MSyncVarOp mop, String instrMethod)
        {
            return SimpleWrap<T>(
                delegate(ClrSyncManager manager)
                {
                    T ret;
                    while (true)
                    {
                        manager.SetMethodInfo(instrMethod);
                        manager.SyncVarAccess(o, mop);
                        try
                        {
                            if (qe())
                            {
                                manager.LocalBacktrack();
                                continue;
                            }
                            ret = b();
                        }
                        catch (Exception e)
                        {
                            manager.CommitSyncVarAccess();
                            throw e;
                        }
                        manager.CommitSyncVarAccess();
                        break;
                    };
                    return ret;
                },
                delegate()
                {
                    return b();
                });
        }

        public delegate Original::Thread MakeThreadDelegate<T>(T del);
        public delegate T WrapThreadStartDelegate<T>(T del, ClrSyncManager manager);

        public static Original::Thread ConstructThread<T>(MakeThreadDelegate<T> mt, T del, WrapThreadStartDelegate<T> wts)
        {
            return Helper.SimpleWrap<Original::Thread>(
                delegate(ClrSyncManager manager)
                {
                    // wrap the delegate
                    Original::Thread ret = mt(wts(del, manager));
                    return ret;
                },
                delegate()
                {
                    return mt(del);
                });
        }

        public static Original::ThreadStart WrapThreadStart(Original::ThreadStart del, ClrSyncManager manager)
        {
            return delegate()
            {
                try
                {
                    Exception exception = null;
                    manager.ThreadBegin(Original::Thread.CurrentThread);
                    MChessChess.LeaveChess();
                    try
                    {
                        CheckThreadReadyForMonitoring();
                        del();
                    }
                    catch (Exception e) // catch recoverable exception in user code
                    {
                        exception = e;
                    }
                    MChessChess.EnterChess();
                    if (manager.BreakDeadlockMode)
                        MChessChess.WakeNextDeadlockedThread(false, true);
                    else if (exception == null)
                        manager.ThreadEnd(Original::Thread.CurrentThread);
                    else
                        manager.Shutdown(exception);
                }
                catch (Exception e) // catch fatal exception in our code
                {
                    manager.Shutdown(e);
                }
            };
        }

        public static Original::ParameterizedThreadStart WrapParamThreadStart(Original::ParameterizedThreadStart del, ClrSyncManager manager)
        {
            return delegate(object o)
            {
                try
                {
                    manager.ThreadBegin(Original::Thread.CurrentThread);
                    MChessChess.LeaveChess();
                    Exception exception = null;
                    try
                    {
                        CheckThreadReadyForMonitoring();
                        del(o);
                    }
                    catch (Exception e) // catch recoverable exception in user code
                    {
                        exception = e;
                    }
                    MChessChess.EnterChess();
                    if (manager.BreakDeadlockMode)
                        MChessChess.WakeNextDeadlockedThread(false, true);
                    else if (exception == null)
                        manager.ThreadEnd(Original::Thread.CurrentThread);
                    else
                        manager.Shutdown(exception);
                }
                catch (Exception e) // catch fatal exception in our code
                {
                    manager.Shutdown(e);
                }
            };
        }

        [Conditional("DEBUG")]
        private static void CheckThreadReadyForMonitoring()
        {
            SafeDebug.Assert(_ThreadContext.IsReady, "_ThreadContext.IsReady");
            //var context = _ThreadContext.Current;
            //if (context == null)
            //{
            //    // SafeDebugger.Break();
            //    context = _ThreadContext.Current;
            //}
            using (_ThreadContext.Acquire())
            {
                // just checking whether everything is okay with this thread for monitoring
            }
        }

        internal class TaskArg
        {
            public System.Threading.Tasks.TaskScheduler taskScheduler;
            public System.Threading.Tasks.Task task;
            public ManualResetEvent e;
            public Semaphore s;
        }

        public static Action<Object> TaskCreateWrapper(ClrSyncManager manager)
        {
            return
                delegate(Object o)
                {
                    try
                    {
                        TaskArg p = (TaskArg) o;
                        p.e.Set();
                        manager.ThreadBegin(p.s);
                        MChessChess.LeaveChess();
                        Exception exception = null;
                        try
                        {
                            var tryExecuteTask = p.taskScheduler.GetType().GetMethod("TryExecuteTask",
                                global::System.Reflection.BindingFlags.NonPublic |
                                global::System.Reflection.BindingFlags.Instance);
                            tryExecuteTask.Invoke(p.taskScheduler, new object[] { p.task });
                        }
                        catch (Exception e) // catch recoverable exception in user code
                        {
                            exception = e;
                        }
                        MChessChess.EnterChess();
                        manager.SetMethodInfo("Task.End");
                        manager.AggregateSyncVarAccess(
                            new object[] {p.task, ((IAsyncResult)p.task).AsyncWaitHandle}, 
                            MSyncVarOp.RWEVENT);
                        manager.CommitSyncVarAccess();
                        if (manager.BreakDeadlockMode)
                            MChessChess.WakeNextDeadlockedThread(false, true);
                        else if (exception == null)
                            manager.ThreadEnd(Original::Thread.CurrentThread);
                        else
                            manager.Shutdown(exception);
                    }
                    catch (Exception e) // catch fatal exception in our code
                    {
                        manager.Shutdown(e);
                    }
                };
        }

        internal class ThreadRoutineArg
        {
            public Semaphore s;
            public object o;
            public WaitCallback wcb;
        }

        public static WaitCallback ThreadCreateWrapper(ClrSyncManager manager) 
        {
            return
                delegate(object o)
                {
                    try
                    {
                        ThreadRoutineArg p = (ThreadRoutineArg)o;
                        manager.ThreadBegin(p.s);
                        MChessChess.LeaveChess();
                        Exception exception = null;
                        try
                        {
                            p.wcb(p.o);
                        }
                        catch (Exception e) // catch recoverable exception in user code
                        {
                            exception = e;
                        }
                        MChessChess.EnterChess();
                        if (manager.BreakDeadlockMode)
                            MChessChess.WakeNextDeadlockedThread(false, true);
                        else if (exception == null)
                            manager.ThreadEnd(Original::Thread.CurrentThread);
                        else
                            manager.Shutdown(exception);
                    }
                    catch (Exception e) // catch fatal exception in our code
                    {
                        manager.Shutdown(e);
                    }
                };
        }

        internal class WaitOrTimerCallbackRoutineArg
        {
            public Semaphore s;
            public AutoResetEvent a;
            public WaitHandle waitObject;
            public WaitOrTimerCallback callback;
            public object state;
            public long millisecondsTimeOutInterval;
            public bool executeOnlyOnce;
            public bool canceled;
            public WaitHandle onCancellation;
            public bool finished;
        }
        
        public static void StartHelper(Original::Thread t, object o, bool parameterized)
        {
            Helper.SimpleDel<bool> common = delegate()
            {
                if (parameterized)
                    t.Start(o);
                else
                    t.Start();
                return false;
            };
            Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    ChessTask child = manager.TaskFork();
                    Original.Semaphore childSem = new Original.Semaphore(0, 1);
                    manager.RegisterTaskSemaphore(child, childSem, true);
                    manager.AddChildHandle(child, t);
                    manager.TaskResume(child);
                    common();
                    return false;
                },
                common);
        }
    }

    [DebuggerNonUserCode]
    internal static class WaitOneHelper
    {
        public delegate T WaitOneDelegatemanagerObject<T>(ClrSyncManager manager, Original::Mutex m);

        public static WaitOneDelegatemanagerObject<bool> WaitOneDelegate =
            delegate(ClrSyncManager manager, Original::Mutex m)
            {
                while (true)
                {
                    manager.SetMethodInfo("Mutex.WaitOne");
                    manager.SyncVarAccess(m, MSyncVarOp.WAIT_ANY);
                    try
                    {
                        if (!m.WaitOne(0, false))
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
                    manager.CommitSyncVarAccess();
                    break;
                };
                return true;
            };

        public static WaitOneDelegatemanagerObject<bool> WaitOneDelegateWithTimeout =
            delegate(ClrSyncManager manager, Original::Mutex m)
            {
                bool b;
                manager.SetMethodInfo("Mutex.WaitOne");
                manager.SyncVarAccess(m, MSyncVarOp.WAIT_ANY);
                try
                {
                    b = m.WaitOne(0, false);
                }
                catch (Exception e)
                {
                    manager.CommitSyncVarAccess();
                    throw e;
                }
                if (! b)
                    manager.MarkTimeout();
                manager.CommitSyncVarAccess();
                return b;
            };
    }

    internal interface IJoinable
    {
        // Join with the thread
        void Join();

        string Name();

        // Dispose relevant state
        void Dispose(SyncVarManager svm);
    }

    internal class ProfilerTimer
    {
        private string name;

        public ProfilerTimer(string n)
        {
            name = n;
        }

        ~ProfilerTimer()
        {
            System.Console.WriteLine(name + " : ");
        }
    }

    internal class ThreadJoinable : IJoinable
    {
        public ThreadJoinable(Thread t)
        {
            thread = t;
        }

        public void Join()
        {
            thread.Join();
        }

        public string Name()
        {
            return thread.Name;
        }

        public void Dispose(SyncVarManager svm)
        {
            svm.RemoveThreadHandleMapping(thread);
        }

        public Thread GetThread()
        {
            return thread;
        }
        private Thread thread;
    }
}
