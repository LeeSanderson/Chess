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
    public class TestClassEntity : TestGroupingEntity, IDefinesTestContexts
    {
        public static readonly XName EntityXName = XNames.TestClass;

        Dictionary<string, TestContextEntityBase> _contexts;

        public TestClassEntity(XElement el)
            : base(el)
        {
        }

        #region Properties

        public TestAssemblyEntity OwningAssembly { get { return (TestAssemblyEntity)base.Parent; } }

        /// <summary>Gets the full name of the class, including namespace.</summary>
        public string ClassFullName { get { return DataElement.Attribute(XNames.AFullName).Value; } }
        public string ClassName
        {
            get
            {
                string fullName = ClassFullName;
                int lastIdx = fullName.LastIndexOf('.');
                return lastIdx == -1 ? fullName : fullName.Substring(lastIdx + 1);
            }
        }

        public override string DisplayName
        {
            get
            {
                string name = DataElement.Attribute(XNames.AFullName).Value;

                // Only return the actual class name
                int idx = name.LastIndexOf('.');
                if (idx != -1)
                    name = name.Substring(idx + 1);

                return name;
            }
        }

        #endregion

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            return DataElement.Elements(XNames.ChessContext, XNames.TestMethod);
        }

        protected override void OnChildrenLoaded()
        {
            base.OnChildrenLoaded();

            _contexts = this.EntitiesOfType<TestContextEntityBase>()
                .ToDictionary(ctx => ctx.Name); // If duplicate keys exist, an error will be thrown
        }

        protected override IEnumerable<XElement> DescendantXRuns()
        {
            return DataElement.Descendants(XSessionNames.MCutTestRun);
        }

        public override string GetInvocationDetails()
        {
            XElement copy = CopyDataElement(DataElement, true, XNames.TestMethod);

            return copy.ToString();
        }

        /// <summary>Gets all the test contexts defined on this class.</summary>
        public IEnumerable<TestContextEntityBase> GetContexts() { return _contexts.Values; }

    }
}
