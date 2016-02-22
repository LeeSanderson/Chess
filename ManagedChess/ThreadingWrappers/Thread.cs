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
using System.Threading;
using System.Security.Permissions;
using ClrThread = System.Int32;
using ChessTask = System.Int32;

using MChess;
using Microsoft.ManagedChess.EREngine;
using System.Diagnostics;

namespace __Substitutions.System.Threading
{
    /// <summary>
    /// Replacement method for System.Threading.Thread constructors
    /// </summary>
    /// 
    [DebuggerNonUserCode]
    public static partial class Thread
    {
        public static void SpinWait(int time)
        {
            Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    if (time != 0)
                    {
                        manager.SpinWait();
                    }
                    return false;
                },
                delegate() { Original::Thread.SpinWait(time); return false; }
                );
        }

        public static void Sleep(int time)
        {
            Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    manager.TaskYield();
                    return false;
                },
                delegate() { Original::Thread.Sleep(time); return false; }
                );
        }

        public static void Sleep(TimeSpan timeout)
        {
            Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    if (timeout.TotalMilliseconds < Timeout.Infinite)
                        throw new ArgumentOutOfRangeException();
                    // sleep with TimeSpan cannot be infinite
                    manager.TaskYield();
                    return false;
                },
                delegate() { Original::Thread.Sleep(timeout); return false; }
                );
        }

        public static Original::Thread ___ctor_newobj(Original::ThreadStart start)
        {
            return Helper.ConstructThread<Original::ThreadStart>(
                delegate(Original::ThreadStart del) { return new Original::Thread(del); },
                start,
                Helper.WrapThreadStart
                );
        }

        public static Original::Thread ___ctor_newobj(Original::ThreadStart start, int size)
        {
            return Helper.ConstructThread<Original::ThreadStart>(
                delegate(Original::ThreadStart delStart) { return new Original::Thread(delStart, size); },
                start,
                Helper.WrapThreadStart
                );
        }

        public static Original::Thread ___ctor_newobj(Original::ParameterizedThreadStart start)
        {
            return Helper.ConstructThread<Original::ParameterizedThreadStart>(
                delegate(Original::ParameterizedThreadStart delStart) { return new Original::Thread(delStart); },
                start,
                Helper.WrapParamThreadStart
                );
        }

        public static Original::Thread ___ctor_newobj(Original::ParameterizedThreadStart start, int size)
        {
            return Helper.ConstructThread<Original::ParameterizedThreadStart>(
                delegate(Original::ParameterizedThreadStart delStart) { return new Original::Thread(delStart, size); },
                start,
                Helper.WrapParamThreadStart
                );
        }

        // Blocks the calling thread until a thread terminates, while continuing to perform standard COM
        // and SendMessage pumping.
        public static void Join(Original::Thread thread)
        {
            Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager m) { JoinRaw(thread, m, -1); return true;},
                delegate() { thread.Join(); return true;}
            );
        }

        // Blocks the calling thread until a thread terminates or the specified time elapses, 
        // while continuing to perform standard COM and SendMessage pumping.
        public static bool Join(Original::Thread thread, int t)
        {
            return Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager m) { return JoinRaw(thread, m, t); },
                delegate() { return thread.Join(t);}
            );
        }

        // Blocks the calling thread until a thread terminates or the specified time elapses, 
        // while continuing to perform standard COM and SendMessage pumping.
        public static bool Join(Original::Thread thread, TimeSpan timespan)
        {
            return Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager m) { return JoinRaw(thread, m, (int) timespan.TotalMilliseconds); },
                delegate() { return thread.Join(timespan);}
            );
        }

        private static bool JoinRaw(Original::Thread t, ClrSyncManager manager, int millisecondsTimeout)
        {
            if (millisecondsTimeout < Timeout.Infinite)
                throw new ArgumentOutOfRangeException();
            bool ret;
            while (true)
            {
                manager.SetMethodInfo("Thread.Join(" + millisecondsTimeout + ")");
                manager.SyncVarAccess(t, MSyncVarOp.TASK_JOIN);
                try
                {
                    ret = t.Join(0);
                }
                catch (Exception e)
                {
                    manager.CommitSyncVarAccess();
                    throw e;
                }
                if (ret)
                    break;  // join succeeded
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

        public static void Start(Original::Thread thread)
        {
            Helper.StartHelper(thread, null, false);
        }

        public static void Start(Original::Thread thread, object obj)
        {
            Helper.StartHelper(thread, obj, true);
        }

        // memory barrier
        public static void MemoryBarrier()
        {
            Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    manager.SetMethodInfo("Thread.MemoryBarrier");
                    manager.SyncVarAccess(Original.Thread.CurrentThread, MSyncVarOp.TASK_FENCE);
                    manager.CommitSyncVarAccess();
                    return true;
                },
                delegate()
                {
                    return true; // memory barrier is no-op under chess
                });
        }
        
        public static int get_ManagedThreadId(Original::Thread thread)
        {
            return Helper.SimpleWrap<int>(
                delegate(ClrSyncManager m) { return 10000 + m.GetChessTask(thread); },
                delegate() { return thread.ManagedThreadId; }
            );
        }
 

        #region Volatile Read (13 overloads)
        public static byte VolatileRead(ref byte address)
        {
            return Helper.SimpleRefWrap<byte>(
                ref address,
                delegate(ref byte tgt) { return Original::Thread.VolatileRead(ref tgt); },
                "VolatileRead");
        }

        public static double VolatileRead(ref double address)
        {
            return Helper.SimpleRefWrap<double>(
                ref address,
                delegate(ref double tgt) { return Original::Thread.VolatileRead(ref tgt); },
                "VolatileRead");
        }

        public static short VolatileRead(ref short address)
        {
            return Helper.SimpleRefWrap<short>(
                ref address,
                delegate(ref short tgt) { return Original::Thread.VolatileRead(ref tgt); },
                "VolatileRead");
        }

        public static int VolatileRead(ref int address)
        {
            return Helper.SimpleRefWrap<int>(
                ref address,
                delegate(ref int tgt) { return Original::Thread.VolatileRead(ref tgt); },
                "VolatileRead");
        }

        public static long VolatileRead(ref long address)
        {
            return Helper.SimpleRefWrap<long>(
                ref address,
                delegate(ref long tgt) { return Original::Thread.VolatileRead(ref tgt); },
                "VolatileRead");
        }

        public static global::System.IntPtr VolatileRead(ref global::System.IntPtr address)
        {
            return Helper.SimpleRefWrap<global::System.IntPtr>(
                ref address,
                delegate(ref global::System.IntPtr tgt) { return Original::Thread.VolatileRead(ref tgt); },
                "VolatileRead");
        }

        public static object VolatileRead(ref object address)
        {
            return Helper.SimpleRefWrap<object>(
                ref address,
                delegate(ref object tgt) { return Original::Thread.VolatileRead(ref tgt); },
                "VolatileRead");
        }

        public static sbyte VolatileRead(ref sbyte address)
        {
            return Helper.SimpleRefWrap<sbyte>(
                ref address,
                delegate(ref sbyte tgt) { return Original::Thread.VolatileRead(ref tgt); },
                "VolatileRead");
        }

        public static float VolatileRead(ref float address)
        {
            return Helper.SimpleRefWrap<float>(
                ref address,
                delegate(ref float tgt) { return Original::Thread.VolatileRead(ref tgt); },
                "VolatileRead");
        }

        public static ushort VolatileRead(ref ushort address)
        {
            return Helper.SimpleRefWrap<ushort>(
                ref address,
                delegate(ref ushort tgt) { return Original::Thread.VolatileRead(ref tgt); },
                "VolatileRead");
        }

        public static uint VolatileRead(ref uint address)
        {
            return Helper.SimpleRefWrap<uint>(
                ref address,
                delegate(ref uint tgt) { return Original::Thread.VolatileRead(ref tgt); },
                "VolatileRead");
        }

        public static ulong VolatileRead(ref ulong address)
        {
            return Helper.SimpleRefWrap<ulong>(
                ref address,
                delegate(ref ulong tgt) { return Original::Thread.VolatileRead(ref tgt); },
                "VolatileRead");
        }

        public static global::System.UIntPtr VolatileRead(ref global::System.UIntPtr address)
        {
            return Helper.SimpleRefWrap<global::System.UIntPtr>(
                ref address,
                delegate(ref global::System.UIntPtr tgt) { return Original::Thread.VolatileRead(ref tgt); },
                "VolatileRead");
        }
        #endregion


        #region VolatileWrite (13 overloads)
        
        public static void VolatileWrite(ref byte address, byte value)
        {
            Helper.SimpleRefWrap<byte>(
                ref address,
                delegate(ref byte tgt) { Original::Thread.VolatileWrite(ref tgt, value); return 0;  },
                "VolatileWrite");
        }

        public static void VolatileWrite(ref double address, double value)
        {
            Helper.SimpleRefWrap<double>(
                ref address,
                delegate(ref double tgt) { Original::Thread.VolatileWrite(ref tgt, value); return 0; },
                "VolatileWrite");
        }

        public static void VolatileWrite(ref short address, short value)
        {
            Helper.SimpleRefWrap<short>(
                ref address,
                delegate(ref short tgt) { Original::Thread.VolatileWrite(ref tgt, value); return 0; },
                "VolatileWrite");
        }

        public static void VolatileWrite(ref int address, int value)
        {
            Helper.SimpleRefWrap<int>(
                ref address,
                delegate(ref int tgt) { Original::Thread.VolatileWrite(ref tgt, value); return 0; },
                "VolatileWrite");
        }

        public static void VolatileWrite(ref long address, long value)
        {
            Helper.SimpleRefWrap<long>(
                ref address,
                delegate(ref long tgt) { Original::Thread.VolatileWrite(ref tgt, value); return 0; },
                "VolatileWrite");
        }

        public static void VolatileWrite(ref global::System.IntPtr address, global::System.IntPtr value)
        {
            Helper.SimpleRefWrap<global::System.IntPtr>(
                ref address,
                delegate(ref global::System.IntPtr tgt) { Original::Thread.VolatileWrite(ref tgt, value); return global::System.IntPtr.Zero; },
                "VolatileWrite");
        }

        public static void VolatileWrite(ref object address, object value)
        {
            Helper.SimpleRefWrap<object>(
                ref address,
                delegate(ref object tgt) { Original::Thread.VolatileWrite(ref tgt, value); return null; },
                "VolatileWrite");
        }

        public static void VolatileWrite(ref sbyte address, sbyte value)
        {
            Helper.SimpleRefWrap<sbyte>(
                ref address,
                delegate(ref sbyte tgt) { Original::Thread.VolatileWrite(ref tgt, value); return 0; },
                "VolatileWrite");
        }

        public static void VolatileWrite(ref float address, float value)
        {
            Helper.SimpleRefWrap<float>(
                ref address,
                delegate(ref float tgt) { Original::Thread.VolatileWrite(ref tgt, value); return 0; },
                "VolatileWrite");
        }

        public static void VolatileWrite(ref ushort address, ushort value)
        {
            Helper.SimpleRefWrap<ushort>(
                ref address,
                delegate(ref ushort tgt) { Original::Thread.VolatileWrite(ref tgt, value); return 0; },
                "VolatileWrite");
        }

        public static void VolatileWrite(ref uint address, uint value)
        {
            Helper.SimpleRefWrap<uint>(
                ref address,
                delegate(ref uint tgt) { Original::Thread.VolatileWrite(ref tgt, value); return 0; },
                "VolatileWrite");
        }

        public static void VolatileWrite(ref ulong address, ulong value)
        {
            Helper.SimpleRefWrap<ulong>(
                ref address,
                delegate(ref ulong tgt) { Original::Thread.VolatileWrite(ref tgt, value); return 0; },
                "VolatileWrite");
        }

        public static void VolatileWrite(ref global::System.UIntPtr address, global::System.UIntPtr value)
        {
            Helper.SimpleRefWrap<global::System.UIntPtr>(
                ref address,
                delegate(ref global::System.UIntPtr tgt) { Original::Thread.VolatileWrite(ref tgt, value); return global::System.UIntPtr.Zero; },
                "VolatileWrite");
        }

        #endregion

        public static void Suspend(Original::Thread thread)
        {
            throw new NotImplementedException("Thread.Suspend has been deprecated");
        }

        public static void Resume(Original::Thread thread)
        {
            throw new NotImplementedException("Thread.Resume has been deprecated");
        }

        public static void Abort(Original::Thread thread)
        {
            throw new NotImplementedException("Thread.Abort (1). You should avoid using this method. See http://msmvps.com/blogs/peterritchie/archive/2007/08/22/thead-abort-is-a-sign-of-a-poorly-designed-program.aspx for more detail.");
        }

        public static void Abort(Original::Thread thread, object o)
        {
            throw new NotImplementedException("Thread.Abort (2). You should avoid using this method. See http://msmvps.com/blogs/peterritchie/archive/2007/08/22/thead-abort-is-a-sign-of-a-poorly-designed-program.aspx for more detail.");
        }

        public static void ResetAbort()
        {
            throw new NotImplementedException("Thread.ResetAbort (1). You should avoid using this method. See http://msmvps.com/blogs/peterritchie/archive/2007/08/22/thead-abort-is-a-sign-of-a-poorly-designed-program.aspx for more detail.");
        }

        public static void Interrupt(Original::Thread thread)
        {
            throw new NotImplementedException("Thread.Interrupt (1).");
        }
    }
}