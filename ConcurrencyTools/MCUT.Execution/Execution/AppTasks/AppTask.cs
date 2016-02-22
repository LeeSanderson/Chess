using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    public abstract class AppTask : IDisposable
    {

        public AppTask()
        {
            XTaskState = new XElement(XSessionNames.TaskState
                , new XAttribute(XSessionNames.AType, GetTaskTypeName())
                // Need to initialize this here so we don't get an ex from setting it via the property when there is no attribute.
                , new XAttribute(XSessionNames.ATaskStatus, AppTaskStatus.Preparing)
                );
        }

        protected AppTask(XElement xtaskState)
        {
            if (xtaskState == null)
                throw new ArgumentNullException();
            if (xtaskState.Name != XSessionNames.TaskState)
                throw new ArgumentException("The xtaskState element should be of the name " + XSessionNames.TaskState);

            // Make sure the required attributes are set
            if (xtaskState.Attribute(XSessionNames.ATaskStatus) == null)
                throw new ArgumentException("xtaskState missing the attribute " + XSessionNames.ATaskStatus.LocalName);

            XTaskState = xtaskState;

            // Make sure we reset ourselves
            XTaskState.SetAttributeValue(XSessionNames.AType, GetTaskTypeName());

            // And set state that's not serialized to xml
            IsComplete = Status == AppTaskStatus.Complete
                || Status == AppTaskStatus.Error
                || Status == AppTaskStatus.Cancelled;

            // Just incase the boolean flag wasn't set on the xml, See if we're setup via the
            // current state.
            if (!IsSetup)
                IsSetup = Status >= AppTaskStatus.Setup && Status <= AppTaskStatus.Running;
        }

        private string GetTaskTypeName()
        {
            Type taskType = this.GetType();

            // This is used in conjunction with the Type.GetType method which says that if the type is within
            // the executing assembly, then it's sufficient to just specify the full name. Otherwise, we
            // need to specify the assembly name too.
            if (taskType.Assembly == System.Reflection.Assembly.GetExecutingAssembly())
                return taskType.FullName;
            else
                return taskType.AssemblyQualifiedName;
        }

        #region Properties

        public IAppTaskController Controller { get; private set; }

        protected XElement XTaskState { get; private set; }

        public int? ID
        {
            get { return (int?)XTaskState.Attribute(XSessionNames.ATaskID); }
            private set { XTaskState.SetAttributeValue(XSessionNames.ATaskID, value); }
        }

        public AppTaskStatus Status
        {
            get { return XTaskState.Attribute(XSessionNames.ATaskStatus).ParseXmlEnum<AppTaskStatus>().Value; }
            private set
            {
                AppTaskStatus oldStatus = Status;
                if (value != oldStatus)
                {
                    if (value < oldStatus)
                        throw new ArgumentException("Cannot set the status to an earlier state.");

                    XTaskState.SetAttributeValue(XSessionNames.ATaskStatus, value);
                    OnStatusChanged(oldStatus, value);
                }
            }
        }

        public bool IsSetup
        {
            get { return (bool?)XTaskState.Attribute(XSessionNames.AIsSetup) ?? false; }
            private set { XTaskState.SetAttributeValue(XSessionNames.AIsSetup, value); }
        }

        public bool IsComplete { get; private set; }

        public XElement XError { get { return XTaskState.Element(XNames.Error); } }
        public Exception ErrorEx { get; private set; }

        /// <summary>Indicates whether this task type uses the working folder.</summary>
        /// <remarks>Note to inheritors: set to true to have the folder automatically created.</remarks>
        public bool UsesTaskFolder { get; protected set; }
        /// <summary>Gets the full path to the folder in which the process is executed.</summary>
        public string TaskFolderPath
        {
            get { return (string)XTaskState.Element(XSessionNames.TaskFolderPath); }
            private set { XTaskState.SetElementValue(XSessionNames.TaskFolderPath, value); }
        }

        /// <summary>
        /// Indicates whether this instance should attempt to delete itself (e.g. its task folder) when disposed.
        /// The default is false.
        /// </summary>
        public bool DeleteOnDispose { get; set; }

        #endregion

        #region Events

        #region StatusChanged Event

        /// <summary>Occurs when the status of this task has changed.</summary>
        public event AppTaskEventHandler<AppTaskStatusChangedEventArgs> StatusChanged;

        protected virtual void OnStatusChanged(AppTaskStatus prevStatus, AppTaskStatus newStatus)
        {
            if (StatusChanged != null)
                StatusChanged(this, new AppTaskStatusChangedEventArgs(prevStatus, newStatus));
        }

        #endregion

        #region TaskSetup Event

        /// <summary>Occurs when this task has been successfully setup.</summary>
        public event AppTaskEventHandler TaskSetup;

        protected virtual void OnTaskSetup()
        {
            if (TaskSetup != null)
                TaskSetup(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        #region Registration and Setup

        internal void Register(IAppTaskController controller, int taskID)
        {
            if (controller == null)
                throw new ArgumentNullException("controller");
            if (taskID <= 0)
                throw new ArgumentOutOfRangeException("taskID", taskID, "ID must be greater than zero.");
            if (this.Controller != null)
                throw new InvalidOperationException("This task has already been registered.");
            if (this.ID.HasValue && taskID != ID)
                throw new InvalidOperationException("This task already has a task ID and the 'taskID' parameter is different. Cannot change the task ID once assigned.");

            Controller = controller;
            ID = taskID;
        }

        internal void Setup(string taskFolderPath)
        {
            if (Status >= AppTaskStatus.Setup)
                throw new InvalidOperationException("This task has already been setup.");

            if (UsesTaskFolder)
            {
                if (String.IsNullOrEmpty(taskFolderPath))
                    throw new ArgumentException("A folder path must be specified for tasks that use a task folder.", "taskFolderPath");
                TaskFolderPath = taskFolderPath;
            }

            try
            {
                OnValidate();

                // Create the working folder before calling OnSetup so there's more flexibility when setting up the task folder
                // because some test tasks will need to have the folder existing before performing a base.OnSetup().
                if (UsesTaskFolder)
                    Directory.CreateDirectory(TaskFolderPath);

                OnPerformSetup();

                IsSetup = true;
                Status = AppTaskStatus.Setup;
                OnTaskSetup();
            }
            catch (Exception ex)
            {
                CompleteWithError(ex);
            }
        }

        protected virtual void OnValidate()
        {
        }

        protected virtual void OnPerformSetup()
        {
        }

        #endregion

        /// <summary>Set by the AppTask controller.</summary>
        internal void SetWaiting()
        {
            if (Status >= AppTaskStatus.Running)
                throw new InvalidOperationException("WARNING: Shouldn't set a test to waiting if it's running or has completed.");

            Status = AppTaskStatus.Waiting;
        }

        #region RunSynchronously()

        public void RunSynchronously()
        {
            if (Status < AppTaskStatus.Setup)
                throw new InvalidOperationException("This task has not been setup yet.");
            if (Status >= AppTaskStatus.Starting)
                throw new InvalidOperationException("This task has already started running or has completed.");

            Status = AppTaskStatus.Running;
            try
            {
                OnRunSynchronously();
                Debug.Assert(Status == AppTaskStatus.Running);
                Complete();
            }
            catch (Exception ex)
            {
                CompleteWithError(ex);
            }

            Debug.Assert(Status == AppTaskStatus.Complete || Status == AppTaskStatus.Error);
        }

        protected abstract void OnRunSynchronously();

        #endregion

        #region Start()

        public void Start()
        {
            if (Status < AppTaskStatus.Setup)
                throw new InvalidOperationException("This task has not been setup yet.");
            if (Status >= AppTaskStatus.Starting)
                throw new InvalidOperationException("This task has already started or has completed.");

            Status = AppTaskStatus.Starting;
            try
            {
                OnStart();
                Status = AppTaskStatus.Running;
            }
            catch (Exception ex)
            {
                CompleteWithError(ex);
            }
        }

        protected abstract void OnStart();

        #endregion

        #region Wait/TryWait()

        /// <summary>
        /// Attempts to Wait on this task to complete if it's finished executing.
        /// If the execution is finished, this method performs the call to <see cref="Wait"/>.
        /// </summary>
        /// <returns>true, if the execution finished; otherwise, false.</returns>
        internal bool TryWait()
        {
            if (Status < AppTaskStatus.Running)
                throw new InvalidOperationException("This task is not currently running.");

            if (Status == AppTaskStatus.Running && IsFinishedExecuting())
            {
                Wait();
                return true;
            }

            return Status > AppTaskStatus.Running;
        }

        /// <summary>
        /// Determines whether this running task has finished executing so it
        /// can be completed.
        /// </summary>
        /// <returns></returns>
        protected abstract bool IsFinishedExecuting();

        /// <summary>
        /// Waits until this task has finished.
        /// </summary>
        public void Wait()
        {
            if (Status < AppTaskStatus.Running)
                throw new InvalidOperationException("This task is not currently running.");

            if (Status == AppTaskStatus.Running)
            {
                try
                {
                    OnWait();
                    Debug.Assert(Status == AppTaskStatus.Running);
                    Complete();
                }
                catch (Exception ex)
                {
                    CompleteWithError(ex);
                }
            }

            Debug.Assert(Status == AppTaskStatus.Complete || Status == AppTaskStatus.Error);
        }

        protected abstract void OnWait();

        #endregion

        #region Complete/Completing, etc

        private void Complete()
        {
            // We're already calling this from within a try-catch block, so no need to put another here.
            // If we allow a derived class to state when it's complete, we may need to add the try-catch to that method

            Debug.Assert(Status == AppTaskStatus.Running || Status == AppTaskStatus.Starting, "Should only be able to call Complete when a task is running.");

            OnCompleting();
            IsComplete = true;
            Status = AppTaskStatus.Complete;
            OnCompleted();
        }

        private void CompleteWithError(Exception ex)
        {
            Debug.Assert(ex != null);
            ErrorEx = ex;

            // Create the node and add it to our state xml
            var xerror = XNames.CreateXError(ex);
            XTaskState.Add(xerror);

            IsComplete = true;
            Status = AppTaskStatus.Error;

            OnCompletedWithError();
        }

        /// <summary>
        /// Allows a task to do any custom processing to complete a task.
        /// </summary>
        protected virtual void OnCompleting() { }
        protected virtual void OnCompleted() { }
        protected virtual void OnCompletedWithError() { }

        #endregion

        /// <summary>
        /// Tries to cancel this task.
        /// The return value indicates whether the task was cancelled successfully or not.
        /// </summary>
        /// <returns>true if this task is already complete, or was successfully cancelled; otherwise, false.</returns>
        public bool Cancel()
        {
            if (IsComplete)
                return true; // Nothing to do because it's already done.

            if (Status == AppTaskStatus.Running)
            {
                // Allow the task to finish executing if possible
                if (TryWait())
                    return true;    // Finished normally w/o needing to actually cancel

                // Otherwise, give the task the opportunity to cancel.
                if (!OnCancelling())
                    return false;
            }

            IsComplete = true;
            Status = AppTaskStatus.Cancelled;
            OnCancelled();
            return true;
        }

        /// <summary>
        /// When overridden in a derived class, performs actions necessary to cancel this task.
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnCancelling() { return true; }
        protected virtual void OnCancelled() { }

        public void Delete()
        {
            if (Status >= AppTaskStatus.Starting && Status <= AppTaskStatus.Running)
                throw new InvalidOperationException("Cannot delete a task when it's running.");

            DeleteTaskFolder();
        }

        private void DeleteTaskFolder()
        {
            try
            {
                Directory.Delete(TaskFolderPath, true);
            }
            catch { } // Ignore any errors if we couldn't delete the folder. Not a big deal usually
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Dispose();
        }

        protected virtual void Dispose()
        {
            if (UsesTaskFolder && DeleteOnDispose)
                DeleteTaskFolder();
        }

        #endregion

        private List<Action<AppTask>> _continuations;

        /// <summary>
        /// Registers an action to perform after this task has completed.
        /// Use the passed in argument to get the AppTask.Status of how the task completed.
        /// </summary>
        /// <param name="body"></param>
        public void ContinueWith(Action<AppTask> body)
        {
            if (_continuations == null)
                _continuations = new List<Action<AppTask>>();

            _continuations.Add(body);
        }

        /// <summary>
        /// Gets an enumeration of any continuation actions registered with this task.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<Action<AppTask>> GetContinuations(bool andClear)
        {
            if (_continuations == null)
                return Enumerable.Empty<Action<AppTask>>();
            else if (andClear)
            {
                var ie = _continuations;
                _continuations = null;
                return ie;
            }
            else
                return _continuations;
        }

    }
}
