using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.Execution.Chess
{
    /// <summary>
    /// Represents a single result from a chess run.
    /// A single run may actually produce multiple results.
    /// </summary>
    [AutoRegisterEntity]
    public class ChessResultEntity : EntityBase
    {
        public static readonly XName EntityXName = XChessNames.Result;

        #region Constructors

        public ChessResultEntity(XElement el)
            : base(el)
        {
        }

        #endregion

        #region Properties

        public override string DisplayName { get { return Description ?? "Untitled Result"; } }

        public string Label { get { return DataElement.Element(XChessNames.Label).Value; } }
        public MChessResultType ResultType { get { return EntityUtil.ParseMChessResultType(Label); } }
        public string Description { get { return (string)DataElement.Element(XChessNames.Description); } }

        public TestResultEntity OwningTestResult { get { return (TestResultEntity)Parent; } }
        public TestRunEntity OwningTestRun { get { return OwningTestResult.OwningTestRun; } }

        /// <summary>Gets the <see cref="ErrorEntity"/> instance if one is associated with this result.</summary>
        public ErrorEntity Error { get { return this.EntityOfType<ErrorEntity>(); } }

        #endregion

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            return DataElement.Elements(XConcurrencyNames.Error);
        }

    }
}
