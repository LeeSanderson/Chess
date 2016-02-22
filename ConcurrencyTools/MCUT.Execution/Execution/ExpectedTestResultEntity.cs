using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Represents the basic 
    /// </summary>
    [AutoRegisterEntity]
    public sealed class ExpectedTestResultEntity : EntityBase
    {
        public static readonly XName EntityXName = XNames.ExpectedTestResult;

        public ExpectedTestResultEntity(XElement el)
            : base(el)
        {
        }

        /// <summary>
        /// Will never be null. Empty string indicates the default for the test.
        /// </summary>
        public string Key { get { return (string)DataElement.Attribute(XNames.AKey) ?? String.Empty; } }

        /// <summary>The expected result type. This must always be specified according to the schema.</summary>
        public TestResultType ResultType { get { return DataElement.Attribute(XNames.AResultType).ParseXmlEnum<TestResultType>().Value; } }
        public string Message { get { return (string)DataElement.Element(XTestResultNames.ResultMessage); } }

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            yield break;
        }

    }
}
