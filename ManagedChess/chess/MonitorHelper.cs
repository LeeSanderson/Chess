/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
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
    internal static class MonitorHelper
    {
        public delegate T WaitDelegatemanagerObject<T>(ClrSyncManager manager, object o, String instrMethod);
        public static WaitDelegatemanagerObject<bool> WaitDelegate =
            delegate(ClrSyncManager manager, object o, String instrMethod)
            {
                ChessTask tid = manager.CurrentTid;

                // release the lock n times
                manager.SetMethodInfo(instrMethod);
                manager.SyncVarAccess(o, MSyncVarOp.LOCK_RELEASE);
                int n = MonitorHelper.GetReentrancyCount(o);
                MonitorHelper.AddTaskToWaitQueue(o, tid);
                try
                {
                    for (int i = 0; i < n; i++)
                    {
                        Original::Monitor.Exit(o);
                        MonitorHelper.DecrementReentrancyCount(o);
                    }
                }
                catch (Exception e)
                {
                    manager.CommitSyncVarAccess();
                    throw e;
                }
                manager.CommitSyncVarAccess();

                manager.TaskYield();

                // wait on the WaitQueue
                while (true)
                {
                    manager.SetMethodInfo(instrMethod);
                    manager.SyncVarAccess(o, MSyncVarOp.LOCK_ACQUIRE);
                    if (MonitorHelper.IsTaskInQueue(o, tid))
                    {
                        manager.LocalBacktrack();
                        continue;
                    }
                    manager.CommitSyncVarAccess();
                    break;
                }

                while (true)
                {
                    manager.SetMethodInfo(instrMethod);
                    manager.SyncVarAccess(o, MSyncVarOp.LOCK_ACQUIRE);
                    try
                    {
                        if (!Original::Monitor.TryEnter(o))
                        {
                            manager.LocalBacktrack();
                            continue;
                        }
                        IncrementReentrancyCount(o);
                        // acquire the lock n-1 times
                        for (int i = 1; i < n; i++)
                        {
                            Original::Monitor.Enter(o);
                            MonitorHelper.IncrementReentrancyCount(o);
                        }
                    }
                    catch (Exception e)
                    {
                        manager.CommitSyncVarAccess();
                        throw e;
                    }
                    manager.CommitSyncVarAccess();
                    break;
                }

                return true;
            };
        public static WaitDelegatemanagerObject<bool> WaitDelegateWithTimeout =
            delegate(ClrSyncManager manager, object o, String instrMethod)
            {
                ChessTask tid = manager.CurrentTid;
                int n = GetReentrancyCount(o);

                // release the lock n times
                manager.SetMethodInfo(instrMethod);
                manager.SyncVarAccess(o, MSyncVarOp.LOCK_RELEASE);
                try
                {
                    for (int i = 0; i < n; i++)
                    {
                        Original::Monitor.Exit(o);
                        DecrementReentrancyCount(o);
                    }
                }
                catch (Exception e)
                {
                    manager.CommitSyncVarAccess();
                    throw e;
                }
                manager.CommitSyncVarAccess();

                manager.TaskYield();

                while (true)
                {
                    manager.SetMethodInfo(instrMethod);
                    manager.SyncVarAccess(o, MSyncVarOp.LOCK_ACQUIRE);
                    try
                    {
                        if (!Original::Monitor.TryEnter(o))
                        {
                            manager.LocalBacktrack();
                            continue;
                        }
                        IncrementReentrancyCount(o);
                        // acquire the lock n-1 times
                        for (int i = 1; i < n; i++)
                        {
                            Original::Monitor.Enter(o);
                            IncrementReentrancyCount(o);
                        }
                    }
                    catch (Exception e)
                    {
                        manager.CommitSyncVarAccess();
                        throw e;
                    }
                    manager.MarkTimeout();
                    manager.CommitSyncVarAccess();
                    break;
                }

                return false;
            };

        public static void Reset()
        {
            ReentrancyCount.Clear();
            WaitQueues.Clear();
        }
        public static Microsoft.ExtendedReflection.Collections.SafeDictionary<object, int> ReentrancyCount = new SafeDictionary<object, int>();
        public static void IncrementReentrancyCount(object o)
        {
            if (!ReentrancyCount.ContainsKey(o))
                ReentrancyCount[o] = 0;
            ReentrancyCount[o]++;
        }
        public static void DecrementReentrancyCount(object o)
        {

            System.Diagnostics.Debug.Assert(ReentrancyCount.ContainsKey(o));
            ReentrancyCount[o]--;
        }
        public static int GetReentrancyCount(object o)
        {
            if (!ReentrancyCount.ContainsKey(o))
                ReentrancyCount[o] = 0;
            return ReentrancyCount[o];
        }

        public static Microsoft.ExtendedReflection.Collections.SafeDictionary<object, Microsoft.ExtendedReflection.Collections.SafeQueue<ChessTask>> WaitQueues = new SafeDictionary<object, SafeQueue<ChessTask>>();
        public static void AddTaskToWaitQueue(object o, ChessTask tid)
        {
            if (!WaitQueues.ContainsKey(o))
                WaitQueues[o] = new Microsoft.ExtendedReflection.Collections.SafeQueue<ChessTask>();
            WaitQueues[o].Enqueue(tid);
        }
        public static void RemoveTaskFromWaitQueue(object o)
        {
            if (!WaitQueues.ContainsKey(o))
                WaitQueues[o] = new Microsoft.ExtendedReflection.Collections.SafeQueue<ChessTask>();
            if (WaitQueues[o].Count > 0)
            {
                int tid = WaitQueues[o].Dequeue();
            }
        }
        public static void RemoveAllTasksFromWaitQueue(object o)
        {
            if (!WaitQueues.ContainsKey(o))
                WaitQueues[o] = new Microsoft.ExtendedReflection.Collections.SafeQueue<ChessTask>();
            WaitQueues[o].Clear();
        }
        public static bool IsTaskInQueue(object o, ChessTask tid)
        {
            if (!WaitQueues.ContainsKey(o))
                WaitQueues[o] = new Microsoft.ExtendedReflection.Collections.SafeQueue<ChessTask>();
            return WaitQueues[o].Contains(tid);
        }
    }
}
