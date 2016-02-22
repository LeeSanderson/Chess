/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.ExtendedReflection.Monitoring;
using Microsoft.ExtendedReflection.Utilities;
using Microsoft.ExtendedReflection.Utilities.Safe.Diagnostics;
using Microsoft.ManagedChess.EREngine;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.ManagedChess.Base
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string[] real_args;
                string executable;
                if (args.Length > 0)
                {
                    executable = args[0];
                    real_args = new string[args.Length - 1];
                    for (int i = 0; i < real_args.Length; i++)
                    {
                        real_args[i] = args[i + 1];
                    }
                    Execute(executable, real_args);
                }
                else
                {
                    Environment.Exit((int)ChessExitCode.ChessFailure);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit((int)ChessExitCode.ChessFailure);
            }
        }
    

        static void Execute(string executable, string[] args)
        {
            var path = Assembly.GetAssembly(typeof(Program)).Location;
            var assembly = Assembly.LoadFrom(executable);

            string[] searchDirectories = new[]{
                Path.GetDirectoryName(path),
                Path.GetDirectoryName(assembly.Location),
            };

            FileInfo configInfo = new FileInfo(Path.GetFullPath(executable) + ".config");
            if (configInfo.Exists)
            {
                // If the assembly has a config file, then use it.
                using (var sandbox = SandboxAppDomain.Create(executable, null, searchDirectories))
                {
                    var engine = sandbox.CreateInstance<RemoteChessEngine>();
                    engine.Execute(executable, args);
                }
            }
            else
            {
                var resolver = new AssemblyResolver();
                Array.ForEach(searchDirectories, d => resolver.AddSearchDirectory(d));
                resolver.Attach();
                var engine = new RemoteChessEngine();
                engine.Execute(executable, args);
            }
        }
    }

    [Serializable]
    internal class RemoteChessEngine : MarshalByRefObject
    {
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Execute(string executable, string[] args)
        {
            try
            {
                var assembly = Assembly.LoadFrom(executable);
                var engine = new MyEngine();
                var envVars = MyEngine.EnvironmentVars;
                var testClassName = envVars.TestClass;
                var unitTestName = envVars.UnitTestName;
                var mco = SetOptions(envVars);
                var main = new ChessMain(mco, assembly, testClassName, unitTestName);
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit((int)ChessExitCode.ChessFailure);
            }
        }

        private MChessOptions SetOptions(EnvironmentVars pce)
        {
            var mco = new MChessOptions();

            // real arguments override whatever we hacked in with properties
            mco.PCT = pce.M_PCT;
            mco.PCT_VB = pce.M_VB_PCT;
            mco.DeRandomizedPCT = pce.M_DeRandomizedPCT;
            mco.PCT_num_of_runs = pce.M_num_of_runs;
            mco.varLabels = pce.VarLabels;
            mco.PCT_bug_depth = pce.M_bug_depth;
            mco.var_bound = pce.M_var_bound;
            mco.PCT_seed = pce.M_pct_seed;
            mco.breakOnAssert = pce.Breaks.Contains("a");
            mco.breakOnDeadlock = pce.Breaks.Contains("d");
            mco.breakOnPreemptions = pce.Breaks.Contains("p");
            mco.breakAfterPreemptions = pce.Breaks.Contains("P");
            mco.breakOnContextSwitch = pce.Breaks.Contains("c");
            mco.breakAfterContextSwitch = pce.Breaks.Contains("C");
            mco.breakOnTimeout = pce.Breaks.Contains("t");
            mco.breakOnTaskResume = pce.Breaks.Contains("f");
            mco.breakOnRace = pce.Breaks.Contains("r");

            mco.checkObservations = pce.CheckObservations;
            mco.loadSchedule = (pce.LoadSchedule || pce.Continue);
            mco.delayBound = pce.MaxDelays;
            mco.preemptionBound = pce.MaxPreemptions;
            mco.observationMode = pce.ObservationMode;
            mco.enumerateObservations = pce.EnumerateObservations;
            mco.finesse = pce.Finesse;
            mco.logging = pce.Logging;
            mco.xmlCommandline = pce.XmlCommandline;
            mco.maxExecutions = pce.MaxExecs;
            mco.maxChessTime = pce.MaxChessTime;
            mco.maxExecTime = pce.MaxExecTime;
            mco.maxStackSize = pce.MaxExecSteps;
            mco.monitorVolatiles = pce.MonitorVolatile;
            mco.monitorAccesses = pce.MonitorAccesses;
            mco.monitorCctors = pce.MonitorCCTOR;
            mco.outputPrefix = pce.OutputPrefix;
            mco.preemptionVars = pce.PreemptionVars;
            mco.preemptAccesses = pce.PreemptAccesses;
            mco.trace = pce.PrintTrace;
            mco.processorCount = pce.ProcessorCount;
            mco.showProgress = pce.ShowProgress;
            mco.sober = pce.Sober;
            mco.soberTargetrace = pce.Targetrace;
            mco.loadScheduleFile = pce.SchedFile;
            mco.recordPreemptMethods = pce.LoadSchedule && !pce.Continue;
            mco.tolerateDeadlock = pce.TolerateDeadlock;
            // Best-first search options
            mco.doDpor = pce.Dpor;
            mco.bounded = pce.Bounded;
            mco.bestFirst = pce.BestFirst;
            if (pce.BestFirst)
            {
                if (pce.Bounded || !pce.Dpor)
                {
                    if (pce.PrioritizeMethods.Count > 0)
                    {
                        // if we're prioritizing methods, add that to the priority function
                        mco.bestFirstPriority = "wdpor_method";
                    }
                    else
                    {
                        mco.bestFirstPriority = "wdpor";
                    }
                }
                else
                {
                    if (pce.PrioritizeMethods.Count > 0)
                    {
                        // if we're prioritizing methods, add that to the priority function
                        mco.bestFirstPriority = "wdpor_pb_method";
                    }
                    else
                    {
                        mco.bestFirstPriority = "wdpor_pb";
                    }
                }
            }

            // TODO: CHECK THIS
            // ??? for load-schedule, we default to max_executions=1
            if (pce.LoadSchedule)
                mco.maxExecutions = 1;

            // Use Reflection to go through the various public members of MChessChessOptions
            // use TryGetProperty to see if we have a property of the particular name
            // convert the value to the proper type (bool, int, string)
            foreach (FieldInfo fi in typeof(MChessOptions).GetFields())
            {
                string res;
                if (pce.TryGetProperty(fi.Name, out res))
                    setField(fi, mco, res);
                // convert camlCase to deprecated, underscores
                string underscored = "";
                for (int i = 0; i < fi.Name.Length; i++)
                {
                    if (char.IsLetter(fi.Name, i) && (char.IsUpper(fi.Name, i)))
                    {
                        underscored += ("_" + char.ToLower(fi.Name[i]));
                    }
                    else
                    {
                        underscored += fi.Name[i];
                    }
                }
                if (pce.TryGetProperty(underscored, out res))
                    setField(fi, mco, res);
            }
            return mco;
        }

        void setField(FieldInfo fieldInfo, MChessOptions options, string value)
        {
#if SHIP_BUILD
            bool ok;
            ok = fieldInfo.Name == "notime" || fieldInfo.Name == "showHbexecs" || fieldInfo.Name == "nopopups" || fieldInfo.Name == "doSleepSets";
            if (!ok)
            {
                // TODO: interaction with ChessBoard??
                Console.WriteLine("WARNING: This version of CHESS ignores the option /p:{0}={1}.", fieldInfo.Name, value);
                return;
            }
#endif
            if (fieldInfo.FieldType == typeof(bool))
            {
                fieldInfo.SetValue(options, Convert.ToBoolean(value));
            }
            else if (fieldInfo.FieldType == typeof(int))
            {
                fieldInfo.SetValue(options, Convert.ToInt32(value));
            }
            else if (fieldInfo.FieldType == typeof(string))
            {
                fieldInfo.SetValue(options, value);
            }
        }
    }
}
