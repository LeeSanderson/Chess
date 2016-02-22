using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.UnitTesting.Chess
{
    /// <summary>
    /// A data structure that represents all the options available on the command line for tweaking mchess.
    /// </summary>
    public class MChessOptions : ICloneable
    {

        // TODO: JAM - Make sure these properties are named the same as the mchess cmd line args
        #region Constructors

        public MChessOptions()
        {
            // Set the property defaults according to mchess
            // see: ManagedChess.Launcher\LauncherOptions.cs
        }

        /// <summary>
        /// Parses a new instance based on the given element.
        /// Any attributes that match actual options will be used.
        /// </summary>
        /// <param name="xopts">An element. May be null. Does not need to be an MChessOptions element.</param>
        public MChessOptions(XElement xopts)
        {
            if (xopts == null) return;  // Nothing more to do
            //if (xopts.Name != XNames.MChessOptions)
            //    throw new ArgumentException("The element name must be " + XNames.MChessOptions.LocalName);

            MaxChessTime = (int?)xopts.Attribute("MaxChessTime");
            MaxPreemptions = (int?)xopts.Attribute("MaxPreemptions");
            MaxExecs = (int?)xopts.Attribute("MaxExecs");
            MaxExecSteps = (int?)xopts.Attribute("MaxExecSteps");
            MaxExecTime = (int?)xopts.Attribute("MaxExecTime");
            ProcessorCount = (int?)xopts.Attribute("ProcessorCount");

            EnableRaceDetection = (bool?)xopts.Attribute("EnableRaceDetection");
            EnableAtomicityChecking = (bool?)xopts.Attribute("EnableAtomicityChecking");
            EnableDeterminismChecking = (bool?)xopts.Attribute("EnableDeterminismChecking");
            PreemptVolatiles = (bool?)xopts.Attribute("PreemptVolatiles");
            PreemptAllAccesses = (bool?)xopts.Attribute("PreemptAllAccesses");

            Break = xopts.Attribute("Break").ParseXmlEnum<MChessBreak>();
            EnableRepro = (bool?)xopts.Attribute("EnableRepro");
            EnableTracing = (bool?)xopts.Attribute("EnableTracing");
            TraceAllSchedules = (bool?)xopts.Attribute("TraceAllSchedules");
            TargetRace = (int?)xopts.Attribute("TargetRace");
            XSchedule = xopts.Element(XChessNames.Schedule);
            ScheduleFilePath = (string)xopts.Attribute("ScheduleFilePath");

            EnumerateObservations = (bool?)xopts.Attribute("EnumerateObservations");
            CheckObservations = (bool?)xopts.Attribute("CheckObservations");
            ObservationFile = (string)xopts.Attribute("ObservationFile");
            ObservationMode = xopts.Attribute("ObservationMode").ParseXmlEnum<MChessObservationMode>();

            ContinueFromLastSchedule = (bool?)xopts.Attribute("ContinueFromLastSchedule");

            IncludedAssemblies = xopts.Elements(XChessNames.InstrumentAssembly).SelectXValues().ToArray();

            FlipPreemptionSense = (bool?)xopts.Attribute("FlipPreemptionSense");
            SetDontPreempts(xopts.Elements(XChessNames.TogglePreemptability));

            ExtraCommandLineArgs = xopts.Elements(XChessNames.CmdLineArg).SelectXValues().ToArray();

            // debugging-like features
            EnableLogging = (bool?)xopts.Attribute("EnableLogging");
            PrintDiagnosticInformation = (bool?)xopts.Attribute("PrintDiagnosticInformation");

            ShowHBExecs = (bool?)xopts.Attribute("ShowHBExecs");
            NoPopups = (bool?)xopts.Attribute("NoPopups");
            NoTime = (bool?)xopts.Attribute("NoTime");

            // If we're parsing from xml, it's potentially user-code, so we should validate.
            Validate();
        }

        /// <summary>
        /// Sets the DontPreemptXXX properties by parsing the valid elements passed in.
        /// </summary>
        /// <param name="togglePreemptElements">The enumerable of <see cref="XChessNames.TogglePreemptability"/> elements to add.</param>
        public void SetDontPreempts(IEnumerable<XElement> togglePreemptElements)
        {
            var dontPreempts = togglePreemptElements
                .Select(xel => new {
                    TargetKind = xel.Attribute("targetKind").ParseXmlEnum<PreemptabilityTargetKind>().Value,
                    Value = (string)xel
                })
                .ToList();
            if (dontPreempts.Count != 0)
            {
                Func<PreemptabilityTargetKind, string[]> getTogglePreemptsOfKind = targetKind =>
                    (from dp in dontPreempts
                     where dp.TargetKind == targetKind
                     select dp.Value)
                     .ToArray();

                DontPreemptAssemblies = getTogglePreemptsOfKind(PreemptabilityTargetKind.Assembly);
                DontPreemptNamespaces = getTogglePreemptsOfKind(PreemptabilityTargetKind.Namespace);
                DontPreemptTypes = getTogglePreemptsOfKind(PreemptabilityTargetKind.Type);
                DontPreemptMethods = getTogglePreemptsOfKind(PreemptabilityTargetKind.Method);
            }
        }

        #endregion

        #region Properties

        #region ChessTestContextable properties

        /// <summary>
        /// Maximum time to allow MChess to execute a test before stopping.
        /// Matches the /mct and /maxchesstime command line arguments for mchess.
        /// </summary>
        public int? MaxChessTime { get; set; }

        /// <summary>
        /// Indicates the number of preemptions mchess should use.
        /// Matches the /mp and /maxpreemptions command line arguments for mchess.
        /// </summary>
        public int? MaxPreemptions { get; set; }

        /// <summary>
        /// Maximum number of executions/schedules to run.
        /// Matches the /me and /maxexecs command line arguments for mchess.
        /// </summary>
        public int? MaxExecs { get; set; }

        /// <summary>
        /// Maximum execution steps per test run.
        /// Matches the /mes and /maxexecsteps command line arguments for mchess.
        /// </summary>
        public int? MaxExecSteps { get; set; }

        /// <summary>
        /// Maximum seconds per test run (-1 = unlimited), i.e. per schedule.
        /// Matches the /met and /maxexectime command line arguments for mchess.
        /// </summary>
        public int? MaxExecTime { get; set; }

        ///// <summary>
        ///// Indicates the number of delays mchess should use.
        ///// Matches the /md and /maxdelays command line arguments for mchess.
        ///// </summary>
        //public int? MaxDelays { get; set; }

        /// <summary>
        /// The simulated processor count. Set to 0 for the full number of processors on the machine running the test.
        /// Matches the /pc and /processorcount command line arguments for mchess.
        /// </summary>
        public int? ProcessorCount { get; set; }

        /// <summary>
        /// Specifies whether to perform race detection.
        /// Matches the /dr and /detectraces command line arguments for mchess.
        /// </summary>
        public bool? EnableRaceDetection { get; set; }

        /// <summary>
        /// Specifies whether to perform atomicity checking.
        /// Matches the /ca and /checkatomicity command line arguments for mchess.
        /// </summary>
        public bool? EnableAtomicityChecking { get; set; }

        /// <summary>
        /// Specifies whether to turn on determinism checking.
        /// Matches the /cd and /checkdeterminism command line arguments for mchess.
        /// </summary>
        public bool? EnableDeterminismChecking { get; set; }

        /// <summary>
        /// Specifies whether to preempt on accesses to volatiles; by default, mchess does.
        /// Matches the /v and /volatile command line arguments for mchess.
        /// </summary>
        public bool? PreemptVolatiles { get; set; }

        /// <summary>
        /// Specifies whether to preempt on all data accesses.
        /// Matches the /pa and /preemptaccesses command line arguments for mchess.
        /// </summary>
        public bool? PreemptAllAccesses { get; set; }

        /// <summary>
        /// Specifies an array of arguments to pass to mchess.
        /// Each element in the array should be an individual argument.
        /// Whitespaces in an element will be escaped.
        /// </summary>
        public string[] ExtraCommandLineArgs { get; set; }

        #endregion



        /// <summary>
        /// Specifies whether to break as a specific event.
        /// Matches the /brk and /break command line arguments for mchess.
        /// </summary>
        public MChessBreak? Break { get; set; }

        /// <summary>
        /// Matches the /ls and /repro command line arguments for mchess.
        /// </summary>
        public bool? EnableRepro { get; set; }

        /// <summary>
        /// Enables tracing by mchess. To control whether to trace for the loaded schedule or for all schedules see the <see cref="TraceAllSchedules"/> property.
        /// Matches the /trace and /printtrace command line arguments for mchess.
        /// </summary>
        public bool? EnableTracing { get; set; }

        private bool? _TraceAllSchedules;
        /// <summary>
        /// When tracing is enabled (by setting <see cref="EnableTracing"/> to true) this indicates whether to trace
        /// all schedules or just the last schedule ran.
        /// Setting this to true automatically sets <see cref="EnableTracing"/> to true.
        /// Matches the /alltraces and /printalltraces command line arguments for mchess.
        /// </summary>
        public bool? TraceAllSchedules
        {
            get { return _TraceAllSchedules; }
            set
            {
                _TraceAllSchedules = value;

                // Automatically set EnableTracing if this feature is turned on
                if (value == true)
                    EnableTracing = true;
            }
        }

        /// <summary>
        /// ...??? not sure what this actually is. Once known, may need to change the name of the property.
        /// Matches the /targetrace command line arguments for mchess.
        /// </summary>
        public int? TargetRace { get; set; }

        /// <summary>
        /// The schedule to use.
        /// This doesn't get serialized to the cmd line, but is used to pass it down
        /// to the controller that would save this to the appropriate file.
        /// </summary>
        public XElement XSchedule { get; set; }
        /// <summary>
        /// Specifies the file path to the schedule file to use.
        /// Matches the /schedulefile command line arguments for mchess.
        /// </summary>
        public string ScheduleFilePath { get; set; }


        /// <summary>
        /// Matches the /enumerateobservations command line arguments for mchess.
        /// </summary>
        public bool? EnumerateObservations { get; set; }

        /// <summary>
        /// Matches the /checkobservations command line arguments for mchess.
        /// </summary>
        public bool? CheckObservations { get; set; }

        /// <summary>
        /// Matches the /observationmode command line arguments for mchess.
        /// </summary>
        public MChessObservationMode? ObservationMode { get; set; }

        /// <summary>
        /// Matches the filename value of the /enumerateobservations and /checkobservations command line arguments for mchess.
        /// </summary>
        public string ObservationFile { get; set; }

        /// <summary>
        /// Indicates whether to continue from the last schedule of the previous run.
        /// Matches the /continue command line argument for mchess.
        /// </summary>
        public bool? ContinueFromLastSchedule { get; set; }

        /// <summary>
        /// Matches the /ia command line arguments for mchess.
        /// </summary>
        public string[] IncludedAssemblies { get; set; }
        //public string[] ExcludedAssemblies { get; set; }
        //public string[] IncludedNamespaces { get; set; }
        //public string[] ExcludedNamespaces { get; set; }
        //public string[] IncludedTypes { get; set; }
        //public string[] ExcludedTypes { get; set; }

        /// <summary>
        /// Indicates whether to disable preemptions by default.
        /// Flips the meaning of the DontPreemptxxx properties.
        /// Matches the /flip and /flippreempt command line arguments for mchess.
        /// </summary>
        public bool? FlipPreemptionSense { get; set; }

        /// <summary>
        /// The assembly names excluded from preemptions.
        /// Matches the /dpa and /dontpreemptassembly command line arguments for mchess.
        /// </summary>
        public string[] DontPreemptAssemblies { get; set; }

        /// <summary>
        /// Indicates the namespaces excluded from preemptions.
        /// Matches the /dpn and /dontpreemptnamespace command line arguments for mchess.
        /// </summary>
        public string[] DontPreemptNamespaces { get; set; }

        /// <summary>
        /// Indicates the types excluded from preemptions.
        /// Matches the /dpt and /dontpreempttype command line arguments for mchess.
        /// </summary>
        public string[] DontPreemptTypes { get; set; }

        /// <summary>
        /// Indicates the methods excluded from preemptions.
        /// Matches the /dpm and /dontpreemptmethod command line arguments for mchess.
        /// </summary>
        public string[] DontPreemptMethods { get; set; }

        /// <summary>
        /// Matches the /logging command line arguments for mchess.
        /// </summary>
        public bool? EnableLogging { get; set; }

        /// <summary>
        /// Indicates whether to also print which types are not instrumented.
        /// Matches the /diagnose command line arguments for mchess.
        /// </summary>
        public bool? PrintDiagnosticInformation { get; set; }

        /// <summary>
        /// Indicates whether MChess should report the number of HB execution steps in the output.
        /// Matches the /p:show_hbexecs command line argument for mchess.
        /// </summary>
        public bool? ShowHBExecs { get; set; }

        /// <summary>
        /// Indicates whether MChess should not display any popups.
        /// This is useful for when running mchess from a cmd-line on a build server.
        /// Matches the /p:nopopups command line argument for mchess.
        /// </summary>
        public bool? NoPopups { get; set; }

        /// <summary>
        /// Indicates whether MChess should display performance times in output.
        /// Matches the /p:nopopups command line argument for mchess.
        /// </summary>
        public bool? NoTime { get; set; }

        #endregion

        #region Merging methods

        public void MergeWith(ChessTestContextAttribute attr)
        {
            if (attr._MaxPreemptions.HasValue)
                MaxPreemptions = attr.MaxPreemptions;
            if (attr._MaxExecs.HasValue)
                MaxExecs = attr.MaxExecs;
            if (attr._MaxExecSteps.HasValue)
                MaxExecSteps = attr.MaxExecSteps;
            if (attr._MaxExecTime.HasValue)
                MaxExecTime = attr.MaxExecTime;
            if (attr._ProcessorCount.HasValue)
                ProcessorCount = attr.ProcessorCount;

            if (attr._EnableRaceDetection.HasValue)
                EnableRaceDetection = attr.EnableRaceDetection;
            if (attr._EnableAtomicityChecking.HasValue)
                EnableAtomicityChecking = attr.EnableAtomicityChecking;
            if (attr._PreemptVolatiles.HasValue) // NOTE: The default is true, so we only specify if it's been set off.
                PreemptVolatiles = attr.PreemptVolatiles;
            if (attr._PreemptAllAccesses.HasValue)
                PreemptAllAccesses = attr.PreemptAllAccesses;

            if (attr.ExtraCommandLineArgs != null)
                ExtraCommandLineArgs = ConcatStringArrays(ExtraCommandLineArgs, attr.ExtraCommandLineArgs);
        }

        private static string[] MergeStringArrays(string[] baseAry, string[] otherAry)
        {
            if (otherAry == null || otherAry.Length == 0)
                return baseAry;

            if (baseAry == null || baseAry.Length == 0)
                return (string[])otherAry.Clone();
            else
                return baseAry
                    .Union(otherAry)
                    .ToArray();
        }

        private static string[] ConcatStringArrays(string[] baseAry, string[] otherAry)
        {
            if (otherAry == null || otherAry.Length == 0)
                return baseAry;

            if (baseAry == null || baseAry.Length == 0)
                return (string[])otherAry.Clone();
            else
                return baseAry
                    .Concat(otherAry)
                    .ToArray();
        }

        /// <summary>Merges in the specified options from other into this instance.</summary>
        /// <param name="other">may be null.</param>
        public void MergeWith(MChessOptions other)
        {
            if (other == null) return;

            // Contextable
            if (other.MaxChessTime.HasValue) this.MaxChessTime = other.MaxChessTime;
            if (other.MaxPreemptions.HasValue) this.MaxPreemptions = other.MaxPreemptions;
            if (other.MaxExecs.HasValue) this.MaxExecs = other.MaxExecs;
            if (other.MaxExecSteps.HasValue) this.MaxExecSteps = other.MaxExecSteps;
            if (other.MaxExecTime.HasValue) this.MaxExecTime = other.MaxExecTime;
            if (other.ProcessorCount.HasValue) this.ProcessorCount = other.ProcessorCount;
            if (other.EnableRaceDetection.HasValue) this.EnableRaceDetection = other.EnableRaceDetection;
            if (other.EnableAtomicityChecking.HasValue) this.EnableAtomicityChecking = other.EnableAtomicityChecking;
            if (other.EnableDeterminismChecking.HasValue) this.EnableDeterminismChecking = other.EnableDeterminismChecking;
            if (other.PreemptVolatiles.HasValue) this.PreemptVolatiles = other.PreemptVolatiles;
            if (other.PreemptAllAccesses.HasValue) this.PreemptAllAccesses = other.PreemptAllAccesses;

            // All others
            if (other.Break.HasValue) this.Break = other.Break;
            if (other.EnableLogging.HasValue) this.EnableLogging = other.EnableLogging;
            if (other.EnableRepro.HasValue) this.EnableRepro = other.EnableRepro;
            if (other.EnableTracing.HasValue) this.EnableTracing = other.EnableTracing;
            if (other.TraceAllSchedules.HasValue) this.TraceAllSchedules = other.TraceAllSchedules;
            if (other.TargetRace.HasValue) this.TargetRace = other.TargetRace;
            if (other.XSchedule != null) this.XSchedule = other.XSchedule;
            if (!String.IsNullOrEmpty(other.ScheduleFilePath)) this.ScheduleFilePath = other.ScheduleFilePath;

            if (other.EnumerateObservations.HasValue) this.EnumerateObservations = other.EnumerateObservations;
            if (other.CheckObservations.HasValue) this.CheckObservations = other.CheckObservations;
            if (other.ObservationMode.HasValue) this.ObservationMode = other.ObservationMode;
            if (other.ObservationFile != null) this.ObservationFile = other.ObservationFile;

            this.ContinueFromLastSchedule = other.ContinueFromLastSchedule;

            // Merge assemblies
            if (other.IncludedAssemblies != null)
                this.IncludedAssemblies = MergeStringArrays(this.IncludedAssemblies, other.IncludedAssemblies);

            // Merge preemption settings
            if (other.FlipPreemptionSense.HasValue) this.FlipPreemptionSense = other.FlipPreemptionSense;
            if (other.DontPreemptAssemblies != null)
                this.DontPreemptAssemblies = MergeStringArrays(this.DontPreemptAssemblies, other.DontPreemptAssemblies);
            if (other.DontPreemptNamespaces != null)
                this.DontPreemptNamespaces = MergeStringArrays(this.DontPreemptNamespaces, other.DontPreemptNamespaces);
            if (other.DontPreemptTypes != null)
                this.DontPreemptTypes = MergeStringArrays(this.DontPreemptTypes, other.DontPreemptTypes);
            if (other.DontPreemptMethods != null)
                this.DontPreemptMethods = MergeStringArrays(this.DontPreemptMethods, other.DontPreemptMethods);

            // Merge extra command line args
            if (other.ExtraCommandLineArgs != null)
                this.ExtraCommandLineArgs = ConcatStringArrays(this.ExtraCommandLineArgs, other.ExtraCommandLineArgs);

            if (other.EnableLogging.HasValue) this.EnableLogging = other.EnableLogging;
            if (other.PrintDiagnosticInformation.HasValue) this.PrintDiagnosticInformation = other.PrintDiagnosticInformation;

            if (other.ShowHBExecs.HasValue) this.ShowHBExecs = other.ShowHBExecs;
            if (other.NoPopups.HasValue) this.NoPopups = other.NoPopups;
            if (other.NoTime.HasValue) this.NoTime = other.NoTime;
        }

        #endregion

        /// <summary>Creates a copy of all the options set on this instance.</summary>
        public MChessOptions Clone()
        {
            MChessOptions copy = new MChessOptions() {
                // ChessTestContextAttributeable
                MaxChessTime = this.MaxChessTime,
                MaxPreemptions = this.MaxPreemptions,
                MaxExecs = this.MaxExecs,
                MaxExecSteps = this.MaxExecSteps,
                MaxExecTime = this.MaxExecTime,
                ProcessorCount = this.ProcessorCount,

                // Non-context properties
                EnableRaceDetection = this.EnableRaceDetection,
                EnableAtomicityChecking = this.EnableAtomicityChecking,
                EnableDeterminismChecking = this.EnableDeterminismChecking,
                PreemptVolatiles = this.PreemptVolatiles,
                PreemptAllAccesses = this.PreemptAllAccesses,

                Break = this.Break,
                EnableRepro = this.EnableRepro,
                EnableTracing = this.EnableTracing,
                TraceAllSchedules = this.TraceAllSchedules,
                TargetRace = this.TargetRace,
                XSchedule = this.XSchedule,
                ScheduleFilePath = this.ScheduleFilePath,

                EnumerateObservations = this.EnumerateObservations,
                CheckObservations = this.CheckObservations,
                ObservationMode = this.ObservationMode,
                ObservationFile = this.ObservationFile,

                ContinueFromLastSchedule = this.ContinueFromLastSchedule,

                IncludedAssemblies = this.IncludedAssemblies == null ? null : (string[])this.IncludedAssemblies.Clone(),

                FlipPreemptionSense = this.FlipPreemptionSense,
                DontPreemptAssemblies = this.DontPreemptAssemblies == null ? null : (string[])this.DontPreemptAssemblies.Clone(),
                DontPreemptNamespaces = this.DontPreemptNamespaces == null ? null : (string[])this.DontPreemptNamespaces.Clone(),
                DontPreemptTypes = this.DontPreemptTypes == null ? null : (string[])this.DontPreemptTypes.Clone(),
                DontPreemptMethods = this.DontPreemptMethods == null ? null : (string[])this.DontPreemptMethods.Clone(),

                ExtraCommandLineArgs = this.ExtraCommandLineArgs == null ? null : (string[])this.ExtraCommandLineArgs.Clone(),

                EnableLogging = this.EnableLogging,
                PrintDiagnosticInformation = this.PrintDiagnosticInformation,

                ShowHBExecs = this.ShowHBExecs,
                NoPopups = this.NoPopups,
                NoTime = this.NoTime,
            };

            return copy;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        /// <summary>Validates the configuration of this instance.</summary>
        public void Validate()
        {
            // TODO: Implement same logic that's in the cpp and ManagedChess code
            System.Diagnostics.Debug.Assert(!(EnumerateObservations == true && CheckObservations == true), "Invalid configuration: Only one of EnumerateObservations or CheckObservations may be enabled.");
            if (EnumerateObservations == true || CheckObservations == true)
                System.Diagnostics.Debug.Assert(!String.IsNullOrWhiteSpace(ObservationFile), "If EnumerateObservations or CheckObservations are enabled then an ObservationFile must be specified.");
        }

        public IEnumerable<string> GetCommandLineArgs()
        {
            Validate();

            if (MaxChessTime.HasValue)
                yield return "/maxchesstime:" + MaxChessTime.ToString();

            if (MaxPreemptions.HasValue)
                yield return "/maxpreemptions:" + MaxPreemptions.ToString();

            if (MaxExecs.HasValue)
                yield return "/maxexecs:" + MaxExecs.ToString();

            if (MaxExecSteps.HasValue)
                yield return "/maxexecsteps:" + MaxExecSteps.ToString();

            if (MaxExecTime.HasValue)
                yield return "/maxexectime:" + MaxExecTime.ToString();

            if (ProcessorCount.HasValue)
                yield return "/processorcount:" + ProcessorCount.ToString();


            //
            if (EnableRaceDetection == true) yield return "/detectraces";
            if (EnableAtomicityChecking == true) yield return "/checkatomicity";
            if (EnableDeterminismChecking == true) yield return "/checkdeterminism";
            if (PreemptVolatiles.HasValue)
            {
                if (PreemptVolatiles.Value)
                    yield return "/volatile";
                else
                    yield return "/volatile-";
            }
            if (PreemptAllAccesses == true) yield return "/preemptaccesses";


            if (Break.HasValue && Break.Value != MChessBreak.NoBreak) yield return "/break:" + Break.Value.ToCommandLineOptionValue();
            if (EnableRepro == true) yield return "/repro";
            if (EnableTracing == true)
            {
                if (TraceAllSchedules == true)
                    yield return "/alltraces";
                else
                    yield return "/trace";
            }
            if (TargetRace.HasValue) yield return "/targetrace:" + TargetRace.Value;
            //if (XSchedule != null) ;  // Doesn't get to the cmd line
            if (!String.IsNullOrEmpty(ScheduleFilePath)) yield return "/schedulefile:" + ScheduleFilePath;

            if ((EnumerateObservations == true || CheckObservations == true) && !String.IsNullOrWhiteSpace(ObservationFile))
            {
                if (EnumerateObservations == true)
                    yield return "/enumerateobservations:" + ObservationFile;
                else
                    yield return "/checkobservations:" + ObservationFile;

                if (ObservationMode.HasValue)
                    yield return "/observationmode:" + ObservationMode.Value.ToCommandLineOptionValue();
            }

            if (ContinueFromLastSchedule == true) yield return "/continue";

            if (IncludedAssemblies != null && IncludedAssemblies.Length != 0)
            {
                foreach (var val in IncludedAssemblies)
                    yield return "/ia:" + val;
            }

            if (FlipPreemptionSense == true) yield return "/flippreempt";
            if (DontPreemptAssemblies != null)
            {
                foreach (var val in DontPreemptAssemblies)
                    yield return "/dpa:" + val;
            }
            if (DontPreemptNamespaces != null)
            {
                foreach (var val in DontPreemptNamespaces)
                    yield return "/dpn:" + val;
            }
            if (DontPreemptTypes != null)
            {
                foreach (var val in DontPreemptTypes)
                    yield return "/dpt:" + val;
            }
            if (DontPreemptMethods != null)
            {
                foreach (var val in DontPreemptMethods)
                    yield return "/dpm:" + val;
            }

            if (EnableLogging == true) yield return "/logging";
            if (PrintDiagnosticInformation == true) yield return "/diagnose";

            if (ShowHBExecs.HasValue) yield return "/p:show_hbexecs=" + ShowHBExecs;
            if (NoPopups.HasValue) yield return "/p:nopopups=" + NoPopups;
            if (NoTime.HasValue) yield return "/p:notime=" + NoTime;

            if (ExtraCommandLineArgs != null && ExtraCommandLineArgs.Length != 0)
            {
                foreach (var arg in ExtraCommandLineArgs)
                    yield return arg;
            }
        }

        public XElement ToXElement()
        {
            // Only write the values that have been specified
            XElement xopts = new XElement(XChessNames.MChessOptions);

            //** Contextable opts
            if (MaxChessTime.HasValue) xopts.Add(new XAttribute("MaxChessTime", MaxChessTime.Value));
            if (MaxPreemptions.HasValue) xopts.Add(new XAttribute("MaxPreemptions", MaxPreemptions.Value));
            if (MaxExecs.HasValue) xopts.Add(new XAttribute("MaxExecs", MaxExecs.Value));
            if (MaxExecSteps.HasValue) xopts.Add(new XAttribute("MaxExecSteps", MaxExecSteps.Value));
            if (MaxExecTime.HasValue) xopts.Add(new XAttribute("MaxExecTime", MaxExecTime.Value));
            if (ProcessorCount.HasValue) xopts.Add(new XAttribute("ProcessorCount", ProcessorCount.Value));

            if (EnableRaceDetection.HasValue) xopts.Add(new XAttribute("EnableRaceDetection", EnableRaceDetection.Value));
            if (EnableAtomicityChecking.HasValue) xopts.Add(new XAttribute("EnableAtomicityChecking", EnableAtomicityChecking.Value));
            if (EnableDeterminismChecking.HasValue) xopts.Add(new XAttribute("EnableDeterminismChecking", EnableDeterminismChecking.Value));
            if (PreemptVolatiles.HasValue) xopts.Add(new XAttribute("PreemptVolatiles", PreemptVolatiles.Value));
            if (PreemptAllAccesses.HasValue) xopts.Add(new XAttribute("PreemptAllAccesses", PreemptAllAccesses.Value));

            if (ShowHBExecs.HasValue) xopts.Add(new XAttribute("ShowHBExecs", ShowHBExecs.Value));
            if (NoPopups.HasValue) xopts.Add(new XAttribute("NoPopups", NoPopups.Value));
            if (NoTime.HasValue) xopts.Add(new XAttribute("NoTime", NoTime.Value));


            //** Non contextable
            if (Break.HasValue && Break.Value != MChessBreak.NoBreak) xopts.Add(new XAttribute("Break", Break.Value));
            if (EnableRepro.HasValue) xopts.Add(new XAttribute("EnableRepro", EnableRepro.Value));
            if (EnableTracing.HasValue)
            {
                xopts.Add(new XAttribute("EnableTracing", EnableTracing.Value));
                if (TraceAllSchedules.HasValue)
                    xopts.Add(new XAttribute("TraceAllSchedules", TraceAllSchedules.Value));
            }
            if (TargetRace.HasValue) xopts.Add(new XAttribute("TargetRace", TargetRace));    // TODO: See how this is serialized
            if (XSchedule != null) xopts.Add(new XElement(XSchedule));
            if (!String.IsNullOrEmpty(ScheduleFilePath)) xopts.Add(new XAttribute("ScheduleFilePath", ScheduleFilePath));

            if (EnumerateObservations.HasValue) xopts.Add(new XAttribute("EnumerateObservations", EnumerateObservations.Value));
            if (CheckObservations.HasValue) xopts.Add(new XAttribute("CheckObservations", CheckObservations.Value));
            if (!String.IsNullOrWhiteSpace(ObservationFile)) xopts.Add(new XAttribute("ObservationFile", ObservationFile));
            if (ObservationMode.HasValue) xopts.Add(new XAttribute("ObservationMode", ObservationMode.Value));

            if (ContinueFromLastSchedule.HasValue) xopts.Add(new XAttribute("ContinueFromLastSchedule", ContinueFromLastSchedule.Value));

            if (EnableLogging.HasValue) xopts.Add(new XAttribute("EnableLogging", EnableLogging.Value));
            if (PrintDiagnosticInformation.HasValue) xopts.Add(new XAttribute("PrintDiagnosticInformation", PrintDiagnosticInformation.Value));

            if (IncludedAssemblies != null)
            {
                foreach (var assy in IncludedAssemblies)
                    xopts.Add(new XElement(XChessNames.InstrumentAssembly, assy));
            }

            if (FlipPreemptionSense.HasValue) xopts.Add(new XAttribute("FlipPreemptionSense", FlipPreemptionSense.Value));
            if (DontPreemptAssemblies != null)
            {
                foreach (var val in DontPreemptAssemblies)
                    xopts.Add(new XElement(XChessNames.TogglePreemptability
                        , new XAttribute("targetKind", PreemptabilityTargetKind.Assembly)
                        , val));
            }
            if (DontPreemptNamespaces != null)
            {
                foreach (var val in DontPreemptNamespaces)
                    xopts.Add(new XElement(XChessNames.TogglePreemptability
                        , new XAttribute("targetKind", PreemptabilityTargetKind.Namespace)
                        , val));
            }
            if (DontPreemptTypes != null)
            {
                foreach (var val in DontPreemptTypes)
                    xopts.Add(new XElement(XChessNames.TogglePreemptability
                        , new XAttribute("targetKind", PreemptabilityTargetKind.Type)
                        , val));
            }
            if (DontPreemptMethods != null)
            {
                foreach (var val in DontPreemptMethods)
                    xopts.Add(new XElement(XChessNames.TogglePreemptability
                        , new XAttribute("targetKind", PreemptabilityTargetKind.Method)
                        , val));
            }

            if (ExtraCommandLineArgs != null)
            {
                foreach (var arg in ExtraCommandLineArgs)
                    if (!String.IsNullOrWhiteSpace(arg))
                        xopts.Add(new XElement(XChessNames.CmdLineArg, arg));
            }

            return xopts;
        }

    }
}
