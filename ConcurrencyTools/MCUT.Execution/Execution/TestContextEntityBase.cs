using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.Execution
{
    public abstract class TestContextEntityBase : EntityBase
    {

        protected TestContextEntityBase(XElement el)
            : base(el)
        {
        }

        /// <summary>
        /// Will never be null. Empty string indicates the default for the test.
        /// </summary>
        public string Name { get { return (string)DataElement.Attribute(XNames.AName) ?? String.Empty; } }
        public override string DisplayName { get { return Name; } }

        /// <summary>
        /// The key to the expected result for this test.
        /// null indicates none expected. An empty string indicates to use the default expected result for the test.
        /// </summary>
        public string ExpectedResultKey { get { return (string)DataElement.Attribute(XNames.AExpectedResultKey); } }

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            yield break;
        }

    }

    public class NullTestContextEntity : TestContextEntityBase
    {

        public static NullTestContextEntity CreateDefaultInstance()
        {
            return new NullTestContextEntity(new XElement(XNames.NULL));
        }

        private NullTestContextEntity(XElement el)
            : base(el)
        {
        }

    }

}
