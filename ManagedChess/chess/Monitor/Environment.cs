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
using System.Text.RegularExpressions;
using Microsoft.ExtendedReflection.Utilities;
using System.Reflection;
using Microsoft.ExtendedReflection.Utilities.Safe.Diagnostics;
using System.Collections.Specialized;
using Microsoft.ExtendedReflection.Utilities.Safe;
using Microsoft.ExtendedReflection.Collections;
using System.IO;
using Microsoft.ManagedChess.EREngine.ComponentModel;
using Microsoft.ExtendedReflection.ComponentModel;
using System.Diagnostics;

namespace Microsoft.ManagedChess.EREngine
{
    /// <summary>
    /// Dealing with ExtendedReflection  environment variables
    /// </summary>
    internal sealed class EnvironmentVars
       : ChessCopComponentBase
       , IService
    {
        // bools
        private const string EnvPCTScheduler = "Probalistic_SCHEDULER";
        private const string EnvPCTVBScheduler = "Probalistic_SCHEDULER_WITH_VB";
        private const string EnvLoadSchedule = "CHESS_LOAD_SCHEDULE";
        private const string EnvTrace = "CHESS_TRACE";
        private const string EnvContinue = "CHESS_CONTINUE";
        private const string EnvVolatile = "CHESS_VOLATILE";
        private const string EnvTrackAccesses = "CHESS_TRACK_ACCESSES";
        private const string EnvPreemptAccesses = "CHESS_PREEMPT_ACCESSES";
        private const string EnvCCTOR = "CHESS_CCTOR";
        private const string EnvFinesse = "CHESS_FINESSE";
        private const string EnvSober = "CHESS_SOBER";
        private const string EnvLogging = "CHESS_LOGGING";
        private const string EnvObservationMode = "CHESS_OBSERVATION_MODE";
        private const string EnvEnumerateObservations = "CHESS_ENUMERATE_OBSERVATIONS";
        private const string EnvCheckObservations = "CHESS_CHECK_OBSERVATIONS";
        private const string EnvDiagnose = "CHESS_DIAGNOSE";
        private const string EnvShowProgress = "CHESS_SHOW_PROGRESS";
        private const string EnvBestFirst = "CHESS_BEST_FIRST";
        private const string EnvDpor = "CHESS_DPOR";
        private const string EnvBounded = "CHESS_BOUNDED";
        private const string EnvTolerateDeadlock = "CHESS_TOLERATE_DEADLOCK";
        private const string EnvFlipPreemptSense = "CHESS_FLIP_PREEMPTION_SENSE";
        private const string EnvDeRandomizedPCT = "DeRandomizedPCT";

        // integers
        private const string EnvBugDepth = "PCT_BUG_DEPTH";
        private const string EnvNumOfRunsPCT = "PCT_Num_Of_Runs";
        private const string EnvPCTSeed = "PCT_Seed";
        private const string EnvVarBounding = "VARIABLE_BOUNDING";
        private const string EnvThreadBounding = "Thread_Boudning";
        private const string EnvDelays = "CHESS_MAXDELAYS";
        private const string EnvPreemptions = "CHESS_MAXPREEMPTIONS";
        private const string EnvMaxExecs = "CHESS_MAXEXECS";
        private const string EnvMaxExecSteps = "CHESS_MAXEXECSTEPS";
        private const string EnvMaxChessTime = "CHESS_MAX_CHESS_TIME";
        private const string EnvMaxExecTime = "CHESS_MAX_EXEC_TIME";
        private const string EnvTargetrace = "CHESS_TARGETRACE";
        private const string EnvProcessorCount = "CHESS_PROCESSOR_COUNT";

        // simple string
        private const string EnvTestClass = "CHESS_TESTCLASS";
        private const string EnvUnitTestName = "CHESS_UNITTESTNAME";
        private const string EnvBreaks = "CHESS_BREAKS";
        private const string EnvSchedFile = "CHESS_SCHED_FILE";
        private const string EnvOutputPrefix = "CHESS_OUTPUT_PREFIX";
        private const string EnvDontPreemptAssemblies = "CHESS_DONTPREMPT_ASSEMBLIES";
        private const string EnvDontPreemptNamespaces = "CHESS_DONTPREEMPT_NAMESPACES";
        private const string EnvDontPreemptTypes = "CHESS_DONTPREMPT_TYPES";
        private const string EnvDontPreemptMethods = "CHESS_DONTPREMPT_METHODS";
        private const string EnvPrioritizeMethods = "CHESS_PRIORITIZE_METHODS";

        // complex string
        private const string EnvProperties = "CHESS_PROPERTIES";
        private const string EnvPreemptionVars = "CHESS_PREEMPTIONVARS";
        private const string EnvXmlCommandline = "CHESS_XML_COMMANDLINE";

        // Debug info
        private const string EnvIncludeAssemblies = "CHESS_INCLUDE_ASSEMBLIES";
        private const string EnvClrMonitorInstrumentAssemblies = "CLRMONITOR_INSTRUMENT_ASSEMBLIES";

        public static void SetEnvironmentVariables(
            StringDictionary envvars,
            string[] PCT_options,
            int var_bound,
            string[] DeRandomizedPCT,
            string breaks,
            string[] properties,
            bool trackVolatile,
            bool trackAccesses,
            string preemptionVars,
            int maxDelays,
            int maxPreemptions,
            bool ls,
            string schedFile,
            string observationMode,
            string enumerateObservations,
            string checkObservations,
            string outputPrefix,
            string xmlCommandline,
            bool cont,
            bool trace,
            bool alltraces,
            bool cctor,
            bool finesse,
            bool sober,
            bool preemptaccesses,
            int maxexecs,
            int maxexecsteps,
            int maxchesstime,
            int maxexectime,
            int targetrace,
            string testclass,
            string unitTestName,
            bool logging,
            int processorCount,
            bool flipPreemptSense,
            string[] dontPreemptAssemblies,
            string[] dontPreemptNamespaces,
            string[] dontPreemptTypes,
            string[] dontPreemptMethods,
            bool diagnose,
            bool showprogress,
            // Best-first search options (BFS)
            string[] prioritizeMethods,
            bool bestFirst,
            bool dpor,
            bool bounded,
            bool tolerateDeadlock,
            string[] includeAssemblies
            )
        {
            envvars.Add(EnvOutputPrefix, outputPrefix);

            var vars = new StringDictionary();
            vars.Add(EnvOutputPrefix, outputPrefix);

            // we assume that the above values are valid
            if (properties != null && properties.Length > 0)
                vars.Add(EnvProperties, SafeString.Join<string>(";", properties));

            if ( DeRandomizedPCT.Length != 0)
            {
                vars.Add(EnvDeRandomizedPCT, "1");
                vars.Add(EnvBugDepth, DeRandomizedPCT[0]);
            }
            else
            {
                vars.Add(EnvDeRandomizedPCT, "0");
            }

            if (PCT_options.Length != 0)
            {
                string[] values = PCT_options[0].Split(':');
                Debug.Assert(values.Length == 2 || values.Length == 3);
                vars.Add(EnvPCTScheduler,"1");
                vars.Add(EnvNumOfRunsPCT,values[0]);
                vars.Add(EnvBugDepth,values[1]);
                if (values.Length == 3) vars.Add(EnvPCTSeed, values[2]);
                else vars.Add(EnvPCTSeed, "-1");
            }
            else
            {
                vars.Add(EnvPCTScheduler,"0");
                vars.Add(EnvNumOfRunsPCT,"-1");
                if(!vars.ContainsKey(EnvBugDepth))
                    vars.Add(EnvBugDepth,"-1");
            }

            vars.Add(EnvBreaks, breaks);
            vars.Add(EnvTestClass, testclass);
            vars.Add(EnvUnitTestName, unitTestName);
            vars.Add(EnvSchedFile, schedFile);
            vars.Add(EnvObservationMode, observationMode);
            vars.Add(EnvEnumerateObservations, enumerateObservations);
            vars.Add(EnvCheckObservations, checkObservations);
            vars.Add(EnvXmlCommandline, xmlCommandline);
            vars.Add(EnvPreemptionVars, preemptionVars);

            vars.Add(EnvVarBounding,var_bound.ToString());
            vars.Add(EnvDelays, maxDelays.ToString());
            vars.Add(EnvPreemptions, maxPreemptions.ToString());
            vars.Add(EnvProcessorCount, processorCount.ToString());
            vars.Add(EnvMaxExecs, maxexecs.ToString());
            vars.Add(EnvMaxChessTime, maxchesstime.ToString());
            vars.Add(EnvMaxExecTime, maxexectime.ToString());
            vars.Add(EnvMaxExecSteps, maxexecsteps.ToString());
            vars.Add(EnvTargetrace, targetrace.ToString());

            if (logging) vars.Add(EnvLogging, "1");
            if (trackVolatile) vars.Add(EnvVolatile, "1");
            if (trackAccesses) vars.Add(EnvTrackAccesses, "1");
            if (ls) vars.Add(EnvLoadSchedule, "1");
            if (cont) vars.Add(EnvContinue, "1");
            if (cctor) vars.Add(EnvCCTOR, "1");
            if (finesse) vars.Add(EnvFinesse, "1");
            if (sober) vars.Add(EnvSober, "1");
            if (preemptaccesses) vars.Add(EnvPreemptAccesses, "1");
            if (diagnose) vars.Add(EnvDiagnose, "1");
            if (showprogress) vars.Add(EnvShowProgress, "1");
            if (alltraces) vars.Add(EnvTrace, "1");
            if (bestFirst) vars.Add(EnvBestFirst, "1");
            if (dpor) vars.Add(EnvDpor, "1");
            if (bounded) vars.Add(EnvBounded, "1");
            if (tolerateDeadlock) vars.Add(EnvTolerateDeadlock, "1");
            if (flipPreemptSense) vars.Add(EnvFlipPreemptSense, "1");
            if (trace)
            {
                vars.Add(EnvTrace, "1");
                if (!ls)
                    vars.Add(EnvLoadSchedule, "1");
            }

            if (dontPreemptAssemblies != null && dontPreemptAssemblies.Length > 0)
                vars.Add(EnvDontPreemptAssemblies, SafeString.Join<string>(";", dontPreemptAssemblies));
            if (dontPreemptNamespaces != null && dontPreemptNamespaces.Length > 0)
                vars.Add(EnvDontPreemptNamespaces, SafeString.Join<string>(";", dontPreemptNamespaces));
            if (dontPreemptTypes != null && dontPreemptTypes.Length > 0)
                vars.Add(EnvDontPreemptTypes, SafeString.Join<string>(";", dontPreemptTypes));
            if (dontPreemptMethods != null && dontPreemptMethods.Length > 0)
                vars.Add(EnvDontPreemptMethods, SafeString.Join<string>(";", dontPreemptMethods));
            if (prioritizeMethods != null && prioritizeMethods.Length > 0)
                vars.Add(EnvPrioritizeMethods, SafeString.Join<string>(";", prioritizeMethods));

            // Passed on just for debug purposes
            if (includeAssemblies != null && includeAssemblies.Length > 0)
                vars.Add(EnvIncludeAssemblies, SafeString.Join<string>(";", includeAssemblies));

            var outputVars = new StreamWriter(outputPrefix + "\\mchess.vars");
            foreach (string v in vars.Keys)
            {
                outputVars.WriteLine("{0}={1}", v, vars[v]);
            }
            outputVars.Dispose();

        }

        StringDictionary envVar = new StringDictionary();
        string _lookup(string k) {
            if (envVar.ContainsKey(k))
                return envVar[k];
            else
                return "";
        }

        internal EnvironmentVars()
        {
            this.OutputPrefix = Environment.GetEnvironmentVariable(EnvOutputPrefix);
            Debug.Assert(this.OutputPrefix != null);
            var inputVars = new StreamReader(this.OutputPrefix + "\\mchess.vars");
            while (!inputVars.EndOfStream)
            {
                var line = inputVars.ReadLine();
                string key = "";
                string val = "";
                if (SafeString.TrySplitInTwo(line, '=', out key, out val))
                {
                    envVar[key] = val == null ? "" : val;
                }
            }
            inputVars.Dispose();

            // strings
            this.Breaks =                   _lookup(EnvBreaks);
            this.TestClass =                _lookup(EnvTestClass);
            this.UnitTestName =             _lookup(EnvUnitTestName);
            this.SchedFile =                _lookup(EnvSchedFile);
            this.ObservationMode =          _lookup(EnvObservationMode);
            this.EnumerateObservations =    _lookup(EnvEnumerateObservations);
            this.CheckObservations =        _lookup(EnvCheckObservations);
            this.XmlCommandline =           _lookup(EnvXmlCommandline);
            this.PreemptionVars =           _lookup(EnvPreemptionVars);

            // ints
            int bugdepth, pctseed, num_of_runs, var_bound, maxdelays, maxpreemptions, maxexectime, targetrace, max, proccount, maxchesstime, maxexecsteps;

            if (!Int32.TryParse(_lookup(EnvBugDepth), out bugdepth) || bugdepth < 0)
                this.M_bug_depth = -1;
            else
                this.M_bug_depth = bugdepth;

            if (!Int32.TryParse(_lookup(EnvPCTSeed), out pctseed) || pctseed < 0)
                this.M_pct_seed = -1;
            else
                this.M_pct_seed = pctseed;

            if (!Int32.TryParse(_lookup(EnvNumOfRunsPCT), out num_of_runs) || num_of_runs < 0)
                this.M_num_of_runs = -1;
            else
                this.M_num_of_runs = num_of_runs;

            if (!Int32.TryParse(_lookup(EnvVarBounding), out var_bound) || var_bound < 0)
                this.M_var_bound = -1;
            else
                this.M_var_bound = var_bound;

            if (!Int32.TryParse(_lookup(EnvDelays), out maxdelays) || maxdelays < 0)
                this.MaxDelays = -1;
            else
                this.MaxDelays = maxdelays;

            if (!Int32.TryParse(_lookup(EnvPreemptions), out maxpreemptions) || maxpreemptions < 0)
                this.MaxPreemptions = 2;
            else
                this.MaxPreemptions = maxpreemptions;

            if (!Int32.TryParse(_lookup(EnvMaxExecs), out max) || max < 0)
                this.MaxExecs = 0;
            else
                this.MaxExecs = max;
            if (!Int32.TryParse(_lookup(EnvProcessorCount), out proccount) || proccount <= 0)
                this.ProcessorCount = 1;
            else
                this.ProcessorCount = proccount;

            if (!Int32.TryParse(_lookup(EnvMaxChessTime), out maxchesstime) || maxchesstime < 0)
                this.MaxChessTime = 0;
            else
                this.MaxChessTime = maxchesstime;
            if (!Int32.TryParse(_lookup(EnvMaxExecTime), out maxexectime) || maxexectime < 0)
                this.MaxExecTime = 10;
            else
                this.MaxExecTime = maxexectime;
            if (!Int32.TryParse(_lookup(EnvMaxExecSteps), out maxexecsteps) || maxexecsteps <= 0)
                this.MaxExecSteps = 20000;
            else
                this.MaxExecSteps = maxexecsteps;
           if (!Int32.TryParse(_lookup(EnvTargetrace), out targetrace) || targetrace < 0)
                this.Targetrace = 0;
            else
                this.Targetrace = targetrace;

           // bools
            this.M_PCT =                _lookup(EnvPCTScheduler) == "1";
            this.M_VB_PCT =             _lookup(EnvPCTVBScheduler) == "1";
            this.Logging =              _lookup(EnvLogging) == "1";
            this.MonitorVolatile =      _lookup(EnvVolatile) == "1";
            this.MonitorAccesses =      _lookup(EnvTrackAccesses) == "1";
            this.LoadSchedule =         _lookup(EnvLoadSchedule) == "1";
            this.Continue =             _lookup(EnvContinue) == "1";
            this.PrintTrace =           _lookup(EnvTrace) == "1";
            this.MonitorCCTOR =         _lookup(EnvCCTOR) == "1";
            this.Finesse =              _lookup(EnvFinesse) == "1";
            this.Sober =                _lookup(EnvSober) == "1";
            this.PreemptAccesses =      _lookup(EnvPreemptAccesses) == "1";
            this.Diagnose =             _lookup(EnvDiagnose) == "1";
            this.ShowProgress =         _lookup(EnvShowProgress) == "1";
            this.BestFirst =            _lookup(EnvBestFirst) == "1";
            this.Dpor =                 _lookup(EnvDpor) == "1";
            this.Bounded =              _lookup(EnvBounded) == "1";
            this.TolerateDeadlock =     _lookup(EnvTolerateDeadlock) == "1";
            this.FlipPreemptSense =     _lookup(EnvFlipPreemptSense) == "1";
            this.M_DeRandomizedPCT = _lookup(EnvDeRandomizedPCT) == "1";
            this.PrioritizeMethods = new SafeSet<string>();
            this.DontPreemptAssemblies = new SafeSet<string>();
            this.DontPreemptNamespaces = new SafeSet<string>();
            this.DontPreemptTypes = new SafeSet<string>();
            this.DontPreemptMethods = new SafeSet<string>();
            this.InstrumentAssemblies = new SafeSet<string>();
            this.IncludeAssemblies = new SafeSet<string>();


            var insta = _lookup(EnvClrMonitorInstrumentAssemblies);
            if (!SafeString.IsNullOrEmpty(insta))
            {
                var assembly = SafeString.Split(insta, ',');
                foreach (var a in assembly) { this.InstrumentAssemblies.Add(a); }
            }

            var ea = _lookup(EnvDontPreemptAssemblies);
            if (!SafeString.IsNullOrEmpty(ea))
            {
                var assembly = SafeString.Split(ea, ';');
                foreach (var a in assembly) { this.DontPreemptAssemblies.Add(a); }
            }
            
            var en = _lookup(EnvDontPreemptNamespaces);
            if (!SafeString.IsNullOrEmpty(en))
            {
                var names = SafeString.Split(en, ';');
                foreach (var n in names) { this.DontPreemptNamespaces.Add(n); }
            }

            var et = _lookup(EnvDontPreemptTypes);
            if (!SafeString.IsNullOrEmpty(et))
            {
                var types = SafeString.Split(et, ';');
                foreach (var t in types) { this.DontPreemptTypes.Add(t); }
            }

            var em = _lookup(EnvDontPreemptMethods);
            if (!SafeString.IsNullOrEmpty(em))
            {
                var methods = SafeString.Split(em, ';');
                foreach (var m in methods) { this.DontPreemptMethods.Add(m); }
            }

            var pm = _lookup(EnvPrioritizeMethods);
            if (!SafeString.IsNullOrEmpty(pm))
            {
                var methods = SafeString.Split(pm, ';');
                foreach (var m in methods) { this.PrioritizeMethods.Add(m); }
            }

            var ia = _lookup(EnvIncludeAssemblies);
            if (!SafeString.IsNullOrEmpty(ia))
            {
                var assemblies = SafeString.Split(ia, ';');
                foreach (var a in assemblies) { this.IncludeAssemblies.Add(a); }
            }
        }

        public bool MonitorVolatile { get; private set; }
        public bool MonitorAccesses { get; private set; }
        public bool Logging { get; private set; }
        public bool Diagnose { get; private set; }
        public bool LoadSchedule { get; private set; }
        public bool Continue { get; private set; }
        public bool PrintTrace { get; private set; }
        public bool MonitorCCTOR { get; private set; }
        public bool Finesse { get; private set; }
        public bool Sober { get; private set; }
        public bool PreemptAccesses { get; private set; }
        public bool ShowProgress { get; private set; }
        public bool BestFirst { get; private set; }
        public bool Dpor { get; private set; }
        public bool Bounded { get; private set; }
        public bool TolerateDeadlock { get; private set; }
        public bool FlipPreemptSense { get; private set; }
	    public bool M_PCT { get; private set;}
        public bool M_VB_PCT { get; private set; }
        public bool M_DeRandomizedPCT { get; private set; }
    	public bool M_variable_bounding { get; private set;}

        public int M_bug_depth { get; private set; }
        public int M_num_of_runs { get; private set; }
        public int M_var_bound { get; private set; }
        public int M_pct_seed { get; private set; }
        public int MaxDelays { get; private set; }
        public int MaxPreemptions { get; private set; }
        public int MaxChessTime { get; private set; }
        public int MaxExecTime { get; private set; }
        public int MaxExecSteps { get; private set; }
        public int Targetrace { get; private set; }
        public int MaxExecs { get; private set; }
        public int ProcessorCount { get; private set; }

        public string Breaks { get; private set;  }
        public string TestClass { get; private set; }
        public string VarLabels { get; private set; }
        public string UnitTestName { get; private set; }
        public string SchedFile { get; private set; }
        public string ObservationMode { get; private set; }
        public string EnumerateObservations { get; private set; }
        public string CheckObservations { get; private set; }
        public string OutputPrefix { get; private set; }
        public string XmlCommandline { get; private set; }
        public string PreemptionVars { get; private set; }

        public SafeSet<string> DontPreemptAssemblies { get; private set; }
        public SafeSet<string> DontPreemptNamespaces { get; private set; }
        public SafeSet<string> DontPreemptTypes { get; private set; }
        public SafeSet<string> DontPreemptMethods { get; private set; }
        public SafeSet<string> PrioritizeMethods { get; private set; }

        public SafeSet<string> IncludeAssemblies { get; private set; }
        public SafeSet<string> InstrumentAssemblies { get; private set; }

        private static SafeDictionary<string, string> properties { get; set; }

        // Tries to get a property value
        public bool TryGetProperty(string name, out string value)
        {
            if (properties == null)
            {
                SafeDictionary<string, string> ps = new SafeDictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                string eprops = _lookup(EnvProperties);
                if (!String.IsNullOrEmpty(eprops))
                {
                    foreach (string p in eprops.Split(';'))
                    {
                        string[] parts = p.Split('=');
                        if (parts.Length != 2)
                        {
                            // for PCP team, also try to split on :
                            string[] parts2 = p.Split(':');
                            if (parts2.Length != 2)
                                throw new ArgumentException("invalid property value " + p);
                            else
                                ps.Add(parts2[0], parts2[1]);
                        }
                        else
                        {
                            ps.Add(parts[0], parts[1]);
                        }
                    }
                }
                properties = ps;
            }
            return properties.TryGetValue(name, out value);
        }
    }
}
