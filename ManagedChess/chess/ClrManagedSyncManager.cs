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
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using System.IO;

using Microsoft.ExtendedReflection.Monitoring; 
using Microsoft.ExtendedReflection.Utilities;
using Microsoft.ExtendedReflection.Collections;
using Microsoft.ExtendedReflection.Utilities.Safe.Diagnostics;

using Microsoft.ManagedChess.EREngine.CallsOnly;

using ChessTask = System.Int32;
using SyncVar = System.Int32;
using ManagedThreadId = System.Int32;
using MChess;
using Microsoft.ExtendedReflection.Metadata;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;


namespace Microsoft.ManagedChess.EREngine
{
    internal delegate void ExitCallback(ChessExitCode code, Exception exception);

    [DebuggerNonUserCode]
    internal class ClrSyncManager : MSyncManager
    {
        public ClrSyncManager(MChessOptions m)
        {
            if (onlyOne == null)
            {
                ClrSyncManager.onlyOne = this;
                Conversions.SetOptions(m);
                this.TrackGC = m.monitorAccesses || m.monitorVolatiles || m.preemptAccesses;
                this.TrackVolatile = m.monitorVolatiles;
                this.allowFinalizers = true;
                this.CurrentTid = -1;
                this.initTask = -1;
                this.syncVarManager = new SyncVarManager();
                this.joinableTasks = new SafeDictionary<ChessTask, bool>();
                this.weakReferences = new SafeDictionary<object, bool>(ObjectIdentity<object>.Comparer);
                this.finalizeThese = new SafeList<object>();
                this.mco = m;
                if (TrackGC || TrackVolatile)
                {
                    // add handlers to get raw reads and writes
                    ObjectAccessThreadMonitor.ReadRawAccess += new RawAccessHandler(this.ObjectAccessThreadMonitor_ReadAccess);
                    ObjectAccessThreadMonitor.WriteRawAccess += new RawAccessHandler(this.ObjectAccessThreadMonitor_WriteAccess);
                }
                if (m.finesse)
                {
                    this.DelayFinalizers = false;
                    ObjectAccessThreadMonitor.ObjectAllocationHandlerEvent += new ObjectAllocationHandler(this.ObjectAccessThreadMonitor_AllocationAccess);
                }
            }
            else
            {
                throw new System.InvalidOperationException();
            }
        }

        public static ClrSyncManager SyncManager
        {
            get { return onlyOne; }
        }

        public static void Remove()
        {
            onlyOne = null;
        }

        public override void EnterChess()
        {
            WrapperSentry.EnterChessImpl();
        }

        public override void LeaveChess()
        {
            WrapperSentry.LeaveChessImpl();
        }

        public override void Init(ChessTask ctid)
        {
            this.IsInitialized = true;
            CurrentTid = initTask = ctid;
            initThread = Thread.CurrentThread;

            Conversions.Init(this);

            syncVarManager.Init();
            Semaphore initSem = new Semaphore(0, 1);
            syncVarManager.RegisterTaskSemaphore(initTask, initSem);
            syncVarManager.AddThreadHandleMapping(ctid, Thread.CurrentThread);

        }

        public override void SetInitState()
        {
            syncVarManager.SetInitState();
            Conversions.SetInitState();
        }

        public void DumpSVM()
        {
            syncVarManager.DumpSyncVars();
        }

        public override void ScheduleTask(ChessTask next, bool atTermination)
        {
            if (CurrentTid == next)
                return;
            Semaphore currSem = null;
            Semaphore nextSem = syncVarManager.GetTaskSemaphore(next);
            Debug.Assert(nextSem != null);
            if (!atTermination)
                currSem = syncVarManager.GetTaskSemaphore(CurrentTid);
            CurrentTid = next;
            nextSem.Release();
            if (!atTermination)
            {
                Debug.Assert(currSem != null);
                currSem.WaitOne();
                OnWakeUp();
            }
        }
        
        public override void Reset()
        {
            syncVarManager.Reset();
            CurrentTid = initTask;
            NumThreads = 0;
            joinableTasks.Clear();
            weakReferences.Clear();
            pinnedObjs.Clear();
            terminatingThread = null;
            rwhToInfo.Clear();
            Conversions.Reset();
            count = 0;
            cwiToInfo.Clear();
        }

        public override void Shutdown(Exception e)
        {
            // finalizers
            IsInitialized = false;
            syncVarManager.ReleaseGCAddresses();
            this.ExitCode = ChessExitCode.TestFailure;
            SafeDebug.Assert(this.exitCallback != null, "this.exitCallBack != null");

            MChessChess.LeaveChess();
            this.exitCallback(ChessExitCode.TestFailure, e);
        }

        public override void Shutdown(int code)
        {
            // finalizers
            IsInitialized = false;
            syncVarManager.ReleaseGCAddresses();
            this.ExitCode = (ChessExitCode)code;
            SafeDebug.Assert(this.exitCallback != null, "this.exitCallBack != null");
            this.exitCallback((ChessExitCode)code,null);
        }
        
        public override void TaskEnd(ChessTask ctid)
        {
            Debug.Assert(CurrentTid == ctid);
            IJoinable t = syncVarManager.GetJoinableFromChessTask(ctid);
            JoinableTaskEnd(t);
            syncVarManager.TaskEnd(ctid);
            NumThreads--;
        }

        public override void DebugBreak()
        {
            SafeDebugger.Break();
        }

        public override bool IsDebuggerPresent()
        {
            return Debugger.IsAttached;
        }

        public override string GetTaskName(int ctid)
        {
            IJoinable j = syncVarManager.GetJoinableFromChessTask(ctid);
            if (j != null)
            {
                string name = j.Name();
                if (name != null)
                {
                    return name;
                }
            }
            return base.GetTaskName(ctid);
        }

        public override string GetDataVarLabel(int datavar)
        {
            return Conversions.GetDataVarLabel(datavar);
        }

        public override bool GetCurrentStackTrace(int n, System.Collections.Generic.List<string> procs,
            System.Collections.Generic.List<string> files, System.Collections.Generic.List<int> lineNos)
        {
            StackTrace st = new StackTrace(true);
            for (int i = 0; i < st.FrameCount; i++)
            {
                System.Diagnostics.StackFrame sf = st.GetFrame(i);
                string assembly_name = sf.GetMethod().Module.Assembly.GetName().ToString();

                // this is fragile
                if (assembly_name.Contains(".ManagedChess") || assembly_name.Contains(".ExtendedReflection"))
                    continue;

                string file = sf.GetFileName();

                if (file == null || file == "")
                {
                    file = "NONE";
                }

                string method = sf.GetMethod().Name;
                int lineno = sf.GetFileLineNumber();
                procs.Add(method);
                files.Add(file);
                lineNos.Add(lineno);

                if (lineNos.Count >= n)
                    break;
            }
            return true;
        }

        public override string GetFullyQualifiedTopProcedure()
        {
            StackTrace st = new StackTrace(false);
            for (int i = 0; i < st.FrameCount; i++)
            {
                System.Diagnostics.StackFrame sf = st.GetFrame(i);
                string assembly_name = sf.GetMethod().Module.Assembly.GetName().ToString();

                // this is fragile
                if (assembly_name.Contains(".ManagedChess") || assembly_name.Contains(".ExtendedReflection"))
                    continue;

                var erMethod = MetadataFromReflection.GetMethod(sf.GetMethod());

                return erMethod.FullName;
            }
            return "";
        }

        public void TaskYield()
        {
            if (!MChessChess.TaskYield())
            {
                ChessDetach();
            }
        }
        
        public void TaskSuspend(ChessTask tid)
        {
            if (!MChessChess.TaskSuspend(tid))
            {
                ChessDetach();
            }
        }

        public int TaskFork()
        {
            MChessInt i = new MChessInt();
            if (!MChessChess.TaskFork(i))
            {
                ChessDetach();
            }
            return i.i;
        }

        public void TaskResume(int ctid)
        {
            if (!MChessChess.TaskResume(ctid))
            {
                ChessDetach();
            }
        }

        public int Choose(int numChoices)
        {
            return MChessChess.Choose(numChoices);
        }


        public bool BreakDeadlockMode = false;

        public void LocalBacktrack()
        {
            if (!MChessChess.LocalBacktrack())
            {
                if (MChessChess.IsBreakingDeadlock())
                {
                    BreakDeadlockMode = true;
                    throw new Exception("BreakDeadlockException");
                }
                else
                    ChessDetach();
            }
        }

        public void RunFinalizers()
        {
            if (finalizeThese.Count > 0)
            {
                bool remember_initialized = IsInitialized;
                allowFinalizers = false;
                IsInitialized = false;
                // make sure we process in correct order
                finalizeThese.Reverse();
                foreach (object o in finalizeThese)
                {
                    ReflectionHelper.InvokeFinalizer(o);
                    GC.SuppressFinalize(o);
                }
                GC.WaitForPendingFinalizers();
                finalizeThese.Clear();
                allowFinalizers = true;
                IsInitialized = remember_initialized;
            }
        }

        // for use by instrumentation layer (not by CHESS)
        public void SyncVarAccess(SyncVar sv, MSyncVarOp mop)
        {
            if (!Conversions.SyncVarAccess(sv, mop))
            {
                ChessDetach();
            }
        }

        public void SyncVarAccess(object o, MSyncVarOp mop)
        {
            if (o == null)
                return;
            if (!Conversions.SyncVarAccess(syncVarManager.GetSyncVarFromObject(o), mop))
            {
                ChessDetach();
            }
        }

        public SyncVar SyncVarAccess(GCAddress o, MSyncVarOp mop)
        {
            SyncVar sv = syncVarManager.GetSyncVarFromGCAddress(o);
            if (!Conversions.SyncVarAccess(sv, mop))
            {
                ChessDetach();
            }
            return sv;
        }

        public SyncVar SyncVarAccess(UIntPtr o, MSyncVarOp mop)
        {
            SyncVar sv = syncVarManager.GetSyncVarFromUIntPtr(o);
            if (!Conversions.SyncVarAccess(sv, mop))
            {
                ChessDetach();
            }
            return sv;
        }

        public void AggregateSyncVarAccess(object[] syncops, MSyncVarOp op) {
            SyncVar[] sarr = new SyncVar[syncops.Length];
            for (int i = 0; i < sarr.Length; i++)
            {
                sarr[i] = syncVarManager.GetSyncVarFromObject(syncops[i]);
            }
            if (!Conversions.AggregateSyncVarAccess(sarr, op))
            {
                ChessDetach();
            }
        }

        public void AggregateSyncVarAccess(SyncVar[] v, MSyncVarOp op)
        {
            if (!Conversions.AggregateSyncVarAccess(v, op))
            {
                ChessDetach();
            }
        }

        public void AddNativeHandleForSyncVar(System.IntPtr handle, SyncVar var)
        {
            syncVarManager.AddNativeHandleForSyncVar(handle, var);
        }

        public bool DuplicateNativeHandle(System.IntPtr orig, System.IntPtr copy)
        {
            return syncVarManager.DuplicateNativeHandle(orig, copy);
        }

        public SyncVar GetSyncVarFromNativeHandle(System.IntPtr handle)
        {
            return syncVarManager.GetSyncVarFromNativeHandle(handle);
        }

        public SyncVar GetSyncVarFromObject(object obj)
        {
            return syncVarManager.GetSyncVarFromObject(obj);
        }

        public void PinObject(object o)
        {
            pinnedObjs.Add(o);
        }

        public void CommitSyncVarAccess()
        {
            if (!Conversions.CommitSyncVarAccess())
            {
                ChessDetach();
            }
        }
        
        public void MarkTimeout()
        {
            MChessChess.MarkTimeout();
        }

        public void SetMethodInfo(String info) {
            Debug.Assert(info != null);
            MChessChess.SetNextEventAttribute((uint) EventAttributeEnum.INSTR_METHOD, info);
        }

        public void DataVarAccess(GCAddress o, bool isWrite)
        {
            if (!Conversions.DataVarAccess(syncVarManager.GetSyncVarFromGCAddress(o), isWrite))
            {
                ChessDetach();
            }
        }

        public void DataVarAccess(UIntPtr o, bool isWrite)
        {
            if (!Conversions.DataVarAccess(syncVarManager.GetSyncVarFromUIntPtr(o), isWrite))
            {
                ChessDetach();
            }
        }

        public void ThreadBegin(Thread t)
        {
            ThreadBegin(syncVarManager.GetTaskSemaphore(syncVarManager.GetChessTask(t)));
        }

        public void ThreadBegin(Semaphore s)
        {
            NumThreads++;
            s.WaitOne();
            if (!MChessChess.TaskBegin())
            {
                ChessDetach();
            }
            OnWakeUp();
            if (MyEngine.EnvironmentVars.FlipPreemptSense)
            {
                MChessChess.PreemptionDisable();
            }
        }

        public void ThreadEnd(Thread t) {
            int ctid = syncVarManager.GetChessTaskFromThread(t);
            ThreadEnd(ctid);
        }

        public void ThreadEnd(int ctid)
        {
            if (ctid > 0)
                Debug.Assert(ctid == CurrentTid);
            if (!MChessChess.TaskEnd())
            {
                if (MChessChess.IsBreakingDeadlock())
                {
                    BreakDeadlockMode = true;
                    MChessChess.WakeNextDeadlockedThread(false, false);
                }
                else
                    ChessDetach();
            }
        }

        public void RegisterTaskSemaphore(ChessTask child, Semaphore sem, bool isJoinable)
        {
            if (isJoinable)
            {
                AddJoinableTask(child);
            }
            syncVarManager.RegisterTaskSemaphore(child, sem);
        }

        public void AddIJoinable(ChessTask child, IJoinable joinable)
        {
            syncVarManager.AddIJoinableMapping(child, joinable);
        }
        public void AddChildHandle(ChessTask child, Thread hChildThread)
        {
            syncVarManager.AddThreadHandleMapping(child, hChildThread);
        }

        public ChessTask GetChessTask(Thread t)
        {
            return syncVarManager.GetChessTask(t);
        }
        
        public void AddJoinableTask(ChessTask t){
            joinableTasks.Add(t,true);
        }

        public bool IsJoinableTask(ChessTask t) {
            return joinableTasks.ContainsKey(t);
        }

        public void RemoveJoinableTask(ChessTask t){
           joinableTasks.Remove(t);
        }

        public void JoinableTaskEnd(IJoinable t){
            if(!IsJoinableTask(CurrentTid))
		        return;
	        RemoveJoinableTask(CurrentTid);
            terminatingThread = t;
        }

        public Exception ObjectAccessThreadMonitor_WriteAccess(UIntPtr address, uint size, bool @volatile)
        {
            return raw_access(address, size, @volatile, true);
        }

        public Exception ObjectAccessThreadMonitor_ReadAccess(UIntPtr address, uint size, bool @volatile)
        {
            return raw_access(address, size, @volatile, false);
        }

        private bool InAllocationAccess = false;

        public Exception ObjectAccessThreadMonitor_AllocationAccess(object allocatedObject)
        {
            // we are screwed if .Acquire allocates objects - *uck
            using (_ProtectingThreadContext.Acquire())
            {
                // This flag prevents recursive calls to new (we allocate objects in the wrapper)
                // This flag is protected by the Lock acquire above
                if (InAllocationAccess)
                {
                    return null;
                }

                try
                {
                    InAllocationAccess = true;
                    if (WrapperSentry.Wrap())
                    {
                        // Note that we are not entering wrappers here
                        // Because we want the thread forked to be under CHESS' control
                        // However, the check for .Wrap() guarantees that this wrapper is disabled for objects allocated by our wrappers
                        System.GC.SuppressFinalize(allocatedObject);

                        ThreadStart finalizerThreadStart = new ThreadStart(() =>
                          {
                              try
                              {
                                  typeof(Object).GetMethod("Finalize", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(allocatedObject, null);
                              }
                              catch (TargetInvocationException e)
                              {
                                  throw e.InnerException;
                              }
                          });

                        Thread finalizerThread = Helper.ConstructThread<ThreadStart>(
                            delegate(ThreadStart del) { return new Thread(del); },
                            finalizerThreadStart,
                            Helper.WrapThreadStart
                            );

                        Helper.StartHelper(finalizerThread, null, false);

                    }
                }
                catch (Exception e)
                {
                    InAllocationAccess = false;
                    return e;
                }
                InAllocationAccess = false;
            }
            return null;
        }

        private Exception raw_access(UIntPtr address, uint size, bool @volatile, bool write)
        {
            using (_ProtectingThreadContext.Acquire())
            {
                try
                {
                    if (WrapperSentry.Wrap())
                    {
                        using (new WrapperSentry())
                        {
                            GCAddress gcAddress;
                            if (TrackGC && GCAddress.FromAddress(address, out gcAddress))
                            {
                                try
                                {
                                    if (@volatile)
                                    {
                                        SetMethodInfo(write ? "vol.write" : "vol.read");
                                        SyncVarAccess(gcAddress, write ? MSyncVarOp.RWVAR_WRITE : MSyncVarOp.RWVAR_READ);
                                        CommitSyncVarAccess();
                                    }
                                    else
                                    {
                                        SetMethodInfo(write ? "write" : "read");
                                        DataVarAccess(gcAddress, write);
                                    }

                                }
                                finally
                                {
                                    gcAddress.Release();
                                }
                            }
                            else
                            {
                                // TODO: take size into account
                                if (@volatile)
                                {
                                    SetMethodInfo(write ? "vol.write" : "vol.read");
                                    SyncVarAccess(address, write ? MSyncVarOp.RWVAR_WRITE : MSyncVarOp.RWVAR_READ);
                                    CommitSyncVarAccess();
                                }
                                else
                                {
                                    SetMethodInfo(write ? "write" : "read");
                                    DataVarAccess(address, write);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    return e;
                }
            }
            return null;
        }

        private void OnWakeUp()
        {
            if (terminatingThread != null)
            {
                // wait for the terminating thread to finish
                terminatingThread.Join();
                terminatingThread = null;
            }
        }

        private void ChessDetach()
        {

        }

        public void MakeStrongReference(object o)
        {
            if (!weakReferences.ContainsKey(o))
            {
                weakReferences.Add(o, true);
            }
        }

        public void FinalizeLater(object o)
        {
            if (allowFinalizers)
                finalizeThese.Add(o);
        }

        public void SetExitCallBack(ExitCallback ec) { this.exitCallback = ec; }
        public ChessExitCode ExitCode { set; get; }
        public ChessTask CurrentTid { get; private set; }
        public bool TrackGC { get; private set; }
        public bool TrackVolatile { get; private set; }
        public bool DelayFinalizers { get; private set; }
        public int NumThreads { get; private set; }
        public bool IsInitialized { get; private set; }
        public int ProcessorCount { get { return mco.processorCount; } }
        public SafeDictionary<RegisteredWaitHandle,
                Helper.WaitOrTimerCallbackRoutineArg> rwhToInfo =
                    new SafeDictionary<RegisteredWaitHandle, Helper.WaitOrTimerCallbackRoutineArg>();
        public SafeDictionary<Object, Object> cwiToInfo = new SafeDictionary<Object, Object>();

        // for dealing with DateTime wrappers
        private global::System.DateTime init = new global::System.DateTime(2009, 1, 1, 0, 0, 0);
        private int incr = 1; // in milliseconds
        private int count = 0;
        public global::System.DateTime Now
        {
            get
            {
                global::System.DateTime retVal = init.AddMilliseconds(count * incr);
                count++;
                return retVal;
            }
        }

        // for dealing with SpinWait
        private object spinWait = new Object();
        public void SpinWait() {
            this.SyncVarAccess(spinWait, MSyncVarOp.RWVAR_READWRITE);
            this.CommitSyncVarAccess();
        }

        private ExitCallback exitCallback;
        private bool allowFinalizers;
        private SyncVarManager syncVarManager;
        private Thread initThread; // the initial thread for passing exceptions
        private ChessTask initTask;    // the inital task
        static private ClrSyncManager onlyOne = null;

        // for dealing with t.Join properly
        private SafeDictionary<ChessTask, bool> joinableTasks;
        private SafeDictionary<object, bool> weakReferences;
        private System.Collections.Generic.List<object> pinnedObjs = new System.Collections.Generic.List<object>();
        private SafeList<object> finalizeThese;
        private IJoinable terminatingThread;
        private MChessOptions mco;
    }
}
