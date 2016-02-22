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
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.Execution.Chess;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Represents a single method that may contain metadata for multiple types of tests.
    /// e.g. May contain info for a Chess test and/or a regular unit test.
    /// </summary>
    [AutoRegisterEntity]
    public class TestMethodEntity : TestGroupingEntity, ITestSource
    {
        public static readonly XName EntityXName = XNames.TestMethod;

        /// <summary>
        /// The XNames for all AutoRegistered entities that inherit from <see cref="MCutTestEntity"/>.
        /// </summary>
        public static XName[] TestEntityXNames { get; private set; }

        public static XName[] ChildEntityXNames { get; private set; }

        static TestMethodEntity()
        {
            // Find all the XNames of AutoRegisteredEntities that inherit from MCutTestEntity
            TestEntityXNames = (from t in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                                where !t.IsAbstract
                                where typeof(MCutTestEntity).IsAssignableFrom(t)
                                let autoRegAttr = (AutoRegisterEntityAttribute)Attribute.GetCustomAttribute(t, typeof(AutoRegisterEntityAttribute))
                                where autoRegAttr != null
                                let entityXName = autoRegAttr.GetEntityXName(t)
                                select entityXName)
                                .ToArray();

            ChildEntityXNames = new[] { XNames.ExpectedTestResult, XNames.ExpectedRegressionTestResult }
                .Concat(TestEntityXNames)
                .ToArray();
        }

        private TestMethodParameter[] _parameters;
        private TestArgs[] _argss;

        private Dictionary<string, ExpectedTestResultEntity> _expResults;

        #region Constructors

        public TestMethodEntity(XElement el)
            : base(el)
        {
            // Parse the parameters
            XElement xparameters = el.Element(XNames.Parameters);
            if (xparameters != null)
            {
                _parameters = (from xparam in xparameters.Elements(XNames.Param)
                               select new TestMethodParameter(xparam))
                               .ToArray();
                _argss = (from xargs in el.Elements(XNames.TestArgs)
                          select new TestArgs(xargs))
                          .ToArray();
            }
            else
            {
                _parameters = new TestMethodParameter[0];
                _argss = new TestArgs[0];
            }
        }

        #endregion

        #region Properties

        public TestClassEntity OwningClass { get { return (TestClassEntity)base.Parent; } }

        public string MethodName { get { return DataElement.Attribute(XNames.AName).Value; } }

        public TestMethodParameter[] Parameters { get { return _parameters; } }
        public bool HasParameters { get { return _parameters.Length != 0; } }

        /// <summary>Returns all the fixed args sets defined for this test.</summary>
        public TestArgs[] AllArgs { get { return _argss; } }

        public override string DisplayName
        {
            get
            {
                if (Parameters.Length != 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(MethodName);
                    sb.Append('(');

                    sb.AppendFormat("{0} {1}", Parameters[0].TypeDisplayName, Parameters[0].Name);
                    foreach (var param in Parameters.Skip(1))
                    {
                        sb.AppendFormat(", {0} {1}", param.TypeDisplayName, param.Name);
                    }

                    sb.Append(')');

                    return sb.ToString();
                }
                else
                    return MethodName + "()";
            }
        }

        #endregion

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            return DataElement.Elements(ChildEntityXNames);
        }

        protected override void OnChildrenLoaded()
        {
            base.OnChildrenLoaded();

            _expResults = DataElement.Elements(XNames.ExpectedTestResult)
                .SelectEntities<ExpectedTestResultEntity>()
                .ToDictionary(e => e.Key); // If duplicate keys exist, an error will be thrown
        }

        protected override IEnumerable<XElement> DescendantXRuns()
        {
            return DataElement
                .Elements(TestEntityXNames)
                .Descendants(XSessionNames.MCutTestRun);
        }

        public override string GetInvocationDetails()
        {
            return CopyDataElement(DataElement, true)
                .ToString();
        }

        /// <summary>
        /// Gets the elements from the test list hierarchy that apply to each test that uses
        /// MChess as its runner. All elements returned are clones.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<XElement> GetApplicableChessElementsSharedAccrossTestTypes()
        {
            // Add only the applicable instrumentation assemblies
            foreach (var assy in ChessTestUtil.GetAssembliesToInstrument(this))
                yield return new XElement(XChessNames.InstrumentAssembly, assy);

            // Add preemption settings
            bool? defaultPreemptability;
            var xtogglePreempts = ChessTestUtil.GetTogglePreemptionElements(this, out defaultPreemptability);
            if (defaultPreemptability.HasValue)
                yield return new XElement(XChessNames.DefaultPreemptability, defaultPreemptability.Value);
            foreach (var x in xtogglePreempts)
                yield return new XElement(x); // Add copies
        }

        public XElement ToXTestCaseSource()
        {
            return new XElement(XTestCaseNames.ManagedTestMethod
                    , new XAttribute(XTestCaseNames.AAssemblyLocation, OwningClass.OwningAssembly.SourceFilePath)
                    , new XAttribute(XTestCaseNames.AFullClassName, OwningClass.ClassFullName)
                    , new XAttribute(XTestCaseNames.AMethodName, MethodName)
                    );
        }

        /// <summary>Finds the expected result with the specified key.</summary>
        public ExpectedTestResultEntity GetExpectedResult(string key)
        {
            // We only support the expected results at the method level, so just look in our entities
            if (_expResults == null)
            {
                if (!String.IsNullOrEmpty(key))
                    throw new AssertFailedException(String.Format("The expected result with key '{0}' could not be found.", key));

                return null;
            }

            ExpectedTestResultEntity expResult;
            if (!_expResults.TryGetValue(key ?? String.Empty, out expResult) && !String.IsNullOrEmpty(key))
                throw new AssertFailedException(String.Format("The expected result with key '{0}' could not be found.", key));

            return expResult;
        }

        public ExpectedRegressionTestResultEntity GetExpectedRegressionTestResult()
        {
            return this.EntityOfType<ExpectedRegressionTestResultEntity>();
        }

    }
}
