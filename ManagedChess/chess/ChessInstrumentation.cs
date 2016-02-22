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
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.Reflection;

using Microsoft.ExtendedReflection.Monitoring;
using Microsoft.ExtendedReflection.Collections;
using Microsoft.ExtendedReflection.ComponentModel;
using Microsoft.ManagedChess.ComponentModel;
using Microsoft.ManagedChess;

using MChess;

namespace Microsoft.ManagedChess.Chess
{
    /// <summary>
    /// Builds a tree of all monitored method calls.
    /// </summary>
    public sealed class ChessInstrumentation : IPexCopDetector
    {
        string IPexCopDetector.Name
        {
            get { return "Chess"; }
        }

        MonitorInstrumentationFlags IPexCopDetector.InstrumentationFlags(PexCopLauncherOptions opt)
        {
            if (opt.MonitorVolatile)
            {
                return MonitorInstrumentationFlags.SpecialOnVolatileMemoryAccess;
            }
            else
            {
                return MonitorInstrumentationFlags.None;
            }
        }


        IIndexable<string> IPexCopDetector.InstrumentedAssemblies
        {
            get
            {
                return Indexable.Empty<string>();
            }
        }

        IIndexable<string> IPexCopDetector.UninstrumentedAssemblies
        {
            get
            {
                return
                    Indexable.Array<string>(new string[] {
                            "Chess", "MChess", "msvcm80d", "System.Drawing", "System.Windows.Forms"});
            }
        }

        IIndexable<string> IPexCopDetector.SubstitutionAssemblies
        {
            get
            {
                Assembly a = typeof(ChessInstrumentation).Assembly;
                return Indexable.One<string>(
                    a.GlobalAssemblyCache ? a.GetName().Name : a.Location
                    );
            }
        }

        static MethodInfo getEntryPoint()
        {
            // TODO: should restrict to the assembly passed in as argument to PexCop
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                if (a.EntryPoint != null)
                    return a.EntryPoint; 
            return null;
            
        }

        void IPexCopDetector.Load(IEngine ie)
        {
            // nothing here
        }

        private MChessOptions mco;
        private PexCopEnvironment pce;
        void IPexCopDetector.Initialize(IPexCopComponent host)
        {
            pce = host.Services.Options;
            if (pce.MonitorVolatile)
            {
                host.Services.MonitorManager.RegisterObjectAccessThreadMonitor();
            }

            // set this up for BeforeMain
            mco = new MChessOptions();
    
            // Use Reflection to go through the various public members of MChessChessOptions
            // use TryGetProperty to see if we have a property of the particular name
            // convert the value to the proper type (bool, int, string)
            foreach (FieldInfo fi in typeof(MChessOptions).GetFields())
            {
                string res;
                if (pce.TryGetProperty(fi.Name, out res))
                {
                    // Console.WriteLine("got {0}={1}", fi.Name, res);
                    if (fi.FieldType == typeof(bool))
                    {
                        fi.SetValue(mco, Convert.ToBoolean(res));
                        // Console.WriteLine("param {0}={1}", fi.Name, (bool)fi.GetValue(mco));
                    }
                    else if (fi.FieldType == typeof(int))
                    {
                        fi.SetValue(mco, Convert.ToInt32(res));
                    }
                    else if (fi.FieldType == typeof(string))
                    {
                        fi.SetValue(mco, res);
                    }
                    else
                    {
                        // print a warning
                    }
                }
            }

            // real arguments override whatever we hacked in with properties

            mco.preemption_bound = pce.CSB;
            mco.load_schedule = pce.LoadSchedule;
            mco.break_on_deadlock = pce.Breaks.Contains("d");
            mco.break_on_context_switch = pce.Breaks.Contains("c");
            mco.break_on_preemptions = pce.Breaks.Contains("p");
            mco.break_on_timeout = pce.Breaks.Contains("t");
        }

        void IPexCopDetector.BeforeMain()
        {
            ChessMain main = new ChessMain(pce, mco, pce.ShowGUI);
            Environment.Exit(0);
        }
    }
}
