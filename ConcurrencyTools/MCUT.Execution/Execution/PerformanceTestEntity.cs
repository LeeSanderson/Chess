using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.Execution.Chess;

namespace Microsoft.Concurrency.TestTools.Execution
{
    [AutoRegisterEntity]
    public class TaskoMeterEntity : EntityBase
    {
        public static readonly XName EntityXName = XNames.TaskoMeter;

        public TaskoMeterEntity(XElement el)
            : base(el)
        {
        }


        /// <summary>Number of warmup repetitions. (Default: 0)</summary>
        public int WarmupRepetitions { get { return (int?)DataElement.Attribute("WarmupRepetitions") ?? 0; } }
        /// <summary>Number of repetitions. (Default: 1)</summary>
        public int Repetitions { get { return (int?)DataElement.Attribute("Repetitions") ?? 1; } }

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            yield break;
        }

    }

    /// <summary>
    /// Represents a normal Unit Test that runs via the UnitTestRunner in mcut.
    /// </summary>
    /// <remarks>
    /// A Unit Test here means a test that is a method in an assembly.
    /// The xml hierarchy is TestAssembly/TestClass/ChessTest.
    /// </remarks>
    [AutoRegisterEntity]
    public class PerformanceTestEntity : MCutTestEntityBase<NullTestContextEntity>
    {

        public static readonly XName EntityXName = XNames.PerformanceTest;

        #region Constructors

        public PerformanceTestEntity(XElement el)
            : base(el)
        {
        }

        #endregion

        #region Properties

        protected override XName ContextXName { get { return XNames.NULL; } }
        public override string TestTypeDisplayName { get { return "Performance Test"; } }

        public int WarmupRepetitions { get { return (int?)DataElement.Attribute("WarmupRepetitions") ?? 0; } }
        public int Repetitions { get { return (int?)DataElement.Attribute("Repetitions") ?? 1; } }

        public override bool RequiresUserInteraction { get { return true; } }

        #endregion

        protected override NullTestContextEntity CreateDefaultContext()
        {
            return NullTestContextEntity.CreateDefaultInstance();
        }

        public override AppTasks.RunMCutTestCaseAppTask CreateRunTestTask(MCutTestRunType runType)
        {
            return new AppTasks.RunPerformanceTestTask(this) { RunType = runType };
        }

    }
}
