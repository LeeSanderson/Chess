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
using System.Diagnostics;
using Microsoft.ManagedChess.EREngine;

namespace __Substitutions.System.Threading
{
    [DebuggerNonUserCode]
    public static class ReaderWriterLockSlim
    {
        /* .NET 3.5 only */

        public enum MODE { READ, WRITE, UPGRADEABLE_READ };

        public static void EnterReadLock(Original::ReaderWriterLockSlim @lock)
        {
            Helper.WrapAcquire(
                delegate() { @lock.EnterReadLock(); return false; },
                delegate() { return @lock.TryEnterReadLock(0); },
                @lock,
                MSyncVarOp.LOCK_ACQUIRE,
                "ReaderWriterLockSlim.EnterReadLock"
                );
        }

        public static void EnterWriteLock(Original::ReaderWriterLockSlim @lock)
        {
            Helper.WrapAcquire(
                delegate() { @lock.EnterWriteLock(); return false; },
                delegate() { return @lock.TryEnterWriteLock(0); },
                @lock,
                MSyncVarOp.LOCK_ACQUIRE,
                "ReaderWriterLockSlim.EnterWriteLock"
                );
        }

        public static void EnterUpgradeableReadLock(Original::ReaderWriterLockSlim @lock)
        {
            Helper.WrapAcquire(
                delegate() { @lock.EnterUpgradeableReadLock(); return false; },
                delegate() { return @lock.TryEnterUpgradeableReadLock(0); },
                @lock,
                MSyncVarOp.LOCK_ACQUIRE,
                "ReaderWriterLockSlim.EnterUpgradeableReadLock"
                );
        }

        public static bool TryEnterReadLock(Original::ReaderWriterLockSlim @lock, int millisecondsTimeout)
        {
            return TryEnterRaw(@lock, millisecondsTimeout, "TryEnterReadLock", MODE.READ);
        }

        public static bool TryEnterReadLock(Original::ReaderWriterLockSlim @lock, TimeSpan timeSpan)
        {
            return TryEnterRaw(@lock, (int) timeSpan.TotalMilliseconds, "TryEnterReadLock", MODE.READ);
        }

        public static bool TryEnterWriteLock(Original::ReaderWriterLockSlim @lock, int millisecondsTimeout)
        {
            return TryEnterRaw(@lock, millisecondsTimeout, "TryEnterWriteLock", MODE.WRITE);
        }

        public static bool TryEnterWriteLock(Original::ReaderWriterLockSlim @lock, TimeSpan timeSpan)
        {
            return TryEnterRaw(@lock, (int) timeSpan.TotalMilliseconds, "TryEnterWriteLock", MODE.WRITE);
        }

        public static bool TryEnterUpgradeableReadLock(Original::ReaderWriterLockSlim @lock, int millisecondsTimeout)
        {
            return TryEnterRaw(@lock, millisecondsTimeout, "TryEnterUpgradeableReadLock", MODE.UPGRADEABLE_READ);
        }

        public static bool TryEnterUpgradeableReadLock(Original::ReaderWriterLockSlim @lock, TimeSpan ts)
        {
            return TryEnterRaw(@lock, (int) ts.TotalMilliseconds, "TryEnterUpgradeableReadLock", MODE.UPGRADEABLE_READ);
        }

        public static bool TryEnterRaw(Original::ReaderWriterLockSlim @lock, int millisecondsTimeout, string name, MODE mode)
        {
            if (@lock == null)
                throw new ArgumentNullException();
            if (millisecondsTimeout < 0 && millisecondsTimeout != Timeout.Infinite)
                throw new ArgumentOutOfRangeException();

            return Helper.SimpleWrap<bool>(
                delegate(ClrSyncManager manager)
                {
                    bool b;
                    while (true)
                    {
                        try
                        {
                            manager.SetMethodInfo(name);
                            manager.SyncVarAccess(@lock, MSyncVarOp.LOCK_ACQUIRE);
                            if (mode == MODE.READ)
                                b = @lock.TryEnterReadLock(0);
                            else if (mode == MODE.UPGRADEABLE_READ)
                                b = @lock.TryEnterUpgradeableReadLock(0);
                            else
                                b = @lock.TryEnterWriteLock(0);
                            if (!b && millisecondsTimeout == Timeout.Infinite)
                            {
                                manager.LocalBacktrack();
                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                            manager.CommitSyncVarAccess();
                            throw e;
                        }
                        if (!b)
                            manager.MarkTimeout();
                        manager.CommitSyncVarAccess();
                        break;
                    }
                    return b;
                },
                delegate()
                {
                    bool b;
                    if (mode == MODE.READ)
                        b = @lock.TryEnterReadLock(millisecondsTimeout);
                    else if (mode == MODE.UPGRADEABLE_READ)
                        b = @lock.TryEnterUpgradeableReadLock(millisecondsTimeout);
                    else
                        b = @lock.TryEnterWriteLock(millisecondsTimeout);
                    return b;
                });
        }

        public static void ExitReadLock(Original::ReaderWriterLockSlim @lock)
        {
            Helper.WrapRelease(@lock, MSyncVarOp.LOCK_RELEASE,
                delegate(object o) { @lock.ExitReadLock(); return false; },
                "ReaderWriterLockSlim.ExitReadLock");
        }

        public static void ExitUpgradeableReadLock(Original::ReaderWriterLockSlim @lock)
        {
            Helper.WrapRelease(@lock, MSyncVarOp.LOCK_RELEASE,
                delegate(object o) { @lock.ExitUpgradeableReadLock(); return false; },
                "ReaderWriterLockSlim.ExitUpgradeableReadLock");
        }

        public static void ExitWriteLock(Original::ReaderWriterLockSlim @lock)
        {
            Helper.WrapRelease(@lock, MSyncVarOp.LOCK_RELEASE,
                delegate(object o) { @lock.ExitWriteLock(); return false; },
                "ReaderWriterLockSlim.ExitWriteLock");
        }

        
        public delegate T PropertyDel<T>();
        public static T WrapProperty<T>(Original::ReaderWriterLockSlim @lock, PropertyDel<T> property, MSyncVarOp mop, string instrMethod)
        {
            return Helper.SimpleWrap<T>(
                delegate(ClrSyncManager manager)
                {
                    manager.SetMethodInfo(instrMethod);
                    manager.SyncVarAccess(@lock, mop);
                    T ret;
                    try { 
                        ret = property(); 
                    } catch (Exception e) {
                        manager.CommitSyncVarAccess();
                        throw e;
                    };
                    manager.CommitSyncVarAccess();
                    return ret;
                },
                delegate()
                {
                    return property();
                });
        }

        public static int get_CurrentReadCount(Original::ReaderWriterLockSlim @lock)
        {
            return WrapProperty<int>(
                @lock,
                delegate() { return @lock.CurrentReadCount; },
                MSyncVarOp.RWVAR_READ, 
                "ReaderWriterLockSlim.CurrentReadCount");
        }

        public static int get_RecursiveReadCount(Original::ReaderWriterLockSlim @lock)
        {
            return WrapProperty<int>(
                @lock,
                delegate() { return @lock.RecursiveReadCount; },
                MSyncVarOp.RWVAR_READ,
                "ReaderWriterLockSlim.RecursiveReadCount");
        }

        public static int get_RecursiveUpgradeCount(Original::ReaderWriterLockSlim @lock)
        {
            return WrapProperty<int>(
                @lock,
                delegate() { return @lock.RecursiveUpgradeCount; },
                MSyncVarOp.RWVAR_READ,
                "ReaderWriterLockSlim.RecursiveUpgradeCount");
        }

        public static int get_RecursiveWriteCount(Original::ReaderWriterLockSlim @lock)
        {
            return WrapProperty<int>(
                @lock,
                delegate() { return @lock.RecursiveWriteCount; },
                MSyncVarOp.RWVAR_READ,
                "ReaderWriterLockSlim.RecursiveWriteCount");
        }

        public static bool get_IsReadLockHeld(Original::ReaderWriterLockSlim @lock)
        {
            return WrapProperty<bool>(
                @lock,
                delegate() { return @lock.IsReadLockHeld; },
                MSyncVarOp.RWVAR_READ,
                "ReaderWriterLockSlim.IsReadLockHeld");
        }

        public static bool get_IsUpgradeableReadLockHeld(Original::ReaderWriterLockSlim @lock)
        {
            return WrapProperty<bool>(
                @lock,
                delegate() { return @lock.IsUpgradeableReadLockHeld; },
                MSyncVarOp.RWVAR_READ,
                "ReaderWriterLockSlim.IsUpgradeableReadLockHeld");
        }

        public static bool get_IsWriteLockHeld(Original::ReaderWriterLockSlim @lock)
        {
            return WrapProperty<bool>(
                @lock,
                delegate() { return @lock.IsWriteLockHeld; },
                MSyncVarOp.RWVAR_READ,
                "ReaderWriterLockSlim.IsWriteLockHeld");
        }

        public static int get_WaitingReadCount(Original::ReaderWriterLockSlim @lock)
        {
            return WrapProperty<int>(
                @lock,
                delegate() { return @lock.WaitingReadCount; },
                MSyncVarOp.RWVAR_READ,
                "ReaderWriterLockSlim.WaitingReadCount");
        }

        public static int get_WaitingUpgradeCount(Original::ReaderWriterLockSlim @lock)
        {
            return WrapProperty<int>(
                @lock,
                delegate() { return @lock.WaitingUpgradeCount; },
                MSyncVarOp.RWVAR_READ,
                "ReaderWriterLockSlim.WaitingUpgradeCount");
        }

        public static int get_WaitingWriteCount(Original::ReaderWriterLockSlim @lock)
        {
            return WrapProperty<int>(
                @lock,
                delegate() { return @lock.WaitingWriteCount; },
                MSyncVarOp.RWVAR_READ,
                "ReaderWriterLockSlim.WaitingWriteCount");
        }       
    }
}