/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.ManagedChess.EREngine
{
    [Serializable]
    internal class ChessResult
    {
        public ChessExitCode Code { get; private set; }
        public Exception Exception { get; private set; }

        public ChessResult(ChessExitCode code, Exception exception)
        {
            this.Code = code;
            this.Exception = exception;
        }

        public ChessResult(ChessExitCode code)
        {
            this.Code = code;
            this.Exception = null;
        }

        public bool Inconclusive
        {
            get
            {
                return this.Code == ChessExitCode.ChessNonDet
                    || this.Code == ChessExitCode.ChessIncompleteInterleavingCoverage;
            }
        }

        public bool InvalidTest { get { return this.Code == ChessExitCode.ChessInvalidTest; } }

        public bool Timeout { get { return this.Code == ChessExitCode.ChessTimeout; } }

        public bool ChessInternalError { get { return (this.Code == ChessExitCode.ChessFailure); } }

        public bool TestFailed { get { return this.Code == ChessExitCode.TestFailure; } }

        public bool ChessFoundFailure
        {
            get
            {
                return this.Code == ChessExitCode.ChessDeadlock
                    || this.Code == ChessExitCode.ChessLivelock
                    || this.Code == ChessExitCode.ChessRace
                    || this.Code == ChessExitCode.ChessInvalidObservation;
            }
        }

        public string CodeToString(ChessExitCode c)
        {
            switch (c)
            {
                case ChessExitCode.Success: return "Success";
                case ChessExitCode.TestFailure: return "TestFailure";
                case ChessExitCode.ChessFailure: return "ChessFailure";
                case ChessExitCode.ChessDeadlock: return "Deadlock";
                case ChessExitCode.ChessLivelock: return "Livelock";
                case ChessExitCode.ChessTimeout: return "Timeout";
                case ChessExitCode.ChessNonDet: return "NonDeterminism";
                case ChessExitCode.ChessInvalidTest: return "InvalidTest";
                case ChessExitCode.ChessRace: return "Race";
                case ChessExitCode.ChessInvalidObservation: return "InvalidObservation";
                case ChessExitCode.ChessIncompleteInterleavingCoverage: return "IncompleteInterleavingCoverage";
                default: throw new ArgumentException();
            }
        }

        public ChessExitCode StringToCode(string s)
        {
            switch (s.ToLower())
            {
                case "success": return ChessExitCode.Success;
                case "testfailure": return ChessExitCode.TestFailure;
                case "chessfailure": return ChessExitCode.ChessFailure;
                case "deadlock": return ChessExitCode.ChessDeadlock;
                case "livelock": return ChessExitCode.ChessLivelock;
                case "timeout": return ChessExitCode.ChessTimeout;
                case "nondeterminism": return ChessExitCode.ChessNonDet;
                case "invalidtest": return ChessExitCode.ChessInvalidTest;
                case "race": return ChessExitCode.ChessRace;
                case "invalidobservation": return ChessExitCode.ChessInvalidObservation;
                case "incompleteinterleavingcoverage": return ChessExitCode.ChessIncompleteInterleavingCoverage;
                default: throw new ArgumentException();
            }
        }

        public override string ToString()
        {
            switch (this.Code)
            {
                case ChessExitCode.Success: return "All tests run by CHESS passed";
                case ChessExitCode.TestFailure:
                    if (this.Exception == null)
                        return "Test failed.";
                    else
                        return @"Test raised unhandled exception:
" + (Exception == null ? "Unknown." : Exception.Message) + @"
" + (Exception == null ? "" : Exception.StackTrace);
                case ChessExitCode.ChessFailure: return "CHESS internal error";
                case ChessExitCode.ChessDeadlock: return "CHESS detected deadlock";
                case ChessExitCode.ChessLivelock: return "CHESS detected livelock";
                case ChessExitCode.ChessTimeout: return "CHESS time limit on test exceeded";
                case ChessExitCode.ChessNonDet: return "CHESS internal error (nondeterminism)";
                case ChessExitCode.ChessInvalidTest: return "CHESS detected that test did not wait for all threads to complete";
                case ChessExitCode.ChessRace: return "CHESS detected data race";
                case ChessExitCode.ChessInvalidObservation: return "CHESS detected invalid observation";
                case ChessExitCode.ChessIncompleteInterleavingCoverage: return "CHESS did not explore all interleavings (make sure test resets initial state)";
                default: return "Illegal CHESS return code";
            }
        }
    }
}
