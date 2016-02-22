using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Execution
{
    [AutoRegisterEntity]
    public class TestListEntity : TestGroupingEntity, IHasTestContainerSourceFile, IDefinesBuild
    {
        public static readonly XName EntityXName = XNames.Testlist;

        public TestListEntity(XElement el)
            : base(el)
        {
        }

        #region Properties

        public override string DisplayName { get { return (string)DataElement.Attribute(XNames.AName); } }

        public string SourceFilePath { get { return (string)DataElement.Attribute(XNames.ALocation); } }

        public BuildEntity BuildEntity { get { return this.EntityOfType<BuildEntity>(); } }

        #endregion

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            return DataElement.Elements(
                XNames.CustomBuild,
                XNames.MSBuild,
                XNames.Testlist,
                XNames.TestProject,
                XNames.TestAssembly,
                XNames.Include,
                XNames.Placeholder
                );
        }

        public override IEnumerable<TaskRunEntity> DescendantRuns()
        {
            // We don't contain runs our self and we don't know the exact type of the xrun
            // so we ignore the DescendantXRuns and just go thru the child entities.
            return GetChildEntities()
                .SelectMany(childEntity => childEntity.DescendantRunsAndSelf());
        }

        public override string GetInvocationDetails()
        {
            return CopyDataElement(DataElement, true
                    , XNames.Testlist
                    , XNames.TestProject
                    , XNames.TestAssembly
                )
                .ToString();
        }

        bool IHasTestContainerSourceFile.SupportsRefresh { get { return true; } }

    }
}
