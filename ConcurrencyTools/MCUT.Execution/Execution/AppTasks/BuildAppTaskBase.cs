using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    /// <summary>
    /// Represents a task used to run a test case via mcut on a separate process.
    /// </summary>
    public abstract class BuildAppTaskBase : AppScriptProcessTask
    {

        protected BuildAppTaskBase()
        {
        }

        protected BuildAppTaskBase(BuildRunEntity run)
            : base(run.DataElement.Element(XSessionNames.TaskState))
        {
            Run = run;
            XParent = run.DataElement.Parent;

            IsRebuild = (bool?)run.DataElement.Attribute(XSessionNames.AIsRebuild) ?? false;
        }

        public BuildRunEntity Run { get; private set; }

        public XElement XParent { get; set; }

        public bool IsRebuild { get; set; }

        #region Task Execution

        protected override void OnValidate()
        {
            base.OnValidate();
            if (XParent == null)
                throw new InvalidOperationException("The ParentEntity property has not been set yet.");
        }

        protected override void OnPerformSetup()
        {
            base.OnPerformSetup();

            // Create the run entity
            XElement xrun = CreateXRun();

            Run = (BuildRunEntity)Controller.Model.EntityBuilder.CreateEntityAndBindToElement(xrun);
            Run.LoadChildren(true);
            // Need to also set our self so the Run has access to it before it adds itself to the xml tree.
            Run.Task = this;

            // Add the run to the parent entity.
            if (XParent != null)
                XParent.Add(Run.DataElement);
        }

        #endregion

        protected XElement CreateXRun()
        {
            return new XElement(XSessionNames.BuildRun
                , new XAttribute(XSessionNames.AIsRebuild, IsRebuild)
                , base.XTaskState   // Add on the task's state
                );
        }

    }
}
