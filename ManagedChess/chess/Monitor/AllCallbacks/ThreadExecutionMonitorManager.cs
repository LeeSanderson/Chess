/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ManagedChess.EREngine.AllCallbacks
{
    internal sealed class ThreadExecutionMonitorManager
    {
        private readonly ThreadExecutionMonitorCollection monitors = new ThreadExecutionMonitorCollection();
        public ThreadExecutionMonitorCollection Monitors
        {
            get { return this.monitors; }
        }
    }
}
