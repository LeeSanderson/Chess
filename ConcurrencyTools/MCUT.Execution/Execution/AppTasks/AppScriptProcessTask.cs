using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    /// <summary>
    /// Represents a task that includes running a background process
    /// in the task's folder.
    /// </summary>
    public abstract class AppScriptProcessTask : AppProcessTask
    {

        public AppScriptProcessTask()
        {
        }

        protected AppScriptProcessTask(XElement xtaskState)
            : base(xtaskState)
        {
        }

        #region Properties

        public string StartScriptFilename { get; protected set; }

        /// <summary>
        /// Indicates whether the task's script file should redirect
        /// the output to the output files specified by <see cref="OutputFilename"/>
        /// and <see cref="ErrorFilename"/>.
        /// </summary>
        public bool RedirectOutputToFiles { get; protected set; }
        public string OutputFilename { get; set; }
        public string OutputFilePath { get { return Path.Combine(TaskFolderPath, OutputFilename); } }
        public string ErrorFilename { get; set; }
        public string ErrorFilePath { get { return Path.Combine(TaskFolderPath, ErrorFilename); } }

        #endregion

        #region Task Validation & Setup

        protected override void OnValidate()
        {
            base.OnValidate();

            if (String.IsNullOrEmpty(StartScriptFilename))
                throw new InvalidOperationException("StartScriptFilename not specified.");

            if (RedirectOutputToFiles)
            {
                if (String.IsNullOrEmpty(OutputFilename))
                    throw new InvalidOperationException("OutputFilename not specified.");
                if (String.IsNullOrEmpty(ErrorFilename))
                    throw new InvalidOperationException("ErrorFilename not specified.");
            }
        }

        protected override void OnPerformSetup()
        {
            base.OnPerformSetup();

            CreateTaskScriptFile();
        }

        private void CreateTaskScriptFile()
        {
            using (var script = new StreamWriter(Path.Combine(TaskFolderPath, StartScriptFilename), false))
            {
                script.WriteLine("@ECHO OFF");
                if (RedirectOutputToFiles)
                {
                    script.WriteLine("SET scriptOutPath=" + OutputFilePath);
                    script.WriteLine("SET scriptErrPath=" + ErrorFilePath);
                }

                // Since the Task folder where the script file is created may be
                // different than the preferred working directory of the executable
                // we always set this.
                script.WriteLine("PUSHD " + WorkingDirectory);

                WritePreExecutableScript(script);

                string executableCmdLine = CommandlineUtil.ComposeCommandline(ExecutableFilePath, GetExecutableArguments());
                script.Write(executableCmdLine);
                if (RedirectOutputToFiles)
                {
                    script.Write(" 1>\"%scriptOutPath%\"");
                    script.Write(" 2>\"%scriptErrPath%\"");
                }
                script.WriteLine();
            }
        }

        /// <summary>
        /// Gives a derived class the opportunity to modify the script just
        /// before the executable's command line.
        /// </summary>
        /// <param name="scriptWriter">The writer for the script.</param>
        protected virtual void WritePreExecutableScript(StreamWriter scriptWriter)
        {
            //foreach (string line in GetPreExecScriptLines())
            //    script.WriteLine(line);
        }

        #endregion

        #region Process Creation

        protected override ProcessStartInfo CreateProcessStartInfo()
        {
            // We'll be executing the script instead
            return new ProcessStartInfo(Path.Combine(TaskFolderPath, StartScriptFilename)) {
                WorkingDirectory = TaskFolderPath, // The script should always start locally

                // NOTE: This assumes the process is a script or command-line executable.
                CreateNoWindow = true,
                UseShellExecute = false,
            };
        }

        #endregion

    }
}
