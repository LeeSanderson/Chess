using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Microsoft.Concurrency.TestTools.UnitTesting.Chess
{
    /// <summary>
    /// Represents non-default context settings for test cases.
    /// Each instance will create a new test context.
    /// When applied to a class, each method in the class will have this context.
    /// For documentation on each property, see the <see cref="MChessOptions"/> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class ChessTestContextAttribute : Attribute
    {

        #region Constructors

        public ChessTestContextAttribute()
        {
        }

        public ChessTestContextAttribute(string name)
            : this()
        {
            Name = name;
        }

        #endregion

        /// <summary>The display name to use for this context instance.</summary>
        public string Name { get; set; }

        /// <summary>The key of the expected result to use.</summary>
        public string ExpectedResultKey { get; set; }

        /// <summary>
        /// When set, indicates the key of the explicit chess test result to expect.
        /// This is mainly used when a test may have different results based on the context.
        /// </summary>
        public string ExpectedChessResultKey { get; set; }

        /// <summary>
        /// Gets the script to execute just before running the command that runs the test.
        /// </summary>
        public string PreRunScript { get; set; }

        #region MChessOptions

        internal int? _MaxPreemptions;
        /// <summary>
        /// A value of -1 indicates unlimited preemptions.
        /// A value of 0 indicates no preemptions.
        /// Don't set this property if you want to use the default number of preemptions used by mchess.
        /// </summary>
        public int MaxPreemptions
        {
            get { return _MaxPreemptions ?? 0; }
            set { _MaxPreemptions = value; }
        }

        internal int? _MaxExecs;
        /// <summary>
        /// Indicates the maximum number of schedules to run.
        /// A value of -1 indicates unlimited schedules.
        /// </summary>
        public int MaxExecs
        {
            get { return _MaxExecs ?? 0; }
            set { _MaxExecs = value; }
        }

        internal int? _MaxExecSteps;
        /// <summary>A value of -1 indicates unlimited execution time.</summary>
        public int MaxExecSteps
        {
            get { return _MaxExecSteps ?? 0; }
            set { _MaxExecSteps = value; }
        }

        internal int? _MaxExecTime;
        /// <summary>A value of -1 indicates unlimited execution time.</summary>
        public int MaxExecTime
        {
            get { return _MaxExecTime ?? 0; }
            set { _MaxExecTime = value; }
        }

        internal int? _ProcessorCount;
        /// <summary>A number, greater than 0. If not set, the max available is used.</summary>
        public int ProcessorCount
        {
            get { return _ProcessorCount ?? 0; }
            set { _ProcessorCount = value; }
        }

        internal bool? _EnableRaceDetection;
        /// <summary>
        /// Specifies whether to perform race detection.
        /// Matches the /dr and /detectraces command line arguments for mchess.
        /// </summary>
        public bool EnableRaceDetection
        {
            get { return _EnableRaceDetection ?? false; }
            set { _EnableRaceDetection = value; }
        }

        internal bool? _EnableAtomicityChecking;
        /// <summary>
        /// Specifies whether to perform atomicity checking.
        /// Matches the /ca and /checkatomicity command line arguments for mchess.
        /// </summary>
        public bool EnableAtomicityChecking
        {
            get { return _EnableAtomicityChecking ?? false; }
            set { _EnableAtomicityChecking = value; }
        }

        internal bool? _PreemptVolatiles;
        /// <summary>
        /// Specifies whether to preempt on accesses to volatiles; Default is true.
        /// Matches the /v and /volatile command line arguments for mchess.
        /// </summary>
        public bool PreemptVolatiles
        {
            get { return _PreemptVolatiles ?? false; }
            set { _PreemptVolatiles = value; }
        }

        internal bool? _PreemptAllAccesses;
        /// <summary>
        /// Specifies whether to preempt on all data accesses.
        /// Matches the /pa and /preemptaccesses command line arguments for mchess.
        /// </summary>
        public bool PreemptAllAccesses
        {
            get { return _PreemptAllAccesses ?? false; }
            set { _PreemptAllAccesses = value; }
        }

        /// <summary>
        /// Specifies extra command line arguments to pass to mchess.
        /// Each arg should be a separate element in this array as spaces will cause the
        /// argument to be surrounded by quotes on the actual command line.
        /// </summary>
        public string[] ExtraCommandLineArgs { get; set; }

        #endregion

    }
}
