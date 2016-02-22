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
    /// Represents a task used to run a custom build script.
    /// </summary>
    public class CustomBuildAppTask : BuildAppTaskBase
    {

        #region Constructors

        public CustomBuildAppTask()
        {
            InitializeTask();
        }

        public CustomBuildAppTask(BuildRunEntity run)
            : base(run)
        {
            InitializeTask();

            // Note: I'm also assuming build runs don't ever get nested (i.e. re-run etc)
            BuildEntity = (CustomBuildEntity)XParent.GetEntity();
        }

        private void InitializeTask()
        {
            StartScriptFilename = "startCustomBuild.cmd";
            RedirectOutputToFiles = true;
            OutputFilename = "build.out";
            ErrorFilename = "build.err";
        }

        #endregion

        public CustomBuildEntity BuildEntity { get; set; }

        public override string ExecutableFilePath
        {
            get { return BuildEntity.ExecutablePath; }
        }

        public override string WorkingDirectory
        {
            get { return BuildEntity.WorkingFolderPath; }
        }

        public override IEnumerable<string> GetExecutableArguments()
        {
            return BuildEntity.GetCommandLineArgs();
        }

        protected override void WritePreExecutableScript(StreamWriter scriptWriter)
        {
            base.WritePreExecutableScript(scriptWriter);

            foreach (var shellLine in BuildEntity.GetShellLines())
                scriptWriter.WriteLine(shellLine);
        }

    }
}
