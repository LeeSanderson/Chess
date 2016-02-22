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
    /// Base class for <see cref="IChessCopComponent"/>.
    /// </summary>
    internal class ChessCopComponentBase : ComponentBase, IChessCopComponent
    {
        IChessCopComponentServices _services;
        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>The services.</value>
        public new IChessCopComponentServices Services
        {
            get
            {
                if (this._services == null)
                    this._services = new ChessCopComponentServices(this);
                return this._services;
            }
        }
    }
}
