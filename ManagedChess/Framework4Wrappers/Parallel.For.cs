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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.ExtendedReflection.Monitoring;
using Microsoft.ManagedChess;
using Microsoft.ManagedChess.EREngine;
using MChess;

using ClrThread = System.Int32;
using ChessTask = System.Int32;
using OThreading = global::System.Threading;
using OTasks = global::System.Threading.Tasks;

namespace __Substitutions.System.Threading.Tasks
{
    public static partial class Parallel
    {

        public static OTasks.ParallelLoopResult For(int from, int to, Action<int> body)
        {
            return ParallelForHelper(
                      from, to, false, from, to, "Parallel.For",
                      (long i) => body((int)i),
                      delegate()
                      {
                          return OTasks.Parallel.For(from, to, body);
                      }
             ).Value;
        }

        public static OTasks.ParallelLoopResult For(int from, int to, Action<int, OTasks.ParallelLoopState> body)
        {
            return Helper.SimpleWrap<OTasks.ParallelLoopResult>(
               (ClrSyncManager m) =>
               {
                   throw new NotImplementedException("This form of Parallel.For is not yet supported by CHESS.");
               },
               () =>
               {
                   return For(from, to, body);
               }
            );
        }

        public static OTasks.ParallelLoopResult For(long from, long to, Action<long> body)
        {
            return ParallelForHelper(
                     from, to, false, from, to, "Parallel.For",
                     body,
                     delegate()
                     {
                         return OTasks.Parallel.For(from, to, body);
                     }
            ).Value;
        }

        public static OTasks.ParallelLoopResult For(long from, long to, Action<long, OTasks.ParallelLoopState> body)
        {
            return Helper.SimpleWrap<OTasks.ParallelLoopResult>(
                (ClrSyncManager m) =>
                {
                    throw new NotImplementedException("This form of Parallel.For is not yet supported by CHESS.");
                },
                () =>
                {
                    return For(from, to, body);
                }
             );
        }

        public static OTasks.ParallelLoopResult For(int from, int to, OTasks.ParallelOptions options, Action<int> body)
        {

            bool singlethread = options.MaxDegreeOfParallelism == 1;
            Parallel.CheckOptions(options);
            return ParallelForHelper(
                      from, to, singlethread, from, to, "Parallel.For",
                      (long i) => body((int)i),
                      delegate()
                      {
                          return OTasks.Parallel.For(from, to, options, body);
                      }
             ).Value;
        }
        
        public static OTasks.ParallelLoopResult For(int from, int to, OTasks.ParallelOptions options, Action<int, OTasks.ParallelLoopState> body)
        {
            return Helper.SimpleWrap<OTasks.ParallelLoopResult>(
                (ClrSyncManager m) =>
                {
                    throw new NotImplementedException("This form of Parallel.For is not yet supported by CHESS.");
                },
                () =>
                {
                    return For(from, to, options, body);
                }
             );
        }

        public static OTasks.ParallelLoopResult For(long from, long to, OTasks.ParallelOptions options, Action<long> body)
        {
            bool singlethread = options.MaxDegreeOfParallelism == 1;
            Parallel.CheckOptions(options);
            return ParallelForHelper(
                     from, to, singlethread, from, to, "Parallel.For",
                     body,
                     delegate()
                     {
                         return OTasks.Parallel.For(from, to, options, body);
                     }
            ).Value;
        }

        public static OTasks.ParallelLoopResult For(long from, long to, OTasks.ParallelOptions options, Action<long, OTasks.ParallelLoopState> body)
        {
            return Helper.SimpleWrap<OTasks.ParallelLoopResult>(
                (ClrSyncManager m) =>
                {
                    throw new NotImplementedException("This form of Parallel.For is not yet supported by CHESS.");
                },
                () =>
                {
                    return For(from, to, options, body);
                }
             );
        }

        public static OTasks.ParallelLoopResult For<TLocal>(int from, int to, Func<TLocal> init, Func<int, OTasks.ParallelLoopState, TLocal, TLocal> body, Action<TLocal> final)
        {
            return Helper.SimpleWrap<OTasks.ParallelLoopResult>(
                (ClrSyncManager m) =>
                {
                    throw new NotImplementedException("This form of Parallel.For is not yet supported by CHESS.");
                },
                () =>
                {
                    return For(from, to, init, body, final);
                }
             );
        }

        public static OTasks.ParallelLoopResult For<TLocal>(int from, int to, OTasks.ParallelOptions options, Func<TLocal> init, Func<int, OTasks.ParallelLoopState, TLocal, TLocal> body, Action<TLocal> final)
        {
            return Helper.SimpleWrap<OTasks.ParallelLoopResult>(
               (ClrSyncManager m) =>
               {
                   throw new NotImplementedException("This form of Parallel.For is not yet supported by CHESS.");
               },
               () =>
               {
                   return For(from, to, options, init, body, final);
               }
            );
        }

        public static OTasks.ParallelLoopResult For<TLocal>(long from, long to, Func<TLocal> init, Func<long, OTasks.ParallelLoopState, TLocal, TLocal> body, Action<TLocal> final)
        {
            return Helper.SimpleWrap<OTasks.ParallelLoopResult>(
                (ClrSyncManager m) =>
                {
                    throw new NotImplementedException("This form of Parallel.For is not yet supported by CHESS.");
                },
                () =>
                {
                    return For(from, to, init, body, final);
                }
             );
        }

        public static OTasks.ParallelLoopResult For<TLocal>(long from, long to, OTasks.ParallelOptions options, Func<TLocal> init, Func<long, OTasks.ParallelLoopState, TLocal, TLocal> body, Action<TLocal> final)
        {
            return Helper.SimpleWrap<OTasks.ParallelLoopResult>(
                (ClrSyncManager m) =>
                {
                    throw new NotImplementedException("This form of Parallel.For is not yet supported by CHESS.");
                },
                () =>
                {
                    return For(from, to, options, init, body, final);
                }
             );
        }

    }
}