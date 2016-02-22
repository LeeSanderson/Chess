using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.UnitTesting.Chess
{
    /// <summary>
    /// The kind of the target being specified by a <see cref="ChessTogglePreemptabilityAttribute"/>.
    /// </summary>
    public enum PreemptabilityTargetKind
    {
        Assembly,
        Namespace,
        Type,
        Method,
    }

    /// <summary>
    /// Indicates when to break.
    /// </summary>
    /// <remarks>
    /// When changing, be sure to update the testlist.xsd.
    /// </remarks>
    [Flags]
    public enum MChessBreak : byte
    {
        NoBreak = 0,
        Start = 0x0001,
        ContextSwitch = 0x0002,
        AfterContextSwitch = 0x0004,
        Preemption = 0x0008,
        AfterPreemption = 0x0010,
        Deadlock = 0x0020,
        Timeout = 0x0040,
        TaskResume = 0x0080,
    }

    public enum MChessObservationMode
    {
        SerialInterleavings,
        CoarseInterleavings,
        AllInterleavings,

        Linearizability,
        LinearizabilityNotBlock,
        SequentialConsistency,
        SequentialConsistencyNotBlock,
    }

    /// <summary>
    /// An action available to perform on the results of an mchess results.xml file
    /// or for a specific result.
    /// </summary>
    public enum MChessResultActionType
    {
        View,
        Repeat,
        Continue,
        Repro,
        ReproLastSchedule,
    }

    /// <summary>One of the possible result types exposed by mchess.</summary>
    public enum MChessResultType
    {
        Notification,
        Warning,
        Error,
        Race,
    }

    /// <summary>
    /// Represents the enumeration of all possible exit codes possible by Chess or MChess.
    /// </summary>
    /// <remarks>
    /// This enumeration should match up with the exit codes defined in Chess\ChessApi.h
    /// of the form CHESS_EXIT_*. Also, when changing, be sure to update the ChessSchema.xsd.
    /// </remarks>
    public enum ChessExitCode : int
    {
        Success = 0,

        //CHESS_START = -0, // This is an invalid integer
        TestFailure = -1,
        ChessDeadlock = -2,
        ChessLivelock = -3,
        ChessTimeout = -4,
        ChessNonDet = -5,
        ChessInvalidTest = -6,
        ChessRace = -7,
        ChessIncompleteInterleavingCoverage = -8,
        ChessInvalidObservation = -9,
        ChessFailure = -100,

        // Non-chess codes (i.e. doesn't occur in the win32 version of chess)
        ChessAtomicityViolation = -10,

        /// <summary>Indicates a Concurrency Unit Test failure due to an unexpected exception.</summary>
        UnitTestException = -201,
        /// <summary>Indicates a Concurrency Unit Test failure due to an unmet assert.</summary>
        UnitTestAssertFailure = -202,
        /// <summary>Indicates a Concurrency Unit Test failure due to an inconclusive assert.</summary>
        UnitTestAssertInconclusive = -203,

        /// <summary>Used when the exit code is unknown (either from the process or results file).</summary>
        Unknown = -999999,
    }
}