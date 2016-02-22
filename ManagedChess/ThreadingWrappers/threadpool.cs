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
using Microsoft.ManagedChess.EREngine;
using System.Diagnostics;
using Microsoft.ExtendedReflection.Collections;

namespace Microsoft.ManagedChess.EREngine
{
    internal static class RegisterWaitHelper
    {
        public static WaitOrTimerCallback RegisterWaitForSingleObjectWrapper(ClrSyncManager manager)
        {
            return
                delegate(object argument, bool timedOut)
                {
                    try
                    {
                        global::System.Diagnostics.Debug.Assert(!timedOut);
                        Helper.WaitOrTimerCallbackRoutineArg p = (Helper.WaitOrTimerCallbackRoutineArg)argument;
                        p.a.Set();

                        manager.ThreadBegin(p.s);
                        WaitHandle cancelObject = null;

                        while (true)
                        {
                            manager.SetMethodInfo("RegisteredWaitForSingleObject.CheckCancellation");
                            manager.SyncVarAccess(p, MSyncVarOp.RWVAR_READWRITE);
                            bool cancelled = p.canceled;
                            cancelObject = p.onCancellation;
                            if (cancelled)
                                p.finished = true;
                            manager.CommitSyncVarAccess();
                            if (cancelled) break;

                            MChessChess.LeaveChess();
                            bool flag = __Substitutions.System.Threading.WaitHandle.WaitOne(p.waitObject, (int)p.millisecondsTimeOutInterval, false);
                            p.callback(p.state, !flag);
                            MChessChess.EnterChess();

                            if (p.executeOnlyOnce)
                            {
                                manager.SetMethodInfo("RegisteredWaitForSingleObject.ReadOnCancellation");
                                manager.SyncVarAccess(p, MSyncVarOp.RWVAR_READWRITE);
                                cancelObject = p.onCancellation;
                                p.finished = true;
                                manager.CommitSyncVarAccess();
                                break;
                            }
                        }

                        MChessChess.LeaveChess();
                        Exception exception = null;
                        try
                        {
                            if (cancelObject != null)
                            {
                                __Substitutions.System.Threading.EventWaitHandle.Set((EventWaitHandle)cancelObject);
                            }
                        }
                        catch (Exception e) // catch recoverable exception in monitored code
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
    }
}

namespace __Substitutions.System.Threading
{
    [DebuggerNonUserCode]
    public static partial class ThreadPool
    {
        public static bool QueueUserWorkItem(WaitCallback callBack)
        {
            return QueueUserWorkItem(callBack, null);
        }

        public static bool QueueUserWorkItem(WaitCallback callBack, object state)
        {
            return QueueUserWorkItemHelper(callBack, state, true);
        }

        public static bool UnsafeQueueUserWorkItem(WaitCallback callBack, object state)
        {
            return QueueUserWorkItemHelper(callBack, state, false);
        }

        public static bool QueueUserWorkItemHelper(WaitCallback callBack, object state, bool isSafe)
        {
            return Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    Helper.ThreadRoutineArg p = new Helper.ThreadRoutineArg();
                    p.s = new Original::Semaphore(0, 1);
                    p.o = state;
                    p.wcb = callBack;
                    bool flag = isSafe ? Original.ThreadPool.QueueUserWorkItem(Helper.ThreadCreateWrapper(manager), p)
                                      : Original.ThreadPool.UnsafeQueueUserWorkItem(Helper.ThreadCreateWrapper(manager), p);
                    if (flag)
                    {
                        ChessTask child = manager.TaskFork();
                        manager.RegisterTaskSemaphore(child, p.s, false);
                        manager.TaskResume(child);
                    }
                    return flag;
                },
                delegate()
                {
                    return isSafe ? Original.ThreadPool.QueueUserWorkItem(callBack, state)
                                  : Original.ThreadPool.UnsafeQueueUserWorkItem(callBack, state);
                }
            );
        }

        static Original.RegisteredWaitHandle RegisterWaitForSingleObjectHelper(
            Original.WaitHandle waitObject,
            Original.WaitOrTimerCallback callBack,
            object state,
            long millisecondsTimeOutInterval,
            bool executeOnlyOnce,
            bool isSafe)
        {
            return Helper.SimpleWrap<Original.RegisteredWaitHandle>(
                delegate(ClrSyncManager manager)
                {
                    Helper.WaitOrTimerCallbackRoutineArg p = new Helper.WaitOrTimerCallbackRoutineArg();
                    p.s = new Original::Semaphore(0, 1);
                    p.a = new AutoResetEvent(false);
                    p.waitObject = waitObject;
                    p.callback = callBack;
                    p.state = state;
                    p.millisecondsTimeOutInterval = millisecondsTimeOutInterval;
                    p.executeOnlyOnce = executeOnlyOnce;
                    p.canceled = false;
                    p.onCancellation = null;
                    p.finished = false;
                    Original.RegisteredWaitHandle returnValue =
                        isSafe ? Original.ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(true),
                                  RegisterWaitHelper.RegisterWaitForSingleObjectWrapper(manager), p, Timeout.Infinite, true)
                               : Original.ThreadPool.UnsafeRegisterWaitForSingleObject(new AutoResetEvent(true), RegisterWaitHelper.RegisterWaitForSingleObjectWrapper(manager), p, Timeout.Infinite, true);
                    if (returnValue != null)
                    {
                        p.a.WaitOne();
                        manager.rwhToInfo[returnValue] = p;
                        ChessTask child = manager.TaskFork();
                        manager.RegisterTaskSemaphore(child, p.s, false);
                        manager.TaskResume(child);
                    }
                    return returnValue;
                },
                delegate()
                {
                    return isSafe ? Original.ThreadPool.RegisterWaitForSingleObject(waitObject, callBack, state, millisecondsTimeOutInterval, executeOnlyOnce)
                                  : Original.ThreadPool.UnsafeRegisterWaitForSingleObject(waitObject, callBack, state, millisecondsTimeOutInterval, executeOnlyOnce);
                }
            );
        }


        public static Original.RegisteredWaitHandle RegisterWaitForSingleObject(
            Original.WaitHandle waitObject,
            Original.WaitOrTimerCallback callBack,
            object state,
            int millisecondsTimeOutInterval,
            bool executeOnlyOnce)
        {
            return RegisterWaitForSingleObjectHelper(waitObject, callBack, state, millisecondsTimeOutInterval, executeOnlyOnce, true);
        }


        public static Original.RegisteredWaitHandle RegisterWaitForSingleObject(
            Original.WaitHandle waitObject,
            Original.WaitOrTimerCallback callBack,
            object state,
            long millisecondsTimeOutInterval,
            bool executeOnlyOnce)
        {
            return RegisterWaitForSingleObjectHelper(waitObject, callBack, state, millisecondsTimeOutInterval, executeOnlyOnce, true);
        }

        public static Original.RegisteredWaitHandle RegisterWaitForSingleObject(
            Original.WaitHandle waitObject,
            Original.WaitOrTimerCallback callBack,
            object state,
            TimeSpan timeOut,
            bool executeOnlyOnce)
        {
            return RegisterWaitForSingleObjectHelper(waitObject, callBack, state, (long) timeOut.TotalMilliseconds, executeOnlyOnce, true);
        }

        public static Original.RegisteredWaitHandle RegisterWaitForSingleObject(
            Original.WaitHandle waitObject,
            Original.WaitOrTimerCallback callBack,
            object state,
            uint millisecondsTimeOutInterval,
            bool executeOnlyOnce)
        {
            return RegisterWaitForSingleObjectHelper(waitObject, callBack, state, millisecondsTimeOutInterval, executeOnlyOnce, true);
        }

        public static Original.RegisteredWaitHandle UnsafeRegisterWaitForSingleObject(
            Original.WaitHandle waitObject,
            Original.WaitOrTimerCallback callBack,
            object state,
            int millisecondsTimeOutInterval,
            bool executeOnlyOnce)
        {
            return RegisterWaitForSingleObjectHelper(waitObject, callBack, state, millisecondsTimeOutInterval, executeOnlyOnce, false);
        }

        public static Original.RegisteredWaitHandle UnsafeRegisterWaitForSingleObject(
            Original.WaitHandle waitObject,
            Original.WaitOrTimerCallback callBack,
            object state,
            long millisecondsTimeOutInterval,
            bool executeOnlyOnce)
        {
            return RegisterWaitForSingleObjectHelper(waitObject, callBack, state, millisecondsTimeOutInterval, executeOnlyOnce, false);
        }

        public static Original.RegisteredWaitHandle UnsafeRegisterWaitForSingleObject(
            Original.WaitHandle waitObject,
            Original.WaitOrTimerCallback callBack,
            object state,
            TimeSpan ts,
            bool executeOnlyOnce)
        {
            return RegisterWaitForSingleObjectHelper(waitObject, callBack, state, (long) ts.TotalMilliseconds, executeOnlyOnce, false);
        }

        public static Original.RegisteredWaitHandle UnsafeRegisterWaitForSingleObject(
            Original.WaitHandle waitObject,
            Original.WaitOrTimerCallback callBack,
            object state,
            uint millisecondsTimeOutInterval,
            bool executeOnlyOnce)
        {
            return RegisterWaitForSingleObjectHelper(waitObject, callBack, state, millisecondsTimeOutInterval, executeOnlyOnce, false);
        }

        unsafe public static bool UnsafeQueueNativeOverlapped(NativeOverlapped* overlapped)
        {
            throw new NotImplementedException("ThreadPool.UnsafeQueueNativeOverlapped");
        }

    }

    public static class RegisteredWaitHandle
    {
        public static bool Unregister(Original.RegisteredWaitHandle handle, Original.WaitHandle waitObject)
        {
            return Helper.SimpleWrap<bool>(
                        delegate(ClrSyncManager manager)
                        {
                            Original.WaitHandle cancelObject = null;
                            Helper.WaitOrTimerCallbackRoutineArg p = manager.rwhToInfo[handle];
                            manager.SetMethodInfo("RegisteredWaitHandle.Unregister");
                            manager.SyncVarAccess(p, MSyncVarOp.RWVAR_READWRITE);
                            if (!p.canceled)
                            {
                                p.canceled = true;
                                p.onCancellation = waitObject;
                                if (p.finished)
                                    cancelObject = waitObject;
                            }
                            manager.CommitSyncVarAccess();

                            global::Microsoft.ManagedChess.MChessChess.LeaveChess();
                            if (cancelObject != null)
                            {
                                __Substitutions.System.Threading.EventWaitHandle.Set((Original.EventWaitHandle)cancelObject);
                            }
                            global::Microsoft.ManagedChess.MChessChess.EnterChess();

                            return handle.Unregister(null);
                        },
                        delegate()
                        {
                            return handle.Unregister(waitObject);
                        }
                    ); 
        }
    }
}