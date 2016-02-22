using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Xml.Linq;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// When regression test asserts are enabled, this entity will assert the final
    /// result of a test.
    /// </summary>
    [AutoRegisterEntity]
    public sealed class ExpectedRegressionTestResultEntity : EntityBase
    {
        public static readonly XName EntityXName = XNames.ExpectedRegressionTestResult;

        public ExpectedRegressionTestResultEntity(XElement el)
            : base(el)
        {

        }

        /// <summary>
        /// Gets the expected result types for the test.
        /// It is possible that more than one result type can be valid.
        /// A null value indicates 
        /// </summary>
        public TestResultType[] ResultTypes
        {
            get
            {
                string results = (string)DataElement.Attribute(XNames.AResultType);
                if (String.IsNullOrWhiteSpace(results))
                    return new TestResultType[0];

                return results
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => (TestResultType)Enum.Parse(typeof(TestResultType), r))
                    .ToArray()
                    ;
            }
        }

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            yield break;
        }

    }
}
