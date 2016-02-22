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
    /// <summary>
    /// Represents the base implementation of a run that performs a build.
    /// </summary>
    public class ErrorEntity : EntityBase
    {

        public ErrorEntity(XElement el)
            : base(el)
        {
        }

        /// <summary>The display message for this error.</summary>
        public string Message { get { return (string)DataElement.Element(XConcurrencyNames.ErrorMessage); } }

        /// <summary>Indicates whether this instance has inner error entities defined.</summary>
        public bool HasInnerErrors { get { return DataElement.Elements(XConcurrencyNames.Error).Any(); } }
        public int InnerErrorsCount { get { return DataElement.Elements(XConcurrencyNames.Error).Count(); } }

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            return DataElement.Elements(XConcurrencyNames.Error);
        }

    }

    /// <summary>Represents an error that describes a thrown exception.</summary>
    public class ExceptionErrorEntity : ErrorEntity
    {

        public ExceptionErrorEntity(XElement el)
            : base(el)
        {
        }

        public string ExceptionTypeName { get { return (string)DataElement.Attribute(XConcurrencyNames.AErrorExceptionType); } }
        public string StackTrace { get { return (string)DataElement.Element(XConcurrencyNames.ErrorStackTrace); } }

        public bool IsAggregateException { get { return typeof(AggregateException).FullName.Equals(ExceptionTypeName); } }

    }

}
