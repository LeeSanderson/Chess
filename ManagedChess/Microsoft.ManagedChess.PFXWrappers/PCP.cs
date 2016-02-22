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
using SyncVar = System.Int32;
using System.Threading;
using MChess;
using Microsoft.ManagedChess.EREngine;
using Microsoft.ManagedChess.Internal;
using System.Diagnostics;
// special stuff for the PCP code

namespace Microsoft.ManagedChess.Internal
{
    [DebuggerNonUserCode]
    public static class OrigNativeMethods
    {
        [DllImport("kernel32.dll")]
        internal static extern void GetSystemInfo(ref SYSTEM_INFO lpSystemInfo);

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            internal int dwOemId;
            internal int dwPageSize;
            internal IntPtr lpMinimumApplicationAddress;
            internal IntPtr lpMaximumApplicationAddress;
            internal IntPtr dwActiveProcessorMask;
            internal int dwNumberOfProcessors;
            internal int dwProcessorType;
            internal int dwAllocationGranularity;
            internal short wProcessorLevel;
            internal short wProcessorRevision;
        }

        public const int CREATE_SUSPENDED = 0x00000004;
        public const int INFINITE = -1;
        public const int WAIT_OBJECT_0 = 0;
        public const int WAIT_TIMEOUT = 258;
        public const int DUPLICATE_SAME_ACCESS = 0x00000002;

        [DllImport("kernel32.dll")]
        internal static extern global::System.IntPtr CreateThread(
            global::System.IntPtr lpThreadAttributes,
            global::System.IntPtr dwStackSize,
            global::System.IntPtr lpStartAddress,
            global::System.IntPtr lpParameter,
            int dwCreationFlags,
            global::System.IntPtr lpThreadId
            );

        // We suppress unmanaged code security to avoid stack walks when we P/Invoke to
        // the SwitchToThread function. This is a slight performance optimization, and
        // because PLINQ is full trust, this is OK anyway.
        [DllImport("kernel32.dll")]
        internal static extern int SwitchToThread();

        [DllImport("kernel32.dll")]
        internal static extern int CloseHandle(global::System.IntPtr handle);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetCurrentThread();

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll")]
        internal static extern bool DuplicateHandle(
            global::System.IntPtr hSourceProcessHandle,
            global::System.IntPtr hSourceHandle,
            global::System.IntPtr hTargetProcessHandle,
            out global::System.IntPtr lpTargetHandle,
            uint dwDesiredAccess,
            bool bInheritHandle,
            uint dwOptions
            );

        [DllImport("kernel32.dll")]
        internal static extern uint SetThreadIdealProcessor(
            global::System.IntPtr handle,
            uint dwIdealProcessor
        );

        [DllImport("kernel32.dll")]
        internal static extern global::System.IntPtr CreateEvent(
            global::System.IntPtr lpEventAttributes,
            bool bManualReset,
            bool bInitialState,
            global::System.String lpName
            );

        [DllImport("kernel32.dll")]
        internal static extern bool SetEvent(
            global::System.IntPtr hEvent
            );

        [DllImport("kernel32.dll")]
        internal unsafe static extern int WaitForMultipleObjects(
            int nCount,
            global::System.IntPtr* lpHandles,
            bool bWaitAll,
            int dwMilliseconds
            );

        [DllImport("kernel32.dll")]
        internal unsafe static extern int WaitForSingleObject(
            global::System.IntPtr lpHandle,
            int dwMilliseconds
            );

        [DllImport("kernel32.dll")]
        internal static extern global::System.IntPtr SetThreadAffinityMask(global::System.IntPtr hThread, global::System.IntPtr dwThreadAffinityMask);
    }
}

namespace __Substitutions.System.Threading.Internal
{
    [DebuggerNonUserCode]
    public static class NativeMethods
    {
        [__NonPublic]
        public static int SwitchToThread()
        {
            return Helper.SimpleWrap<int>(
                delegate(ClrSyncManager manager)
                {
                    manager.TaskYield();
                    return OrigNativeMethods.SwitchToThread();
                },
                delegate() { return OrigNativeMethods.SwitchToThread(); });
        }

        //[__NonPublic]
        //public static void GetSystemInfo(ref OrigNativeMethods.SYSTEM_INFO lpSystemInfo)
        //{
        //    OrigNativeMethods.GetSystemInfo(ref lpSystemInfo);
        //    lpSystemInfo.dwNumberOfProcessors = 1;
        //}

        //[DllImport("kernel32.dll")]
        //internal static extern IntPtr GetCurrentProcess();

        //[DllImport("kernel32.dll")]
        //internal static extern IntPtr GetCurrentThread();

        //[DllImport("kernel32.dll")]
        //internal static extern UIntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);

        //[DllImport("kernel32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //internal static extern bool GetProcessAffinityMask(IntPtr hProcess, out UIntPtr lpProcessAffinityMask, out UIntPtr lpSystemAffinityMask);
    }

    [DebuggerNonUserCode]
    public static class Platform
    {
        [__NonPublic]
        public static int get_ProcessorCount()
        {
            return 1;
        }

        [__NonPublic]
        public static void Yield()
        {
            Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    manager.TaskYield(); return false;
                },
                delegate() { OrigNativeMethods.SwitchToThread(); return false; });
        }

        [__NonPublic]
        public static void VolatileRead(object location)
        {
            Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    manager.SetMethodInfo("Internal.VolatileRead");
                    manager.SyncVarAccess(location, MSyncVarOp.RWVAR_READ);
                    manager.CommitSyncVarAccess();
                    return false;
                },
                delegate() { return false; }
                );
        }

        [__NonPublic]
        public static void VolatileWrite(object location)
        {
            Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    manager.SetMethodInfo("Internal.VolatileWrite");
                    manager.SyncVarAccess(location, MSyncVarOp.RWVAR_WRITE);
                    manager.CommitSyncVarAccess();
                    return false;
                },
                delegate() { return false; }
                );
        }
    }
}

namespace __Substitutions.System.Threading.ConcurrencyScheduler
{
    [DebuggerNonUserCode]
    public static class NativeMethods
    {
        [DllImport("Microsoft.ManagedChess.MChess.dll")]
        internal static extern int CallThreadFunction(global::System.IntPtr lpStartAddress, global::System.IntPtr lpArg);
        private delegate int Win32ThreadStartDelegate(global::System.IntPtr lpParameter);

        internal class NativeThreadJoinable : IJoinable
        {
            public NativeThreadJoinable(global::System.IntPtr thread)
            {
                handle = thread;
            }

            public NativeThreadJoinable(global::System.IntPtr thread, string title)
            {
                handle = thread;
                name = title;
            }

            public void Join()
            {
                OrigNativeMethods.WaitForSingleObject(handle, OrigNativeMethods.INFINITE);
            }

            public string Name()
            {
                return name;
            }

            public void Dispose(SyncVarManager svm)
            {
                svm.RemoveNativeHandle(handle);
            }

            private global::System.IntPtr handle;
            private string name;
        }


        [__NonPublic]
        public static global::System.IntPtr CreateThread(
            global::System.IntPtr lpThreadAttributes,
            global::System.IntPtr dwStackSize,
            global::System.IntPtr lpStartAddress,
            global::System.IntPtr lpParameter,
            int dwCreationFlags,
            global::System.IntPtr lpThreadId
            )
        {
            return Helper.SimpleWrap<global::System.IntPtr>(
                delegate(ClrSyncManager manager)
                {
                    Original.Semaphore semaphore = new Original.Semaphore(0, 1);
                    int childThread = manager.TaskFork();

                    Win32ThreadStartDelegate wrapper =
                        delegate(global::System.IntPtr argPtr)
                        {
                            try
                            {
                                manager.ThreadBegin(semaphore);
                                int returnValue = 0;
                                Exception exception = null;
                                Microsoft.ManagedChess.MChessChess.LeaveChess();
                                try
                                {
                                    returnValue = CallThreadFunction(lpStartAddress, lpParameter);
                                }
                                catch (Exception e) // catch recoverable exception in monitored code
                                {
                                    exception = e;
                                }
                                Microsoft.ManagedChess.MChessChess.EnterChess();
                                if (manager.BreakDeadlockMode)
                                    Microsoft.ManagedChess.MChessChess.WakeNextDeadlockedThread(false, true);
                                else if (exception == null)
                                    manager.ThreadEnd(childThread);
                                else
                                    manager.Shutdown(exception);
                                return returnValue;
                            }
                            catch (Exception e) // catch fatal exception in our code
                            {
                                manager.Shutdown(e);
                                return -1;
                            }
                        };

                    //make sure wrapper does not get GCed
                    manager.PinObject(wrapper);

                    global::System.IntPtr wrapperPointer = Marshal.GetFunctionPointerForDelegate(wrapper);

                    global::System.IntPtr returnVal = 
                        OrigNativeMethods.CreateThread(lpThreadAttributes, dwStackSize, wrapperPointer, 
                        global::System.IntPtr.Zero, dwCreationFlags, lpThreadId);
                    if ((dwCreationFlags & OrigNativeMethods.CREATE_SUSPENDED) == 0)
                    {
                        manager.TaskResume(childThread);
                    }
                    manager.RegisterTaskSemaphore(childThread, semaphore, true);
                    global::System.IntPtr childHandleCp;
                    bool dupret = OrigNativeMethods.DuplicateHandle(
                        OrigNativeMethods.GetCurrentProcess(), returnVal,
                        OrigNativeMethods.GetCurrentProcess(), out childHandleCp,
                        0, false, OrigNativeMethods.DUPLICATE_SAME_ACCESS);

                    global::System.Diagnostics.Debug.Assert(dupret);

                    manager.AddIJoinable(childThread, new NativeThreadJoinable(childHandleCp));
                    manager.AddNativeHandleForSyncVar(returnVal, childThread);
                    return returnVal;
                },
                delegate()
                {
                    // default to direct call
                    return OrigNativeMethods.CreateThread(lpThreadAttributes, dwStackSize,
                        lpStartAddress, lpParameter, dwCreationFlags, lpThreadId);
                }
                );
        }

        [__NonPublic]
        public static int SwitchToThread()
        {
            return Helper.SimpleWrap<int>(
                delegate(ClrSyncManager manager)
                {
                    manager.TaskYield();
                    return OrigNativeMethods.SwitchToThread(); 
                },
                delegate() { return OrigNativeMethods.SwitchToThread(); });
        }

        [__NonPublic]
        public static bool SetEvent(
            global::System.IntPtr hEvent
            )
        {
            return Helper.WrapRelease(
                hEvent,
                MSyncVarOp.RWEVENT,
                delegate(object o) { return OrigNativeMethods.SetEvent((global::System.IntPtr)o); },
                "Win32Event.Set"
            );
        }

        [__NonPublic]
        public static bool DuplicateHandle(
            global::System.IntPtr hSourceProcessHandle,
            global::System.IntPtr hSourceHandle,
            global::System.IntPtr hTargetProcessHandle,
            out global::System.IntPtr lpTargetHandle,
            uint dwDesiredAccess,
            bool bInheritHandle,
            uint dwOptions
            )
        {
            global::System.IntPtr dupHandle = global::System.IntPtr.Zero;
            bool retVal = Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    bool flag = OrigNativeMethods.DuplicateHandle(hSourceProcessHandle, hSourceHandle, hTargetProcessHandle, out dupHandle, dwDesiredAccess, bInheritHandle, dwOptions);
                    if (flag)
                    {
                        if (hSourceHandle == OrigNativeMethods.GetCurrentThread())
                        {
                            // associate the dup handle with the current running tid
                            // Also, we want a Native IJoinable for this thread as there can be race
                            // when the CLR thinks a thread is dead, but Win32 does not
                            //  So, a subsequent WaitForMultipleObjects() can fail
                            global::System.IntPtr childHandleCp;
                            bool dupret = OrigNativeMethods.DuplicateHandle(
                                OrigNativeMethods.GetCurrentProcess(), dupHandle,
                                OrigNativeMethods.GetCurrentProcess(), out childHandleCp,
                                0, false, OrigNativeMethods.DUPLICATE_SAME_ACCESS);

                            global::System.Diagnostics.Debug.Assert(dupret);

                            manager.AddIJoinable(manager.CurrentTid, new NativeThreadJoinable(childHandleCp, global::System.Threading.Thread.CurrentThread.Name));
                            manager.AddNativeHandleForSyncVar(dupHandle, manager.CurrentTid);
                        }
                        else
                        {
                            manager.DuplicateNativeHandle(hSourceHandle, dupHandle);
                        }
                    }
                    return flag;
                },
                delegate()
                {
                    return OrigNativeMethods.DuplicateHandle(hSourceProcessHandle, hSourceHandle, hTargetProcessHandle, out dupHandle, dwDesiredAccess, bInheritHandle, dwOptions);
                });
            lpTargetHandle = dupHandle;
            return retVal;

        }


        [__NonPublic]
        public unsafe static int WaitForMultipleObjects(
            int nCount,
            global::System.IntPtr* lpHandles,
            bool bWaitAll,
            int dwMilliseconds
            )
        {
            return Helper.SimpleWrap<int>(
                delegate(ClrSyncManager manager)
                {
                    SyncVar[] v = new SyncVar[nCount];
                    for (int i = 0; i < nCount; i++)
                    {
                        v[i] = manager.GetSyncVarFromNativeHandle(lpHandles[i]);
                    }

                    int returnVal;
                    while (true)
                    {
                        manager.SetMethodInfo(bWaitAll ? "Win32WaitForMultipleObjects::WAIT_ALL" : "Win32WaitForMultipleObjects::WAIT_ANY");
                        manager.AggregateSyncVarAccess(v, bWaitAll ? MSyncVarOp.WAIT_ALL : MSyncVarOp.WAIT_ANY);
                        try
                        {
                            returnVal = OrigNativeMethods.WaitForMultipleObjects(nCount, lpHandles, bWaitAll, 0);
                        }
                        catch (Exception e)
                        {
                            manager.CommitSyncVarAccess();
                            throw e;
                        }
                        if (OrigNativeMethods.WAIT_OBJECT_0 <= returnVal && returnVal <= OrigNativeMethods.WAIT_OBJECT_0 + nCount - 1)
                        {
                            // success
                            manager.CommitSyncVarAccess();
                            return returnVal;
                        }
                        global::System.Diagnostics.Debug.Assert(returnVal == OrigNativeMethods.WAIT_TIMEOUT);
                        if (dwMilliseconds != OrigNativeMethods.INFINITE)
                        {
                            // timeout
                            manager.MarkTimeout();
                            manager.CommitSyncVarAccess();
                            manager.TaskYield();
                            return returnVal;
                        }
                        manager.LocalBacktrack();
                    }

                },
                delegate()
                {
                    return OrigNativeMethods.WaitForMultipleObjects(nCount, lpHandles, bWaitAll, dwMilliseconds);
                });
        }

    }

    [DebuggerNonUserCode]
    public static class SpinCount
    {
        public static int get_Value()
        {
            return 0;
        }
    }
}
