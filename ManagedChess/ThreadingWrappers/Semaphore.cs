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

namespace __Substitutions.System.Threading
{
    [DebuggerNonUserCode]
    public static class Semaphore
    {
        public static int Release(Original::Semaphore semaphore)
        {
            return Release(semaphore, 1);
        }

        public static int Release(Original::Semaphore semaphore, int times)
        {
            return
                Helper.SimpleWrap<int>(
                    delegate(ClrSyncManager manager)
                    {
                        manager.SetMethodInfo("Semaphore.Release(x" + times + ")");
                        manager.SyncVarAccess(semaphore, MSyncVarOp.LOCK_RELEASE);
                        int returnValue;
                        try
                        {
                            returnValue = semaphore.Release(times);
                        }
                        catch (Exception e)
                        {
                            manager.CommitSyncVarAccess();
                            throw e;
                        }
                        manager.CommitSyncVarAccess();
                        return returnValue;
                    },
                    delegate()
                    {
                        return semaphore.Release(times);
                    }
                );
        }
    }
}