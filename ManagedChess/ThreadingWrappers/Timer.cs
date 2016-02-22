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
using MChess;
using Microsoft.ManagedChess.EREngine;
using System.Diagnostics;

namespace Microsoft.ManagedChess.EREngine
{
    [DebuggerNonUserCode]
    internal static class TimerHelpers
    {
        internal static SafeDictionary<Timer, TimerRoutineArg> timer2tra = new SafeDictionary<Timer, TimerRoutineArg>();

        internal class TimerRoutineArg
        {
            public Original::TimerCallback callback;
            public object state;
            public TimeSpan dueTime;
            public TimeSpan Period;
            public Semaphore selfSemaphore;
            public Semaphore parentSemaphore;
            public Timer timer;
            public bool disposed;
            public bool changed;
            public bool inLoop;
        }

        // dueTime:  the amount of time to delay before callback is invoked, in milliseconds. 
        // Specify Timeout.Infinite to prevent the timer from starting. 
        //  Specify zero (0) to start the timer immediately. 
        // period: The time interval between invocations of callback, in milliseconds. 
        // Specify Timeout.Infinite to disable periodic signaling.

        // The method specified for callback should be reentrant, because it is called on ThreadPool threads. 
        // The method can be executed simultaneously on two thread pool threads if the timer interval is less 
        // than the time required to execute the method, or if all thread pool threads are in use and the method 
        // is queued multiple times.

        public static void TimerCreateWrapper(object start) {
            try
            {
                TimerRoutineArg argument = (TimerRoutineArg)start;
                argument.inLoop = true;
                argument.changed = false;
                argument.parentSemaphore.Release();
                ClrSyncManager manager = ClrSyncManager.SyncManager;
                manager.ThreadBegin(argument.selfSemaphore);
                Exception exception = null;
                while (true)
                {

                    manager.SetMethodInfo("Timer.read");
                    manager.SyncVarAccess(argument.timer, MSyncVarOp.RWVAR_READWRITE);
                    manager.CommitSyncVarAccess();

                    if (timer2tra[argument.timer].disposed)
                    {
                        // the timer only goes away when it is disposed of
                        // it may be disabled, but a Change can reenable it
                        break;
                    }

                    manager.TaskYield();    // until fairness deals with unbounded thread creation
                    manager.TaskYield();    // we yield twice

                    if (!argument.dueTime.Equals(TimeSpan.FromTicks(Timeout.Infinite)))
                    {
                        MChessChess.LeaveChess();
                        try
                        {
                            argument.callback(argument.state);

                        }
                        catch (Exception e) // catch recoverable exception in monitored code
                        {
                            exception = e;
                        }
                        MChessChess.EnterChess();
                        // If period is zero (0) or Infinite and dueTime is not Infinite, callback is invoked once; 
                        // the periodic behavior of the timer is disabled, but can be re-enabled using the Change method.
                    }

                    if (exception != null)
                        break;

                    if (argument.changed)
                    {
                        argument.changed = false;
                        continue;
                    }

                    if (argument.Period.Equals(TimeSpan.FromTicks(Timeout.Infinite)) || argument.Period.Equals(TimeSpan.FromTicks(0)))
                        break;
                }
                argument.inLoop = false;
                if (timer2tra[argument.timer].disposed)
                    timer2tra.Remove(argument.timer);

                if (manager.BreakDeadlockMode)
                    MChessChess.WakeNextDeadlockedThread(false, true);
                else if (exception == null)
                    manager.ThreadEnd(System.Threading.Thread.CurrentThread);
                else
                    manager.Shutdown(exception);
            }
            catch (Exception e) // catch fatal error in our code
            {
                ClrSyncManager manager = ClrSyncManager.SyncManager;
                manager.Shutdown(e);
            }
        }

        public static Timer CreateTimer(ClrSyncManager manager, Original::TimerCallback timerCallback, object state, TimeSpan dueTime, TimeSpan period) {
            TimerRoutineArg argument = new TimerRoutineArg();
            argument.callback = timerCallback;
            argument.state = state;
            argument.dueTime = dueTime;
            argument.Period = period;
            argument.selfSemaphore = new Semaphore(0, 1);
            argument.parentSemaphore = new Semaphore(0, 1);
            argument.disposed = false;
            argument.changed = true;

            Timer ret = new Timer(TimerCreateWrapper, argument, 0, Timeout.Infinite);
            argument.timer = ret;
            timer2tra.Add(ret, argument);
            int child = manager.TaskFork();
            argument.parentSemaphore.WaitOne();
            manager.RegisterTaskSemaphore(child, argument.selfSemaphore, false);
            manager.TaskResume(child);

            return ret;
        }
    }
}

namespace __Substitutions.System.Threading
{
    [DebuggerNonUserCode]
    public static class Timer
    {

        /* Use a TimerCallback delegate to specify the method you want the Timer to execute. 
         * The timer delegate is specified when the timer is constructed, and cannot be changed. 
         * The method does not execute on the thread that created the timer; it executes on a ThreadPool 
         * thread supplied by the system. 
         * 
         * When you create a timer, you can specify an amount of time to wait before the first execution 
         * of the method (due time), and an amount of time to wait between subsequent executions (period). 
         * You can change these values, or disable the timer, using the Change method.
         * 
         * Note: As long as you are using a Timer, you must keep a reference to it. As with any managed object, 
         * a Timer is subject to garbage collection when there are no references to it. The fact that a Timer is 
         * still active does not prevent it from being collected.
         * 
         * When a timer is no longer needed, use the Dispose method to free the resources held by the timer. To 
         * receive a signal when the timer is disposed, use the Dispose(WaitHandle) method overload that takes a 
         * WaitHandle. The WaitHandle is signaled when the timer has been disposed.
         * 
         * The callback method executed by the timer should be reentrant, because it is called on ThreadPool threads. 
         * The callback can be executed simultaneously on two thread pool threads if the timer interval is less than 
         * the time required to execute the callback, or if all thread pool threads are in use and the callback is 
         * queued multiple times.
         * 
         * Note: System.Threading.Timer is a simple, lightweight timer that uses callback methods and is served by 
         * threadpool threads. You might also consider System.Windows.Forms.Timer for use with Windows forms, and 
         * System.Timers.Timer for server-based timer functionality. These timers use events and have additional features.
         */

        public static Original::Timer ___ctor_newobj(Original::TimerCallback tc)
        {
            return Helper.SimpleWrap<Original::Timer>(
                delegate(ClrSyncManager manager) { return TimerHelpers.CreateTimer(manager, tc, null, new TimeSpan(Timeout.Infinite), 
                                                                                new TimeSpan(Timeout.Infinite)); },
                delegate() { return new Original::Timer(tc); }
            );
        }

        /* 
         * If dueTime is zero (0), callback is invoked immediately. If dueTime is Timeout.Infinite, callback is not invoked; 
         * the timer is disabled, but can be re-enabled by calling the Change method. 
         * If period is zero (0) or Infinite and dueTime is not Infinite, callback is invoked once; 
         * the periodic behavior of the timer is disabled, but can be re-enabled using the Change method.
         * The method specified for callback should be reentrant, because it is called on ThreadPool threads. 
         * The method can be executed simultaneously on two thread pool threads if the timer interval is less 
         * than the time required to execute the method, or if all thread pool threads are in use and the method 
         * is queued multiple times.
         */
        public static Original::Timer ___ctor_newobj(Original::TimerCallback tc, object state, int dueTime, int period)
        {
            if (dueTime < 0 && dueTime != Timeout.Infinite)
                throw new ArgumentOutOfRangeException();
            if (period < 0 && period != Timeout.Infinite)
                throw new ArgumentOutOfRangeException();
            return Helper.SimpleWrap<Original::Timer>(
                delegate(ClrSyncManager manager) { return TimerHelpers.CreateTimer(manager, tc, state, new TimeSpan(dueTime), new TimeSpan(period)); },
                delegate() { return new Original::Timer(tc,state,dueTime,period); }
            );
        }

        public static Original::Timer ___ctor_newobj(Original::TimerCallback tc, object state, long dueTime, long period)
        {
            if (dueTime < 0 && dueTime != Timeout.Infinite)
                throw new ArgumentOutOfRangeException();
            if (period < 0 && period != Timeout.Infinite)
                throw new ArgumentOutOfRangeException();
            return Helper.SimpleWrap<Original::Timer>(
                delegate(ClrSyncManager manager) { return TimerHelpers.CreateTimer(manager, tc, state, new TimeSpan(dueTime), new TimeSpan(period)); },
                delegate() { return new Original::Timer(tc, state, dueTime, period); }
            );
        }

        public static Original::Timer ___ctor_newobj(Original::TimerCallback tc, object state, TimeSpan dueTime, TimeSpan period)
        {
            if (dueTime.TotalMilliseconds< 0 && dueTime.TotalMilliseconds != Timeout.Infinite)
                throw new ArgumentOutOfRangeException();
            if (period.TotalMilliseconds < 0 && period.TotalMilliseconds != Timeout.Infinite)
                throw new ArgumentOutOfRangeException();
            return Helper.SimpleWrap<Original::Timer>(
                delegate(ClrSyncManager manager) { return TimerHelpers.CreateTimer(manager, tc, state, dueTime, period); },
                delegate() { return new Original::Timer(tc, state, dueTime, period); }
            );
        }

        public static Original::Timer ___ctor_newobj(Original::TimerCallback tc, object state, uint dueTime, uint period)
        {
            return Helper.SimpleWrap<Original::Timer>(
                delegate(ClrSyncManager manager) { return TimerHelpers.CreateTimer(manager, tc, state, new TimeSpan(dueTime), new TimeSpan(period)); },
                delegate() { return new Original::Timer(tc, state, dueTime, period); }
            );
        }

        public static void Dispose(Original::Timer t)
        {
            Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager) { 
                    t.Dispose();
                    TimerHelpers.TimerRoutineArg tra;
                    if (!TimerHelpers.timer2tra.TryGetValue(t, out tra))
                    {
                        // TODO: this is very bad. We have an untracked Timer!
                        return true;
                    }
                    tra.disposed = true;
                    return true;
                },
                delegate() { t.Dispose(); return true; }
            );
        }

        public static bool Dispose(Original::Timer t, Original::WaitHandle h)
        {
            throw new NotImplementedException("Timer.Dispose(WaitHandle)");
        }

        // ObjectDisposedException The Timer has already been disposed. 
        // ArgumentOutOfRangeException The dueTime or period parameter is negative and is not equal to Infinite. 
        public static bool Change(Original::Timer t, int dueTime, int period)
        {
            return Change(t, (long)dueTime, (long)period);
        }

        public static bool Change(Original::Timer t, long dueTime, long period)
        {
            return Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    if (dueTime < 0 && dueTime != Timeout.Infinite)
                        throw new ArgumentOutOfRangeException();
                    if (period < 0 && period != Timeout.Infinite)
                        throw new ArgumentOutOfRangeException();
                    // lookup the timer
                    TimerHelpers.TimerRoutineArg tra;
                    if (TimerHelpers.timer2tra.TryGetValue(t, out tra))
                    {
                        if (!tra.disposed)
                        {
                            tra.dueTime = new TimeSpan(dueTime);
                            tra.Period = new TimeSpan(period);
                            if (tra.inLoop)
                            {
                                // reuse the current worker thread, which is still executing
                                tra.changed = true;
                            }
                            else
                            {
                                // create a new work thread
                                t.Change(0, Timeout.Infinite);
                                int child = manager.TaskFork();
                                tra.parentSemaphore.WaitOne();
                                manager.RegisterTaskSemaphore(child, tra.selfSemaphore, false);
                                manager.TaskResume(child);
                            }
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return t.Change(dueTime, period);
                    }
                },
                delegate() { return t.Change(dueTime, period); });
        }

        public static bool Change(Original::Timer t, TimeSpan dueTime, TimeSpan period)
        {
            return Change(t, (long) dueTime.TotalMilliseconds, (long) period.TotalMilliseconds);
        }

        public static bool Change(Original::Timer t, uint dueTime, uint period)
        {
            return Change(t, (long)dueTime, (long)period);
        }
    }
}