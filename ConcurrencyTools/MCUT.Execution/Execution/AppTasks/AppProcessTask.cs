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
    public abstract class AppProcessTask : AppTask
    {

        public AppProcessTask()
        {
            UsesTaskFolder = true;
        }

        protected AppProcessTask(XElement xtaskState)
            : base(xtaskState)
        {
            UsesTaskFolder = true;

            XProcessState = xtaskState.Element(XSessionNames.ProcessState);
            if (Status >= AppTaskStatus.Starting && Status <= AppTaskStatus.Running)
            {
                if (XProcessState != null)
                {
                    // Find out if the process is still executing
                    try
                    {
                        var p = System.Diagnostics.Process.GetProcessById(ProcessID.Value);
                        if (p != null && p.StartTime == ProcessStartTime.Value)
                        {
                            // Then we've found the process still running.
                            Process = p;
                        }
                    }
                    catch { } // Ignore if we can't find the process anymore
                }
            }
        }

        #region Properties

        protected XElement XProcessState { get; private set; }

        /// <summary>
        /// Gets the working directory to use for running the process.
        /// The default implementation uses the <see cref="TaskFolderPath"/>.
        /// </summary>
        public virtual string WorkingDirectory { get { return TaskFolderPath; } }

        /// <summary>
        /// Gets the name of the file to execute.
        /// This path should be relative to the <see cref="WorkingDirectory"/>.
        /// </summary>
        public abstract string ExecutableFilePath { get; }

        public bool KillProcessToCancel { get; set; }

        protected Process Process { get; private set; }

        public int? ProcessID
        {
            get { return XProcessState == null ? (int?)null : (int)XProcessState.Attribute(XSessionNames.AProcessID); }
        }

        public DateTime? ProcessStartTime
        {
            get { return XProcessState == null ? (DateTime?)null : (DateTime)XProcessState.Attribute(XSessionNames.AStartTime); }
        }

        /// <summary>The exit code for the process, if the process has exited and it's available.</summary>
        public int? ExitCode
        {
            get { return XProcessState == null ? (int?)null : (int)XProcessState.Attribute(XSessionNames.AProcessID); }
            private set { XProcessState.SetAttributeValue(XSessionNames.AProcessID, value); }
        }

        #endregion

        /// <summary>Gets the raw arguments for the executable specified by the <see cref="ExecutableFilePath"/> property.</summary>
        /// <remarks>The values returned here SHOULD NOT be quote escaped as it is handled automatically.</remarks>
        public abstract IEnumerable<string> GetExecutableArguments();

        #region Task Validation & Setup

        protected override void OnValidate()
        {
            base.OnValidate();
            if (String.IsNullOrEmpty(WorkingDirectory))
                throw new InvalidOperationException("The WorkingDirectory must be specified.");
            if (String.IsNullOrEmpty(ExecutableFilePath))
                throw new InvalidOperationException("The ExecutableFilePath must be specified.");
        }

        #endregion

        #region Task Lifecycle Implementation

        protected override void OnRunSynchronously()
        {
            // from Start()
            Process = CreateProcess();
            StartProcess();

            // from Wait()
            WaitForProcessToExit();
        }

        protected override void OnStart()
        {
            if (Process != null)
                throw new InvalidOperationException("The task already has a process associated with it.");

            var p = Process = CreateProcess();

            // Since Alpaca may close and restart this task, we can't make guarantees to whether using the Exited
            // event will fire between the GetProcessById call and setting up the event. Therefore, we don't rely
            // on this and instead stick with our WaitForExit(0) technique in the IsFinishedExecuting method.
            ////var tcs = new TaskCompletionSource<AppTaskStatus>();
            //p.EnableRaisingEvents = true;
            //p.Exited += (obj, args) => {
            //    //// S. Toub says that we should need to make this call if using the Exited event.
            //    ////// Will this cause a problem when in the event handler for Exited?
            //    ////p.WaitForExit(); // Make sure we're really done so the outputs have been flushed and all resources closed (i.e. files)
            //    //var status = OnProcessExited();
            //    //tcs.SetResult(status);

            //    _isProcessExited = true;
            //};

            StartProcess();
        }

        protected override bool IsFinishedExecuting()
        {
            // See documentation for HasExited.
            // If it is true, then we aren't guaranteed that all the output
            // streams and async operations have finished yet, so we need to
            // be sure it has fully exited using WaitForExit. By specifying
            // 0, it will return without blocking.

            // According to MSDN documentation, after a call to WaitForExit(Int32)
            // we should follow up with a call to WaitForExit() to ensure
            // all asynchronous events for redirected output have completed too

            return Process == null || (Process.HasExited && Process.WaitForExit(0));
        }

        protected override void OnWait()
        {
            WaitForProcessToExit();
        }

        protected override bool OnCancelling()
        {
            Debug.Assert(Status == AppTaskStatus.Running, "The AppTask.Cancel code won't call this unless we're running.");

            if (Process == null)
                return false;

            if (!Process.HasExited)
            {
                if (KillProcessToCancel)
                {
                    Process.Kill();
                    if (!Process.WaitForExit(2000))
                        return false;
                    Process.WaitForExit();  // To be sure all async event handling is done
                }
                else // The task doesn't support killing the process
                    return false;
            }

            return base.OnCancelling();
        }

        #endregion

        #region Process Lifecycle

        protected virtual Process CreateProcess()
        {
            return new Process() {
                StartInfo = CreateProcessStartInfo()
            };

            //if (RedirectProcessOutputToFiles)
            //    OutputReader = new ProcessOutputReader(p) {
            //        EchoToConsole = EchoOutputToConsole,
            //        OutputFilePath = Path.Combine(TaskFolderPath, OutputFilename),
            //        ErrorFilePath = Path.Combine(TaskFolderPath, ErrorFilename),
            //    };
        }

        protected virtual ProcessStartInfo CreateProcessStartInfo()
        {
            return new ProcessStartInfo(ExecutableFilePath) {
                Arguments = CommandlineUtil.ComposeCommandlineArguments(GetExecutableArguments()),
                WorkingDirectory = TaskFolderPath, // The script should always start locally

                // NOTE: This assumes the process is a script or command-line executable.
                CreateNoWindow = true,
                UseShellExecute = false,
            };
        }

        private void StartProcess()
        {
            Process.Start();

            // Initialize the process' xml state and collect the info to be serialized
            XProcessState = new XElement(XSessionNames.ProcessState
                , new XAttribute(XSessionNames.AProcessID, Process.Id)
                , new XAttribute(XSessionNames.AStartTime, Process.StartTime)
                );
            XTaskState.Add(XProcessState);

            OnProcessStarted();
        }

        private void WaitForProcessToExit()
        {
            if (Process != null)
            {
                Process.WaitForExit();
                ExitCode = Process.ExitCode;
            }
            else
            {
                // Since we can't know what it exited with, assume it exited with zero
                // For testing tasks that use result files, the ExitCode is 2ndary to
                // what's found in the result file.
                ExitCode = 0;
            }
            OnProcessExited();

            // Dispose of the process now
            if (Process != null)
            {
                Process.Dispose();
                Process = null;
            }
        }

        /// <summary>
        /// Notifies a derived class that the process has been started.
        /// This is called whether being started using the RunSynchronously
        /// or Start methods.
        /// </summary>
        protected virtual void OnProcessStarted()
        {
            //if (RedirectProcessOutputToFiles)
            //    OutputReader.BeginReading();
        }

        /// <summary>
        /// Notifies a derived class that the process has exited and allows
        /// any further processing dependant on the process to execute.
        /// Note: If the task has been reconstituted, and the process had already exited,
        /// you will not be given access to the Process information.
        /// The default returns <see cref="AppTaskStatus.Complete"/>.
        /// </summary>
        /// <returns>The final status of this task. Should be a status greater than Running.</returns>
        protected virtual void OnProcessExited()
        {
            //if (RedirectProcessOutputToFiles)
            //    OutputReader.End();
        }

        #endregion

        protected override void Dispose()
        {
            if (Process != null)
                Process.Dispose();

            base.Dispose();
        }

    }
}
