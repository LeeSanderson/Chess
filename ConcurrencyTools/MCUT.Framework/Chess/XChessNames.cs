using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.Concurrency.TestTools.UnitTesting.Chess
{
    /// <summary>
    /// XName instances for names used by wchess and mchess.
    /// i.e. results.xml files.
    /// </summary>
    public static class XChessNames
    {

        private const string NamespaceURI = "http://research.microsoft.com/chess";
        public static readonly XNamespace ChessNS = XNamespace.Get(NamespaceURI);

        public static XAttribute CreateXmlnsAttribute(bool asDefaultNS)
        {
            if (asDefaultNS)
                return new XAttribute("xmlns", ChessNS.NamespaceName);
            else
                return new XAttribute(XNamespace.Xmlns + "chess", ChessNS.NamespaceName);
        }

        // MChessOptions
        public static readonly XName MChessOptions = ChessNS + "mchessOptions";
        public static readonly XName InstrumentAssembly = ChessNS + "instrumentAssembly";
        public static readonly XName DefaultPreemptability = ChessNS + "defaultPreemptability";
        public static readonly XName TogglePreemptability = ChessNS + "togglePreemptability";
        /// <summary>Element for an extra cmd line arg to pass to MChess.</summary>
        public static readonly XName CmdLineArg = ChessNS + "cmdLineArg";


        // mchess - results.xml
        /// <summary>
        /// The element name reported by chess. This gets switched to <see cref="ChessResults"/> 
        /// once the chess result file has been processed.
        /// </summary>
        public static readonly XName Results = ChessNS + "results";
        public static readonly XName ChessResults = ChessNS + "chessResults";
        public static readonly XName CommandLine = ChessNS + "commandline";
        public static readonly XName Result = ChessNS + "result";
        public static readonly XName Label = ChessNS + "label";
        public static readonly XName Description = ChessNS + "description";
        public static readonly XName Action = ChessNS + "action";
        public static readonly XName Schedule = ChessNS + "schedule";
        public static readonly XName EndTime = ChessNS + "endtime";

        public static readonly XName FinalStats = ChessNS + "finalStats";
        public static readonly XName AExitCode = "exitCode";
        public static readonly XName ASchedulesRan = "schedulesRan";
        public static readonly XName ALastThreadCount = "lastThreadCount";
        public static readonly XName ALastExecSteps = "lastExecSteps";
        public static readonly XName ALastHBExecSteps = "lastHBExecSteps";
        //public static readonly XName Schedfile = ChessNS + "schedfile";

        public static readonly XName ARace = "race";
        public static readonly XName AFormat = "format";

        // Values
        public const string VHex = "hex";

        /// <summary>
        /// Provides the list of actions provided by chess.
        /// </summary>
        public static class Actions
        {
            public const string View = "View";
            public const string Repeat = "Repeat";
            public const string Continue = "Continue";
            public const string Repro = "Repro";
            public const string ReproLastSchedule = "Repro Last Schedule";
        }

    }
}
