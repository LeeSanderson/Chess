using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Microsoft.Concurrency.TestTools.Execution
{

    public enum ProcessOutputDestination
    {
        Console,
    }

    /// <summary>
    /// Helper that reads the standard output and standard error of a process using background
    /// threads. Create an instance before calling the <see cref="Process.Start"/> method.
    /// </summary>
    public abstract class ProcessOutputReader
    {

        #region full readers

        class OutputToConsole : ProcessOutputReader
        {

            internal OutputToConsole(Process p) : base(p) { }

            protected override string OnGetOutputText()
            {
                throw new NotSupportedException("When sending output to the current application's Console, the output is not cached and therefore cannot be retrieved.");
            }

            protected override string OnGetErrorText()
            {
                throw new NotSupportedException("When sending output to the current application's Console, the output is not cached and therefore cannot be retrieved.");
            }

            protected override void OnOutputLineRead(string line)
            {
                Console.Out.WriteLine(line);
            }

            protected override void OnErrorLineRead(string line)
            {
                Console.Error.WriteLine(line);
            }

        }

        class OutputToBuffer : ProcessOutputReader
        {

            private StringBuilder _outputSB = new StringBuilder();
            private StringBuilder _errorSB = new StringBuilder();

            internal OutputToBuffer(Process p) : base(p) { }

            protected override string OnGetOutputText()
            {
                return _outputSB.ToString();
            }

            protected override string OnGetErrorText()
            {
                return _errorSB.ToString();
            }

            protected override void OnOutputLineRead(string line)
            {
                _outputSB.AppendLine(line);
            }

            protected override void OnErrorLineRead(string line)
            {
                _errorSB.AppendLine(line);
            }

        }

        class OutputToFiles : ProcessOutputReader
        {

            internal OutputToFiles(Process p) : base(p) { }

            public string OutputFilePath { get; internal set; }
            public string ErrorFilePath { get; internal set; }

            protected override string OnGetOutputText()
            {
                return File.ReadAllText(OutputFilePath);
            }

            protected override string OnGetErrorText()
            {
                return File.ReadAllText(ErrorFilePath);
            }

            protected override void OnOutputLineRead(string line)
            {
                File.AppendAllLines(OutputFilePath, new[] { line });
            }

            protected override void OnErrorLineRead(string line)
            {
                File.AppendAllLines(ErrorFilePath, new[] { line });
            }

            protected override void OnBeginningToRead()
            {
                base.OnBeginningToRead();

                if (String.IsNullOrEmpty(OutputFilePath))
                    throw new InvalidOperationException("OutputFilePath must be specified.");
                if (String.IsNullOrEmpty(ErrorFilePath))
                    throw new InvalidOperationException("ErrorFilePath must be specified.");

                // Create the new files, this will pass up IOExceptions if the folders don't exits etc.
                File.WriteAllText(OutputFilePath, "");
                File.WriteAllText(ErrorFilePath, "");
            }

        }

        #endregion

        #region Static Members

        public static ProcessOutputReader CreateOutputToBuffer(Process p)
        {
            return new ProcessOutputReader.OutputToBuffer(p);
        }

        public static ProcessOutputReader CreateOutputToConsole(Process p)
        {
            return new ProcessOutputReader.OutputToConsole(p);
        }

        public static ProcessOutputReader CreateOutputToFiles(Process p, string outputFilePath, string errorFilePath)
        {
            return new ProcessOutputReader.OutputToFiles(p) {
                OutputFilePath = outputFilePath,
                ErrorFilePath = errorFilePath,
            };
        }

        #endregion

        private Process _process;
        private bool _isReading;

        private object _outputSync = new object();
        private object _errorSync = new object();

        /// <summary>Creates a new reader instance.</summary>
        /// <param name="process">The process to read from.</param>
        protected ProcessOutputReader(Process process)
        {
            _process = process;

            process.StartInfo.RedirectStandardOutput = true;
            process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);

            process.StartInfo.RedirectStandardError = true;
            process.ErrorDataReceived += new DataReceivedEventHandler(process_ErrorDataReceived);
        }

        public string LinePrefix { get; set; }

        /// <summary>Gets the current output text.</summary>
        public string GetOutputText()
        {
            lock (_outputSync)
                return OnGetOutputText();
        }

        /// <summary>Gets the current error text.</summary>
        public string GetErrorText()
        {
            lock (_errorSync)
                return OnGetErrorText();
        }

        protected abstract string OnGetOutputText();
        protected abstract string OnGetErrorText();

        void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            string line = String.Concat(LinePrefix, e.Data);
            lock (_outputSync)
                OnOutputLineRead(line);
        }

        void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            string line = String.Concat(LinePrefix, e.Data);
            lock (_errorSync)
                OnErrorLineRead(line);
        }

        protected abstract void OnOutputLineRead(string line);
        protected abstract void OnErrorLineRead(string line);

        /// <summary>
        /// Starts the asynchronous reading of the output and error streams for the process.
        /// In general, calling the <see cref="Process.WaitForExit()"/> method (after calling
        /// this method) will allow this reader to handle the reading of the output from the
        /// process.
        /// </summary>
        public virtual void BeginReading()
        {
            if (_isReading)
                throw new InvalidOperationException("BeginReading is already running.");

            OnBeginningToRead();

            _isReading = true;
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        protected virtual void OnBeginningToRead()
        {
        }

        /// <summary>
        /// This should be called after calling the <see cref="Process.WaitForExit()"/> method
        /// to ensure that all output has been read.
        /// </summary>
        public void End()
        {
            if (!_isReading)
                throw new InvalidOperationException("BeginReading hasn't been called.");

            _process.CancelOutputRead();
            _process.CancelErrorRead();
            _isReading = false;
        }

    }
}
