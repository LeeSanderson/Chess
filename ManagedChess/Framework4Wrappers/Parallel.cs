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
using ClrThread = System.Int32;
using ChessTask = System.Int32;
using MChess;
using System.Diagnostics;
using Microsoft.ManagedChess;
using Microsoft.ManagedChess.EREngine;

using OThreading = global::System.Threading;
using OTasks = global::System.Threading.Tasks;

namespace __Substitutions.System.Threading.Tasks
{
#if !DEBUG
  [DebuggerNonUserCode]
#endif
    public static partial class Parallel
    {
        // check for unsupported options
        internal static void CheckOptions(OTasks.ParallelOptions options)
        {
            if (options.TaskScheduler != (new OTasks.ParallelOptions().TaskScheduler))
                throw new NotImplementedException("Do not support custom task schedulers");
        }

        public struct Range
        {
            public long from;
            public long to;
            public long minsplit;
            public long maxsplit;
            public Range(long f, long t, long min, long max) { from = f; to = t; minsplit = min; maxsplit = max; }
        }

        // fixed-range parallel for loop helper
        internal static OTasks.ParallelLoopResult? ParallelForHelper(
                                                    long from, long to,
                                                    bool singlethread,
                                                    long minsplit, long maxsplit,
                                                    string name,
                                                    global::System.Action<long> body,
                                                    Helper.SimpleDel<OTasks.ParallelLoopResult?> orig)
        {
            return ParallelForHelper(() => new Range(from, to, minsplit, maxsplit), singlethread, name, body, orig);
        }

        // variable-range parallel for loop helper
        internal static OTasks.ParallelLoopResult? ParallelForHelper(
                                                    Func<Range> prologue,
                                                    bool singlethread,
                                                    string name,
                                                    global::System.Action<long> body,
                                                    Helper.SimpleDel<OTasks.ParallelLoopResult?> orig)
        {
            return
                 Helper.SimpleContextExposingWrap(
                     delegate(ClrSyncManager manager, ref IDisposable protectingcontext)
                     {
                         Range range = new Range();

                         // do prologue outside chess... and switch protecting context
                         MChessChess.LeaveChess();
                         protectingcontext.Dispose();
                         Exception ex = null;
                         try
                         {
                             range = prologue();
                         }
                         catch (Exception e) // catch recoverable exception in user code
                         {
                             ex = e;
                         }
                         IDisposable pc = _ProtectingThreadContext.Acquire();
                         MChessChess.EnterChess();

                         if (ex != null)
                         {
                             global::System.Diagnostics.Debug.Fail("Not implemented: exceptions in enumerators of Parallel foreach");
                             throw ex;
                         }

                         if (range.to <= range.from) // empty loop
                             return orig();

                         long split;
                         if (!singlethread)
                             split = range.minsplit + manager.Choose((int)(range.maxsplit - range.minsplit + 1));
                         else
                             split = (manager.Choose(2) == 0) ? range.minsplit : range.maxsplit;

                         MChessChess.TraceEvent(name + "(" + range.from + "," + range.to + "), split " + split);

                         SimplePartitioner<long> partitioner = new SimplePartitioner<long>(
                              manager, range.from, range.to, split,
                              (long i) => { return i; },
                              true);

                         // store protecting context
                         partitioner.protectingcontext = pc;

                         // TODO better job here
                         OTasks.ParallelOptions options = new OTasks.ParallelOptions();
                         options.MaxDegreeOfParallelism = 2;

                         OTasks.ParallelLoopResult result = OTasks.Parallel.ForEach<long>(partitioner, options, partitioner.WrapBody(body));

                         // switch protecting context back
                         partitioner.protectingcontext.Dispose();
                         protectingcontext = _ProtectingThreadContext.Acquire();

                         return result;
                     },
                     orig
                 );
        }
    }
}