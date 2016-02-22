using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    public interface IAppTaskController
    {

        IEntityModel Model { get; }
        int MaxConcurrentTasks { get; set; }

        /// <summary>Enumerates thru all tasks controlled by this instance.</summary>
        IEnumerable<AppTask> EnumerateTasks();

        /// <summary>Adds a task to be executed by the controller.</summary>
        void AddTask(AppTask task);

        /// <summary>Allows the controller to process its registered tasks, update statuses, exec waiting tasks, etc.</summary>
        void ProcessTasks();

        /// <summary>Waits for all tasks to finish executing.</summary>
        void WaitForAllTasksToComplete();

        /// <summary>Occurs when a task has started to execute.</summary>
        event AppTaskEventHandler TaskStarted;
        /// <summary>Occurs when a task has completed executing.</summary>
        event AppTaskEventHandler TaskCompleted;

    }
}
