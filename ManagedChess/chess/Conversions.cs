/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Text;
using MChess;
using System.Diagnostics;

using ChessTask = System.Int32;
using SyncVar = System.Int32;
using ManagedThreadId = System.Int32;

namespace Microsoft.ManagedChess.EREngine
{
    internal static class Conversions
    {
        // parameters
        private static bool TreatAllDataAsVolatiles = false;
        private static bool Active = false;
        private static bool TraceCalls = false;
        private static bool TraceConversions = false;

        private enum Varstate
        {
            convert_data_to_sync,
            sofar_untouched,
            sofar_data,
            sofar_sync,
            sofar_mixed
        }

        private class Varinfo
        {
            internal string location;
            internal int seqno;
            internal Varstate state;
            internal int varnum;
            internal string GetLabel() { /* return location + seqno; */ return varnum.ToString(); }
        }
        
        // holds variable info
        private static Dictionary<int, Varinfo> varinfo = new Dictionary<int, Varinfo>();

        // holds preemption variables
        private static Dictionary<string, bool> preemptionvars = new Dictionary<string, bool>();

        // used for labeling accesses
        private static Dictionary<string, int> seqno = new Dictionary<string, int>();
        private static Dictionary<string, int> initstate_seqno = new Dictionary<string, int>();
       
        private static ClrSyncManager syncmanager;

        public static void SetOptions(MChessOptions m)
        {
            if (m.finesse || m.preemptAccesses)
                TreatAllDataAsVolatiles = true;
            if (m.sober)
                Active = true;
            if (!String.IsNullOrEmpty(m.preemptionVars))
            {
                Active = true;
                foreach (string varlabel in m.preemptionVars.Split(','))
                    preemptionvars.Add(varlabel, true);
            }
        }

        public static void Init(ClrSyncManager syncmanager)
        {
            if (TraceCalls)
                System.Console.WriteLine("Conversions: Init()");
            Conversions.syncmanager = syncmanager;
        }

        public static void SetInitState()
        {
           if (TraceCalls)
                System.Console.WriteLine("Conversions: SetInitState()");
           initstate_seqno = new Dictionary<string, int>(seqno);
        }

        public static void Reset()
        {
           if (TraceCalls)
                System.Console.WriteLine("Conversions: Reset()");
           seqno = new Dictionary<string, int>(initstate_seqno);
        }

        public static void NewVar(SyncVar syncvar)
        {
           if (Active)
           {
               Varinfo info = varinfo[syncvar] = new Varinfo(); ;
               info.location = GetLocation();
               info.varnum = syncvar;
               info.seqno = GetSeqno(info.location);

               if (preemptionvars.ContainsKey(info.GetLabel()))
                   info.state = Varstate.convert_data_to_sync;
               else
                   info.state = Varstate.sofar_untouched;

                if (TraceCalls)
                    System.Console.WriteLine("Conversions: NewVar(" + syncvar + ") = " + info.GetLabel() + info.state.ToString());
           }
        }
        
        private static string GetLocation()
        {
            return syncmanager.CurrentTid.ToString() + "/";   
        }
        
        private static int GetSeqno(string location)
        {
            return (seqno.ContainsKey(location)) ? (++seqno[location]) : (seqno[location] = 1);
        }

        public static string GetDataVarLabel(int datavar)
        {
            return GetVarDescriptor(datavar/4);  // undo the *4 mult done for data variables
        }

        public static string GetVarDescriptor(SyncVar sv)
        {
            System.Diagnostics.Debug.Assert(varinfo.ContainsKey(sv));
            return varinfo[sv].GetLabel();
        }

        public static bool AggregateSyncVarAccess(int[] vars, MSyncVarOp op)
        {
            if (TraceCalls)
                System.Console.WriteLine("Conversions: AggregateSyncVarAccess(int[], " + op.ToString() + ")");
            return MChessChess.AggregateSyncVarAccess(vars, op);
        }

        public static bool SyncVarAccess(int varnum, MSyncVarOp op)
        {
            if (TraceCalls)
                System.Console.WriteLine("Conversions: SyncVarAccess(" + varnum + ", " + op.ToString() + ")");

            if (Active)
            {
                Varinfo info = varinfo[varnum];
                if (info.state == Varstate.sofar_untouched)
                {
                    info.state = Varstate.sofar_sync;
                }
                else if (info.state == Varstate.sofar_data)
                {
                    int alignedvarnum = varnum * 4;  // for compatibility with monitors that expect aligned pointers
                    MChessChess.MergeSyncAndDataVar(varnum, alignedvarnum);
                    info.state = Varstate.sofar_mixed;
                }
            }

            return MChessChess.SyncVarAccess(varnum, op);
        }

        public static bool CommitSyncVarAccess()
        {
            if (TraceCalls)
                System.Console.WriteLine("Conversions: CommitSyncVarAccess()");
            return MChessChess.CommitSyncVarAccess();
        }


       public static bool DataVarAccess(int varnum, bool isWrite)
        {
            Varinfo info = null;

            // quick path for conversion  ... sets info if active
            if (TreatAllDataAsVolatiles || (Active && ((info = varinfo[varnum]).state == Varstate.convert_data_to_sync)))
            {
                if (TraceConversions)
                    System.Console.WriteLine("Conversions: DATA->SYNC (" + varnum + ")");
                syncmanager.SetMethodInfo(isWrite ? "write" : "read");
                bool ok = true;
                ok &= SyncVarAccess(varnum, isWrite ? MSyncVarOp.RWVAR_WRITE : MSyncVarOp.RWVAR_READ);
                ok &= CommitSyncVarAccess();
                return ok;
            }

            if (TraceCalls)
                System.Console.WriteLine("Conversions: DataVarAccess(" + varnum + ", " + isWrite.ToString() + ")");

            int alignedvarnum = varnum * 4;  // for compatibility with monitors that expect aligned pointers

            if (Active)
            {
                if (info.state == Varstate.sofar_untouched)
                    {
                        info.state = Varstate.sofar_data;
                    }
                    else if (info.state == Varstate.sofar_sync)
                    {
                        MChessChess.MergeSyncAndDataVar(varnum, alignedvarnum);
                        info.state = Varstate.sofar_mixed;
                    }
            }

            MChessChess.DataVarAccess(alignedvarnum, isWrite);
            return true;
        }
    }
}
