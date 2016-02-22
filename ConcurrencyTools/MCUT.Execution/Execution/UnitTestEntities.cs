using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting;

namespace Microsoft.Concurrency.TestTools.Execution
{

    /// <summary>
    /// Represents a normal Unit Test that runs via the UnitTestRunner in mcut.
    /// </summary>
    /// <remarks>
    /// A Unit Test here means a test that is a method in an assembly.
    /// The xml hierarchy is TestAssembly/TestClass/ChessTest.
    /// </remarks>
    [AutoRegisterEntity]
    public class UnitTestEntity : MCutTestEntityBase<NullTestContextEntity>
    {

        public static readonly XName EntityXName = XNames.UnitTest;

        #region Constructors

        public UnitTestEntity(XElement el)
            : base(el)
        {
        }

        #endregion

        #region Properties

        protected override XName ContextXName { get { return XNames.NULL; } }
        public override string TestTypeDisplayName { get { return "Unit Test"; } }

        #endregion

        protected override NullTestContextEntity CreateDefaultContext()
        {
            return NullTestContextEntity.CreateDefaultInstance();
        }

        public override AppTasks.RunMCutTestCaseAppTask CreateRunTestTask(MCutTestRunType runType)
        {
            return new AppTasks.RunUnitTestTask(this) { RunType = runType };
        }

    }
}
