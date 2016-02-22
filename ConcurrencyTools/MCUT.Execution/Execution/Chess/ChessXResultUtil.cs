using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.Execution.Chess
{
    /// <summary>
    /// Utility class for processing the results.xml file returned by chess.
    /// </summary>
    public static class ChessXResultUtil
    {

        /// <summary>
        /// Converts a chess results.xml file into the xml element representing a MCUT test result.
        /// </summary>
        /// <param name="run"></param>
        /// <param name="xtaskResults"></param>
        /// <returns></returns>
        public static XElement ChessXResultsToXTestResult(XElement xtaskResults, int? processExitCode, int? taskID)
        {
            // default to processing the chess results
            if (xtaskResults == null || xtaskResults.Name != XChessNames.Results)
                return null;

            // Rename the chess results element
            xtaskResults.Name = XChessNames.ChessResults;

            // For Chess results, we really only care about the results and actions available for each result
            var results = (from xr in xtaskResults.Elements(XChessNames.Result)
                           let rType = EntityUtil.ParseMChessResultType((string)xr.Element(XChessNames.Label))
                           let acts = from xa in xr.Elements(XChessNames.Action)
                                      select new {
                                          Type = EntityUtil.ParseMChessResultActionType((string)xa.Attribute(XNames.AName)),
                                          Race = (int?)xa.Attribute(XChessNames.ARace)
                                      }
                           select new {
                               Type = rType,
                               Description = (string)xr.Element(XChessNames.Description),
                               XError = xr.Element(XConcurrencyNames.Error),
                               Actions = acts,
                               XSchedule = xr.Element(XChessNames.Schedule),
                           })
                           .ToList();
            //var resultsActions = (from xa in xtaskResults.Elements(XChessNames.Action)
            //                      select new {
            //                          Type = EntityUtil.ParseMChessResultActionType((string)xa.Attribute(XNames.AName)),
            //                          Race = (int?)xa.Attribute(XChessNames.ARace)
            //                      });

            var errorResults = results.Where(r => r.Type == MChessResultType.Error).ToList();
            // Lets just assert that we only have one error, or else the 2nd error is the "Abnormal termination" error
            Debug.Assert(errorResults.Count <= 1 || (errorResults.Count == 2 && errorResults[1].Description.StartsWith("Abnormal termination"))
                , "According to the ResultsPrinter.cpp, there should only ever be one Error returned from Chess."
                );

            ChessExitCode? chessExitCode = null;
            if (processExitCode.HasValue)
                chessExitCode = (ChessExitCode)processExitCode.Value;

            // First try to read from the results xml file
            XElement xchessFinalStats = xtaskResults.Element(XChessNames.FinalStats);
            ChessExitCode? resultExitCode = null;
            if (xchessFinalStats != null)
                resultExitCode = xchessFinalStats.Attribute(XChessNames.AExitCode).ParseXmlEnum<ChessExitCode>();

            ChessExitCode exitCode = resultExitCode ?? chessExitCode ?? ChessExitCode.Unknown;

            Debug.WriteLineIf(chessExitCode.HasValue && resultExitCode.HasValue && resultExitCode.Value != chessExitCode.Value
                , String.Format("Task ({0}): The exit code from the process doesn't match the exit code reported in the results.xml file. process: {1}; result file: {2}."
                , taskID ?? -1
                , chessExitCode
                , resultExitCode
                ));

            TestResultType resultType;
            string msg;

            // Detect unit test errors first
            if (exitCode == ChessExitCode.UnitTestAssertFailure)
            {
                resultType = TestResultType.AssertFailure;
                Debug.Assert(errorResults.Count != 0);
                msg = errorResults[0].Description;
            }
            else if (exitCode == ChessExitCode.UnitTestAssertInconclusive)
            {
                resultType = TestResultType.Inconclusive;
                Debug.Assert(errorResults.Count != 0);
                msg = errorResults[0].Description;
            }
            else if (exitCode == ChessExitCode.UnitTestException)
            {
                resultType = TestResultType.Exception;
                Debug.Assert(errorResults.Count != 0);
                msg = errorResults[0].Description;
            }
            else if (errorResults.Count != 0)
            {
                msg = errorResults[0].Description;

                if (exitCode == ChessExitCode.ChessRace)
                    resultType = TestResultType.DataRace;
                else if (exitCode == ChessExitCode.ChessLivelock)
                    resultType = TestResultType.Livelock;
                else if (exitCode == ChessExitCode.ChessDeadlock)
                    resultType = TestResultType.Deadlock;
                else
                    resultType = TestResultType.Error;
            }
            else if (results.Any(r => r.Type == MChessResultType.Race))
            {
                resultType = TestResultType.DataRace;
                msg = results.Where(r => r.Type == MChessResultType.Race).First().Description;
            }
            else if (exitCode != ChessExitCode.Success)
            {
                resultType = TestResultType.Error;
                msg = String.Format("Chess returned a non-zero exit code: {0}({1})", (int)exitCode, exitCode);
            }
            else
            {
                resultType = TestResultType.Passed;
                msg = "Passed";   // Just let the UI use it's own msg for a passing test.
            }
            Debug.Assert(msg != null);

            // If there's a single xerror, then propagate it to the root
            var xerrors = (from r in results
                           where r.XError != null
                           select r.XError)
                           ;
            var singleXError = xerrors.Count() == 1 ? xerrors.Single() : null;

            // Compose the final test result element
            return TestResultUtil.CreateXTestResult(resultType, msg, null
                , new XAttribute(XTestResultNames.AChessExitCode, exitCode)
                , singleXError
                , xtaskResults
                );
        }

    }
}
