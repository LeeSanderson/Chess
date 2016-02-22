using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using Microsoft.Concurrency.TestTools.Execution.AppTasks;

namespace Microsoft.Concurrency.TestTools.Execution
{
    public abstract class TaskRunEntity : EntityBase
    {

        public static readonly XName[] TaskRunXNames = new[]{
            XSessionNames.MCutTestRun,
            XSessionNames.BuildRun,
        };

        private XElement _xtaskState;

        #region Constructors

        public TaskRunEntity(XElement el)
            : base(el)
        {
        }

        #endregion

        #region Properties

        public override string DisplayName { get { return "Task"; } }

        protected XElement XTaskState
        {
            get
            {
                // In general, if it exists, it won't be deleted
                if (_xtaskState == null)
                    _xtaskState = DataElement.Element(XSessionNames.TaskState);
                return _xtaskState;
            }
        }

        /// <summary>The task bound to this run.</summary>
        public AppTask Task
        {
            get { return XTaskState.Annotation<AppTask>(); }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Cannot set to null.");
                if (Task != null)
                    throw new InvalidOperationException("The Task has already been set for this run.");
                XTaskState.AddAnnotation(value);
            }
        }

        public int TaskID { get { return (int)XTaskState.Attribute(XSessionNames.ATaskID); } }
        public string TaskFolderPath { get { return (string)XTaskState.Element(XSessionNames.TaskFolderPath); } }
        public AppTaskStatus TaskStatus { get { return XTaskState.Attribute(XSessionNames.ATaskStatus).ParseXmlEnum<AppTaskStatus>().Value; } }

        #endregion

        public override IEnumerable<TaskRunEntity> DescendantRunsAndSelf()
        {
            return new[] { this }
                .Concat(base.DescendantRunsAndSelf());
        }

        public void AssociateTaskFromRun(TaskRunEntity run)
        {
            if (Task == null)
                Task = AppTaskBuilder.BuildTaskFromRun(run);
        }

        public override void Delete()
        {
            if (Task != null)
            {
                Task.Delete();
                ((IDisposable)Task).Dispose();
            }

            base.Delete();
        }

    }
}
