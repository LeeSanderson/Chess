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
using Microsoft.ExtendedReflection.Utilities.Safe.Diagnostics;
using Microsoft.ExtendedReflection.ComponentModel;

namespace Microsoft.ManagedChess.EREngine.ComponentModel
{
    /// <summary>
    /// Base class of ChessCop component elements
    /// </summary>
    internal abstract class ComponentElementBase : IChessCopComponentElement
    {
        IChessCopComponent _host;
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="host">The host.</param>
        protected ComponentElementBase(IChessCopComponent host)
        {
            SafeDebug.AssumeNotNull(host, "host");

            this._host = host;
        }

        #region IChessCopComponentElement Members

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>The host.</value>
        public IChessCopComponent Host
        {
            get { return this._host; }
        }

        #endregion

        #region IComponentElement Members

        IComponent IComponentElement.Host
        {
            get { return this.Host; }
        }

        #endregion
    }
}
