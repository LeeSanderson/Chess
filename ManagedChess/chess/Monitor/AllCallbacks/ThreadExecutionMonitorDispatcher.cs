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
using Microsoft.ExtendedReflection.Interpretation;
using Microsoft.ExtendedReflection.Metadata;
using Microsoft.ExtendedReflection.Utilities.Safe.Diagnostics;
using Microsoft.ExtendedReflection.Collections;
using Microsoft.ManagedChess.EREngine.AllCallbacks;
using Microsoft.ExtendedReflection.Logging;
using Microsoft.ManagedChess.EREngine.CallsOnly;
using System.Diagnostics;

namespace Microsoft.ManagedChess.EREngine.AllCallbacks
{
    /// <summary>
    /// Ignores all callbacks, except method/constructor calls and corresponding returns.
    /// </summary>
    /// <remarks>
    /// Cheap brother of InstructionInterpreter.
    /// </remarks>
    internal class ThreadExecutionMonitorDispatcher : ExtendedReflection.Monitoring.ThreadExecutionMonitorEmpty
    {
        // shared across all threads
        static SafeDictionary<int,bool> disabledMethods = new SafeDictionary<int,bool>();
        // static SafeDictionary<int, bool> prioritizedMethods = new SafeDictionary<int, bool>();
        static SafeDictionary<int, bool> instrumentedUninstrumentedMethods = new SafeDictionary<int, bool>();
        static SafeDictionary<int, Method> instrumentedUninstrumentedMethods2 = new SafeDictionary<int, Method>();
        static SafeSet<string> newDontPreemptTypes = new SafeSet<string>();
        static SafeSet<string> newDontPreemptMethods = new SafeSet<string>();
        static SafeSet<string> newPrioritizeMethods = new SafeSet<string>();
        static bool firstTime = true;
        // one of these per thread!
        readonly int threadIndex;
        readonly SafeLinkedList<CallFrame> callStack;
        readonly IThreadMonitor callMonitor;
        readonly IEventLog log;
        CallFrame current;   //aliases last ("top") element of callStack.

        public static SafeDictionary<TypeEx,bool> GetUninstrumentedTypes() {
            var result = new SafeDictionary<TypeEx,bool>();
            foreach (var m in instrumentedUninstrumentedMethods.Keys)
            {
                TypeEx type = null;
                if (instrumentedUninstrumentedMethods2[m].TryGetDeclaringType(out type))
                {
                    result.Add(type, instrumentedUninstrumentedMethods[m]);
                }
            }
            return result;
        }

        private static string eliminateParameterizedTypes(string s)
        {
            // find all occurrences of [...] and eliminate
            var res = (string) s.Clone();
            var stack = new SafeList<int>();
            stack.Add(res.IndexOf("[", 0));
            while (stack.Count > 0)
            {
                var topIndex = stack[stack.Count - 1];
                if (topIndex == -1)
                    break;
                var nextLeft = res.IndexOf("[",topIndex + 1);
                var nextRight = res.IndexOf("]", topIndex + 1);
                if (nextLeft != -1 && nextLeft < nextRight)
                {
                    stack.Add(nextLeft);
                    continue;
                }
                res = res.Remove(topIndex, nextRight - topIndex + 1);
                stack.RemoveAt(stack.Count - 1);
                if (stack.Count == 0 && topIndex < res.Length)
                {
                    stack.Add(res.IndexOf("[", topIndex));
                }
            }
            return res;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public ThreadExecutionMonitorDispatcher(IEventLog log, int threadIndex, IThreadMonitor callMonitor) 
            : base(threadIndex)
        {
            SafeDebug.AssertNotNull(callMonitor,"callMonitor");

            this.log = log;
            this.threadIndex = threadIndex;
            this.callMonitor = callMonitor;
            this.callStack = new SafeLinkedList<CallFrame>();

            this.current = new CallFrame(default(ICallFrame), default(Method), 0); //fake caller.
            this.callStack.AddFirst(new SafeLinkedList<CallFrame>.Node(this.current));

            if (firstTime)
            {
                // get rid of all [T] from types and methods 
                var env = MyEngine.EnvironmentVars;
                firstTime = false;
                foreach (var t in env.DontPreemptTypes)
                    newDontPreemptTypes.Add(eliminateParameterizedTypes(t));
                foreach (var m in env.DontPreemptMethods)
                    newDontPreemptMethods.Add(eliminateParameterizedTypes(m));
                foreach (var m in env.PrioritizeMethods)
                    newPrioritizeMethods.Add(eliminateParameterizedTypes(m));
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        protected void Trace(string arg)
        {

        }

        /// <summary>
        /// Log an exception
        /// </summary>
        protected void LogException(Exception e)
        {
            // throw e;
        }

        [DebuggerNonUserCodeAttribute]
        public override void Load(UIntPtr location, uint size, int codeLabel, bool is_volatile)
        {
            Exception exceptionToThrow = null;
            try
            {
                exceptionToThrow = this.callMonitor.Load(location, size, is_volatile);
                Trace("Load");
            }
            catch (Exception e)
            {
                LogException(e);
            }
            if (exceptionToThrow != null)
            {
                Microsoft.ExtendedReflection.Monitoring.ObjectTracking.StopMonitoring();
                throw exceptionToThrow;
            }
        }

        [DebuggerNonUserCodeAttribute]
        public override void Store(UIntPtr location, uint size, int codeLabel, bool is_volatile)
        {
            Exception exceptionToThrow = null;
            try
            {
                exceptionToThrow = this.callMonitor.Store(location, size, is_volatile);
                Trace("Store");
            }
            catch (Exception e)
            {
                LogException(e);
            }
            if (exceptionToThrow != null)
            {
                Microsoft.ExtendedReflection.Monitoring.ObjectTracking.StopMonitoring();
                throw exceptionToThrow;
            }
        }

        [DebuggerNonUserCodeAttribute]
        public override void AfterNewobjObject(object newObject)
        {
            Exception exceptionToThrow = null;
            try
            {
                exceptionToThrow = this.callMonitor.ObjectAllocationAccess(newObject);
                Trace("AfterNewobjObject");
            }
            catch (Exception e)
            {
                LogException(e);
            }
            if (exceptionToThrow != null)
            {
                Microsoft.ExtendedReflection.Monitoring.ObjectTracking.StopMonitoring(); 
                throw exceptionToThrow;
            }
        }


        private void PreemptionDisable()
        {
            if (MyEngine.EnvironmentVars.FlipPreemptSense)
            {
                MChessChess.PreemptionEnable();
            }
            else
            {
                MChessChess.PreemptionDisable();
            }
        }

        private void PreemptionEnable()
        {
            if (MyEngine.EnvironmentVars.FlipPreemptSense)
            {
                MChessChess.PreemptionDisable();
            }
            else
            {
                MChessChess.PreemptionEnable();
            }
        }

        #region Method call

        /// <summary>
        /// At start of method body.
        /// </summary>
        /// <remarks>Only one to push on callstack.</remarks>
        public override bool EnterMethod(Method method)
        {
            try
            {
                var env = MyEngine.EnvironmentVars;
                var alreadyDisabled = callStack.Count > 0 ? callStack.First.Value.PreemptionsDisabled : false;
                var alreadyPrioritized = callStack.Count > 0 ? callStack.First.Value.Prioritized : false;

                // Console.WriteLine("ENTER {0}:{1}:{2}:{3}", this.threadIndex, this.callStack.Count, method.FullName,alreadyDisabled);

                CallFrame newFrame = new CallFrame(
                    this.current, method, this.current.Index + 1);
                this.callStack.AddFirst(new SafeLinkedList<CallFrame>.Node(newFrame));
                this.current = newFrame;

                // TODO: check for diagnose flag
                if (env.Diagnose)
                {
                    instrumentedUninstrumentedMethods[method.GlobalIndex] = true;
                    instrumentedUninstrumentedMethods2[method.GlobalIndex] = method;
                }

                if (alreadyDisabled)
                {
                    // optimize, disabledness propagates from callers to callees
                    newFrame.PreemptionsDisabled = true;
                    return false;
                }

                if (alreadyPrioritized)
                {
                    // prioritization propagates from callers to callees as well, but we can't just return
                    // afterwards because this new method may be disabled still.
                    newFrame.Prioritized = true;
                }
               
                if (!disabledMethods.ContainsKey(method.GlobalIndex))
                {
                    TypeEx typ = null;
                    string type = "";
                    string namesp = "";
                    string assembly = "";
                    if (method.TryGetDeclaringType(out typ))
                    {
                        type = typ.FullName;
                        namesp = typ.Namespace;
                    }
                    try
                    {
                        assembly = method.Definition.Module.Assembly.ShortName;
                    }
                    catch (Exception) { }

                    disabledMethods.Add(method.GlobalIndex,
                        newDontPreemptTypes.Contains(eliminateParameterizedTypes(type)) ||
                        newDontPreemptMethods.Contains(eliminateParameterizedTypes(method.FullName)) ||
                        env.DontPreemptNamespaces.Contains(namesp) ||
                        env.DontPreemptAssemblies.Contains(assembly));
                    // Debug.Assert(!prioritizedMethods.ContainsKey(method.GlobalIndex), "ManagedChess.Engine assertion failed:\n   prioritizedMethods already contains the method: " + method.FullName);
                    // prioritizedMethods.Add(method.GlobalIndex, newPrioritizeMethods.Contains(eliminateParameterizedTypes(method.FullName)));
                }

                if (disabledMethods[method.GlobalIndex])
                {
                    newFrame.PreemptionsDisabled = true;
                    this.PreemptionDisable();
                }
                //if (prioritizedMethods[method.GlobalIndex])
                //{
                //    newFrame.Prioritized = true;
                //    MChessChess.PrioritizePreemptions();
                //}
            }
            catch (Exception e)
            {
                LogException(e);
            }

            return false;
        }


        /// <summary>
        /// Just before returning from method body.
        /// </summary>
        /// <remarks>Only method allowed to pop from callstack.</remarks>
        public override void LeaveMethod()
        {
            try
            {
                if (callStack.Count == 0)
                {
                    SafeDebug.Fail("cannot leave empty method");    //should not happen with the fake frame.
                    return;
                }

                if (callStack.Count == 1)
                {
                    this.callMonitor.RunCompleted();
                    return;    //exit from Main, if we did not get Enter(Main).
                }

                CallFrame poppedFrame = callStack.First.Value;
                callStack.RemoveFirst();
                this.current = callStack.First.Value;

                if (poppedFrame.PreemptionsDisabled && !this.current.PreemptionsDisabled)
                {
                    this.PreemptionEnable();
                }

                if (poppedFrame.Prioritized && !this.current.Prioritized)
                {
                    MChessChess.UnprioritizePreemptions();
                }
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        /// <summary>
        /// Constructor call.
        /// </summary>
        /// <param name="method"></param>
        public override void Newobj(Method method)
        {
            var env = MyEngine.EnvironmentVars;
            if (!env.Diagnose)
                return;
            try
            {
                this.current.Callee = method;
                this.current.CalleeReceiver = null;
                instrumentedUninstrumentedMethods[method.GlobalIndex] = false;
                instrumentedUninstrumentedMethods2[method.GlobalIndex] = method;
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        /// <summary>
        /// Regular instruction; <see cref="System.Reflection.Emit.OpCodes.Call"/>
        /// </summary>
        /// <param name="method">callee</param>
        public override void Call(Method method)
        {
            var env = MyEngine.EnvironmentVars;
            if (!env.Diagnose)
                return;
            try
            {
                this.current.Callee = method;
                this.current.CalleeReceiver = null;
                instrumentedUninstrumentedMethods[method.GlobalIndex] = false;
                instrumentedUninstrumentedMethods2[method.GlobalIndex] = method;
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }


        /// <summary>
        /// Regular instruction; <see cref="System.Reflection.Emit.OpCodes.Callvirt"/>
        /// </summary>
        /// <param name="method">callee before vtable lookup</param>
        public override void Callvirt(Method method)
        {
            var env = MyEngine.EnvironmentVars;
            if (!env.Diagnose)
                return;
            try
            {
                this.current.Callee = method;
                this.current.CalleeReceiver = null;

                //no callback to callmonitor, wait for CallvirtType.
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        /// <summary>
        /// This method is called after <see cref="Callvirt"/> to indicate the 
        /// actual type of the receiver object.
        /// </summary>
        /// <remarks>
        /// This method is only called when the receiver was not null,
        /// and an exception is thrown otherwise.
        /// When the receiver is a boxed value, 
        /// <paramref name="type"/> is the value type.
        /// When a constrained call is performed, 
        /// the type is the type pointed to by the receiver pointer.
        /// When the receiver is a remote object, <paramref name="type"/>
        /// might not be a class but an interface.
        /// </remarks>
        /// <param name="codeLabel">code label</param>
        /// <param name="type">actual receiver type</param>
        public override void CallvirtType(TypeEx type, int codeLabel)
        {
            var env = MyEngine.EnvironmentVars;
            if (!env.Diagnose)
                return;
            try
            {
                this.current.CalleeReceiver = null;
                //Trace("CallVirtType: " + (type == null ? "null" : type.FullName));

                if (type == null || this.current.Callee == null)
                    return; 

                TypeEx calleeType;
                if (this.current.Callee.TryGetDeclaringType(out calleeType)
                    && this.current.Callee.IsVirtual
                    && type.IsAssignableTo(calleeType)
                    && !type.IsAbstract)
                    this.current.Callee = type.VTableLookup(this.current.Callee);
                instrumentedUninstrumentedMethods[this.current.Callee.GlobalIndex] = false;
                instrumentedUninstrumentedMethods2[this.current.Callee.GlobalIndex] = this.current.Callee;
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        #endregion
    }
}

