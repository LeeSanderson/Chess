using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting;

namespace Microsoft.Concurrency.TestTools.Execution.Chess
{

    public interface IMChessTestContext
    {
        MChessOptions ToMChessOptions();
    }

    [AutoRegisterEntity]
    public class ChessTestContextEntity : TestContextEntityBase, IMChessTestContext
    {
        public static readonly XName EntityXName = XNames.ChessContext;

        internal static ChessTestContextEntity CreateDefaultContext(MChessTestEntity owningTest)
        {
            // This element should not have a parent
            XElement xctx = new XElement(XNames.ChessContext
                // Have the test use the default expected results (if they're specified)
                , new XAttribute(XNames.AExpectedResultKey, String.Empty)
                , new XAttribute(XNames.AExpectedChessResultKey, String.Empty)
                );

            return (ChessTestContextEntity)owningTest.Model.EntityBuilder.CreateEntityAndBindToElement(xctx);
        }

        public ChessTestContextEntity(XElement el)
            : base(el)
        {
        }

        /// <summary>
        /// The key to the expected results from MChess for this test.
        /// null indicates none expected. An empty string indicates to use the default expected result for the test.
        /// </summary>
        public string ExpectedChessResultKey { get { return (string)DataElement.Attribute(XNames.AExpectedChessResultKey); } }

        /// <summary>
        /// Gets the script to execute just before running the test.
        /// This script is directly inserted into the run script for the test.
        /// </summary>
        public string PreRunScript { get { return (string)DataElement.Element(XNames.MChessPreRunScript); } }

        /// <summary>
        /// Converts this context entity into a new <see cref="MChessOptions"/>
        /// instance with the options set as specified in this context.
        /// </summary>
        public MChessOptions ToMChessOptions()
        {
            return new MChessOptions(DataElement);
        }

    }

    [AutoRegisterEntity]
    public class ExpectedChessTestResultEntity : EntityBase
    {
        public static readonly XName EntityXName = XNames.ExpectedChessResult;

        public static XElement CreateXElement(ChessExitCode? exitCode)
        {
            return new XElement(XNames.ExpectedChessResult
                , exitCode.HasValue ? new XAttribute("ExitCode", exitCode) : null
                );
        }

        public ExpectedChessTestResultEntity(XElement el)
            : base(el)
        {
        }

        /// <summary>
        /// Will never be null. Empty string indicates the default for the test.
        /// </summary>
        public string Key { get { return (string)DataElement.Attribute(XNames.AKey) ?? String.Empty; } }

        public ChessExitCode? ExitCode { get { return DataElement.Attribute("ExitCode").ParseXmlEnum<ChessExitCode>(); } }
        public int? SchedulesRan { get { return (int?)DataElement.Attribute("SchedulesRan"); } }
        public int? LastThreadCount { get { return (int?)DataElement.Attribute("LastThreadCount"); } }
        public int? LastExecSteps { get { return (int?)DataElement.Attribute("LastExecSteps"); } }
        public int? LastHBExecSteps { get { return (int?)DataElement.Attribute("LastHBExecSteps"); } }

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            yield break;
        }

    }

    /// <summary>
    /// Represents a Concurrency Unit Test that runs against the Chess Tool.
    /// </summary>
    /// <remarks>
    /// A Unit Test here means a test that is a method in an assembly.
    /// The xml hierarchy is TestAssembly/TestClass/ChessTest.
    /// </remarks>
    [AutoRegisterEntity]
    public class MChessTestEntity : MCutTestEntityBase<ChessTestContextEntity>, ITestUsesMChess
    {

        public static readonly XName EntityXName = XNames.ChessUnitTest;

        private Dictionary<string, ExpectedChessTestResultEntity> _expChessResults;

        #region Constructors

        public MChessTestEntity(XElement el)
            : base(el)
        {
        }

        #endregion

        #region Properties

        protected override XName ContextXName { get { return XNames.ChessContext; } }
        public override string TestTypeDisplayName { get { return "Chess Test"; } }

        #endregion

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            return DataElement
                .Elements(XNames.ChessContext, XNames.ExpectedChessResult)
                .Union(base.GetChildEntityElements());
        }

        protected override void OnChildrenLoaded()
        {
            base.OnChildrenLoaded();

            _expChessResults = DataElement.Elements(XNames.ExpectedChessResult)
                .SelectEntities<ExpectedChessTestResultEntity>()
                .ToDictionary(e => e.Key); // If duplicate keys exist, an error will be thrown
        }

        protected override ChessTestContextEntity CreateDefaultContext()
        {
            return ChessTestContextEntity.CreateDefaultContext(this);
        }

        public override string GetInvocationDetails()
        {
            XElement copy = CopyDataElement(DataElement, true);

            copy.Add(this.OwningTestMethod.GetApplicableChessElementsSharedAccrossTestTypes());

            return copy.ToString();
        }

        /// <summary>Finds the expected mchess result with the specified key.</summary>
        public ExpectedChessTestResultEntity GetExpectedChessResult(string key)
        {
            // We only support the expected results at the method level, so just look in our entities
            if (_expChessResults == null)
            {
                if (!String.IsNullOrEmpty(key))
                    throw new AssertFailedException(String.Format("The expected result with key '{0}' could not be found.", key));

                return null;
            }

            ExpectedChessTestResultEntity expResult;
            if (!_expChessResults.TryGetValue(key ?? String.Empty, out expResult) && !String.IsNullOrEmpty(key))
                throw new AssertFailedException(String.Format("The expected result with key '{0}' could not be found.", key));

            return expResult;
        }

        public override AppTasks.RunMCutTestCaseAppTask CreateRunTestTask(MCutTestRunType runType)
        {
            return new AppTasks.RunMChessTestTask(this) { RunType = runType };
        }

    }
}
