using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    /// <summary>
    /// Represents a task used to run a test case via mcut on a separate process.
    /// </summary>
    public class MSBuildAppTask : BuildAppTaskBase
    {

        #region Constructors

        public MSBuildAppTask()
        {
            InitializeTask();
        }

        public MSBuildAppTask(BuildRunEntity run)
            : base(run)
        {
            InitializeTask();

            MSBuildSettings = XParent
                .AncestorsAndSelf()
                .SelectEntities()
                .Where(e => e != null)
                .OfType<IMSBuildSettings>()
                .FirstOrDefault();
        }

        private void InitializeTask()
        {
            StartScriptFilename = "startMSBuild.cmd";
            RedirectOutputToFiles = true;
            OutputFilename = "build.out";
            ErrorFilename = "build.err";
        }

        #endregion

        public override string ExecutableFilePath
        {
            // Since we'll be executing the VS CmdLineVars bat file before the exe, we can just use the filename
            get { return MCutToolsUtil.MSBuildExecutableFilename; }
        }

        public IMSBuildSettings MSBuildSettings { get; set; }

        public override string WorkingDirectory
        {
            get { return Path.GetDirectoryName(MSBuildSettings.ProjectFullPath); }
        }

        public override IEnumerable<string> GetExecutableArguments()
        {
            if (IsRebuild)
                yield return "/t:rebuild";

            // configuration
            string config = MSBuildSettings.Configuration;
            if (!String.IsNullOrEmpty(config))
                yield return "/p:Configuration=" + config; // NOTE: I don't expect there to be whitespace in the config attribute

            //// platform
            //string platform = MSBuildSettings.Platform;
            //if (!String.IsNullOrEmpty(platform))
            //    yield return "/p:Platform=" + platform; // NOTE: I don't expect there to be whitespace in the platform attribute

            // custom properties
            if (!String.IsNullOrEmpty(MSBuildSettings.OtherProperties))
                yield return "/p:" + MSBuildSettings.OtherProperties;

            // finally, the project file
            yield return Path.GetFileName(MSBuildSettings.ProjectFullPath);
        }

        protected override void WritePreExecutableScript(StreamWriter scriptWriter)
        {
            base.WritePreExecutableScript(scriptWriter);

            // Enable the VS command environment
            string vsCmdLineVarsPath = MCutToolsUtil.GetVSCmdLineVarsFilePath();
            if (vsCmdLineVarsPath == null)
                throw new Exception("Either Visual Studio is not installed or is not a supported version. Supported versions: 2008, 2010.");
            scriptWriter.WriteLine("CALL \"" + vsCmdLineVarsPath + "\"");
        }

    }
}
