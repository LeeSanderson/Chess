using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    /// <summary>
    /// The data regarding the change of a task's status.
    /// </summary>
    public class AppTaskStatusChangedEventArgs : EventArgs
    {

        public AppTaskStatusChangedEventArgs(AppTaskStatus prevStatus, AppTaskStatus currentStatus)
        {
            PreviousStatus = prevStatus;
            CurrentStatus = currentStatus;
        }

        public AppTaskStatus PreviousStatus { get; private set; }
        public AppTaskStatus CurrentStatus { get; private set; }

    }
}
