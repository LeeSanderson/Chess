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
using Microsoft.ExtendedReflection.Collections;
using Microsoft.ExtendedReflection.Utilities.Safe.Diagnostics;
using Microsoft.ExtendedReflection.Logging;

namespace Microsoft.ManagedChess.EREngine.CallsOnly
{
    /// <summary>
    /// Plain frame.
    /// </summary>
    internal sealed class CallFrame : ICallFrame
    {
        // instance fields
        private readonly ICallFrame parent;
        private readonly int index;
        private readonly Method method;
        private Method callee;
        private object calleeReceiver;  // may be weak

        /// <summary>
        /// Constructor
        /// </summary>
        public CallFrame(
            ICallFrame parent,
            Method method,
            int index
            )
        {
            this.parent = parent;
            this.method = method;
            this.index = index;
        }

        public int Index
        {
            get { return this.index; }
        }

        public ICallFrame Parent
        {
            get { return parent; }
        }

        public Method ExecutingMethod
        {
            get { return method; }
        }

        public Method Callee
        {
            get { return callee; }
            internal set { callee = value; }
        }

        public object CalleeReceiver
        {
            get { return calleeReceiver; }
            internal set { calleeReceiver = value; }
        }

        public bool PreemptionsDisabled
        {
            get;
            set;
        }

        // For prioritizing methods in the best-first search
        public bool Prioritized
        {
            get;
            set;
        }
    }
}
