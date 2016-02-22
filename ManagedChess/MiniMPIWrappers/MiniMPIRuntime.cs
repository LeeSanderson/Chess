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

namespace __Substitutions.MiniMPI
{
    static public class MiniMPIRuntime
    {
        static public void BlockOnInstructionWaitHandle(OMini.MiniMPIRuntime self, OMini.IBlockingInstruction instr)
        {
            Helper.SimpleWrap<bool>(
                 delegate(ClrSyncManager manager)
                 {
                     Console.WriteLine("BlockOnInstructionWaitHandle {0}", instr.ToString());
                     manager.SyncVarAccess(self, MSyncVarOp.RWVAR_READWRITE);
                     while (!instr.IsCompleted)
                     {
                         manager.LocalBacktrack();
                     }
                     manager.CommitSyncVarAccess();
                     return false;
                 },
                 delegate() { instr.WaitHandle.WaitOne(); return false; });
        }
    }
}
