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
using Microsoft.ExtendedReflection.XmlDocumentation;

namespace Microsoft.ManagedChess.EREngine.ComponentModel
{
    /// <summary>
    /// List of available ChessCop services
    /// </summary>
    internal class ChessCopComponentServices : ComponentServices, IChessCopComponentServices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChessCopComponentServices"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        public ChessCopComponentServices(IComponent host)
            : base(host)
        { }

        EnvironmentVars _options;
        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>The options.</value>
        public new EnvironmentVars Options
        {
            get
            {
                if (this._options == null)
                    this._options = this.GetService<EnvironmentVars>();
                return this._options;
            }
        }

        IMonitorManager _monitorManager;
        /// <summary>
        /// Gets the monitor manager.
        /// </summary>
        /// <value>The monitor manager.</value>
        public IMonitorManager MonitorManager
        {
            get
            {
                if (this._monitorManager == null)
                    this._monitorManager = this.GetService<MonitorManager>();
                return this._monitorManager;
            }
        }

    }
}
