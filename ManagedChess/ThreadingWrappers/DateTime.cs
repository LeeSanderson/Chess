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
using Microsoft.ExtendedReflection.Monitoring;
using Microsoft.ExtendedReflection.Collections;
using System.Runtime.InteropServices;
using System.Security;
using ClrThread = System.Int32;
using ChessTask = System.Int32;
using System.Threading;
using System.Diagnostics;

using MChess;
using Microsoft.ManagedChess;
using Microsoft.ManagedChess.EREngine;


namespace __Substitutions.System
{
    [DebuggerNonUserCode]
    public static class DateTime
    {
        public static global::System.DateTime get_Now()
        {
            return Helper.SimpleWrap<global::System.DateTime>(
                delegate(ClrSyncManager manager)
                {
                    return manager.Now;
                },
                delegate()
                {
                    return global::System.DateTime.Now;
                });
        }

        public static global::System.DateTime get_UtcNow()
        {
            return Helper.SimpleWrap<global::System.DateTime>(
                delegate(ClrSyncManager manager)
                {
                    return manager.Now.ToUniversalTime();
                },
                delegate()
                {
                    return global::System.DateTime.UtcNow;
                });
        }

        public static global::System.DateTime get_Today()
        {
            return Helper.SimpleWrap<global::System.DateTime>(
                delegate(ClrSyncManager manager)
                {
                    return manager.Now.Date;
                },
                delegate()
                {
                    return global::System.DateTime.Today;
                });
        }
    }
}