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
    public class TestAssemblyEntity : TestGroupingEntity, IHasTestContainerSourceFile
    {
        public static readonly XName EntityXName = XNames.TestAssembly;

        public TestAssemblyEntity(XElement el)
            : base(el)
        {
        }

        #region Properties

        /// <summary>Gets the <see cref="TestProjectEntity"/> for this assembly if it was created from one.</summary>
        public TestProjectEntity OwningProject { get { return base.Parent as TestProjectEntity; } }

        public override string DisplayName
        {
            get { return Path.GetFileNameWithoutExtension(SourceFilePath); }
        }

        public string SourceFilePath { get { return DataElement.Attribute(XNames.ALocation).Value; } }

        /// <summary>
        /// Gets the id for this entity within the session.
        /// </summary>
        public int EntityID
        {
            get { return (int?)DataElement.Attribute(XSessionNames.AEntityID) ?? 0; }
            set { DataElement.SetAttributeValue(XSessionNames.AEntityID, value); }
        }

        /// <summary>
        /// Gets the folder path (relative to the current ISessionEntity.FolderPath) to
        /// store all observation files generated from this test assembly.
        /// </summary>
        public string ObservationFilesFolderPath
        {
            get { return (string)DataElement.Attribute(XSessionNames.AObservationsFolderPath); }
            set { DataElement.SetAttributeValue(XSessionNames.AObservationsFolderPath, value); }
        }

        /// <summary>Gets the full path to the observation files for this assembly.</summary>
        public string ObservationFilesFullFolderPath
        {
            get
            {
                string assyFldrPath = ObservationFilesFolderPath;
                if (String.IsNullOrEmpty(assyFldrPath))
                    return null;
                return Path.Combine(Model.Session.FolderPath, assyFldrPath);
            }
        }

        /// <summary>
        /// Gets the relative folder path (relative to the SourceFilePath) to the golden
        /// observation files folder for this test assembly.
        /// </summary>
        public string GoldenObservationFilesFolderPath { get { return (string)DataElement.Element(XNames.GoldenObservationFilesFolderPath); } }

        /// <summary>Gets the full path to the observation files for this assembly if it has been specified.</summary>
        public string GoldenObservationFilesFullFolderPath
        {
            get
            {
                string goldenFldrPath = GoldenObservationFilesFolderPath;
                if (String.IsNullOrEmpty(goldenFldrPath))
                    return null;
                return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(SourceFilePath), goldenFldrPath));
            }
        }

        #endregion

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            return DataElement.Elements(
                XNames.TestClass
                , XNames.Placeholder
                , XConcurrencyNames.Error
                );
        }

        protected override IEnumerable<XElement> DescendantXRuns()
        {
            return DataElement.Descendants(XSessionNames.MCutTestRun);
        }

        public override string GetInvocationDetails()
        {
            return CopyDataElement(DataElement, true, XNames.TestClass)
                .ToString();
        }

        bool IHasTestContainerSourceFile.SupportsRefresh { get { return true; } }

        /// <summary>Finds the test method with the specified full name.</summary>
        /// <param name="methodFullName">The full name of the method, i.e. "ClassFullName.MethodName"</param>
        /// <returns>The test method entity with the name specified; otherwise, null.</returns>
        internal TestMethodEntity FindTestMethod(string methodFullName)
        {
            int idx = methodFullName.LastIndexOf('.');
            string fullClassName = methodFullName.Substring(0, idx);
            string methodName = methodFullName.Substring(idx + 1);

            return this.EntitiesOfType<TestClassEntity>()
                .Where(e => e.ClassFullName == fullClassName)
                .SelectMany(e => e.EntitiesOfType<TestMethodEntity>())
                .Where(e => e.MethodName == methodName)
                .SingleOrDefault();
        }

    }
}
