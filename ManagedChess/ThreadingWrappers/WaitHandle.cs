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

namespace __Substitutions.System.Threading
{
    [DebuggerNonUserCode]
    internal static class WaitHandleHelper
    {
        public static int WaitGeneral(Original::WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext,
            string op)
        {
            return Helper.SimpleWrap<int>(
                delegate(ClrSyncManager manager)
                {
                    return WaitGeneralRaw(manager, waitHandles, millisecondsTimeout, exitContext, op);
                },
                delegate() { return
                                (op == "WaitAll" 
                                ? (Original::WaitHandle.WaitAll(waitHandles, millisecondsTimeout, exitContext) ? 0 : Original.WaitHandle.WaitTimeout)
                                : (Original::WaitHandle.WaitAny(waitHandles, millisecondsTimeout, exitContext)));
                });
        }

        public static int WaitGeneralRaw(
            ClrSyncManager manager,
            Original::WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext,
            string op)
        {
            if (waitHandles == null)
                throw new ArgumentNullException();
            foreach (var h in waitHandles)
            {
                if (h == null)
                    throw new ArgumentNullException();
            }
            if (millisecondsTimeout < Timeout.Infinite)
                throw new ArgumentOutOfRangeException();
            int ret;
            while (true)
            {
                manager.SetMethodInfo("WaitHandle."+op);
                manager.AggregateSyncVarAccess(
                    waitHandles, 
                    (op == "WaitAll") ? MSyncVarOp.WAIT_ALL : MSyncVarOp.WAIT_ANY);
                try
                {
                    ret = (op == "WaitAll" 
                        ? (Original::WaitHandle.WaitAll(waitHandles, 0, false) ? 0 : Original.WaitHandle.WaitTimeout)
                        : (Original::WaitHandle.WaitAny(waitHandles, 0, false)));
                }
                catch (Exception e)
                {
                    manager.CommitSyncVarAccess();
                    throw e;
                }
                if (ret != Original::WaitHandle.WaitTimeout)
                    break;  // operation succeeded
                if (millisecondsTimeout >=0)
                {
                        manager.MarkTimeout();
                        manager.CommitSyncVarAccess();
                        manager.TaskYield();
                        return ret;
                }
                manager.LocalBacktrack();
            }
            manager.CommitSyncVarAccess();
            return ret;
        }
    }

    [DebuggerNonUserCode]
    public static class WaitHandle
    {
        #region .Net 2.0 WaitOne documentation
        // WaitHandle.WaitOne ()  When overridden in a derived class, blocks the current thread until the current 
        // WaitHandle receives a signal.  
        // WaitHandle.WaitOne (Int32, Boolean)  When overridden in a derived class, blocks the current thread 
        // until the current WaitHandle receives a signal, using 32-bit signed integer to measure the time interval 
        // and specifying whether to exit the synchronization domain before the wait. 
        // WaitHandle.WaitOne (TimeSpan, Boolean)  When overridden in a derived class, blocks the current thread until 
        // the current instance receives a signal, using a TimeSpan to measure the time interval and specifying whether
        // to exit the synchronization domain before the wait.  
        #endregion

        public static bool WaitOne(Original::WaitHandle h)
        {
            if (h == null)
                throw new NullReferenceException();
            return WaitOne(h, Timeout.Infinite, false);
        }
        public static bool WaitOne(Original::WaitHandle h, int millisecondsTimeout)
        {
            if (h == null)
                throw new NullReferenceException();
            return WaitOne(h, millisecondsTimeout, false);
        }

        public static bool WaitOne(Original::WaitHandle h, int millisecondsTimeout, bool exitContext)
        {
            if (h == null)
                throw new NullReferenceException();
            var ah = new Original::WaitHandle[] { h };
            return (WaitHandleHelper.WaitGeneral(ah, millisecondsTimeout, exitContext, "WaitOne")!=Original::WaitHandle.WaitTimeout);
        }

        public static bool WaitOne(Original::WaitHandle h, TimeSpan timeout)
        {
            if (h == null)
                throw new NullReferenceException();
            return WaitOne(h, timeout, false);
        }

        public static bool WaitOne(Original::WaitHandle h, TimeSpan timeout, bool exitContext)
        {
            if (h == null)
                throw new NullReferenceException();
            return WaitOne(h, (int) timeout.TotalMilliseconds, exitContext);
        }

        public static int WaitAny(Original::WaitHandle[] waitHandles)
        {
            return WaitAny(waitHandles, Timeout.Infinite, false);            
        }

        public static int WaitAny(Original::WaitHandle[] waitHandles, int millisecondsTimeout)
        {
            return WaitAny(waitHandles, millisecondsTimeout, false);
        }

        public static int WaitAny(Original::WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext)
        {
            return WaitHandleHelper.WaitGeneral(waitHandles, millisecondsTimeout, exitContext, "WaitAny");   
        }

        public static int WaitAny(Original::WaitHandle[] waitHandles, TimeSpan timeout)
        {
            return WaitAny(waitHandles, timeout, false);
        }

        public static int WaitAny(Original::WaitHandle[] waitHandles, TimeSpan timeout, bool exitContext)
        {
            return WaitAny(waitHandles, (int) timeout.TotalMilliseconds, exitContext);
        }

        public static bool WaitAll(Original::WaitHandle[] waitHandles)
        {
            return WaitAll(waitHandles, Timeout.Infinite, false); 
        }

        public static bool WaitAll(Original::WaitHandle[] waitHandles, int millisecondsTimeout)
        {
            return WaitAll(waitHandles, millisecondsTimeout, false);
        }

        public static bool WaitAll(Original::WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext)
        {
            return (WaitHandleHelper.WaitGeneral(waitHandles, millisecondsTimeout, exitContext, "WaitAll") != Original::WaitHandle.WaitTimeout);               
        }

        public static bool WaitAll(Original::WaitHandle[] waitHandles, TimeSpan timeout)
        {
            return WaitAll(waitHandles, timeout, false);
        }
        
        public static bool WaitAll(Original::WaitHandle[] waitHandles, TimeSpan timeout, bool exitContext)
        {
            return WaitAll(waitHandles, (int) timeout.TotalMilliseconds, exitContext); 
        }

        public static bool SignalAndWait(Original::WaitHandle toSignal, Original::WaitHandle toWaitOn)
        {
            return SignalAndWait(toSignal, toWaitOn, Timeout.Infinite, false);
        }

        // SignalObjectAndWait(h1,h2, x, y) is equivalent to
        //     r = SignalObjectAndWait(h1, h2, 0, y);
        //     if(r == WAIT_TIMEOUT && x == INFINITE) 
        //         WaitForSingleObject(h2, x, y);
        //
        public static bool SignalAndWait(
            Original::WaitHandle toSignal,
            Original::WaitHandle toWaitOn, 
            int millisecondsTimeout, bool exitContext)
        {
            return Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    if (toSignal == null)
                        throw new NullReferenceException();
                    if (toWaitOn == null)
                        throw new ArgumentNullException();
                    if (millisecondsTimeout < Timeout.Infinite)
                        throw new ArgumentOutOfRangeException();
                    var ao = new object[] { toSignal, toWaitOn };
                    manager.SetMethodInfo("WaitHandle.SignalAndWait");
                    manager.AggregateSyncVarAccess(ao, MSyncVarOp.RWVAR_READWRITE);
                    bool ret = Original::WaitHandle.SignalAndWait(toSignal, toWaitOn, 0, false);
                    if (!ret)
                       manager.MarkTimeout();
                    manager.CommitSyncVarAccess();
                    if (ret)
                        return true;
                    if (!(millisecondsTimeout <= Timeout.Infinite))
                    {
                        manager.TaskYield();
                        return false;
                    }
                    var ah = new Original::WaitHandle[] { toWaitOn };
                    return (WaitHandleHelper.WaitGeneralRaw(manager, ah, millisecondsTimeout, exitContext, "WaitOne") != Original::WaitHandle.WaitTimeout);
                },
                delegate()
                {
                    return Original::WaitHandle.SignalAndWait(toSignal, toWaitOn, millisecondsTimeout, exitContext);
                });
        }

        public static bool SignalAndWait(
            Original::WaitHandle toSignal,
            Original::WaitHandle toWaitOn,
            TimeSpan timeout,
            bool exitContext)
        {
            return SignalAndWait(toSignal, toWaitOn, (int) timeout.TotalMilliseconds, exitContext);
        }
    }

    // class is the base class of AutoReset and ManualResetEvent
    [DebuggerNonUserCode]
    public static class EventWaitHandle
    {
        public static bool Set(Original::EventWaitHandle h)
        {
            if (h == null)
                throw new NullReferenceException();
            return Helper.WrapRelease(
                h,
                MSyncVarOp.RWEVENT,
                delegate(object o) { return ((Original::EventWaitHandle)o).Set(); },
                "EventWaitHandle.Set"
            );
        }

        public static bool Reset(Original::EventWaitHandle h)
        {
            if (h == null)
                throw new NullReferenceException();
            return Helper.WrapRelease(
                h,
                MSyncVarOp.RWEVENT,
                delegate(object o) { return ((Original::EventWaitHandle)o).Reset(); },
                "EventWaitHandle.Reset"
            );
        }

        // the following code should never be called, as WaitHandle is the base class for EventWaitHandle

        public static bool WaitOne(Original::WaitHandle h)
        {
            throw new NotImplementedException();
        }

        public static bool WaitOne(Original::WaitHandle waitHandle, int millisecondsTimeout, bool exitContext)
        {
            throw new NotImplementedException();
        }

        public static bool WaitOne(Original::WaitHandle waitHandle, TimeSpan timeout, bool exitContext)
        {
            throw new NotImplementedException();
        }

        public static int WaitAny(Original::WaitHandle[] waitHandles)
        {
            throw new NotImplementedException();
        }

        public static int WaitAny(Original::WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext)
        {
            throw new NotImplementedException();
        }

        public static int WaitAny(Original::WaitHandle[] waitHandles, TimeSpan timeout, bool exitContext)
        {
            throw new NotImplementedException();
        }

        public static bool WaitAll(Original::WaitHandle[] waitHandles)
        {
            throw new NotImplementedException();
        }

        public static bool WaitAll(Original::WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext)
        {
            throw new NotImplementedException();
        }

        public static bool WaitAll(Original::WaitHandle[] waitHandles, TimeSpan timeout, bool exitContext)
        {
            throw new NotImplementedException();
        }

        public static bool SignalAndWait(Original::WaitHandle toSignal, Original::WaitHandle toWaitOn)
        {
            throw new NotImplementedException();
        }

        public static bool SignalAndWait(
            Original::WaitHandle toSignal,
            Original::WaitHandle toWaitOn, int millisecondsTimeout, bool exitContext)
        {
            throw new NotImplementedException();
        }

        public static bool SignalAndWait(
            Original::WaitHandle toSignal,
            Original::WaitHandle toWaitOn,
            TimeSpan timeout,
            bool exitContext)
        {
            throw new NotImplementedException();
        }
    }
}