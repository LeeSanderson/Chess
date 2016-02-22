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
using Microsoft.ExtendedReflection.CommandLine;
using Microsoft.ExtendedReflection.Utilities.Safe;
using Microsoft.ExtendedReflection.Collections;
using Microsoft.ExtendedReflection.Metadata;
using Microsoft.ExtendedReflection.Utilities;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.ManagedChess.Launcher
{
    internal sealed class LauncherOptions : ICommandLineDestination
    {
        static internal class Categories
        {
            public const string Main = "Main";
            public const string Misc = "Misc";
            public const string Bounding = "Bounding";
            public const string Monitoring = "Monitoring";
            public const string Troubleshooting = "Troubleshooting";
            public const string Debug = "Debug";
            public const string Chess = "Chess";
            public const string PCT = "PCT";
        }

        /// <summary>Core assembly file to instrument.</summary>
        [DefaultArgument(
            ArgumentType.AtMostOnce,
            Category = Categories.Main,
            IsSimple = true,
            DefaultValue = null,
            HelpText = "application file to execute",
            LongHelpText = @"
File name of application to monitor and analyze.

Example:

mchess MyApplication.exe
"
            )]
        public string FileName { get; set; }

        [Argument(
            ArgumentType.Multiple,
            Category = Categories.PCT,
            IsSimple = true,
            DefaultValue = null,
            HelpText = "Probabilistic Randomized Scheduler",
            ShortName = "pct",
            LongName = "pct",
            LongHelpText = "Probabilistic Randomized Scheduler"
            )]
        public string[] PCT_Arguments { get; set; }

        [Argument(
            ArgumentType.Multiple,
            Category = Categories.PCT,
            IsSimple = true,
            DefaultValue = null,
            HelpText = "DeRadomized Probalistic Randomized Scheduler",
            ShortName = "drpct",
            LongName = "derandomizedpct",
            LongHelpText = "DeRadomized Probalistic Randomized Scheduler"
            )]
        public string[] DeRandomized_PCT { get; set; }

        /// <summary>Core assembly file to instrument.</summary>
        [Argument(
            ArgumentType.Multiple,
            Category = Categories.Bounding,
            IsSimple = true,
            DefaultValue = -1,
            HelpText = "variable bounding",
            ShortName = "vb",
            LongName = "variablebound",
            LongHelpText = "variable bouding "
            )]
        public int vb_bound { get; set; }
        

        /// <summary>
        /// Option
        /// </summary>
        [Argument(
            ArgumentType.Multiple,
            Category = Categories.Main,
            IsSimple = true,
            DefaultValue = null,
            HelpText = "argument to monitored application",
            ShortName = "arg",
           LongName = "argument",
           LongHelpText = @"
Option to pass through arguments to the monitored application.

Examples:

mchess MyApplication.exe /arg:SomeArgumentToApplication
"
           )]
        public string[] Arguments { get; set; }

        /// <summary>
        /// Contains the complete command line in xml format
        /// </summary>
        public string XmlCommandline;

        internal const string DefaultChessTestClass = "ChessTest";
        /// <summary>
        /// class with Chess methods (default "ChessTest")
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = DefaultChessTestClass,
            HelpText = "Fully qualified name of class with Chess methods",
            ShortName = "tc",
            LongName = "testclass")]
        public string ChessTestClass { get; set; }

        /// <summary>
        /// The name of the Concurrency Unit Test method to run.
        /// Can not be specified along with the 'testclass' option.
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = "",
            HelpText = "Fully qualified name of Concurrency Unit Test method to run.",
            ShortName = "t",
            LongName = "test",
            LongHelpText = @"
Fully qualified name of Concurrency Unit Test method to run. Test methods
should be marked with the ChessMethodAttribute attribute. An error will
occur if there is more than one method with the specified name.

Examples:
mchess MyRegressionTests.dll /test:Company.App.Tests.ClassName.TestMethodName
mchess MyRegressionTests.dll /test:ClassName.TestMethodName

When specifying arguments for test methods, the arguments are handled in the
order they appear on the commandline and are converted to the correct type.

Example:
mchess MyRegressionTests.dll /test:ServerTests.ServerClientTest /arg:1
This command line will run a test method with the following signature:
   [ChessMethod]
   public void ServerClientTest(int numClients);

"
           )]
        public string UnitTestName { get; set; }

        /// <summary>
        /// output prefix 
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = ".\\",
            HelpText = "prefix for output files",
            ShortName = "op",
            LongName = "outputprefix")]
        public string OutputPrefix { get; set; }

        // CHESS-specific diagnostics

        /// <summary>
        /// show progress
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = false,
            HelpText = "show progress of CHESS in exploration mode",
            ShortName = "sp",
            LongName = "showprogress")]
        public bool ShowProgress { get; set; }

        /// <summary>
        /// Option
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            IsSimple = false,
            DefaultValue = false,
            HelpText = "list which types are not instrumented",
            ShortName = "di",
            LongName = "diagnose"
           )]
        public bool Diagnose { get; set; }

        // which things to instrument and how to figure out what's instrumented,
        // what's not

        /// <summary>
        /// Option
        /// </summary>
        [Argument(
            ArgumentType.Multiple,
            Category = Categories.Monitoring,
            DefaultValue = null,
            HelpText = "assemblies to monitor",
            ShortName = "ia",
            LongName = "includeassembly")]
        public string[] IncludedAssemblies { get; set; }

        /// <summary>
        /// Option
        /// </summary>
        [Argument(
            ArgumentType.Multiple,
            Category = Categories.Monitoring,
            DefaultValue = null,
            HelpText = "assemblies to exclude from monitoring",
            ShortName = "ea",
            LongName = "excludeassembly")]
        public string[] ExcludedAssemblies { get; set; }

        /// <summary>
        /// Option
        /// </summary>
        [Argument(
            ArgumentType.Multiple,
            Category = Categories.Monitoring,
            DefaultValue = null,
            HelpText = "namespaces to monitor",
            ShortName = "in",
            LongName = "includenamespace")]
        public string[] IncludedNamespaces { get; set; }

        /// <summary>
        /// Option
        /// </summary>
        [Argument(
            ArgumentType.Multiple,
            Category = Categories.Monitoring,
            DefaultValue = null,
            HelpText = "namespaces to exclude from monitoring",
            ShortName = "en",
            LongName = "excludenamespace")]
        public string[] ExcludedNamespaces { get; set; }

        /// <summary>
        /// Option
        /// </summary>
        [Argument(
            ArgumentType.Multiple,
            Category = Categories.Monitoring,
            DefaultValue = null,
            HelpText = "types to monitor",
            ShortName = "it",
            LongName = "includetype")]
        public string[] IncludedTypes { get; set; }

        /// <summary>
        /// Option
        /// </summary>
        [Argument(
            ArgumentType.Multiple,
            Category = Categories.Monitoring,
            DefaultValue = null,
            HelpText = "types to exclude from monitoring",
            ShortName = "et",
            LongName = "excludetype")]
        public string[] ExcludedTypes { get; set; }

        // Repro, Tracing, and Continue

        /// <summary>
        /// load the schedule
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = false,
            HelpText = "load schedule file",
            ShortName = "ls",
            LongName = "repro")]
        public bool LoadSchedule { get; set; }

        /// <summary>
        /// schedule file 
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = "sched",
            HelpText = "name of schedule file",
            ShortName = "sf",
            LongName = "schedulefile")]
        public string ScheduleFile { get; set; }

        /// <summary>
        /// print trace file
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = false,
            HelpText = "print trace file (for loaded schedule only)",
            LongName = "printtrace",
            ShortName = "trace")]
        public bool PrintTrace { get; set; }

        /// <summary>
        /// print trace file
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = false,
            HelpText = "print trace file (for all schedules)",
            LongName = "printalltraces",
            ShortName = "alltraces")]
        public bool PrintAllTraces { get; set; }

        /// <summary>
        /// continue from the schedule
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = false,
            HelpText = "continue from schedule file",
            ShortName = "cont",
            LongName = "continue")]
        public bool Continue { get; set; }

        // monitoring options for controlling search

        /// <summary>
        /// delay bound (-1 default)
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = -1,
            HelpText = "maximum number of delays",
            ShortName = "md",
            LongName = "maxdelays")]
        public int MaxDelays { get; set; }

        /// <summary>
        /// preemption bound (0=none, 2 default)
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = 2,
            HelpText = "maximum number of preemptions",
            ShortName = "mp",
            LongName = "maxpreemptions")]
        public int MaxPreemptions { get; set; }


        /// <summary>
        /// maxexecsteps (default=20000)
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = 20000,
            HelpText = "maximum execution steps per test run",
            ShortName = "mes",
            LongName = "maxexecsteps")]
        public int MaxExecSteps { get; set; }

        /// <summary>
        /// maxexecs (0=unlimited, default)
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = 10,
            HelpText = "maximum seconds per test run (0=unlimited)",
            ShortName = "met",
            LongName = "maxexectime")]
        public int MaxExecTime { get; set; }

        /// <summary>
        /// maxexecs (0=unlimited, default)
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = 0,
            HelpText = "maximum number of executions",
            ShortName = "me",
            LongName = "maxexecs")]
        public int MaxExecs { get; set; }

        /// <summary>
        /// timeout (0=unlimited, default)
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = 0,
            HelpText = "timeout on CHESS run (seconds)",
            ShortName = "mct",
            LongName = "maxchesstime")]
        public int MaxChessTime { get; set; }

        // options to prevent preemptions in various places in the code

        /// <summary>
        /// continue from the schedule
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = false,
            HelpText = "disable preemptions by default (flip sense of preemptions)",
            ShortName = "flip",
            LongName = "flippreempt")]
        public bool FlipPreemptionSense { get; set; }

        /// <summary>
        /// Option
        /// </summary>
        [Argument(
            ArgumentType.Multiple,
            Category = Categories.Monitoring,
            DefaultValue = null,
            HelpText = "don't preempt code in this assembly",
            ShortName = "dpa",
            LongName = "dontpreemptassembly")]
        public string[] DontPreemptAssemblies { get; set; }

        [Argument(
            ArgumentType.Multiple,
            Category = Categories.Monitoring,
            DefaultValue = null,
            HelpText = "don't preempt code in this namespace",
            ShortName = "dpn",
            LongName = "dontpreemptnamespace")]
        public string[] DontPreemptNamespaces { get; set; }

        /// <summary>
        /// Option
        /// </summary>
        [Argument(
            ArgumentType.Multiple,
            Category = Categories.Monitoring,
            DefaultValue = null,
            HelpText = "don't preempt code in this type",
            ShortName = "dpt",
            LongName = "dontpreempttype")]
        public string[] DontPreemptTypes { get; set; }

        /// <summary>
        /// Option
        /// </summary>
        [Argument(
            ArgumentType.Multiple,
            Category = Categories.Monitoring,
            DefaultValue = null,
            HelpText = "don't preempt code in this method",
            ShortName = "dpm",
            LongName = "dontpreemptmethod")]
        public string[] DontPreemptMethods { get; set; }

        // Best-first search and DPOR options (Katie)

        /// <summary>
        /// Best-first search
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = false,
            HelpText = "use the best-first search strategy",
            LongName = "bestfirst",
            ShortName = "bf")]
        public bool BestFirst { get; set; }

        /// <summary>
        /// Dynamic partial-order reduction
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = false,
            HelpText = "use dynamic partial-order reduction (automatically removes preemption bound, use /bound to override)",
            LongName = "dynpor",
            ShortName = "dpor")]
        public bool Dpor { get; set; }

        /// <summary>
        /// Force a bounded search even when DPOR is specified
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = false,
            HelpText = "force a bounded search even when DPOR is specified",
            LongName = "bounded",
            ShortName = "bound")]
        public bool Bounded { get; set; }

        /// <summary>
        /// Prioritize preemptions in this method
        /// </summary>
        [Argument(
            ArgumentType.Multiple,
            Category = Categories.Monitoring,
            DefaultValue = null,
            HelpText = "prioritize preemptions in this method",
            ShortName = "pm",
            LongName = "prioritizemethod")]
        public string[] PrioritizeMethods { get; set; }

        // debugging options

        /// <summary>
        /// Option
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Debug,
            DefaultValue = "",
            HelpText = "breaks on condition [s=start,c=context-switch,p=preemption,d=deadlock,t=timeout,a=assertion]",
            ShortName = "brk",
            LongName = "break")]
        public string Breaks { get; set; }


        /// <summary>
        /// Tolerate Deadlocks
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = false,
            HelpText = "continue schedule exploration on deadlock",
            LongName = "toleratedeadlock",
            ShortName = "td")]
        public bool TolerateDeadlock { get; set; }

        // dealing with memory locations

        /// <summary>
        /// Perform Race Detection
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = false,
            HelpText = "Perform race detection",
            ShortName = "dr",
            LongName = "detectraces")]
        public bool RaceDetection { get; set; }

        /// <summary>
        /// targetrace
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = 0,
            HelpText = "which race to repro",
            ShortName = "tr",
            LongName = "targetrace")]
        public int TargetRace { get; set; }

        /// <summary>
        /// Option
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = "",
            HelpText = "comma-separated list of variable labels on which to preempt",
            ShortName = "pv",
            LongName = "preemptionvariables")]
        public string PreemptionVariables { get; set; }

        /// <summary>
        /// Promote all variables to volatile
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = false,
            HelpText = "Insert preemptions on all memory accesses",
            ShortName = "pa",
            LongName = "preemptaccesses")]
        public bool PreemptAccesses { get; set; }

        /// <summary>
        /// Option
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            IsSimple = false,
            DefaultValue = true,
            HelpText = "monitor volatile accesses",
            ShortName = "v",
            LongName = "volatile"
           )]
        public bool MonitorVolatile { get; set; }

        /// <summary>
        /// Allow store buffer race detection
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = false,
            HelpText = "Detect Store Buffer Vulnerabilities",
            ShortName = "sb",
            LongName = "sober")]
        public bool Sober { get; set; }

        // refinement checking options

        /// <summary>
        /// observation mode
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = "",
            HelpText = "choose specific mode for refinement checking",
            ShortName = "om",
            LongName = "observationmode")]
        public string ObservationMode { get; set; }

        /// <summary>
        /// record observation set to specified file
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = "",
            HelpText = "enumerate serial observations and save to specified file name",
            ShortName = "eo",
            LongName = "enumerateobservations")]
        public string EnumerateObservations { get; set; }

        /// <summary>
        /// record observation set to specified file
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = "",
            HelpText = "check that observations are contained in specified file",
            ShortName = "co",
            LongName = "checkobservations")]
        public string CheckObservations { get; set; }

        /// <summary>
        /// record observation set to specified file
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = false,
            HelpText = "check that operations are conflict-serializable",
            ShortName = "ca",
            LongName = "checkatomicity")]
        public bool CheckAtomicity { get; set; }

        /// <summary>
        /// record observation set to specified file
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = "",
            HelpText = "instead of running test, add test to a testlist, specified as filename:listname:...:listname:testname",
            ShortName = "at",
            LongName = "addtesttolist")]
        public string AddTestToList { get; set; }

        /// <summary>
        /// processor count
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = 1,
            HelpText = "simulated processor count",
            ShortName = "pc",
            LongName = "processorcount")]
        public int ProcessorCount { get; set; }

        // Experimental (not for release)

#if DEBUG
        /// <summary>
        /// monitor CCTORs
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = false,
            HelpText = "monitor cctors",
            ShortName = "cctor")]
#endif
        public bool MonitorStaticConstructors { get; set; }

#if DEBUG
        /// <summary>
        /// Do Finalizer checking with CHESS
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Chess,
            DefaultValue = false,
            HelpText = "Finess: Finalizer checking with CHESS",
            ShortName = "finesse",
            LongName = "finalizerCHESS")]
#endif
        public bool Finesse { get; set; }

        // Misc. Not CHESS specific

        /// <summary>
        /// Additional name/value pairs for detectors
        /// </summary>
        [Argument(
            ArgumentType.Multiple,
            Category = Categories.Debug,
            DefaultValue = null,
            HelpText = "<name>=<value> pairs",
            ShortName = "p",
            LongName = "property")]
        public string[] Properties { get; set; }

        /// <summary>
        /// Option
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Misc,
            DefaultValue = null,
            HelpText = "specify on which clr the application should run",
            ShortName = "clr")]
        public string TargetClr { get; set; }

        /// <summary>
        /// Option
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Debug,
            IsSimple = true,
            DefaultValue = false,
            LongHelpText = "enable logging",
            HelpText = "enable logging",
            ShortName = "log",
            LongName = "logging"
           )]
        public bool Logging { get; set; }

        /// <summary>
        /// Driver
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Troubleshooting,
            DefaultValue = null,
            HelpText = "Driver for executing the monitored process")]
        public string Driver { get; set; }

#if DEBUG
        /// <summary>
        /// Option
        /// </summary>
        [Argument(
            ArgumentType.AtMostOnce,
            Category = Categories.Debug,
            DefaultValue = null,
            HelpText = "Filename for logs of the instrumentation framework (may only work in debug builds).",
            ShortName = "li")]
#endif
        public string LogInstrumentation { get; set; }

        /// <summary>
        /// Validates the input to the console
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            if (this.FileName == null)
            {
                System.Console.WriteLine("Error: Assembly to analyze not specified.");
                System.Console.WriteLine("For help, type 'mchess help'.");
                return false;
            }
            if (PCT_Arguments.Length != 0)
            {
                if (DeRandomized_PCT.Length != 0)
                {
                    System.Console.WriteLine("Error: Randomized and DeRandomzied can't run simultanously.");
                    return false;
                }
                if (PCT_Arguments.Length != 3)
                {
                    // todo
                }
            }

            if (DeRandomized_PCT.Length != 0)
            {
                if (PCT_Arguments.Length != 0)
                {
                    System.Console.WriteLine("Error: Randomized and DeRandomzied can't run simultanously.");
                    return false;
                }
            }
            if (vb_bound != -1)
            {
                if (vb_bound <= 0)
                {
                    System.Console.WriteLine("Error: /vb:N, N must be > 0.");
                    return false;
                }
            }

            if (this.MaxExecTime < 0)
            {
                System.Console.WriteLine("Error: /maxexectime:N, N must be >=0.");
                return false;
            }
            if (this.MaxChessTime < 0)
            {
                System.Console.WriteLine("Error: /maxchesstime:N, N must be >=0.");
                return false;
            }
            if (this.ProcessorCount <= 0)
            {
                System.Console.WriteLine("Error: /processorcount:N, N must be >=1.");
                return false;
            }
            if (this.MaxExecs < 0)
            {
                System.Console.WriteLine("Error: /maxexecs:N, N must be >=0.");
                return false;
            }
            if (this.MaxExecSteps < 1)
            {
                System.Console.WriteLine("Error: /maxexecsteps:N, N must be >=1.");
                return false;
            }
            if (this.MaxPreemptions < 0)
            {
                System.Console.WriteLine("Error: /maxpreemptions:N, N must be >=0.");
                return false;
            }
            if (!String.IsNullOrEmpty(this.EnumerateObservations) && !String.IsNullOrEmpty(this.CheckObservations))
            {
                System.Console.WriteLine("Error: may not specify both /enumerateobservations and /checkobservations");
                return false;
            }
            if (!String.IsNullOrEmpty(this.EnumerateObservations) && this.CheckAtomicity)
            {
                System.Console.WriteLine("Error: may not specify both /enumerateobservations and /checkatomicity");
                return false;
            }
            if (!String.IsNullOrEmpty(this.CheckObservations) && this.CheckAtomicity)
            {
                System.Console.WriteLine("Error: may not specify both /checkobservations and /checkatomicity");
                return false;
            }
            if (this.Continue || this.LoadSchedule || this.PrintTrace)
            {
                if (!File.Exists(this.ScheduleFile))
                {
                    System.Console.WriteLine("Error: /repro, /continue, or /trace specified, but file '{0}' is not present", this.ScheduleFile);
                    return false;
                }
            }

            // can't have a bounded search with DPOR (or at least not if you want to preserve completeness) (Katie)
            if (!this.Dpor)
            {
                this.Bounded = true;
            }
            else if (!this.Bounded)
            {
                System.Console.WriteLine("WARNING: Removing preemption bound due to DPOR.  Use /bound to override.");
            }
            
            if (this.PrioritizeMethods.Length > 0)
            {
                if (!this.BestFirst)
                {
                    System.Console.WriteLine("WARNING: Turning on best-first search with method prioritization.");
                    this.BestFirst = true;
                }
            }

            // TODO: more checking needed!!!
            
            try
            {
                var fileinfo = new FileInfo(this.FileName);
                var filename = fileinfo.FullName;
                Assembly.LoadFile(filename);
            }
            catch (FileLoadException)
            {
                System.Console.WriteLine("Error: Assembly cannot be loaded.");
                return false;
            }
            catch (FileNotFoundException)
            {
                System.Console.WriteLine("Error: Assembly cannot be found.");
                return false;
            }
            catch (BadImageFormatException)
            {
                System.Console.WriteLine("Error: Assembly image invalid.");
                return false;
            }
            catch (Exception)
            {
                System.Console.WriteLine("Error: Failure to load assembly");
                return false;
            }

            // Only one of /test or /testclass may be specified
            if (!String.IsNullOrEmpty(ChessTestClass) && ChessTestClass != DefaultChessTestClass && !String.IsNullOrEmpty(UnitTestName))
            {
                Console.WriteLine("Error: Only one of the following switches may be specified: /test or /testclass");
                return false;
            }

            return true;
        }

        #region ICommandLineDestination Members

        string ICommandLineDestination.DescribeCategory(string category)
        {
            switch (category)
            {
                case Categories.Main: return "a list of the major options";
                case Categories.Troubleshooting: return "diagnosing critical errors in user-code";
                case Categories.Misc: return "other options";
                case Categories.Debug: return "debugging CHESS-internal errors";
                case Categories.Monitoring: return "selective code instrumentation";
                case Categories.Chess: return "CHESS-specific options";
                default: return "?";
            }
        }

        string ICommandLineDestination.UserManualLocation
        {
            get { return "http://research.microsoft.com/chess/"; }
        }

        string ICommandLineDestination.ToolName
        {
            get { return "mchess.exe"; }
        }

        IIndexable<TypeEx> ICommandLineDestination.GetEnvironmentSettingTypes()
        {
            return Indexable.Array<TypeEx>(
                MetadataFromReflection.GetType(typeof(ExtendedReflectionEnvironmentSettings))
                );
        }
        #endregion
    }
}