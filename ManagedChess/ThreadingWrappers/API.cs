/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ManagedChess;
using Microsoft.ManagedChess.EREngine;

namespace __Substitutions.Microsoft.Concurrency.TestTools.UnitTesting.Chess
{
    // all calls from testcases into MChess go through this class
    
    public static class ChessAPI
    {

        #region standard chess api

        public static int Choose(int numChoices)
        {
            using (new WrapperSentry())
                return MChessChess.Choose(numChoices);
        }
        
        public static void TraceEvent(string info)
        {
            using (new WrapperSentry())
                MChessChess.TraceEvent(info);	
        }
        
        public static void PreemptionDisable()
        {
            using (new WrapperSentry())
            {
                // need to insert dummy access before turning off preemptions
                // - otherwise too many schedules get eliminated
                MChessChess.SyncVarAccess(509, MSyncVarOp.RWVAR_READWRITE);
                MChessChess.CommitSyncVarAccess();

                MChessChess.PreemptionDisable();
            }
        }

        public static void PreemptionEnable()
        {
            using (new WrapperSentry())
            {
                MChessChess.PreemptionEnable();
            }
        }

        // prioritize/unprioritize steps for best-first search (Katie)
        public static void PrioritizePreemptions()
        {
            using (new WrapperSentry())
                MChessChess.PrioritizePreemptions();
        }

        public static void UnprioritizePreemptions()
        {
            using (new WrapperSentry())
                MChessChess.UnprioritizePreemptions();
        }

        #endregion

        #region Primitive Observation Methods

        public static void ObserveOperationCall(object obj, string name)
        {
            using (new WrapperSentry())
            {
                int objnumber = (obj == null) ? 0 : ClrSyncManager.SyncManager.GetSyncVarFromObject(obj);
                MChessChess.ObserveOperationCall(objnumber, name);
            }
        }
        public static void ObserveOperationCall(string name)
        {
            using (new WrapperSentry())
                MChessChess.ObserveOperationCall(0, name);
        }
        public static void ObserveOperationReturn()
        {
            using (new WrapperSentry())
                MChessChess.ObserveOperationReturn();
        }
       public static void ObserveCallback(object obj, string name)
        {
            using (new WrapperSentry())
            {
                int objnumber = (obj == null) ? 0 : ClrSyncManager.SyncManager.GetSyncVarFromObject(obj);
                MChessChess.ObserveCallback(objnumber, name);
            }
        }
        public static void ObserveCallback(string name)
        {
            using (new WrapperSentry())
                MChessChess.ObserveCallback(0, name);
        }
        public static void ObserveCallbackReturn()
        {
            using (new WrapperSentry())
                MChessChess.ObserveCallbackReturn();
        }
        public static void ObserveInteger(string label, int val)
        {
            using (new WrapperSentry())
                MChessChess.ObserveIntValue(label, (long)val);
        }
        public static void ObserveInteger(string label, uint val)
        {
            using (new WrapperSentry())
                MChessChess.ObserveIntValue(label, (long)val);
        }
        public static void ObserveInteger(string label, long val)
        {
            using (new WrapperSentry())
                MChessChess.ObserveIntValue(label, val);
        }
        public static void ObserveInteger(string label, ulong val)
        {
            using (new WrapperSentry())
                MChessChess.ObserveStringValue(label, val.ToString());
        }
        public static void ObserveBoolean(string label, bool val)
        {
            using (new WrapperSentry())
                MChessChess.ObserveStringValue(label, val.ToString());
        }
        public static void ObserveString(string label, string val)
        {
            using (new WrapperSentry())
                MChessChess.ObserveStringValue(label, val);
        }
        public static void ObserveObjectIdentity(string label, object obj)
        {
            using (new WrapperSentry())
            {
                int objnumber = (obj == null) ? 0 : ClrSyncManager.SyncManager.GetSyncVarFromObject(obj);
                MChessChess.ObservePointerValue(label, objnumber);
            }
        }
        public static bool IsBreakingDeadlock()
        {
            using (new WrapperSentry())
            {
                return MChessChess.IsBreakingDeadlock();
            }
        }

        #endregion

        #region Derived Observation Methods

        #endregion

    }
}

