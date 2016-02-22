/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Microsoft.ManagedChess.EREngine
{
    internal class WrapperSentry : IDisposable
    {
        [ThreadStatic]
        static long not_tracked = 0;  // 0 by default, means thread is *NOT* tracked by CHESS
        public WrapperSentry()
        {
            MChessChess.EnterChess();
        }


        public void Dispose()
        {
            MChessChess.LeaveChess();
        }

        static public void EnterChessImpl()
        {
            Debug.Assert(not_tracked > 0);
            not_tracked--;
        }

        static public void LeaveChessImpl()
        {
            not_tracked++;
            Debug.Assert(not_tracked > 0);
        }

        static public bool Wrap()
        {
            ClrSyncManager manager = ClrSyncManager.SyncManager;
            return manager != null && manager.IsInitialized && not_tracked > 0;
        }
    }
}
