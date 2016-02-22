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
using System.Runtime.InteropServices;
using System.Security;
using MChess;
using System.Diagnostics;
using Microsoft.ManagedChess;
using Microsoft.ManagedChess.EREngine;

using ClrThread = System.Int32;
using ChessTask = System.Int32;
using OThreading = global::System.Threading;
using OTasks = global::System.Threading.Tasks;

namespace __Substitutions.System.Threading.Tasks
{
    public static partial class Parallel
    {

        public static void Invoke(params Action[] actions)
        {
            Invoke(new OTasks.ParallelOptions(), actions);
        }

        public static void Invoke(OTasks.ParallelOptions options, params Action[] actions)
        {
            int minsplit = 0;
            int maxsplit = actions.Length;
            bool singlethread = options.MaxDegreeOfParallelism == 1;

            CheckOptions(options);

            // accommodate expectation of true concurrency for small numbers of delegates
            if (!singlethread && actions.Length > 1 && actions.Length < 5)
            {
                minsplit++;
                maxsplit--;
            }

            ParallelForHelper(
                   0, actions.Length, singlethread, minsplit, maxsplit,
                   "Parallel.Invoke",
                   (long i) => actions[i](),
                   () => { OTasks.Parallel.Invoke(options, actions); return null; }
            );
        }

    }
}