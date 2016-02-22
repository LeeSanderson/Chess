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
using Original = global::System;
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

namespace __Substitutions.System
{
    [DebuggerNonUserCode]
    public static class Environment
    {
        [__NonPublic]
        public static int get_ProcessorCount()
        {
            return Helper.SimpleWrap<int>(
                delegate(ClrSyncManager manager)
                {
                    return manager.ProcessorCount;
                },
                delegate() { return global::System.Environment.ProcessorCount; }
                );
        }
    }
}
