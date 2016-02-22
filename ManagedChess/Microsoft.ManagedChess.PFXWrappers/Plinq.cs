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
using System.Diagnostics;
using Microsoft.ManagedChess.EREngine;
using System.Collections;

namespace __Substitutions.System.Threading
{
    //[DebuggerNonUserCode]
    public static class Parallel
    {
        /* .NET 3.5 only */

        public static void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            ParameterizedThreadStart threadStart = (object x) => body((TSource)x);
            var ts = new List<Original.Thread>();
            try
            {
                foreach (var s in source)
                {
                    var t = new Original.Thread(threadStart);
                    ts.Add(t);
                    t.Start(s);
                }
            }
            finally
            {
                foreach (var t in ts)
                {
                    try { }
                    finally
                    {
                        t.Join();
                    }
                }
            }
        }
    }
}

namespace __Substitutions.System.Linq
{
    public static class ParallelEnumerable
    {
        public static global::System.Linq.IParallelEnumerable<TSource> Where<TSource>(global::System.Linq.IParallelEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return new PFXWrappers.ChessEnumerable<TSource>(source, predicate);
        }
    }
}