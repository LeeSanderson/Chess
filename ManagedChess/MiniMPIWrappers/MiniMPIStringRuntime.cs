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
using Microsoft.ManagedChess;
using Microsoft.ManagedChess.EREngine;

using OMini = global::MiniMPI;

namespace __Substitutions.MiniMPI
{
    static public class MiniMPIStringRuntime
    {
        static internal void BlockOnInstructionWaitHandle(OMini.MiniMPIStringRuntime self, OMini.IBlockingInstruction instr)
        {
            Helper.SimpleWrap<bool>(
                 delegate(ClrSyncManager manager)
                 {
                     //Console.WriteLine("BlockOnInstructionWaitHandle {0}", instr.ToString());
                     manager.SyncVarAccess(self, MSyncVarOp.RWVAR_READWRITE);
                     while (!instr.IsCompleted)
                     {
                         manager.LocalBacktrack();
                     }
                     manager.CommitSyncVarAccess();
                     return false;
                 },
                 delegate() { self.BlockOnInstructionWaitHandle(instr); return false; });
        }

        static internal int ChooseWildcardReceiveToMatch(OMini::MiniMPIStringRuntime self, int[] wildRecvIdxs)
        {
            return Helper.SimpleWrap<int>(
               delegate(ClrSyncManager manager)
               {
                   // Don't match unless all our processes are blocked
                   if (!self.AreAllProcessesBlocked())
                       return -1;
                   // let CHESS choose for us
                   return wildRecvIdxs[MChessChess.Choose(wildRecvIdxs.Length)];
               },
               delegate() {
                   // Don't match unless all our processes are blocked
                   if (!self.AreAllProcessesBlocked())
                       return -1;
                   return self.ChooseWildcardReceiveToMatch(wildRecvIdxs);
               }
               );
        }

        static internal int ChooseSendToMatchWithWildcardReceive(OMini::MiniMPIStringRuntime self, List<int> sendInstrIdxs)
        {
            return Helper.SimpleWrap<int>(
               delegate(ClrSyncManager manager)
               {
                   // Don't match unless all our processes are blocked
                   if (!self.AreAllProcessesBlocked())
                       return -1;
                   // let CHESS choose for us
                   return sendInstrIdxs[MChessChess.Choose(sendInstrIdxs.Count)];
               },
               delegate()
               {
                   // Don't match unless all our processes are blocked
                   if (!self.AreAllProcessesBlocked())
                       return -1;
                   return self.ChooseSendToMatchWithWildcardReceive(sendInstrIdxs);
               }
               );
        }
    }
}
