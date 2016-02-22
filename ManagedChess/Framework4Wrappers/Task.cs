// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
using System;
using System.Collections.Generic;
using System.Text;
using Original = global::System.Threading.Tasks;
using Microsoft.ExtendedReflection.Monitoring;
using System.Runtime.InteropServices;
using System.Security;
using ClrThread = System.Int32;
using ChessTask = System.Int32;
using System.Threading;
using MChess;
using System.Diagnostics;
using Microsoft.ManagedChess.EREngine;
using System.Threading.Tasks;

namespace __Substitutions.System.Threading.Tasks
{
    [DebuggerNonUserCode]
    public static class ThreadPoolTaskScheduler
    {
        [__NonPublicReceiver]
        [__NonPublic]
        public static void QueueTask___lateredirect(global::System.Object o, Original::Task task)
        {
            var self = (Original::TaskScheduler)o;
            Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    //global::System.Console.WriteLine("Intercepting ThreadPoolTaskScheduler.QueueTask");
                    Helper.TaskArg p = new Helper.TaskArg();
                    p.s = new global::System.Threading.Semaphore(0, 1);
                    p.e = new global::System.Threading.ManualResetEvent(false);
                    p.task = task;
                    p.taskScheduler = self;
                    Original.Task.Factory.StartNew(
                        Helper.TaskCreateWrapper(manager), 
                        p, 
                        global::System.Threading.CancellationToken.None, 
                        Original.TaskCreationOptions.None, 
                        self);
                    p.e.WaitOne();
                    ChessTask child = manager.TaskFork();
                    manager.RegisterTaskSemaphore(child, p.s, false);
                    manager.TaskResume(child);
                    return true;
                },
                delegate()
                {
                    var queueTask = self.GetType().GetMethod("QueueTask",
                        global::System.Reflection.BindingFlags.NonPublic | 
                        global::System.Reflection.BindingFlags.Instance);
                    queueTask.Invoke(self, new object[] { task });
                    return true;
                }
            );
        }

        [__NonPublicReceiver]
        [__NonPublic]
        public static bool TryExecuteTaskInline___lateredirect(global::System.Object o, Original::Task task, bool taskWasPreviouslyQueued)
        {
            using (_ProtectingThreadContext.Acquire())
            {
                //global::System.Console.WriteLine("Intercepting ThreadPoolTaskScheduler.TryExecuteTaskInline");
                Original::TaskScheduler self = (Original::TaskScheduler)o;
                var tryExecuteTaskInline = self.GetType().GetMethod("TryExecuteTaskInline",
                            global::System.Reflection.BindingFlags.NonPublic |
                            global::System.Reflection.BindingFlags.Instance);
                return (bool)tryExecuteTaskInline.Invoke(self, new object[] { task, taskWasPreviouslyQueued });
            }
        }

        [__NonPublicReceiver]
        [__NonPublic]
        public static bool TryDequeue___lateredirect(global::System.Object o, Original::Task task)
        {
            using (_ProtectingThreadContext.Acquire())
            {
                //global::System.Console.WriteLine("Intercepting ThreadPoolTaskScheduler.TryDequeue");
                Original::TaskScheduler self = (Original::TaskScheduler)o;
                var tryDequeue = self.GetType().GetMethod("TryDequeue",
                            global::System.Reflection.BindingFlags.NonPublic |
                            global::System.Reflection.BindingFlags.Instance);
                return (bool)tryDequeue.Invoke(self, new object[] { task });
            }
        }

        [__NonPublicReceiver]
        [__NonPublic]
        public static void NotifyWorkItemProgress___lateredirect(global::System.Object o)
        {
            using (_ProtectingThreadContext.Acquire())
            {
                //global::System.Console.WriteLine("Intercepting ThreadPoolTaskScheduler.NotifyWorkItemProgress");
                Original::TaskScheduler self = (Original::TaskScheduler)o;
                var notifyWorkItemProgress = self.GetType().GetMethod("NotifyWorkItemProgress",
                            global::System.Reflection.BindingFlags.NonPublic |
                            global::System.Reflection.BindingFlags.Instance);
                notifyWorkItemProgress.Invoke(self, new object[] { });
            }
        }

        [__NonPublicReceiver]
        [__NonPublic]
        public static IEnumerable<Original::Task> GetScheduledTasks___lateredirect(global::System.Object o)
        {
            using (_ProtectingThreadContext.Acquire())
            {
                //global::System.Console.WriteLine("Intercepting ThreadPoolTaskScheduler.GetScheduledTasks");
                Original::TaskScheduler self = (Original::TaskScheduler)o;
                var getScheduledTasks = self.GetType().GetMethod("GetScheduledTasks",
                            global::System.Reflection.BindingFlags.NonPublic |
                            global::System.Reflection.BindingFlags.Instance);
                return (IEnumerable<Original::Task>)getScheduledTasks.Invoke(self, new object[] { });
            }
        }

        [__NonPublicReceiver]
        [__NonPublic]
        public static bool get_RequiresAtomicStartTransition___lateredirect(global::System.Object o)
        {
            using (_ProtectingThreadContext.Acquire())
            {
                //global::System.Console.WriteLine("Intercepting ThreadPoolTaskScheduler.get_RequiresAtomicStartTransition");
                Original::TaskScheduler self = (Original::TaskScheduler)o;
                var get_requiresAtomicStartTransition = self.GetType().GetMethod("get_RequiresAtomicStartTransition",
                            global::System.Reflection.BindingFlags.NonPublic |
                            global::System.Reflection.BindingFlags.Instance);
                return (bool)get_requiresAtomicStartTransition.Invoke(self, new object[] { });
            }
        }

        /*
        [__NonPublicReceiver]
        [__NonPublic]
        public static Original::TaskScheduler ___ctor_newobj___lateredirect()
        {

        }
        */ 
    }
    
    [DebuggerNonUserCode]
    public static class Task
    {
        /*
        // constructors
        public static Original::Task ___ctor_newobj(Action action) { throw new NotImplementedException("Task"); }
        public static Original::Task ___ctor_newobj(Action<object> action, object state) { throw new NotImplementedException("Task"); }
        public static Original::Task ___ctor_newobj(Action action, CancellationToken cancellationToken) { throw new NotImplementedException("Task"); }
        public static Original::Task ___ctor_newobj(Action action, TaskCreationOptions creationOptions) { throw new NotImplementedException("Task"); }
        public static Original::Task ___ctor_newobj(Action<object> action, object state, CancellationToken cancellationToken) { throw new NotImplementedException("Task"); }
        public static Original::Task ___ctor_newobj(Action<object> action, object state, TaskCreationOptions creationOptions) { throw new NotImplementedException("Task"); }
        public static Original::Task ___ctor_newobj(Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions) { throw new NotImplementedException("Task"); }
        public static Original::Task ___ctor_newobj(Action<object> action, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions) { throw new NotImplementedException("Task"); }

        public static object AsyncState { get; }
        public static TaskCreationOptions CreationOptions { get; }
        public static static int? CurrentId { get; }
        public static AggregateException Exception { get; }
        public static static TaskFactory Factory { get; }
        public static int Id { get; }
        public static bool IsCanceled { get; }
        public static bool IsCompleted { get; }
        public static bool IsFaulted { get; }
        public static TaskStatus Status { get; }

        public static Original::Task ContinueWith(
            Original::Task self, Action<Original::Task> continuationAction) { throw new NotImplementedException("Task"); }
        public static Original::Task<TResult> ContinueWith<TResult>(
            Original::Task self, Func<Original::Task, TResult> continuationFunction) { throw new NotImplementedException("Task"); }
        public static Original::Task ContinueWith(
            Original::Task self, Action<Original::Task> continuationAction, CancellationToken cancellationToken) { throw new NotImplementedException("Task"); }
        public static Original::Task ContinueWith(
            Original::Task self, Action<Original::Task> continuationAction, TaskContinuationOptions continuationOptions) { throw new NotImplementedException("Task"); }
        public static Original::Task ContinueWith(
            Original::Task self, Action<Original::Task> continuationAction, TaskScheduler scheduler) { throw new NotImplementedException("Task"); }
        public static Original::Task<TResult> ContinueWith<TResult>(
            Original::Task self, Func<Original::Task, TResult> continuationFunction, CancellationToken cancellationToken) { throw new NotImplementedException("Task"); }
        public static Original::Task<TResult> ContinueWith<TResult>(
            Original::Task self, Func<Original::Task, TResult> continuationFunction, TaskContinuationOptions continuationOptions) { throw new NotImplementedException("Task"); }
        public static Original::Task<TResult> ContinueWith<TResult>(
            Original::Task self, Func<Original::Task, TResult> continuationFunction, TaskScheduler scheduler) { throw new NotImplementedException("Task"); }
        public static Original::Task ContinueWith(
            Original::Task self, Action<Original::Task> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler) { throw new NotImplementedException("Task"); }
        public static Original::Task<TResult> ContinueWith<TResult>(
            Original::Task self, Func<Original::Task, TResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler) { throw new NotImplementedException("Task"); }

        public static void RunSynchronously(
            Original::Task self) { throw new NotImplementedException("Task"); }
        public static void RunSynchronously(
            Original::Task self, TaskScheduler scheduler) { throw new NotImplementedException("Task"); }
        
        public static void Start(
            Original::Task self) { throw new NotImplementedException("Task"); }
        public static void Start(
            Original::Task self, TaskScheduler scheduler) { throw new NotImplementedException("Task"); }
        */

        public static bool WaitRaw(Original::Task self, int timeout, CancellationToken cancellationToken)
        {
            return WaitAllRaw(new Original::Task[] { self }, timeout, cancellationToken);
        }
        public static void Wait(Original::Task self) 
        {
            WaitRaw(self, Timeout.Infinite, CancellationToken.None);
        }
        public static void Wait(Original::Task self, CancellationToken cancellationToken) 
        {
            WaitRaw(self, Timeout.Infinite, cancellationToken); 
        }
        public static bool Wait(Original::Task self, int millisecondsTimeout) 
        {
            return WaitRaw(self, millisecondsTimeout, CancellationToken.None); 
        }
        public static bool Wait(Original::Task self, TimeSpan timeout) 
        {
            return WaitRaw(self, (int) timeout.TotalMilliseconds, CancellationToken.None); 
        }
        public static bool Wait(Original::Task self, int millisecondsTimeout, CancellationToken cancellationToken) 
        {
            return WaitRaw(self, millisecondsTimeout, cancellationToken);
        }

        private static bool WaitAllRaw(Original::Task[] tasks, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (tasks == null)
                throw new ArgumentNullException();
            foreach (var h in tasks)
            {
                if (h == null)
                    throw new ArgumentNullException();
            }
            if (millisecondsTimeout != Timeout.Infinite && millisecondsTimeout < 0)
                throw new ArgumentOutOfRangeException(); 
            
            return Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    return WaitAllRawHelper(manager, tasks, millisecondsTimeout, cancellationToken);
                },
                delegate()
                {
                    return Original::Task.WaitAll(tasks, millisecondsTimeout, cancellationToken);
                });
        }

        private static int WaitAnyRaw(Original::Task[] tasks, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (tasks == null)
                throw new ArgumentNullException();
            foreach (var h in tasks)
            {
                if (h == null)
                    throw new ArgumentNullException();
            }
            if (millisecondsTimeout != Timeout.Infinite && millisecondsTimeout < 0)
                throw new ArgumentOutOfRangeException();

            return Helper.SimpleWrap<int>(
                delegate(ClrSyncManager manager)
                {
                    return WaitAnyRawHelper(manager, tasks, millisecondsTimeout, cancellationToken);
                },
                delegate()
                {
                    return Original::Task.WaitAny(tasks, millisecondsTimeout, cancellationToken);
                });
        }

        private static bool WaitAllRawHelper(ClrSyncManager manager, Original::Task[] tasks, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            bool ret;
            while (true)
            {
                manager.SetMethodInfo("Task.WaitAll");
                manager.AggregateSyncVarAccess(tasks, MSyncVarOp.WAIT_ALL);
                try
                {
                    ret = Original::Task.WaitAll(tasks, 0, cancellationToken);
                }
                catch (Exception e)
                {
                    manager.CommitSyncVarAccess();
                    throw e;
                }
                if (ret)
                    break;  // operation succeeded
                if (millisecondsTimeout >= 0)
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

        private static int WaitAnyRawHelper(ClrSyncManager manager, Original::Task[] tasks, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            int ret;
            while (true)
            {
                manager.SetMethodInfo("Task.WaitAny");
                manager.AggregateSyncVarAccess(tasks, MSyncVarOp.WAIT_ANY);
                try
                {
                    ret = Original::Task.WaitAny(tasks, 0, cancellationToken);
                }
                catch (Exception e)
                {
                    manager.CommitSyncVarAccess();
                    throw e;
                }
                if (ret != -1)
                    break;  // operation succeeded
                if (millisecondsTimeout >= 0)
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

        // these are static (no self)
        public static void WaitAll(params Original::Task[] tasks)
        {
            WaitAllRaw(tasks, Timeout.Infinite, CancellationToken.None);
        }
        public static void WaitAll(Original::Task[] tasks, CancellationToken cancellationToken)
        {
            WaitAllRaw(tasks, Timeout.Infinite, cancellationToken);
        }
        public static bool WaitAll(Original::Task[] tasks, int millisecondsTimeout)
        {
            return WaitAllRaw(tasks, millisecondsTimeout, CancellationToken.None);
        }
        public static bool WaitAll(Original::Task[] tasks, TimeSpan timeout)
        {
            return WaitAllRaw(tasks, (int) timeout.TotalMilliseconds, CancellationToken.None);
        }
        public static bool WaitAll(Original::Task[] tasks, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return WaitAllRaw(tasks, millisecondsTimeout, cancellationToken);
        }
        public static int WaitAny(params Original::Task[] tasks)
        {
            return WaitAnyRaw(tasks, Timeout.Infinite, CancellationToken.None);
        }
        public static int WaitAny(Original::Task[] tasks, CancellationToken cancellationToken)
        {
            return WaitAnyRaw(tasks, Timeout.Infinite, cancellationToken);
        }
        public static int WaitAny(Original::Task[] tasks, int millisecondsTimeout)
        {
            return WaitAnyRaw(tasks, millisecondsTimeout, CancellationToken.None);
        }
        public static int WaitAny(Original::Task[] tasks, TimeSpan timeout)
        {
            return WaitAnyRaw(tasks, (int) timeout.TotalMilliseconds, CancellationToken.None);
        }
        public static int WaitAny(Original::Task[] tasks, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return WaitAnyRaw(tasks, millisecondsTimeout, cancellationToken);
        }
    }

    [DebuggerNonUserCode]
    public static class Task<TResult>
    {
        public static TResult get_Result(Original::Task<TResult> self)
        {
            global::System.Console.WriteLine("Task.get_Result intercepted");
            return Helper.SimpleWrap<TResult>(
                delegate(ClrSyncManager manager)
                {
                    Task.WaitRaw(self, Timeout.Infinite, CancellationToken.None);
                    return self.Result;
                },
                delegate()
                {
                    return self.Result;
                }
                );
        }
    }
}