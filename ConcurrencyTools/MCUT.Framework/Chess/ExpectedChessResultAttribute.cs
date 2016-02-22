using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.UnitTesting.Chess
{
    /// <summary>
    /// Indicates the expected mchess results for this test.
    /// NOTE: This only verifies the chess results xml, not the overall test result.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class ExpectedChessResultAttribute : Attribute
    {

        #region Constructors

        public ExpectedChessResultAttribute()
        {
        }

        public ExpectedChessResultAttribute(string key)
        {
            Key = key;
        }

        public ExpectedChessResultAttribute(string key, ChessExitCode exitCode)
        {
            Key = key;
            ExitCode = exitCode;
        }

        public ExpectedChessResultAttribute(ChessExitCode exitCode)
        {
            ExitCode = exitCode;
        }

        #endregion

        /// <summary>The programmatic key used to identify this result from a multi-test case scenario.</summary>
        public string Key { get; private set; }

        internal ChessExitCode? _exitCode;
        /// <summary>
        /// The exit code expected to be returned from mchess.
        /// If set to any value, the IsExitCodeSet property is set to true, thus
        /// indicating to the testing framework to assert the exit code.
        /// </summary>
        public ChessExitCode ExitCode
        {
            get { return _exitCode.Value; }
            set { _exitCode = value; }
        }

        /// <summary>
        /// The expected number of schedule ran.
        /// A default value of 0 indicates this property is not asserted.
        /// </summary>
        public int SchedulesRan { get; set; }

        /// <summary>
        /// [NotImplemented]
        /// The expected number of threads in the last schedule ran.
        /// A default value of 0 indicates this property is not asserted.
        /// </summary>
        public int LastThreadCount { get; set; }

        /// <summary>
        /// [NotImplemented]
        /// The expected number of execution steps in the last schedule ran.
        /// A default value of 0 indicates this property is not asserted.
        /// </summary>
        public int LastExecSteps { get; set; }

        /// <summary>
        /// [NotImplemented]
        /// The expected number of happens-before execution steps in the last schedule ran.
        /// A default value of 0 indicates this property is not asserted.
        /// </summary>
        public int LastHBExecSteps { get; set; }

    }
}
