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
using Microsoft.ExtendedReflection.Metadata;

namespace Microsoft.ManagedChess.EREngine.CallsOnly
{
    /// <summary>
    /// One frame of the call stack.
    /// </summary>
    public interface ICallFrame
    {
        /// <summary>
        /// Gets the frame index
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Frame (method) that called us and thereby created this frame.
        /// </summary>
        ICallFrame Parent { get; }

        /// <summary>
        /// We created this frame to execute this method.
        /// </summary>
        Method ExecutingMethod { get; }

        /// <summary>
        /// Just temporary info for assembling a call.
        /// When this call succeeds, the calle will get his own frame.
        /// </summary>
        Method Callee { get; }

        /// <summary>
        /// Just temporary. May be null or arbitrary.
        /// </summary>
        object CalleeReceiver { get; }
    }
}
