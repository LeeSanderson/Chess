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

        public static OTasks.ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, Action<TSource, OTasks.ParallelLoopState, long> body)
        {
            return Helper.SimpleWrap<OTasks.ParallelLoopResult>(
               (ClrSyncManager m) => {
                   throw new NotImplementedException("This form of Parallel.ForEach is not yet supported by CHESS.");
               },
               () => {
                   return OTasks.Parallel.ForEach(source, body);
               }
            );
        }

        public static OTasks.ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, Action<TSource, OTasks.ParallelLoopState> body)
        {
            return Helper.SimpleWrap<OTasks.ParallelLoopResult>(
               (ClrSyncManager m) => {
                   throw new NotImplementedException("This form of Parallel.ForEach is not yet supported by CHESS.");
               },
               () => {
                   return OTasks.Parallel.ForEach<TSource>(source, body);
               }
            );
        }

        public static OTasks.ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            List<TSource> elts = new List<TSource>();

            return ParallelForHelper(
                     () => {
                         // this is called outside of chess
                         IEnumerator<TSource> enumerator = source.GetEnumerator();
                         while (enumerator.MoveNext())
                             elts.Add(enumerator.Current);
                         return new Range(0, elts.Count, 0, elts.Count);
                     },
                     false,
                     "Parallel.ForEach",
                     (long i) => body(elts[(int)i]),
                     delegate() {
                         return OTasks.Parallel.ForEach<TSource>(source, body);
                     }
            ).Value;
        }

        //public static ParallelLoopResult ForEach<TSource>(OrderablePartitioner<TSource> source, Action<TSource, ParallelLoopState, long> body);
        public static OTasks.ParallelLoopResult ForEach<TSource>(global::System.Collections.Concurrent.OrderablePartitioner<TSource> part, Action<TSource, OTasks.ParallelLoopState, long> body)
        {
            return Helper.SimpleWrap<OTasks.ParallelLoopResult>(
               (ClrSyncManager m) => {
                   throw new NotImplementedException("This form of Parallel.ForEach is not yet supported by CHESS.");
               },
               () => {
                   return OTasks.Parallel.ForEach<TSource>(part, body);
               }
            );
        }

        //public static ParallelLoopResult ForEach<TSource>(Partitioner<TSource> source, Action<TSource, ParallelLoopState> body);
        public static OTasks.ParallelLoopResult ForEach<TSource>(global::System.Collections.Concurrent.Partitioner<TSource> part, Action<TSource, OTasks.ParallelLoopState> body)
        {
            return Helper.SimpleWrap<OTasks.ParallelLoopResult>(
               (ClrSyncManager m) => {
                   throw new NotImplementedException("This form of Parallel.ForEach is not yet supported by CHESS.");
               },
               () => {
                   return OTasks.Parallel.ForEach<TSource>(part, body);
               }
            );
        }

        //public static ParallelLoopResult ForEach<TSource>(Partitioner<TSource> source, Action<TSource> body);
        public static OTasks.ParallelLoopResult ForEach<TSource>(global::System.Collections.Concurrent.Partitioner<TSource> part, Action<TSource> body)
        {
            return Helper.SimpleWrap<OTasks.ParallelLoopResult>(
               (ClrSyncManager m) => {
                   throw new NotImplementedException("This form of Parallel.ForEach is not yet supported by CHESS.");
               },
               () => {
                   return OTasks.Parallel.ForEach<TSource>(part, body);
               }
            );
        }

        public static OTasks.ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, OTasks.ParallelOptions options, Action<TSource, OTasks.ParallelLoopState, long> body)
        {
            return Helper.SimpleWrap<OTasks.ParallelLoopResult>(
               (ClrSyncManager m) => {
                   throw new NotImplementedException("This form of Parallel.ForEach is not yet supported by CHESS.");
               },
               () => {
                   return OTasks.Parallel.ForEach(source, options, body);
               }
            );
        }

        public static OTasks.ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, OTasks.ParallelOptions options, Action<TSource, OTasks.ParallelLoopState> body)
        {
            return Helper.SimpleWrap<OTasks.ParallelLoopResult>(
               (ClrSyncManager m) => {
                   throw new NotImplementedException("This form of Parallel.ForEach is not yet supported by CHESS.");
               },
               () => {
                   return OTasks.Parallel.ForEach<TSource>(source, options, body);
               }
            );
        }

        public static OTasks.ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, OTasks.ParallelOptions options, Action<TSource> body)
        {
            List<TSource> elts = new List<TSource>();
            bool singlethread = options.MaxDegreeOfParallelism == 1;
            Parallel.CheckOptions(options);

            return ParallelForHelper(
                     () => {
                         // this is called outside of chess
                         IEnumerator<TSource> enumerator = source.GetEnumerator();
                         while (enumerator.MoveNext())
                             elts.Add(enumerator.Current);
                         return new Range(0, elts.Count, 0, elts.Count);
                     },
                     singlethread,
                     "Parallel.ForEach",
                     (long i) => body(elts[(int)i]),
                     delegate() {
                         return OTasks.Parallel.ForEach<TSource>(source, body);
                     }
            ).Value;
        }

        public static OTasks.ParallelLoopResult ForEach<TSource>(global::System.Collections.Concurrent.OrderablePartitioner<TSource> part, OTasks.ParallelOptions options, Action<TSource, OTasks.ParallelLoopState, long> body)
        {
            return Helper.SimpleWrap<OTasks.ParallelLoopResult>(
               (ClrSyncManager m) => {
                   throw new NotImplementedException("This form of Parallel.ForEach is not yet supported by CHESS.");
               },
               () => {
                   return OTasks.Parallel.ForEach<TSource>(part, options, body);
               }
            );
        }

        public static OTasks.ParallelLoopResult ForEach<TSource>(global::System.Collections.Concurrent.Partitioner<TSource> part, OTasks.ParallelOptions options, Action<TSource, OTasks.ParallelLoopState> body)
        {
            return Helper.SimpleWrap<OTasks.ParallelLoopResult>(
               (ClrSyncManager m) => {
                   throw new NotImplementedException("This form of Parallel.ForEach is not yet supported by CHESS.");
               },
               () => {
                   return OTasks.Parallel.ForEach<TSource>(part, options, body);
               }
            );
        }

        public static OTasks.ParallelLoopResult ForEach<TSource>(global::System.Collections.Concurrent.Partitioner<TSource> part, OTasks.ParallelOptions options, Action<TSource> body)
        {
            return Helper.SimpleWrap<OTasks.ParallelLoopResult>(
               (ClrSyncManager m) => {
                   throw new NotImplementedException("This form of Parallel.ForEach is not yet supported by CHESS.");
               },
               () => {
                   return OTasks.Parallel.ForEach<TSource>(part, options, body);
               }
            );
        }


        public static OTasks.ParallelLoopResult ForEach<TSource, TLocal>(IEnumerable<TSource> source, Func<TLocal> localInit, Func<TSource, OTasks.ParallelLoopState, long, TLocal, TLocal> body, Action<TLocal> localFinally)
        {
            throw new NotImplementedException("The Parallel.ForEach<TSource, TLocal>(...) overloads are not instrumented by Chess.");
        }

        public static OTasks.ParallelLoopResult ForEach<TSource, TLocal>(IEnumerable<TSource> source, Func<TLocal> localInit, Func<TSource, OTasks.ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally)
        {
            throw new NotImplementedException("The Parallel.ForEach<TSource, TLocal>(...) overloads are not instrumented by Chess.");
        }

        public static OTasks.ParallelLoopResult ForEach<TSource, TLocal>(global::System.Collections.Concurrent.OrderablePartitioner<TSource> source, Func<TLocal> localInit, Func<TSource, OTasks.ParallelLoopState, long, TLocal, TLocal> body, Action<TLocal> localFinally)
        {
            throw new NotImplementedException("The Parallel.ForEach<TSource, TLocal>(...) overloads are not instrumented by Chess.");
        }

        public static OTasks.ParallelLoopResult ForEach<TSource, TLocal>(global::System.Collections.Concurrent.Partitioner<TSource> source, Func<TLocal> localInit, Func<TSource, OTasks.ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally)
        {
            throw new NotImplementedException("The Parallel.ForEach<TSource, TLocal>(...) overloads are not instrumented by Chess.");
        }

        public static OTasks.ParallelLoopResult ForEach<TSource, TLocal>(IEnumerable<TSource> source, OTasks.ParallelOptions parallelOptions, Func<TLocal> localInit, Func<TSource, OTasks.ParallelLoopState, long, TLocal, TLocal> body, Action<TLocal> localFinally)
        {
            throw new NotImplementedException("The Parallel.ForEach<TSource, TLocal>(...) overloads are not instrumented by Chess.");
        }

        public static OTasks.ParallelLoopResult ForEach<TSource, TLocal>(IEnumerable<TSource> source, OTasks.ParallelOptions parallelOptions, Func<TLocal> localInit, Func<TSource, OTasks.ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally)
        {
            throw new NotImplementedException("The Parallel.ForEach<TSource, TLocal>(...) overloads are not instrumented by Chess.");
        }

        public static OTasks.ParallelLoopResult ForEach<TSource, TLocal>(global::System.Collections.Concurrent.OrderablePartitioner<TSource> source, OTasks.ParallelOptions parallelOptions, Func<TLocal> localInit, Func<TSource, OTasks.ParallelLoopState, long, TLocal, TLocal> body, Action<TLocal> localFinally)
        {
            throw new NotImplementedException("The Parallel.ForEach<TSource, TLocal>(...) overloads are not instrumented by Chess.");
        }

        public static OTasks.ParallelLoopResult ForEach<TSource, TLocal>(global::System.Collections.Concurrent.Partitioner<TSource> source, OTasks.ParallelOptions parallelOptions, Func<TLocal> localInit, Func<TSource, OTasks.ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally)
        {
            throw new NotImplementedException("The Parallel.ForEach<TSource, TLocal>(...) overloads are not instrumented by Chess.");
        }

    }
}