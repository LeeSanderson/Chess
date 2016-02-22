using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.Execution.AppTasks;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    /// <summary>
    /// Base Command that spawns a task to be executed against the model's task controller.
    /// </summary>
    abstract class SpawnAppTaskCommand<TTask> : Command
        where TTask : AppTask
    {

        protected SpawnAppTaskCommand(bool interactive)
            : base(interactive)
        {
        }

        /// <summary>The task to spawn.</summary>
        public TTask Task { get; private set; }

        /// <summary>Indicates whether the run should be selected in the UI once spawned.</summary>
        public bool SelectRun { get; set; }

        protected abstract TTask CreateTask();

        protected override bool PerformExecute(Model model)
        {
            Task = CreateTask();
            if (Task == null)
                throw new NotImplementedException("This command must set the Task property before PerformExecute gets called.");

            // start a run
            model.tasksController.AddTask(Task);

            // select the submitted run
            if (SelectRun)
                model.selection.SelectRunOnTaskSetup(Task);

            return true;
        }

    }
}
