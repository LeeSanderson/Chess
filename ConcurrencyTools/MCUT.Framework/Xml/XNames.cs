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
    /// The XNames used in testlist xml files.
    /// </summary>
    public static class XNames
    {
        public const string UnitTestingNamespaceURI = "http://research.microsoft.com/concurrency/unittesting";
        public static readonly XNamespace UnitTestingNS = XNamespace.Get(UnitTestingNamespaceURI);

        public static XAttribute CreateXmlnsAttribute(bool asDefaultNS)
        {
            if (asDefaultNS)
                return new XAttribute("xmlns", UnitTestingNS.NamespaceName);
            else
                return new XAttribute(XNamespace.Xmlns + "mcut", UnitTestingNS.NamespaceName);
        }


        // elements
        public static readonly XName NULL = XName.Get("_NULL_", String.Empty);
        public static readonly XName Error = XConcurrencyNames.Error;
        public static readonly XName AErrorExceptionType = XConcurrencyNames.AErrorExceptionType;
        public static readonly XName ErrorMessage = XConcurrencyNames.ErrorMessage;
        public static readonly XName ErrorStackTrace = XConcurrencyNames.ErrorStackTrace;

        public static readonly XName Placeholder = UnitTestingNS + "placeholder";

        //public static readonly XName Name = UnitTestingNS + "name";   // Prefer setting a name as an attribute
        //public static readonly XName Test = UnitTestingNS + "test";
        public static readonly XName Testlist = UnitTestingNS + "testlist";
        //public static readonly XName Tip = UnitTestingNS + "tip";
        public static readonly XName CustomBuild = UnitTestingNS + "build";
        //public static readonly XName Chessarg = UnitTestingNS + "carg";
        /// <summary>The element representing a generic argument to be passed into a test.</summary>
        public static readonly XName Arg = UnitTestingNS + "arg";
        public static readonly XName Commandline = UnitTestingNS + "commandline";
        public static readonly XName Include = UnitTestingNS + "include";
        public static readonly XName Executable = UnitTestingNS + "executable";
        public static readonly XName Shellline = UnitTestingNS + "shellline";
        public static readonly XName WorkingDirectory = UnitTestingNS + "workingDirectory";

        public static readonly XName MSBuild = UnitTestingNS + "msbuild";
        /// <summary>'properties' element for the 'msbuild' element.</summary>
        public static readonly XName MSBuildProperties = UnitTestingNS + "properties";
        public static readonly XName AConfiguration = "configuration";
        //public static readonly XName ADefaultConfiguration = "defaultConfiguration";

        // testAssembly related
        public static readonly XName TestProject = UnitTestingNS + "testProject";
        public static readonly XName GoldenObservationFilesFolderPath = UnitTestingNS + "goldenObservationFilesFolderPath";

        public static readonly XName TestAssembly = UnitTestingNS + "testAssembly";
        public static readonly XName TestClass = UnitTestingNS + "testClass";
        public static readonly XName TestMethod = UnitTestingNS + "testMethod";
        public static readonly XName TestArgs = UnitTestingNS + "testArgs";
        /// <summary>The defined parameters defined for a unit test.</summary>
        public static readonly XName Parameters = UnitTestingNS + "parameters";
        public static readonly XName Param = UnitTestingNS + "param";

        public static readonly XName ExpectedTestResult = UnitTestingNS + "expectedTestResult";
        public static readonly XName ExpectedRegressionTestResult = UnitTestingNS + "expectedRegressionTestResult";

        /// <summary>Element for an MChess unit test.</summary>
        public static readonly XName ChessUnitTest = UnitTestingNS + "chessTest";
        public static readonly XName ChessContext = UnitTestingNS + "chessContext";
        /// <summary>The element used to specify any command-line commands to run before running mchess.</summary>
        public static readonly XName MChessPreRunScript = UnitTestingNS + "preRunScript";
        public static readonly XName ExpectedChessResult = UnitTestingNS + "expectedChessResult";

        /// <summary>Element for a standard unit test.</summary>
        public static readonly XName UnitTest = UnitTestingNS + "unitTest";
        public static readonly XName DataRaceTest = UnitTestingNS + "dataRaceTest";
        public static readonly XName ScheduleTest = UnitTestingNS + "scheduleTest";
        public static readonly XName DeterminismTest = UnitTestingNS + "determinismTest";
        public static readonly XName ConflictSerializabilityTest = UnitTestingNS + "conflictSerializabilityTest";
        public static readonly XName ObservationGenerator = UnitTestingNS + "observationGenerator";
        public static readonly XName ObservationTest = UnitTestingNS + "observationTest";

        public static readonly XName PerformanceTest = UnitTestingNS + "performanceTest";
        public static readonly XName TaskoMeter = UnitTestingNS + "taskoMeter";



        // attributes
        // NOTE: No need to actually specify a namespace for an attribute
        public static readonly XName AName = "name";
        public static readonly XName AKey = "key";
        //public static readonly XName AFormat = "format";
        public static readonly XName AObservationFile = "observationfile";
        public static readonly XName AProject = "project";
        public static readonly XName ALocation = "location";
        public static readonly XName AFullName = "fullName";
        public static readonly XName AType = "type";
        public static readonly XName AExclude = "exclude";
        public static readonly XName AResultType = "resultType";
        public static readonly XName AExpectedResultKey = "expectedResultKey";
        public static readonly XName AExpectedChessResultKey = "expectedChessResultKey";



        // helpers

        public static XElement CreateXError(Exception ex)
        {
            return XConcurrencyNames.CreateXError(ex);
        }

        public static XElement CreateXError(string message, Exception ex = null)
        {
            return XConcurrencyNames.CreateXError(message, ex);
        }

    }
}
