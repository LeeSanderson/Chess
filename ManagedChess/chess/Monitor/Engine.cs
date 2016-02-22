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
using System.IO;
using System.Reflection;
using System.Diagnostics;

using Microsoft.ExtendedReflection.Interpretation;
using Microsoft.ExtendedReflection.Logging;
using Microsoft.ExtendedReflection.Monitoring;
using Microsoft.ExtendedReflection.Symbols;
using Microsoft.ExtendedReflection.Metadata;
using Microsoft.ExtendedReflection.Utilities.Safe.Diagnostics;
using Microsoft.ExtendedReflection.Utilities.Safe.IO;
using Microsoft.ExtendedReflection.Utilities;
using Microsoft.ExtendedReflection.ComponentModel;

using Microsoft.ManagedChess.EREngine.CallsOnly;
using Microsoft.ManagedChess.EREngine.AllCallbacks;
using Microsoft.ManagedChess.EREngine.ComponentModel;
using System.Runtime.CompilerServices;

namespace Microsoft.ManagedChess.EREngine
{
    /// <summary>
    /// Extended Reflection Engine
    /// </remarks>
    internal sealed class MyEngine : Microsoft.ExtendedReflection.ComponentModel.Engine
    {
        private static MyEngine theOnlyOne;
        public static void DisposeExecutionMonitors()
        {
            Debug.Assert(theOnlyOne != null);
            theOnlyOne.GetService<IMonitorManager>().DisposeExecutionMonitors();
        }

        internal static EnvironmentVars EnvironmentVars
        {
            get
            {
                SafeDebug.Assert(theOnlyOne != null, "theOnlyOne != null");
                return theOnlyOne.Options;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyEngine"/> class.
        /// </summary>
        public MyEngine()
            : base(new Container(),  new EngineOptions(), 
                   new MonitorManager(), new EnvironmentVars(), new ThreadMonitorManager())
        {
            // ensure that all substitution assemblies are loaded 
            foreach (var assemblyName in new string[]{
                    "Microsoft.ManagedChess.ThreadingWrappers",
                    "Microsoft.ManagedChess.Framework35Wrappers",
                    "Microsoft.ManagedChess.Framework4Wrappers",
                    "Microsoft.ManagedChess.MiniMPIWrappers",
                    "Microsoft.ManagedChess.PFXWrappers",
                    "ThreadPool40InternalWrappers"})   // add more wrapper like the ThreadPoolPatch one if you need
            {
#pragma warning disable 0618
                var a = Assembly.LoadWithPartialName(assemblyName); // returns 'null' when assembly is not found
#pragma warning restore 0618
                if (a != null)
                {
                    foreach (var m in a.GetModules())
                        System.Runtime.CompilerServices.RuntimeHelpers.RunModuleConstructor(m.ModuleHandle); // makes sure module is 'loaded'
                }
            }

            if (theOnlyOne != null)
            {
                throw new InvalidOperationException("MyEngine created more than once");
            }
            theOnlyOne = this;

            if (this.Options.Breaks.Contains("s"))
                SafeDebugger.Break();

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            this.GetService<ISymbolManager>().AddStackFrameFilter(new StackFrameFilter());

            if (this.Options.Logging)
                Console.WriteLine(".NET version: {0}", typeof(object).Assembly.GetName().Version);

            if (!ControllerEnvironment.IsMonitoringEnabled)
            {
                Console.WriteLine("ExtendedReflection monitor not enabled");
                throw new NotImplementedException("ExtendedReflection monitor not enabled");
            }

            ((IMonitorManager)this.GetService<MonitorManager>()).RegisterThreadMonitor(
               new ThreadMonitorFactory(
                   (ThreadMonitorManager)this.GetService<ThreadMonitorManager>()
                   ));

            if (this.Options.MonitorVolatile || this.Options.MonitorAccesses)
            {
                ((IMonitorManager)this.GetService<MonitorManager>()).RegisterObjectAccessThreadMonitor();
            }
            this.ExecutionMonitor.Initialize();
            var tid = this.ExecutionMonitor.CreateThread();
            _ThreadContext.Start(this.ExecutionMonitor, tid);
            // _ThreadContext.Stop(false);
        }

        /// <summary>
        /// Adds the symbol manager.
        /// </summary>
        protected override void AddSymbolManager()
        {
            base.AddSymbolManager();
        }

        /// <summary>
        /// Adds the components.
        /// </summary>
        protected override void AddComponents()
        {
            base.AddComponents();
        }

        private IExecutionMonitor ExecutionMonitor
        {
            get { return (IExecutionMonitor)this.GetService<MonitorManager>(); }
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>The options.</value>
        internal EnvironmentVars Options
        {
            get { return this.GetService<EnvironmentVars>(); }
        }

        private sealed class StackFrameFilter : IStackFrameFilter
        {
            public bool Exclude(StackFrameName frame)
            {
                string v = frame.Value;
                return
                    v.Contains("Microsoft.ManagedChess") ||
                    v.Contains("Microsoft.ExtendedReflection") ||
                    v.Contains("___redirect") ||
                    v.Contains("___lateredirect") ||
                    v.Contains("__Substitutions")
                    ;
            }
        }

        void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            this.ExecutionMonitor.Terminate();
            this.Log.Close();
        }
    }
}
