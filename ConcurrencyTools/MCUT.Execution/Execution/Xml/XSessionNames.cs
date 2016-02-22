using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.Execution.Xml
{
    /// <summary>
    /// XName instances for names used in session xml documents.
    /// </summary>
    public static class XSessionNames
    {

        private const string NamespaceURI = XNames.UnitTestingNamespaceURI + "/session";
        public static readonly XNamespace SessionNS = XNamespace.Get(NamespaceURI);

        public static XAttribute CreateXmlnsAttribute(bool asDefaultNS)
        {
            if (asDefaultNS)
                return new XAttribute("xmlns", SessionNS.NamespaceName);
            else
                return new XAttribute(XNamespace.Xmlns + "s", SessionNS.NamespaceName);
        }

        // elements
        //public static readonly XName Error = SessionNS + "error";
        //public static readonly XName ErrorMessage = SessionNS + "message";
        //public static readonly XName ErrorStackTrace = SessionNS + "stackTrace";

        public static readonly XName Session = SessionNS + "session";
        public static readonly XName SessionState = SessionNS + "state";


        // Runs
        public static readonly XName AType = "type";
        // Boolean value indicating whether the run should enable the regression test assertions
        //[Obsolete("This has been moved to the TestExecutor framework.")]
        //public static readonly XName AEnableRegressionTestAsserts = "enableRegressionTestAsserts";

        // AppTask state
        public static readonly XName TaskState = SessionNS + "taskState";
        public static readonly XName ATaskID = "taskID";
        public static readonly XName ATaskStatus = "status";
        public static readonly XName AIsSetup = "isSetup";
        public static readonly XName TaskFolderPath = SessionNS + "taskFolderPath";

        // Process state
        public static readonly XName ProcessState = SessionNS + "processState";
        public static readonly XName AProcessID = "pid";
        public static readonly XName AStartTime = "startTime";
        public static readonly XName AExitCode = "exitCode";

        // Runs
        public static readonly XName MCutTestRun = SessionNS + "mcutTestRun";
        public static readonly XName AMCutTestRunType = "testRunType";
        public static readonly XName AHasTestSourceChanged = "hasTestSourceChanged";
        public static readonly XName AUseGoldenObservationFile = "useGoldenObservationFile";
        public static readonly XName ARunInteractively = "runInteractively";
        public static readonly XName AEnableRegressionTestAsserts = "enableRegressionTestAsserts";

        public static readonly XName BuildRun = SessionNS + "buildRun";
        public static readonly XName AIsRebuild = "isRebuild";

        //// Plugins
        //public static readonly XName Pluginengine = SessionNS + "pluginengine";
        //public static readonly XName Plugintype = SessionNS + "plugintype";
        //public static readonly XName Engineid = SessionNS + "engineid";
        //public static readonly XName Pluginstate = SessionNS + "pluginstate";
        //public static readonly XName Job = SessionNS + "job";
        //public static readonly XName TestPrefix = SessionNS + "testprefix";

        //public static readonly XName Bucket = SessionNS + "bucket";
        public static readonly XName PreemptionVars = SessionNS + "preemptionvars";
        public static readonly XName PreemptionMethods = SessionNS + "preemptionmethod";
        public static readonly XName Counter = SessionNS + "counter";


        // Annotation attributes for entities
        public static readonly XName AEntityID = SessionNS + "entityID";
        public static readonly XName AObservationsFolderPath = SessionNS + "observationsFolderPath";
        public static readonly XName ADetectChanges = SessionNS + "detectChanges";

    }
}
