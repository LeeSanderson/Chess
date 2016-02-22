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
using OIntPtr = System.IntPtr;
using System.Threading;
using MChess;
using Microsoft.ManagedChess.EREngine;
using System.Diagnostics;

namespace __Substitutions.System.Threading
{
    [DebuggerNonUserCode]
    public static class Interlocked
    {
        public static long Read(ref long location)
        {
            return
                Helper.SimpleRefWrap<long>(
                  ref location,
                  // IMPORTANT: wrapping of Read in delegate required!!!
                  delegate (ref long target) { return Original::Interlocked.Read(ref target); },
                  "Interlocked.Read"
                  );
        }

        // Add int
        public static int Add(ref int location, int value)
        {
            return AddRaw(ref location, value, "Interlocked.Add");
        }
        public static int Increment(ref int location)
        {
            return AddRaw(ref location, 1, "Interlocked.Increment");
        }
        public static int Decrement(ref int location)
        {
            return AddRaw(ref location, -1, "Interlocked.Decrement");
        }
        private static int AddRaw(ref int location1, int value, string name)
        {
            return
                Helper.SimpleRefWrap<int>(
                    ref location1,
                    delegate(ref int target) { return Original::Interlocked.Add(ref target, value); },
                    name
            );
        }

        // Add long
        public static long Add(ref long location, long value)
        {
            return AddRaw(ref location, value, "Interlocked.Add");
        }
        public static long Increment(ref long location)
        {
            return AddRaw(ref location, 1, "Interlocked.Increment");
        }
        public static long Decrement(ref long location)
        {
            return AddRaw(ref location, -1, "Interlocked.Decrement");
        }
        private static long AddRaw(ref long location, long value, string name)
        {
            return
                Helper.SimpleRefWrap<long>(
                    ref location,
                    delegate(ref long target) { return Original::Interlocked.Add(ref target, value); },
                    name
            );
        }

        public static T CompareExchange<T>(ref T location, T value1, T value2) where T : class
        {
            return
                Helper.SimpleRefWrap<T>(
                ref location,
                delegate(ref T target) { return Original::Interlocked.CompareExchange<T>(ref target, value1, value2); },
                "Interlocked.CompareExchange"
            );
        }
        public static T Exchange<T>(ref T location, T value1) where T : class
        {
            return
                Helper.SimpleRefWrap<T>(
                ref location,
                delegate(ref T target) { return Original::Interlocked.Exchange<T>(ref target, value1); },
                "Interlocked.Exchange"
            );
        }

        #region code for rest of overloads of CompareExchange

        public static int CompareExchange(ref int location, int value1, int value2)
        {
            return
                Helper.SimpleRefWrap<int>(
                ref location,
                delegate(ref int target) { return Original::Interlocked.CompareExchange(ref target, value1, value2); },
                "Interlocked.CompareExchange"
            );
        }
        public static int Exchange(ref int location, int value1)
        {
            return
                Helper.SimpleRefWrap<int>(
                ref location,
                delegate(ref int target) { return Original::Interlocked.Exchange(ref target, value1); },
                "Interlocked.Exchange"
            );
        }

        public static object CompareExchange(ref object location, object value1, object value2)
        {
            return
                Helper.SimpleRefWrap<object>(
                ref location,
                delegate(ref object target) { return Original::Interlocked.CompareExchange(ref target, value1, value2); },
                "Interlocked.CompareExchange"
            );
        }
        public static object Exchange(ref object location, object value1)
        {
            return
                Helper.SimpleRefWrap<object>(
                ref location,
                delegate(ref object target) { return Original::Interlocked.Exchange(ref target, value1); },
                "Interlocked.Exchange"
            );
        }

        public static double CompareExchange(ref double location, double value1, double value2)
        {
            return
                Helper.SimpleRefWrap<double>(
                ref location,
                delegate(ref double target) { return Original::Interlocked.CompareExchange(ref target, value1, value2); },
                "Interlocked.CompareExchange"
            );
        }
        public static double Exchange(ref double location, double value1)
        {
            return
                Helper.SimpleRefWrap<double>(
                ref location,
                delegate(ref double target) { return Original::Interlocked.Exchange(ref target, value1); },
                "Interlocked.Exchange"
            );
        }

        public static long CompareExchange(ref long location, long value1, long value2)
        {
            return
                Helper.SimpleRefWrap<long>(
                ref location,
                delegate(ref long target) { return Original::Interlocked.CompareExchange(ref target, value1, value2); },
                "Interlocked.CompareExchange"
            );
        }
        public static long Exchange(ref long location, long value1)
        {
            return
                Helper.SimpleRefWrap<long>(
                ref location,
                delegate(ref long target) { return Original::Interlocked.Exchange(ref target, value1); },
                "Interlocked.Exchange"
            );
        }

        public static OIntPtr CompareExchange(ref OIntPtr location, OIntPtr value1, OIntPtr value2)
        {
            return
                Helper.SimpleRefWrap<OIntPtr>(
                ref location,
                delegate(ref OIntPtr target) { return Original::Interlocked.CompareExchange(ref target, value1, value2); },
                "Interlocked.CompareExchange"
            );
        }
        public static OIntPtr Exchange(ref OIntPtr location, OIntPtr value1)
        {
            return
                Helper.SimpleRefWrap<OIntPtr>(
                ref location,
                delegate(ref OIntPtr target) { return Original::Interlocked.Exchange(ref target, value1); },
                "Interlocked.Exchange"
            );
        }
     

        public static float CompareExchange(ref float location, float value1, float value2)
        {
            return
                Helper.SimpleRefWrap<float>(
                ref location,
                delegate(ref float target) { return Original::Interlocked.CompareExchange(ref target, value1, value2); },
                "Interlocked.CompareExchange"
            );
        }
        public static float Exchange(ref float location, float value1)
        {
            return
                Helper.SimpleRefWrap<float>(
                ref location,
                delegate(ref float target) { return Original::Interlocked.Exchange(ref target, value1); },
                "Interlocked.Exchange"
            );
        }
      
        #endregion
    }
}