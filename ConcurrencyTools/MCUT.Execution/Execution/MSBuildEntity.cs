using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.Execution
{
    [AutoRegisterEntity]
    public class MSBuildEntity : BuildEntity, IMSBuildSettings
    {
        public static readonly XName EntityXName = XNames.MSBuild;

        public MSBuildEntity(XElement el)
            : base(el)
        {
            SupportsRebuild = true;
        }

        public override string DisplayName { get { return Path.GetFileName(ProjectFullPath); } }

        /// <summary>Gets the full path to the project or solution this element is to build.</summary>
        public string ProjectFullPath { get { return Path.GetFullPath(DataElement.Attribute(XNames.AProject).Value); } }

        public string Configuration { get { return (string)DataElement.Attribute(XNames.AConfiguration); } }
        public string MSBuildProperties { get { return (string)DataElement.Element(XNames.MSBuildProperties); } }

        public override AppTasks.BuildAppTaskBase CreateBuildTask()
        {
            return new AppTasks.MSBuildAppTask() {
                MSBuildSettings = this,
                XParent = this.DataElement
            };
        }


        #region IMSBuildSettings Members

        string IMSBuildSettings.ProjectFullPath { get { return this.ProjectFullPath; } }
        string IMSBuildSettings.Configuration { get { return this.Configuration; } }
        string IMSBuildSettings.Platform { get { return null; } }
        string IMSBuildSettings.OtherProperties { get { return this.MSBuildProperties; } }

        #endregion
    }
}
