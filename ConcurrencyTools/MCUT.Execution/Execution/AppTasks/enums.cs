using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    /// <summary>Represents that status of a task being executed.</summary>
    public enum AppTaskStatus
    {
        /// <summary>The task is being prepared and has not been completely setup yet.</summary>
        Preparing,
        /// <summary>The <see cref="AppTask.Setup"/> method has been called.</summary>
        Setup,
        /// <summary>The task has been placed in the waiting queue of a task scheduler.</summary>
        Waiting,
        /// <summary>The task is starting to run.</summary>
        Starting,
        /// <summary>The task is executing.</summary>
        Running,
        /// <summary>The task has completed.</summary>
        Complete,
        /// <summary>The task has been cancelled.</summary>
        Cancelled,
        /// <summary>Indicates an error occurred while trying to execute the task.</summary>
        Error,
    }
}
