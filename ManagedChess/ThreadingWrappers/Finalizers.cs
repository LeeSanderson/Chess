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
using Microsoft.ExtendedReflection.Utilities;
using System.Runtime.InteropServices;
using System.Security;
using ClrThread = System.Int32;
using ChessTask = System.Int32;
using System.Threading;
using MChess;
using Microsoft.ManagedChess.EREngine;
using System.Diagnostics;
using Microsoft.ExtendedReflection.Metadata;

namespace __Substitutions.System
{
    [DebuggerNonUserCode]
    [__DoNotInstrument] // substitutions should not be instrumented
    public static class Object
    {
        [__NonPublic]
        public static void Finalize___lateredirect(object target)
        {
            Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    if (manager.DelayFinalizers)
                    {
                        // add object to list of object to be finalized at end of test
                        manager.FinalizeLater(target);
                        // resurrect
                        GC.ReRegisterForFinalize(target);
                        return false;
                    }
                    else
                    {
                        ReflectionHelper.InvokeFinalizer(target);
                        return false;
                    }
                },
                delegate()
                {
                    ReflectionHelper.InvokeFinalizer(target);
                    return false;
                }
                );
        }

        // this is the wrapper we use to do the callback synchronously
        class WrappedAsyncResult : IAsyncResult
        {
            public Exception CalleeThrown { get; set; }
            public bool Completed { get; set; }
            public object Result { get; set; }
            public WaitCallback MyDelegate { get; set; }
            public IAsyncResult RealAsyncResult { get; set; }
            private object[] ByRefResults;
            public readonly object State;
            public EventWaitHandle MyHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            public void SetByRefResults(object[] byRefResults)
            {
                this.ByRefResults = byRefResults;
            }
            public object[] GetByRefResults() { return ByRefResults; }
            public WrappedAsyncResult(object state)
            {
                this.Result = null;
                this.ByRefResults = null;
                this.State = state;
                this.Completed = false;
                this.MyDelegate = null;
                this.RealAsyncResult = null;
                this.CalleeThrown = null;
            }

            #region IAsyncResult Members

            public object AsyncState
            {
                get { return this.State; }
            }

            public global::System.Threading.WaitHandle AsyncWaitHandle
            {
                get { return MyHandle; }
            }

            public bool CompletedSynchronously
            {
                get { return false; }
            }

            public bool IsCompleted
            {
                get { 
                    return this.Completed; 
                }
            }

            #endregion
        }

        public static object Invoke(object untypedMethod, object[] arguments)
        {
            return Helper.SimpleWrap<object>(
            delegate(ClrSyncManager manager)
            {
                var method = (Method)untypedMethod;
                if (method.ShortName == "BeginInvoke")
                {
                    // extract the delegate arguments, and callback
                    Debug.Assert(arguments.Length >= 3);
                    var delegateArguments = new object[arguments.Length - 3];
                    var callback = (AsyncCallback)arguments[arguments.Length - 2];
                    var asyncState = arguments[arguments.Length - 1];
                    var @delegate = (global::System.Delegate)arguments[0];
                    global::System.Array.Copy(arguments, 1, delegateArguments, 0, delegateArguments.Length);

                    // create the IAsyncResult that everyone shares
                    var asyncResult = new WrappedAsyncResult(asyncState);

                    // we wrap the actual invoke
                    // so that CHESS gets control, as usual
                    Helper.ThreadRoutineArg p = new Helper.ThreadRoutineArg();
                    p.s = new Original::Semaphore(0, 1);
                    p.o = null;
                    p.wcb =
                        (o =>
                        {
                            manager.SetMethodInfo("BeginInvoke(Begin)");
                            manager.SyncVarAccess(asyncResult, MSyncVarOp.LOCK_ACQUIRE);
                            manager.CommitSyncVarAccess();
                            // synchronous call of the actual method
                            object result = null;
                            try
                            {
                                result = @delegate.DynamicInvoke(delegateArguments);
                                // make sure to copy values of out params
                                List<object> byRefResults = new List<object>();
                                var delegateParameters = @delegate.GetType().GetMethod("Invoke").GetParameters();
                                for (int i = 0; i < delegateParameters.Length; i++)
                                    if (delegateParameters[i].ParameterType.IsByRef)
                                    {
                                        byRefResults.Add(delegateArguments[i]);
                                    }
                                asyncResult.SetByRefResults(byRefResults.ToArray());
                            }
                            catch (Exception e)
                            {
                                // From ECMA:
                                // Note: the callee can throw exceptions. 
                                // Any unhandled exception propagates to the caller via the EndInvoke method.
                                asyncResult.CalleeThrown = e;
                            }
                            // this is the actual return results of the method, which becomes the return value
                            // of the EndInvoke
                            asyncResult.Result = result;
                            manager.SetMethodInfo("BeginInvoke(Completed)");
                            manager.SyncVarAccess(asyncResult, MSyncVarOp.LOCK_RELEASE);
                            // we set completed before the callback is done because
                            // the callback may call EndInvoke.
                            asyncResult.Completed = true;
                            asyncResult.MyHandle.Set();
                            manager.CommitSyncVarAccess();
                            // From ECMA: The VES shall call this delegate when the value [the callback]
                            // is computed or an exception has been raised indicating that the result will
                            // not be available.
                            // TODO: but how is the callback supposed to "know" whether or not an exception was raised?
                            if (callback != null)
                                callback(asyncResult);
                        });
                    // create the wrapper
                    WaitCallback call = Helper.ThreadCreateWrapper(manager);
                    // store it aware for later use
                    asyncResult.MyDelegate = call;

                    // TODO: how do we insure this one is done without instrumentation?
                    IAsyncResult invokeResult = call.BeginInvoke(p, null, null);
                    // we squirrel this way because we have to match the above BeginInvoke
                    // with the EndInvoke later
                    asyncResult.RealAsyncResult = invokeResult;

                    // let the asynch task proceed
                    ChessTask child = manager.TaskFork();
                    manager.RegisterTaskSemaphore(child, p.s, false);
                    manager.TaskResume(child);

                    return asyncResult;
                }
                else if (method.ShortName == "EndInvoke")
                {
                    WrappedAsyncResult asyncResult = (WrappedAsyncResult)arguments[arguments.Length - 1];

                    // wait for the BeginInvoke to complete
                    while (true)
                    {
                        manager.SetMethodInfo("EndInvoke(Begin)");
                        manager.SyncVarAccess(asyncResult, MSyncVarOp.LOCK_ACQUIRE);
                        if (!asyncResult.Completed)
                        {
                            manager.LocalBacktrack();
                            continue;
                        }
                        manager.CommitSyncVarAccess();
                        break;
                    }

                    // for good measure, do the EndInvoke, to match the BeginInvoke
                    asyncResult.MyDelegate.EndInvoke(asyncResult.RealAsyncResult);
                    manager.SetMethodInfo("EndInvoke(Completed)");
                    manager.SyncVarAccess(asyncResult, MSyncVarOp.LOCK_RELEASE);
                    manager.CommitSyncVarAccess();

                    if (asyncResult.CalleeThrown != null)
                    {
                        throw asyncResult.CalleeThrown;
                    }
                    else
                    {
                        // copy the results back!
                        global::System.Array.Copy(asyncResult.GetByRefResults(), 0, arguments, 1, asyncResult.GetByRefResults().Length);
                    }
                    return asyncResult.Result;
                }
                else
                    throw new InvalidOperationException("unexpected method: " + method.FullName);
            },
            delegate()
            {
                // TODO: no instrumentation
                return null;
            });
        }
    }
}
