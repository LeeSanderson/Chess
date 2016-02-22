using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    public class AppTaskController : IAppTaskController
    {

        #region Static Members

        /// <summary>
        /// Executes the task synchronously.
        /// This does not make use of a controller and therefore the <see cref="AppTask.ID"/>
        /// property will not get set.
        /// </summary>
        /// <param name="task">The task to run.</param>
        /// <param name="taskFolderPath">
        /// The folder path to use when running this task.
        /// If not specified, the <see cref="Environment.CurrentDirectory"/> will be used.
        /// </param>
        public static void ExecuteTaskInline(AppTask task, string taskFolderPath = null)
        {
            if (String.IsNullOrEmpty(taskFolderPath))
                taskFolderPath = Environment.CurrentDirectory;

            task.Setup(taskFolderPath);
            if (!task.IsComplete)    // i.e. has not completed with an error during setup.
                task.RunSynchronously();
        }

        #endregion

        private List<AppTask> _tasks = new List<AppTask>();
        private Queue<AppTask> _pendingTasks = new Queue<AppTask>();
        private Queue<AppTask> _waitingTasks = new Queue<AppTask>();
        private List<AppTask> _executingTasks = new List<AppTask>();

        private IEntityModel _model;
        IEntityModel IAppTaskController.Model { get { return _model; } }

        #region Constructors

        /// <summary>
        /// Creates a new controller of tasks.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="baseFolderPath">The base folder path to put task folders.</param>
        /// <param name="taskFolderNameFormatString"></param>
        /// <param name="taskFolderNameWildcardPattern"></param>
        public AppTaskController(IEntityModel model
            , string baseFolderPath
            , string taskFolderNameFormatString = "taskdir.{0:D7}"
            , string taskFolderNameWildcardPattern = "taskdir.*"
            )
        {
            if (model == null)
                throw new ArgumentNullException("model");
            if (String.IsNullOrEmpty(baseFolderPath))
                throw new ArgumentNullException("model");
            if (String.IsNullOrEmpty(taskFolderNameFormatString))
                throw new ArgumentNullException("taskFolderNameFormatString");
            if (String.IsNullOrEmpty(taskFolderNameWildcardPattern))
                throw new ArgumentNullException("taskFolderNameWildcardPattern");

            MaxConcurrentTasks = 1; // Default to sequential execution
            _model = model;

            BaseFolderPath = baseFolderPath;
            TaskFolderNameFormatString = taskFolderNameFormatString;
            TaskFolderNameWildcardPattern = taskFolderNameWildcardPattern;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Path to the folder in which all process tasks will run.
        /// </summary>
        public string BaseFolderPath { get; private set; }

        internal string TaskFolderNameFormatString { get; private set; }
        internal string TaskFolderNameWildcardPattern { get; private set; }

        private int _maxConcurrentTasks;
        /// <summary>
        /// Gets or sets the maximum number of tasks this controller will allow to execute.
        /// A value of 1 forces the tasks to execute sequentially.
        /// </summary>
        public int MaxConcurrentTasks
        {
            get { return _maxConcurrentTasks; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("MaxConcurrentTasks", value, "Must be greater than zero.");
                _maxConcurrentTasks = value;
            }
        }

        #endregion

        #region Events

        #region TaskStarted Event

        /// <summary>Occurs when a task has started to execute.</summary>
        public event AppTaskEventHandler TaskStarted;

        protected virtual void OnTaskStarted(AppTask task)
        {
            if (TaskStarted != null)
                TaskStarted(task, EventArgs.Empty);
        }

        #endregion

        #region TaskCompleted Event

        /// <summary>Occurs when a task has completed executing.</summary>
        public event AppTaskEventHandler TaskCompleted;

        protected virtual void OnTaskCompleted(AppTask task)
        {
            if (TaskCompleted != null)
                TaskCompleted(task, EventArgs.Empty);
        }

        #endregion

        #endregion

        /// <summary>
        /// Associates a task with this controller and schedules it for execution.
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(AppTask task)
        {
            RegisterTask(task);

            switch (task.Status)
            {
                case AppTaskStatus.Preparing:
                    _pendingTasks.Enqueue(task);
                    break;
                case AppTaskStatus.Setup:
                    // Shouldn't really ever happen, but just in case, set it to waiting here
                    task.SetWaiting();
                    break;
                case AppTaskStatus.Waiting:
                    _waitingTasks.Enqueue(task);
                    break;
                case AppTaskStatus.Running:
                    _executingTasks.Add(task);
                    break;
                case AppTaskStatus.Complete:
                case AppTaskStatus.Error:
                    // Finished tasks, no need to do anything more
                    break;
                default:
                    throw new NotImplementedException("Unknown enum value: " + task.Status);
            }
        }

        private void RegisterTask(AppTask task)
        {
            if (task.Controller != null)
                throw new InvalidOperationException("The task has already been added to a controller.");

            // Make sure the task has an ID
            int taskID;
            if (task.ID.HasValue)
            {
                // If an id exists, then we don't want to change it or its task folder path.
                taskID = task.ID.Value;
            }
            else
            {
                // Find a folder that doesn't exist
                string taskFolderPath;
                do
                {
                    taskID = _model.Session.GetNextTaskID();
                    taskFolderPath = Path.Combine(BaseFolderPath, String.Format(TaskFolderNameFormatString, taskID));
                } while (Directory.Exists(taskFolderPath));
            }

            _tasks.Add(task);
            task.StatusChanged += new AppTaskEventHandler<AppTaskStatusChangedEventArgs>(task_StatusChanged);
            task.TaskSetup += new AppTaskEventHandler(task_TaskSetup);
            task.Register(this, taskID);
        }

        void task_TaskSetup(AppTask task, EventArgs e)
        {
            task.SetWaiting();
        }

        void task_StatusChanged(AppTask task, AppTaskStatusChangedEventArgs e)
        {
            switch (e.CurrentStatus)
            {
                case AppTaskStatus.Preparing:
                case AppTaskStatus.Setup:
                    break; // Nothing to do

                case AppTaskStatus.Waiting:
                    _waitingTasks.Enqueue(task);
                    break;

                case AppTaskStatus.Starting:
                    _executingTasks.Add(task);
                    break;

                case AppTaskStatus.Running:
                    Debug.Assert(_executingTasks.Contains(task));
                    OnTaskStarted(task);
                    break;

                case AppTaskStatus.Complete:
                case AppTaskStatus.Cancelled:
                case AppTaskStatus.Error:
                    if (e.PreviousStatus == AppTaskStatus.Starting || e.PreviousStatus == AppTaskStatus.Running)
                        _executingTasks.Remove(task);
                    OnTaskCompleted(task);
                    AddContinuations(task);
                    break;

                default:
                    throw new NotImplementedException("The CurrentStatus is not handled: " + e.CurrentStatus);
            }
        }

        /// <summary>
        /// Adds any continuations registered with the task to get queued.
        /// </summary>
        private void AddContinuations(AppTask task)
        {
            foreach (Action<AppTask> continuation in task.GetContinuations(true))
            {
                AddTask(new ContinuationAppTask(task, continuation));
            }
        }

        private void SetupTask(AppTask task)
        {
            if (task.Status > AppTaskStatus.Waiting)
                throw new InvalidOperationException("The task is already setup.");


            // Setup the task
            string taskFolderPath = task.TaskFolderPath;
            if (String.IsNullOrEmpty(taskFolderPath))
                taskFolderPath = Path.Combine(BaseFolderPath, String.Format(TaskFolderNameFormatString, task.ID));

            task.Setup(taskFolderPath);
        }


        #region IAppTaskController Members

        public IEnumerable<AppTask> EnumerateTasks()
        {
            return _tasks;
        }

        private int _isProcessingTasks;
        public void ProcessTasks()
        {
            if (System.Threading.Interlocked.CompareExchange(ref _isProcessingTasks, 1, 0) != 0)
                throw new InvalidOperationException("ProcessTasks is already executing. Be sure it can only be called autonomously.");

            // Process pending tasks first
            while (_pendingTasks.Count != 0)
            {
                var task = _pendingTasks.Dequeue();
                SetupTask(task);
                Debug.Assert(task.Status >= AppTaskStatus.Setup);
            }

            // Allow the running tasks to complete
            // NOTE: Need to actualize _executingTasks so if it changes the enumerator is still valid
            foreach (var task in _executingTasks.ToArray())
                task.TryWait(); // Non-blocking

            // Allow waiting tasks to start if possible
            ScheduleWaitingTasks();
            System.Threading.Interlocked.Decrement(ref _isProcessingTasks);
        }

        private void ScheduleWaitingTasks()
        {
            while (_executingTasks.Count < MaxConcurrentTasks && _waitingTasks.Count != 0)
            {
                var task = _waitingTasks.Dequeue();
                task.Start();
                Debug.Assert(task.Status >= AppTaskStatus.Running);
            }
        }

        public void WaitForAllTasksToComplete()
        {
            while (_executingTasks.Count != 0 || _waitingTasks.Count != 0 || _pendingTasks.Count != 0)
            {
                ProcessTasks();
                System.Threading.Thread.Sleep(1000);
            }
        }

        #endregion

        public void DeleteAllTaskFolders()
        {
            // Delete all the existing task folders
            foreach (var taskDir in Directory.EnumerateDirectories(BaseFolderPath, TaskFolderNameWildcardPattern))
            {
                try { Directory.Delete(taskDir, true); }
                catch { }
            }
        }

    }
}
