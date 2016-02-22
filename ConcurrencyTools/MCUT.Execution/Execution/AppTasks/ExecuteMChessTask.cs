using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.Execution.Chess;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    public class ExecuteMChessTask : AppScriptProcessTask
    {

        /// <summary>The name of the start script file when running instances of this task.</summary>
        public const string StartMChessScriptFilename = "startMChess.cmd";

        public const string InputChessScheduleFilename = "inputsched";
        private ProcessOutputReader _processOutputReader;

        public ExecuteMChessTask()
        {
            StartScriptFilename = StartMChessScriptFilename;
            RedirectOutputToFiles = false;
        }

        public override string ExecutableFilePath { get { return MCutToolsUtil.MChessExecutableFilePath; } }

        public override string WorkingDirectory { get { return Path.GetDirectoryName(TestCase.AssemblyLocation); } }

        public TestCaseEntity TestCase { get; set; }

        public string OutputPathPrefix { get { return TaskFolderPath + '\\'; } }

        public XElement XMChessResults { get; private set; }
        public XElement XTestResult { get; private set; }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (TestCase == null)
                throw new InvalidOperationException("The TestCase property has not been set yet.");
        }

        private MChessOptions _mcopts;
        protected override void OnPerformSetup()
        {
            SetupMChessOptions();

            base.OnPerformSetup();

            // Save the schedule file if specified
            if (_mcopts.XSchedule != null)
                ChessScheduleUtil.SaveXScheduleToFile(_mcopts.XSchedule, _mcopts.ScheduleFilePath);

            // NOTE: At this point, the input observation file should already exist.
            if (_mcopts.ObservationMode.HasValue
                && _mcopts.ObservationMode.Value.RequiresInputObservationFile())
            {
                if (String.IsNullOrEmpty(_mcopts.ObservationFile))
                    throw new InvalidOperationException("The mchess observation mode specified requires an input observation file but none is specified.");
                if (!File.Exists(_mcopts.ObservationFile))
                    throw new InvalidOperationException("The mchess observation mode specified requires an input observation file but the file wasn't found in the task folder.");
            }
        }

        private void SetupMChessOptions()
        {
            // Add cmd line switches for mchess options
            _mcopts = new MChessOptions(TestCase.XMChessOptions);

            // This should not be serialized, and only set in the runner code
            // If the expected results are looking for HBExecs, we need to turn it on so it gets reported
            var expResult = TestCase.EntityOfType<ExpectedChessTestResultEntity>();
            if (expResult != null && expResult.LastHBExecSteps.HasValue)
                _mcopts.ShowHBExecs = true;

            // Need to save the schedule file if specified
            if (_mcopts.XSchedule != null)
                _mcopts.ScheduleFilePath = Path.Combine(TaskFolderPath, InputChessScheduleFilename);

            // If an observation filename is specified, we need to make sure it's relative
            // to the task folder
            if (!String.IsNullOrEmpty(_mcopts.ObservationFile))
                _mcopts.ObservationFile = Path.Combine(TaskFolderPath, _mcopts.ObservationFile);
        }

        protected override void WritePreExecutableScript(StreamWriter scriptWriter)
        {
            base.WritePreExecutableScript(scriptWriter);

            string preRunScript = TestCase.MChessPreRunScript;
            if (!String.IsNullOrWhiteSpace(preRunScript))
                scriptWriter.WriteLine(preRunScript);

            // NOTE: The previous executor added a POPD after the command line. But that shouldn't have any affect.
        }

        public override IEnumerable<string> GetExecutableArguments()
        {
            foreach (var arg in _mcopts.GetCommandLineArgs())
                yield return arg;

            // Add the arg for specifying the unit test method to run
            yield return Path.GetFileName(TestCase.AssemblyLocation);
            yield return "/test:" + TestCase.FullTestMethodName;

            // Add the test case args
            TestArgs testArgs = TestCase.GetTestArgs();
            if (testArgs != null)
            {
                foreach (var tcArg in testArgs.Values)
                    yield return "/arg:" + tcArg;
            }

            // set directory for output files
            yield return "/outputprefix:" + OutputPathPrefix;
        }

        protected override System.Diagnostics.Process CreateProcess()
        {
            var p = base.CreateProcess();

            _processOutputReader = ProcessOutputReader.CreateOutputToConsole(p);

            return p;
        }

        protected override void OnProcessStarted()
        {
            base.OnProcessStarted();
            _processOutputReader.BeginReading();
        }

        protected override void OnProcessExited()
        {
            base.OnProcessExited();

            _processOutputReader.End();
        }

        protected override void OnCompleting()
        {
            base.OnCompleting();

            // Read the chess result
            XMChessResults = new ChessResultsReader(OutputPathPrefix).GetChessTestResults();
            XTestResult = ChessXResultUtil.ChessXResultsToXTestResult(XMChessResults, ExitCode, ID)
                ?? TestResultUtil.CreateErrorXTestResult("The chess results file could not be found.");
        }

    }
}
