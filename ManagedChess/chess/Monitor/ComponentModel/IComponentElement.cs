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
using Microsoft.ExtendedReflection.ComponentModel;

namespace Microsoft.ManagedChess.EREngine.ComponentModel
{
    /// <summary>
    /// ChessCop component element
    /// </summary>
    internal interface IChessCopComponentElement : IComponentElement
    {
        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>The host.</value>
        new IChessCopComponent Host { get; }
    }
}
