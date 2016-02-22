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
using MChess;

using ChessTask = System.Int32;
using SyncVar = System.Int32;
using ManagedThreadId = System.Int32;

namespace Microsoft.ManagedChess.EREngine
{
    internal class SyncVarManager
    {
        internal class WeakReferenceComparer : EqualityComparer<WeakReference>
        {
            public override bool Equals(WeakReference x, WeakReference y)
            {
                object o_x, o_y;
                o_x = x.Target;
                o_y = y.Target;
                if (o_x == null && o_y == null)
                    return true;
                if (o_x != null && o_y != null)
                {
                    return ObjectIdentity<object>.Comparer.Equals(o_x, o_y);
                }
                return false;
            }

            public override int GetHashCode(WeakReference obj)
            {
                return ObjectIdentity<object>.Comparer.GetHashCode(obj.Target);
            }
        }

        public SyncVarManager()
        {
            // Chess Tasks get allocated in range 0..255
            obj_to_syncvar = new SafeDictionary<WeakReference, SyncVar>(new WeakReferenceComparer());
            gca_to_syncvar = new SafeDictionary<GCAddress, int>();
            initState.obj_to_syncvar = new SafeDictionary<WeakReference, SyncVar>(new WeakReferenceComparer());
            ctid_to_sem = new SafeDictionary<ChessTask, Semaphore>();
            tid_to_ctid = new SafeDictionary<ManagedThreadId, ChessTask>();
            ctid_to_tid = new SafeDictionary<ChessTask, IJoinable>();
            exited_threads = new SafeDictionary<ChessTask, bool>();
            handleToSyncVar = new SafeDictionary<IntPtr, int>();
            initState.handleToSyncVar = new SafeDictionary<IntPtr, int>();
            uintptr_to_syncvar = new SafeDictionary<ulong, int>();
        }

        ~SyncVarManager()
        {
            ReleaseGCAddresses();
        }

        public void Init()
        {
            // nothing, for now

        }
        public void SetInitState()
        {
            initState.obj_to_syncvar.Clear();
            foreach (SafeKeyValuePair<WeakReference, SyncVar> de in obj_to_syncvar)
                initState.obj_to_syncvar.Add(de.Key, de.Value);
            foreach (SafeKeyValuePair<System.IntPtr, SyncVar> de in handleToSyncVar)
                initState.handleToSyncVar.Add(de.Key, de.Value);
        }
        public void Reset()
        {
            obj_to_syncvar.Clear();
            foreach (SafeKeyValuePair<WeakReference, SyncVar> de in initState.obj_to_syncvar)
                obj_to_syncvar.Add(de.Key, de.Value);
            // good bye to exited threads
            foreach (SafeKeyValuePair<ChessTask, bool> de in exited_threads)
            {
                IJoinable t = null;
                if (ctid_to_tid.TryGetValue(de.Key, out t))
                {
                    t.Dispose(this);
                    ctid_to_tid.Remove(de.Key);
                }
            }

            handleToSyncVar.Clear();
            foreach (SafeKeyValuePair<System.IntPtr, SyncVar> de in initState.handleToSyncVar)
                handleToSyncVar.Add(de.Key, de.Value);
            // TODO: we need to keep track of init case, as with initState.obj
            // we need to explicitly release GC addresses
            ReleaseGCAddresses();
            exited_threads.Clear();

            // TODO: initialization as with others?
            uintptr_to_syncvar.Clear();
            // reset wrapper state
            MonitorHelper.Reset();
        }

        public void RemoveThreadHandleMapping(Thread t)
        {
            lock (this)
            {
                tid_to_ctid.Remove(t.ManagedThreadId);
            }
        }

        public void AddThreadHandleMapping(ChessTask ctid, Thread t)
        {
            lock (this)
            {
                // Console.WriteLine("Mapping tid={0} to ctid={1}", t.ManagedThreadId, ctid); 
                Conversions.NewVar(ctid);
                tid_to_ctid.Add(t.ManagedThreadId, ctid);
                ctid_to_tid.Add(ctid, new ThreadJoinable(t));
            }
        }

        public void AddIJoinableMapping(ChessTask ctid, IJoinable t)
        {
            lock (this)
            {
                // overwrite any previous mapping
                ctid_to_tid[ctid] = t;
            }
        }

        public void RegisterTaskSemaphore(ChessTask ctid, Semaphore initSem)
        {
            lock (this)
            {
                ctid_to_sem.Add(ctid, initSem);
            }
        }

        public ChessTask GetChessTask(Thread t)
        {
            lock (this)
            {
                ChessTask ret;
                tid_to_ctid.TryGetValue(t.ManagedThreadId, out ret);
                return ret;
            }
        }

        public ChessTask GetChessTaskFromThread(Thread t)
        {
            lock (this)
            {
                ChessTask ret;
                tid_to_ctid.TryGetValue(t.ManagedThreadId, out ret);
                return ret;
            }
        }

        public Semaphore GetTaskSemaphore(ChessTask ctid)
        {
            lock (this)
            {
                Semaphore ret = null;
                ctid_to_sem.TryGetValue(ctid, out ret);
                return ret;
            }
        }

        public IJoinable GetJoinableFromChessTask(ChessTask ctid)
        {
            lock (this)
            {
                IJoinable t;
                ctid_to_tid.TryGetValue(ctid, out t);
                return t;
            }
        }

        public SyncVar GetSyncVarFromGCAddress(GCAddress gca)
        {
            SyncVar ret;
            if (!gca_to_syncvar.TryGetValue(gca, out ret))
            {
                // NOTE: we are ignoring the (highly weird) case where the object might be a Thread
                ret = (SyncVar)MChessChess.GetNextSyncVar();
                Conversions.NewVar(ret);
                gca.AddRef();   // VERY IMPORTANT: must ref count to keep unmanaged cell alive
                gca_to_syncvar.Add(gca, ret);
            }
            return ret;
        }

        public SyncVar GetSyncVarFromObject(object o)
        {
            SyncVar ret;
            WeakReference w = new WeakReference(o);

            if (o.GetType() == typeof(System.Threading.Thread))
            {
                // in this case, the SyncVar value is the ChessTask id
                ret = tid_to_ctid[(o as System.Threading.Thread).ManagedThreadId];
                return ret;
            }
            else if (!obj_to_syncvar.TryGetValue(w, out ret))
            {
                ret = (SyncVar)MChessChess.GetNextSyncVar();
                Conversions.NewVar(ret);
                obj_to_syncvar.Add(w, ret);
            }
            return ret;
        }

        public SyncVar GetSyncVarFromUIntPtr(UIntPtr o)
        {
            SyncVar ret;
            if (!uintptr_to_syncvar.TryGetValue(o.ToUInt64(), out ret))
            {
                ret = (SyncVar)MChessChess.GetNextSyncVar();
                Conversions.NewVar(ret);
                uintptr_to_syncvar.Add(o.ToUInt64(), ret);
            }
            return ret;
        }

        public void AddNativeHandleForSyncVar(System.IntPtr handle, SyncVar var)
        {
            lock (this)
            {
                // erase previous handle information, if any
                handleToSyncVar[handle] = var;
            }
        }

        public bool DuplicateNativeHandle(System.IntPtr orig, System.IntPtr copy)
        {
            lock (this)
            {
                // the only state for NativeHandle is handleToSyncVar
                // Also, calling GetSyncVarFromNativeHandle allocates a sync var if it has not been allocated yet
                // This code relies on the fact that Handles for threads are immediately registered
                SyncVar v = GetSyncVarFromNativeHandle(orig);
                handleToSyncVar[copy] = v;
                return false;
            }
        }

        public void RemoveNativeHandle(System.IntPtr handle)
        {
            lock (this)
            {
                handleToSyncVar.Remove(handle);
            }
        }

        public SyncVar GetSyncVarFromNativeHandle(System.IntPtr handle)
        {
            lock (this)
            {
                SyncVar ret;
                if (handleToSyncVar.TryGetValue(handle, out ret))
                {
                    return ret;
                }
                ret = (SyncVar)MChessChess.GetNextSyncVar();
                Conversions.NewVar(ret);
                handleToSyncVar[handle] = ret;
                return ret;
            }
        }

        public void DumpSyncVars()
        {
            lock (this)
            {
                foreach (SafeKeyValuePair<WeakReference, SyncVar> de in obj_to_syncvar)
                {
                    object tgt = de.Key.Target;
                    if (tgt != null)
                    {
                        Console.WriteLine("still have {0}", tgt.GetType());
                    }
                }
            }
        }

        public void TaskEnd(ChessTask ctid)
        {
            // remove everything to do with ctid
            lock (this)
            {
                ctid_to_sem.Remove(ctid);
                exited_threads[ctid] = true;
            }
        }

        public void ReleaseGCAddresses()
        {
            lock (this)
            {
                lock (gca_to_syncvar)
                {
                    foreach (SafeKeyValuePair<GCAddress, SyncVar> de in gca_to_syncvar)
                    {
                        de.Key.Release();
                    }
                    gca_to_syncvar.Clear();
                }
            }
        }


        private struct InternalState
        {
            public SafeDictionary<WeakReference, SyncVar> obj_to_syncvar;
            public SafeDictionary<System.IntPtr, SyncVar> handleToSyncVar;
        };
        private InternalState initState;

        private SafeDictionary<ChessTask, Semaphore> ctid_to_sem;
        private SafeDictionary<ManagedThreadId, ChessTask> tid_to_ctid;
        private SafeDictionary<ChessTask, IJoinable> ctid_to_tid;
        private SafeDictionary<WeakReference, SyncVar> obj_to_syncvar;
        private SafeDictionary<UInt64, SyncVar> uintptr_to_syncvar;
        private SafeDictionary<GCAddress, SyncVar> gca_to_syncvar;
        private SafeDictionary<ChessTask, bool> exited_threads;
        private SafeDictionary<System.IntPtr, SyncVar> handleToSyncVar;
    }
}
