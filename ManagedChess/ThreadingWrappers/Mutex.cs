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
using Microsoft.ExtendedReflection.Utilities;
using System.Reflection;
using System.IO;

namespace __Substitutions.System.Threading
{
    [DebuggerNonUserCode]
    public static class Mutex
    {
        public static void ReleaseMutex(Original::Mutex mutex)
        {
            if (mutex == null)
                throw new ArgumentNullException();
            Helper.SimpleWrap<bool>(
                            delegate(ClrSyncManager manager)
                            {
                                manager.SetMethodInfo("Mutex.ReleaseMutex");
                                manager.SyncVarAccess(mutex, MSyncVarOp.LOCK_RELEASE);
                                try
                                {
                                    mutex.ReleaseMutex();
                                }
                                catch (Exception e)
                                {
                                    manager.CommitSyncVarAccess();
                                    throw e;
                                };
                                manager.CommitSyncVarAccess();
                                return true;
                            },
                            delegate()
                            {
                                mutex.ReleaseMutex();
                                return true;
                            });
        }

        public static bool WaitOne(Original::Mutex mutex)
        {
            return WaitOneRaw(mutex, Timeout.Infinite, false);
        }

        public static bool WaitOne(Original::Mutex mutex, int timeOut, bool exitContext)
        {
            return WaitOneRaw(mutex, timeOut, exitContext);
        }

        public static bool WaitOne(Original::Mutex mutex, TimeSpan timeOut, bool exitContext)
        {
            return WaitOneRaw(mutex, (int) timeOut.TotalMilliseconds, exitContext);
        }

        private static bool WaitOneRaw(Original::Mutex mutex, int timeOut, bool domain)
        {
            if (mutex == null)
                throw new ArgumentNullException();
            if (timeOut < 0 && timeOut != Timeout.Infinite)
                throw new ArgumentOutOfRangeException();

            return Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    if (timeOut == Timeout.Infinite)
                        return WaitOneHelper.WaitOneDelegate(manager, mutex);
                    else
                        return WaitOneHelper.WaitOneDelegateWithTimeout(manager, mutex);
                },
                delegate()
                {
                    return mutex.WaitOne(timeOut, domain);
                });
        }
    }
}