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
using Microsoft.ExtendedReflection.Collections;
using Microsoft.ExtendedReflection.Monitoring;

namespace Microsoft.ManagedChess.EREngine.AllCallbacks
{
    /// <summary>
    /// Collection of <see cref="IThreadExecutionMonitor"/>
    /// </summary>
    internal sealed class ThreadExecutionMonitorCollection
        : SafeList<IThreadExecutionMonitor>
    { }
}
