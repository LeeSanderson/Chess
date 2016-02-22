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

namespace __Substitutions.System
{
    [DebuggerNonUserCode]
    public static class WeakReference
    {
        // weak references can go away when a GC takes place, introducing 
        // non-determinism. We'll handle this by making a strong reference to them
        public static global::System.WeakReference ___ctor_newobj(object location)
        {
            return Helper.SimpleWrap<global::System.WeakReference>(
                    delegate(ClrSyncManager manager)
                    {
                        manager.MakeStrongReference(location);
                        return new global::System.WeakReference(location);
                    },
                    delegate()
                    {
                        return new global::System.WeakReference(location);
                    }
                );
        }

        public static global::System.WeakReference ___ctor_newobj(object location, bool flag)
        {
            return Helper.SimpleWrap<global::System.WeakReference>(
                    delegate(ClrSyncManager manager)
                    {
                        manager.MakeStrongReference(location);
                        return new global::System.WeakReference(location, flag);
                    },
                    delegate()
                    {
                        return new global::System.WeakReference(location, flag);
                    }
                );
        }

        public static void set_Target(global::System.WeakReference location, object target)
        {
            Helper.SimpleWrap<bool>(
                   delegate(ClrSyncManager manager)
                   {
                       manager.MakeStrongReference(target);
                       location.Target = target;
                       return true;
                   },
                   delegate()
                   {
                       location.Target = 0;
                       return true;
                   }
               );
        }
    }
}
