using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using Microsoft.Concurrency.TestTools;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Represents the overall result from a test run.
    /// </summary>
    [AutoRegisterEntity]
    public class TestResultEntity : EntityBase
    {
        public static readonly XName EntityXName = XTestResultNames.TestResult;

        #region Constructors

        public TestResultEntity(XElement el)
            : base(el)
        {
        }

        #endregion

        #region Properties

        /// <summary>The result from the mcut test.</summary>
        public TestResultType ResultType { get { return DataElement.Attribute(XTestResultNames.ATestResultType).ParseXmlEnum<TestResultType>().Value; } }

        public int ExitCode { get { return (int)DataElement.Attribute(XTestResultNames.AExitCode); } }

        public string Message { get { return (string)DataElement.Element(XTestResultNames.ResultMessage); } }

        public ChessExitCode? ChessExitCode { get { return DataElement.Attribute(XTestResultNames.AChessExitCode).ParseXmlEnum<ChessExitCode>(); } }

        public TestRunEntity OwningTestRun { get { return (TestRunEntity)Parent; } }

        /// <summary>Gets the <see cref="ErrorEntity"/> instance if one is associated with this result.</summary>
        public ErrorEntity Error { get { return this.EntityOfType<ErrorEntity>(); } }

        #endregion

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            // NOTE: I'm making sure these get returned in the order in the schema
            // since I'm doing a custom union with nested elements
            return DataElement.Elements(XConcurrencyNames.Error)
                .Union(DataElement.Elements(XChessNames.ChessResults).Elements(XChessNames.Result))
                .Union(DataElement.Elements(TestRunEntity.TestRunXNames))
                ;
        }

        public override string GetInvocationDetails()
        {
            return CopyDataElement(DataElement, true)
                .ToString();
        }

    }
}
