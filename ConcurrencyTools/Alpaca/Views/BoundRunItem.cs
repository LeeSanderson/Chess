using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.AppTasks;
using Microsoft.Concurrency.TestTools.Alpaca.Actions;

namespace Microsoft.Concurrency.TestTools.Alpaca.Views
{
    // TODO: When sorting by TaskStatus, compare using the value, rather than the text representation.

    public class BoundRunItem : INotifyPropertyChanged
    {

        public BoundRunItem(TaskRunEntity run)
        {
            Run = run;

            // Bind the text now since it won't ever change
            DisplayText = run.DisplayName ?? run.MenuItemTypeName();

            Run.Task.StatusChanged += Run_StatusChangeEvt;
            Update();
        }

        #region Properties

        public TaskRunEntity Run { get; private set; }

        public int TaskID { get { return Run.TaskID; } }

        public string DisplayText { get; private set; }

        private AppTaskStatus _taskStatus;
        public AppTaskStatus TaskStatus
        {
            get { return _taskStatus; }
            set
            {
                if (value != TaskStatus)
                {
                    _taskStatus = value;
                    OnPropertyChanged("TaskStatus");
                }
            }
        }

        private TestResultType? _testResult;
        public TestResultType? TestResult
        {
            get { return _testResult; }
            set
            {
                if (value != TestResult)
                {
                    _testResult = value;
                    OnPropertyChanged("TestResult");
                }
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Run_StatusChangeEvt(object sender, EventArgs args)
        {
            //System.Diagnostics.Debug.Assert(sender == (object)Run);

            Update();
        }

        public void Update()
        {
            TestResultType? testResult = null;
            var testRun = Run as TestRunEntity;
            if (testRun != null && testRun.HasResult)
                testResult = testRun.Result.ResultType;

            this.TestResult = testResult;
            this.TaskStatus = Run.TaskStatus;
        }

    }
}
