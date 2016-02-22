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
using System.Reflection;
using System.IO;
using System.Diagnostics;
using Microsoft.ExtendedReflection.Monitoring;
using Microsoft.ExtendedReflection.Collections;
using Microsoft.ManagedChess.EREngine;
using Microsoft.ExtendedReflection.Utilities.Safe.Diagnostics;
using System.Collections.ObjectModel;
using Microsoft.ExtendedReflection.Utilities.Safe;
using Microsoft.Concurrency.TestTools;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.ManagedChess.Launcher
{
    internal sealed class MCLauncher
    {
        private readonly LauncherOptions options;

        internal LauncherOptions Options
        {
            get { return this.options; }
        }

        internal MCLauncher(LauncherOptions options)
        {
            this.options = options;
        }

        internal int Execute()
        {
            var shortname = Path.GetFileNameWithoutExtension(this.options.Arguments[0]);
            using (Process process = Run(false, new string[] { shortname}, null))
            {
                if (process == null)
                    return (int)ChessExitCode.ChessFailure;
                
                process.WaitForExit();
                return process.ExitCode;
            }
        }

        internal Process ExecuteProcess(bool openWindow, string[] dlls, EventHandler eh)
        {
            return Run(openWindow, dlls, eh);
        }

        private Process Run(bool openWindow, string[] assembliesToMonitor, EventHandler eh)
        {
            SafeDebug.AssumeNotNull(assembliesToMonitor, "assembliesToMonitor");
            // assemblies to instrument
            SafeSet<string> includedAssemblies = new SafeSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var a in assembliesToMonitor)
               includedAssemblies.Add(a);

            if (options.IncludedAssemblies != null)
                foreach (var a in options.IncludedAssemblies)
                    includedAssemblies.Add(a);

            // substitutions
            SafeSet<string> substitutionAssemblies = new SafeSet<string>(StringComparer.OrdinalIgnoreCase);
            Assembly a1 = typeof(MCLauncher).Assembly;
            var dir = Path.GetDirectoryName(a1.Location);
            // TODO: should check for existence and fail if required file not present
            substitutionAssemblies.Add(dir + "\\Microsoft.ManagedChess.ThreadingWrappers.dll");
            substitutionAssemblies.Add(dir + "\\Microsoft.ManagedChess.Framework35Wrappers.dll");
            if (File.Exists(dir + "\\Microsoft.ManagedChess.Framework4Wrappers.dll"))
            {
                substitutionAssemblies.Add(dir + "\\Microsoft.ManagedChess.Framework4Wrappers.dll");
                string[] newIncludedTypes = new string[options.IncludedTypes.Length+1];
                int i = 0;
                for (; i < options.IncludedTypes.Length; i++) {
                  newIncludedTypes[i] = options.IncludedTypes[i];
                }
                newIncludedTypes[i] = "System.Threading.Tasks.ThreadPoolTaskScheduler";
                options.IncludedTypes = newIncludedTypes;
            }
            if (File.Exists(dir + "\\Microsoft.ManagedChess.MiniMPIWrappers.dll"))
            {
                substitutionAssemblies.Add(dir + "\\Microsoft.ManagedChess.MiniMPIWrappers.dll");
            }

            // make sure we wrap strings, since ER doesn't
            for (int i = 0; i < options.Arguments.Length; i++)
            {
                options.Arguments[i] = WrapString(options.Arguments[i]);
            }

            
            if (options.CheckAtomicity)
            {
                //// CheckAtomicity implies race detection
                //options.RaceDetection = true;
                // checkatomicity is encoded using observationmode and checkobservations
                options.ObservationMode = "atom";
                options.CheckObservations = "fakefilename";
            }

            // tolerate deadlock for refinement or atomicity checking
            if (!string.IsNullOrEmpty(options.EnumerateObservations) || !string.IsNullOrEmpty(options.CheckObservations))
                options.TolerateDeadlock = true;

            // TODO
            bool trackAccesses = options.Finesse
               || options.PreemptAccesses
               || options.RaceDetection
               || options.CheckAtomicity
               || !String.IsNullOrEmpty(options.PreemptionVariables);

            // Best-first search requires gc tracking (BFS)
            bool gcTracking = trackAccesses || options.MonitorVolatile || options.BestFirst;

            // the following options imply that sober is needed
            if (options.RaceDetection || options.CheckAtomicity)
                options.Sober = true;

            // Method prioritization works exactly like DontPreemptMethods (BFS)
            bool enterLeave = options.Diagnose ||
                                (options.DontPreemptAssemblies != null && options.DontPreemptAssemblies.Length > 0) ||
                                (options.DontPreemptNamespaces != null && options.DontPreemptNamespaces.Length > 0) ||
                                (options.DontPreemptTypes != null && options.DontPreemptTypes.Length > 0) ||
                                (options.DontPreemptMethods != null && options.DontPreemptMethods.Length > 0) ||
                                (options.PrioritizeMethods != null && options.PrioritizeMethods.Length > 0);

            bool call = (options.Diagnose);

            // set up extended reflection
           //ProcessStartInfo dm = ControllerSetUp.GetMonitorableProcessStartInfo(

//            options.Finesse = true;

            ProcessStartInfo info = ControllerSetUp.GetMonitorableProcessStartInfo(
                options.FileName,
                options.Arguments,
                (( (call)  ? MonitorInstrumentationFlags.MethodCalls | MonitorInstrumentationFlags.SpecialOnCalling
                        : MonitorInstrumentationFlags.None) |
                ( enterLeave? MonitorInstrumentationFlags.EnterLeaveMethod : MonitorInstrumentationFlags.None) |
                (trackAccesses ? MonitorInstrumentationFlags.SpecialOnMemoryAccess :
                   options.MonitorVolatile ? MonitorInstrumentationFlags.SpecialOnVolatileMemoryAccess :
                   MonitorInstrumentationFlags.None) |
                   (options.Finesse ? MonitorInstrumentationFlags.SpecialOnNew : MonitorInstrumentationFlags.None) |
                   MonitorInstrumentationFlags.GeneralSubstitutionsOnDelegateBeginEndInvoke),
                gcTracking,

                null, // we don't monitor process at startup since it loads the DLL to monitor
                null, // ibid.
                
                substitutionAssemblies.ToArray(),
                options.IncludedTypes,
                options.ExcludedTypes,
                options.IncludedNamespaces,
                options.ExcludedNamespaces,
                includedAssemblies.ToArray(),
                options.ExcludedAssemblies,
                
                null, 
                null, null, null, 
                null, null,
                
                options.LogInstrumentation,
                false,
                options.TargetClr,
                !options.MonitorStaticConstructors,
                false,
                ProfilerInteraction.Fail,
                options.Driver, "", ""
                );

            info.CreateNoWindow = openWindow;

            // setup enviroment
            Microsoft.ManagedChess.EREngine.EnvironmentVars.SetEnvironmentVariables(
                info.EnvironmentVariables,
                options.PCT_Arguments,
                options.vb_bound,
                options.DeRandomized_PCT,
                options.Breaks,
                options.Properties,
                options.MonitorVolatile,
                trackAccesses,
                options.PreemptionVariables,
                options.MaxDelays,
                options.MaxPreemptions,
                options.LoadSchedule,
                options.ScheduleFile,
                options.ObservationMode,
                options.EnumerateObservations,
                options.CheckObservations,
                options.OutputPrefix,
                options.XmlCommandline,
                options.Continue,
                options.PrintTrace,
                options.PrintAllTraces,
                options.MonitorStaticConstructors,
                options.Finesse,
                options.Sober,
                options.PreemptAccesses,
                options.MaxExecs,
                options.MaxExecSteps,
                options.MaxChessTime,
                options.MaxExecTime,
                options.TargetRace,
                options.ChessTestClass,
                options.UnitTestName,
                options.Logging,
                options.ProcessorCount,
                options.FlipPreemptionSense,
                options.DontPreemptAssemblies, options.DontPreemptNamespaces, 
                options.DontPreemptTypes, options.DontPreemptMethods,
                options.Diagnose,
                options.ShowProgress,
                // best-first search options (BFS)
                options.PrioritizeMethods,
                options.BestFirst,
                options.Dpor,
                options.Bounded,
                options.TolerateDeadlock,
                // debug info only
                includedAssemblies.ToArray()
                );

            Console.WriteLine("ManagedCHESS. Copyright (C) Microsoft Corporation, 2008.");

            if (ControllerEnvironment.CheckOtherProfilerAlreadyLoaded())
            {
                Console.WriteLine("Another profiler already loaded!");
                return null;
            }

            // Console.WriteLine("launching application in instrumented process...");
            Console.WriteLine("Analyzing {0}  ...", info.Arguments);
            var process = new Process();
            process.StartInfo = info;
            if (eh != null)
            {
                process.EnableRaisingEvents = true;
                process.Exited += new EventHandler(eh);
            }
            if (process.Start())
                return process;
            else
                return null;
        }

        private static string WrapString(string value)
        {
            if (value == null)
                return value;
            else
                return
                    SafeString.IndexOf(value, ' ') != -1 ? "\"" + value.TrimEnd('\\') + "\"" : value;
        }
    }
}