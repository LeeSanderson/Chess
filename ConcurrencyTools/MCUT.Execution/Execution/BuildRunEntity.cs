using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using System.Text;
using System.IO;

namespace Microsoft.Concurrency.TestTools.Execution
{
    [AutoRegisterEntity]
    public class BuildRunEntity : TaskRunEntity
    {

        public static readonly XName EntityXName = XSessionNames.BuildRun;

        #region Constructors

        public BuildRunEntity(XElement el)
            : base(el)
        {
        }

        #endregion

        public override string DisplayName { get { return (IsRebuild ? "Rebuild " : "Build ") + OwnerBuild.DisplayName; } }

        public IBuildableEntity OwnerBuild { get { return (IBuildableEntity)base.Parent; } }

        public bool IsRebuild { get { return (bool?)DataElement.Attribute(XSessionNames.AIsRebuild) ?? false; } }

        new public AppTasks.BuildAppTaskBase Task
        {
            get { return (AppTasks.BuildAppTaskBase)base.Task; }
            set { base.Task = value; }
        }

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            yield break;
        }

        public override string GetInvocationDetails()
        {
            StringBuilder sb = new StringBuilder();

            var startScriptPath = Path.Combine(Task.TaskFolderPath, Task.StartScriptFilename);
            sb.AppendLine(" " + Task.StartScriptFilename);
            sb.AppendLine(new string('*', Task.StartScriptFilename.Length + 2));
            if (File.Exists(startScriptPath))
                sb.Append(File.ReadAllText(startScriptPath));
            sb.AppendLine();

            // And finally add the run's xml
            sb.AppendLine(" Run Xml");
            sb.AppendLine("*********");
            sb.AppendLine(CopyDataElement(DataElement, true).ToString());

            return sb.ToString();
        }

    }
}
