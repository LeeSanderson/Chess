using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Execution
{
    // TODO: create an NMake build element. This would also want to use the GetVSCmdLineVarsFilePath() in one of the scripts lines.

    /// <summary>
    /// Represents the base implementation of a run that performs a build.
    /// </summary>
    public abstract class BuildEntity : EntityBase, IBuildableEntity
    {

        public BuildEntity(XElement el)
            : base(el)
        {
        }

        bool IBuildableEntity.IsBuildable { get { return true; } }

        public bool SupportsRebuild { get; protected set; }

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            return DataElement.Elements(XSessionNames.BuildRun);
        }

        protected override IEnumerable<XElement> DescendantXRuns()
        {
            return DataElement.Descendants(XSessionNames.BuildRun);
        }

        public abstract AppTasks.BuildAppTaskBase CreateBuildTask();

    }
}
