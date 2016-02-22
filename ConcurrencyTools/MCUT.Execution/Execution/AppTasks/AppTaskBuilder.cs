using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    /// <summary>
    /// Builds a task from a task run entity.
    /// </summary>
    public static class AppTaskBuilder
    {

        public static AppTask BuildTaskFromRun(TaskRunEntity run)
        {
            var xtaskState = run.DataElement.Element(XSessionNames.TaskState);
            if (xtaskState == null)
                throw new ArgumentException("The run doesn't have a TaskState child element.");

            string typeName = (string)xtaskState.Attribute(XSessionNames.AType);
            Type taskType = Type.GetType(typeName);
            if (taskType == null)
                throw new ArgumentException("Could not find the task type: " + typeName);
            if (!typeof(AppTask).IsAssignableFrom(taskType))
                throw new ArgumentException(String.Format("The run's task type {0} must inherit from {1}.", taskType.Name, typeof(AppTask).Name));

            // Try to find a constructor that takes a run entity
            Type runType = run.GetType();
            var validCtors = (from ctor in taskType.GetConstructors()
                              let parameters = ctor.GetParameters()
                              where parameters.Length == 1
                              where parameters[0].ParameterType.IsAssignableFrom(runType)
                              select ctor)
                            .ToArray();
            if (validCtors.Length == 0)
                throw new ArgumentException(String.Format("Could not create task of type {0} because a constructor that accepts a {1} doesn't exist or is not visible from the MCUT.Execution assembly.", taskType.Name, typeof(TaskRunEntity).Name));
            if (validCtors.Length > 1)
                throw new ArgumentException(String.Format("Could not create task of type {0} because more than one constructor accepts a {1} as its only parameter.", taskType.Name, typeof(TaskRunEntity).Name));
            var ctorInfo = validCtors[0];

            return (AppTask)ctorInfo.Invoke(new[] { run });
        }

    }
}
