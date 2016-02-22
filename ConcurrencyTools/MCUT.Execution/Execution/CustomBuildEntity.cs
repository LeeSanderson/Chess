using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// The generic build element in a testlist.
    /// </summary>
    [AutoRegisterEntity]
    public class CustomBuildEntity : BuildEntity
    {
        public static readonly XName EntityXName = XNames.CustomBuild;

        public CustomBuildEntity(XElement el)
            : base(el)
        {
        }

        public override string DisplayName { get { return (string)DataElement.Attribute(XNames.AName) ?? "Custom"; } }

        public TestListEntity OwningTestList { get { return (TestListEntity)Parent; } }

        public string WorkingFolderPath
        {
            get
            {
                XElement xstartDir = DataElement.Element(XNames.WorkingDirectory);
                if (xstartDir != null)
                    return (string)xstartDir;

                // If not explicitly declared on the build element then use
                // the path to the containing task list.
                var tl = DataElement.Ancestors(XNames.Testlist)
                    .Where(xtl => xtl.Attribute(XNames.ALocation) != null)
                    .SelectEntities<TestListEntity>()
                    .First();   // There has to be at least one with a location specified
                return Path.GetDirectoryName(tl.SourceFilePath);
            }
        }

        public string ExecutablePath { get { return (string)DataElement.Element(XNames.Executable); } }

        public IEnumerable<string> GetShellLines()
        {
            return DataElement
                .Elements(XNames.Shellline)
                .SelectXValues();
        }

        public IEnumerable<string> GetCommandLineArgs()
        {
            return DataElement
                .Elements(XNames.Arg)
                .SelectXValues();
        }

        public override AppTasks.BuildAppTaskBase CreateBuildTask()
        {
            return new AppTasks.CustomBuildAppTask() {
                BuildEntity = this,
                XParent = this.DataElement,
            };
        }

    }
}
