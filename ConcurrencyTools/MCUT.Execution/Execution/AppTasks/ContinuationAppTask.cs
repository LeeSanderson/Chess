using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    /// <summary>
    /// Represents a task that is created from a continuation of another task.
    /// </summary>
    public class ContinuationAppTask : AppTask
    {

        private AppTask _antecedentTask;
        private Action<AppTask> _body;
        private System.Threading.Tasks.Task _tplTask;

        internal ContinuationAppTask(AppTask antecedentTask, Action<AppTask> body)
        {
            if (antecedentTask == null)
                throw new ArgumentNullException("antecendantTask");
            if (body == null)
                throw new ArgumentNullException("body");

            _antecedentTask = antecedentTask;
            _body = body;
        }

        protected override void OnRunSynchronously()
        {
            if (_antecedentTask.Status < AppTaskStatus.Complete)
                throw new InvalidOperationException("Cannot execute a continuation task until it's antecedent task has completed.");
            _body(_antecedentTask);
        }

        protected override void OnStart()
        {
            if (_antecedentTask.Status < AppTaskStatus.Complete)
                throw new InvalidOperationException("Cannot execute a continuation task until it's antecedent task has completed.");
            _tplTask = System.Threading.Tasks.Task.Factory.StartNew(() => _body(_antecedentTask));
        }

        protected override bool IsFinishedExecuting()
        {
            return _tplTask.IsCompleted;
        }

        protected override void OnWait()
        {
            _tplTask.Wait();
        }


    }
}
