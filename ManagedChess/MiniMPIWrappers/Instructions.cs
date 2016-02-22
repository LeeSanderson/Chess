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
using System.Runtime.InteropServices;
using System.Security;
using System.Diagnostics;
//using MChess;
//using Microsoft.ManagedChess;
using Microsoft.ManagedChess.EREngine;

using OMini = global::MiniMPI;
using System.Threading;

namespace __Substitutions.MiniMPI
{
    static public class AsyncSendInstruction
    {
        static public OMini.AsyncSendInstruction ___ctor_newobj(OMini::MpiProcess owner, int id, int destRank, string payload, bool eager)
        {
            Console.WriteLine("AsyncSendInstruction {0} {1} {2}", owner.Rank, id, destRank);
            return new OMini.AsyncSendInstruction(owner, id, destRank, payload, eager);
        }
    }

    static public class AsyncReceiveInstruction
    {
        static public OMini::AsyncReceiveInstruction ___ctor_newobj(OMini::MpiProcess owner, int id, int? senderRank)
        {
            Console.WriteLine("AsyncReceiveInstruction {0} {1} {2}", owner.Rank, id, senderRank);
            return new OMini.AsyncReceiveInstruction(owner, id, senderRank);
        }
    }

    static public class WaitInstruction
    {
        static public OMini::WaitInstruction ___ctor_newobj(OMini::MpiProcess owner, int id, OMini::Handle handle)
        {
            Console.WriteLine("WaitInstruction {0} {1}", owner.Rank, id);
            OMini.WaitInstruction wait = new OMini.WaitInstruction(owner, id, handle);
            //Helper.WrapAcquire(
            //    delegate() { /* NOP */ return false; },
            //    delegate() { return true; /* should return the status of the Wait: true=not blocked, false=blocked*/ },
            //    owner,
            //    MSyncVarOp.LOCK_ACQUIRE,
            //    "MiniMPI.WaitInstruction"
            //);
            return wait;
        }
    }

    static public class BarrierInstruction
    {
        public static OMini::BarrierInstruction ___ctor_newobj(OMini::MpiProcess owner, int id, EventWaitHandle waitHandle)
        {
            Console.WriteLine("BarrierInstruction {0} {1}", owner.Rank, id);
            var barrier = new OMini.BarrierInstruction(owner, id, waitHandle);
            //Helper.WrapAcquire(
            //    delegate() { /* NOP */ return false; },
            //    delegate() { return true; 
            //          /* should return the status of the Barrier: 
            //           * true=not blocked, false=blocked*/ },
            //    owner,
            //    MSyncVarOp.LOCK_ACQUIRE,
            //    "MiniMPI.BarrierInstruction"
            //);
            return barrier;
        }
    }

    static public class FinalizeBarrierInstruction
    {
        public static OMini::FinalizeBarrierInstruction ___ctor_newobj(OMini::MpiProcess owner, int id, EventWaitHandle waitHandle)
        {
            Console.WriteLine("BarrierInstruction {0} {1}", owner.Rank, id);
            var barrier = new OMini.FinalizeBarrierInstruction(owner, id, waitHandle);
            //Helper.WrapAcquire(
            //    delegate() { /* NOP */ return false; },
            //    delegate() { return true; 
            //          /* should return the status of the Barrier: 
            //           * true=not blocked, false=blocked*/ },
            //    owner,
            //    MSyncVarOp.LOCK_ACQUIRE,
            //    "MiniMPI.BarrierInstruction"
            //);
            return barrier;
        }
    }
}
