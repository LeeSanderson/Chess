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
using Original = global::System.Diagnostics;
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
using MCUT = global::Microsoft.Concurrency.TestTools.UnitTesting;

namespace __Substitutions.System.Diagnostics
{
    /// <summary>
    /// Replacement method for System.Threading.Thread constructors
    /// </summary>
    /// 
    [DebuggerNonUserCode]
    public static class Debug
    {

        public static void Assert(bool condition)
        {
            Assert(condition, null, null);
        }

        public static void Assert(bool condition, string message)
        {
            Assert(condition, message, null);
        }

        public static void Assert(bool condition, string message, string detailMessage)
        {
            if (!condition)
                throw new MCUT.DebugAssertFailedException(message, detailMessage, null);
        }

        public static void Assert(bool condition, string message, string detailMessageFormat, params object[] args)
        {
            if (!condition)
            {
                string detailMessage = global::System.String.Format(detailMessageFormat, args);
                throw new MCUT.DebugAssertFailedException(message, detailMessage, null);
            }
        }

    }
}