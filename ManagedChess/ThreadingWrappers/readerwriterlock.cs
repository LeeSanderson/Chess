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
    public static class ReaderWriterLock
    {
        public static void AcquireReaderLock(Original::ReaderWriterLock @lock, int millisecondsTimeout)
        {
            throw new NotImplementedException("ReaderWriterLock.AcquireReaderLock"); 
        }
        public static void AcquireReaderLock(Original::ReaderWriterLock @lock, TimeSpan timeout)
        {
            throw new NotImplementedException("ReaderWriterLock.AcquireReaderLock");
        }

        public static void AcquireWriterLock(Original::ReaderWriterLock @lock, int millisecondsTimeout)
        {
            throw new NotImplementedException("ReaderWriterLock.AcquireWriterLock");
        }
        
        public static void DowngradeFromWriterLock(Original::ReaderWriterLock @lock, ref LockCookie lockCookie)
        {
            throw new NotImplementedException("ReaderWriterLock.DowngradeFromWriterLock");            
        }

        public static LockCookie ReleaseLock(Original::ReaderWriterLock @lock)
        {
            throw new NotImplementedException("ReaderWriterLock.ReleaseLock");
        }

        public static void ReleaseReaderLock(Original::ReaderWriterLock @lock)
        {
            throw new NotImplementedException("ReaderWriterLock.ReleaseReaderLock");
        }

        public static void ReleaseWriterLock(Original::ReaderWriterLock @lock)
        {
            throw new NotImplementedException("ReaderWriterLock.ReleaseWriterLock");
        }
        public static void RestoreLock(Original::ReaderWriterLock @lock, ref LockCookie lockCookie)
        {
            throw new NotImplementedException("ReaderWriterLock.RestoreLock");
        }
    }
}
