using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.Execution
{

    /// <summary>
    /// Represents the complete information that an engine (e.g. mcut runTestCase) should
    /// need to run a test case. Including options under which to run a test.
    /// This doesn't include run-time environment information such as the WorkinDirectory,
    /// output file paths, etc.
    /// </summary>
    [AutoRegisterEntity]
    public class TestCaseEntity : EntityBase
    {
        public static readonly XName EntityXName = XTestCaseNames.TestCase;

        public TestCaseEntity(XElement el)
            : base(el)
        {
        }

        #region Properties

        public override string DisplayName { get { return (string)DataElement.Attribute(XNames.AName); } }

        public string TestTypeName { get { return (string)DataElement.Attribute(XTestCaseNames.ATestTypeName); } }

        public string ContextName { get { return (string)DataElement.Attribute(XTestCaseNames.AContextName); } }

        public ExpectedTestResultEntity ExpectedTestResult { get { return this.EntityOfType<ExpectedTestResultEntity>(); } }
        public ExpectedRegressionTestResultEntity ExpectedRegressionTestResult { get { return this.EntityOfType<ExpectedRegressionTestResultEntity>(); } }

        public string AssemblyLocation { get { return (string)DataElement.Element(XTestCaseNames.ManagedTestMethod).Attribute(XTestCaseNames.AAssemblyLocation); } }
        /// <summary>Gets the fully qualified name of the test method (full class name + method name).</summary>
        public string FullTestMethodName
        {
            get
            {
                var xmgdTestMthd = DataElement.Element(XTestCaseNames.ManagedTestMethod);
                return (string)xmgdTestMthd.Attribute(XTestCaseNames.AFullClassName)
                    + '.'
                    + (string)xmgdTestMthd.Attribute(XTestCaseNames.AMethodName);
            }
        }
        public string MChessPreRunScript { get { return (string)DataElement.Element(XNames.MChessPreRunScript); } }

        public XElement XMChessOptions { get { return DataElement.Element(XChessNames.MChessOptions); } }

        #endregion

        public TestArgs GetTestArgs()
        {
            XElement xargs = DataElement.Element(XNames.TestArgs);
            return xargs == null ? null : new TestArgs(xargs);
        }

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            return DataElement
                .Elements(
                    XNames.ExpectedTestResult
                    , XNames.ExpectedRegressionTestResult
                    , XNames.ExpectedChessResult
                    , XNames.TaskoMeter
                    );
        }

    }

}
