/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.UnitTesting.Xml
{
    /// <summary>
    /// The XNames used to describe test results.
    /// </summary>
    public static class XTestResultNames
    {

        /// <summary>The container element for the overall result of a single test run.</summary>
        public static readonly XName TestResult = XNames.UnitTestingNS + "testResult";
        public static readonly XName ResultMessage = XNames.UnitTestingNS + "message";

        // attributes
        // NOTE: No need to actually specify a namespace for an attribute
        /// <summary>The <see cref="TestResultType"/> describing the final result of the test.</summary>
        public static readonly XName ATestResultType = "type";
        /// <summary>The integer exit code.</summary>
        public static readonly XName AExitCode = "exitCode";
        /// <summary>The <see cref="UnitTesting.Chess.ChessExitCode"/> describing the exit code returned from Chess.</summary>
        public static readonly XName AChessExitCode = "chessExitCode";

    }
}
