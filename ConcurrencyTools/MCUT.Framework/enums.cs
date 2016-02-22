using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>
    /// Represents the enumeration of all possible exit codes that can be returned by mcut.
    /// Not all test runners will emit all of these results.
    /// </summary>
    /// <remarks>
    /// Note: This enum should match the TestResultTypeXEnum simple type in the schema file.
    /// </remarks>
    public enum TestResultType
    {
        Passed = 0,
        /// <summary>Indicates a general unit test error occured. e.g. non-zero exit code.</summary>
        Error,

        /// <summary>Indicates the unit test threw an exception.</summary>
        Exception,
        /// <summary>Indicates a unit test assertion failed.</summary>
        AssertFailure,
        /// <summary>Indicates a unit test assertion was inconclusive.</summary>
        Inconclusive,

        /// <summary>Indicates an assertion on the result of a test failed.</summary>
        ResultAssertFailure,
        /// <summary>Indicates an assertion on the result of a test was inconclusive.</summary>
        ResultInconclusive,

        /// <summary>Indicates an assertion that applies to running a test in regression mode has failed.</summary>
        RegressionAssertFailure,

        /// <summary>Indicates that a race was found in the unit test.</summary>
        DataRace,
        /// <summary>Indicates that the unit test deadlocked.</summary>
        Deadlock,
        /// <summary>Indicates that the unit test deadlocked.</summary>
        Livelock,
    }

    /// <summary>
    /// Defines the granularity for which an Observation Generator should record at.
    /// </summary>
    public enum ObservationGranularity
    {
        /// <summary>Serial interleaving.</summary>
        Serial,

        /// <summary>Coarse interleaving.</summary>
        Coarse,

        /// <summary>All interleaving.</summary>
        All,
    }

    /// <summary>
    /// The checking mode to run an Observation Test under.
    /// </summary>
    public enum ObservationTestCheckingMode
    {
        Linearizability,
        LinearizabilityNotBlock,
        SequentialConsistency,
        SequentialConsistencyNotBlock,
    }

}