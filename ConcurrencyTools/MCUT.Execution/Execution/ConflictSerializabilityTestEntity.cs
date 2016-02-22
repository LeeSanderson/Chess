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
    /// <summary>
    /// Represents a unit test that runs via the MChessTestRunner in mcut but
    /// just uses the settings in MChess that enable data race detection while
    /// exploring all schedules.
    /// </summary>
    [AutoRegisterEntity]
    public class ConflictSerializabilityTestEntity : MChessBasedTestEntity
    {

        public static readonly XName EntityXName = XNames.ConflictSerializabilityTest;

        #region Constructors

        public ConflictSerializabilityTestEntity(XElement el)
            : base(el)
        {
        }

        #endregion

        #region Properties

        public override string TestTypeDisplayName { get { return "Conflict Serializability Test"; } }

        #endregion

        public override void SetBaseMChessOptionsForTestExecution(AppTasks.RunMChessBasedTestTask runTestTask, MChessOptions opts)
        {
            base.SetBaseMChessOptionsForTestExecution(runTestTask, opts);

            opts.EnableAtomicityChecking = true;
        }

    }
}
