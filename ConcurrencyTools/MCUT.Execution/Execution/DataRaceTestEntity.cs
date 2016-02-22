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
    public class DataRaceTestEntity : MChessBasedTestEntity
    {

        public static readonly XName EntityXName = XNames.DataRaceTest;

        #region Constructors

        public DataRaceTestEntity(XElement el)
            : base(el)
        {
        }

        #endregion

        #region Properties

        public override string TestTypeDisplayName { get { return "Data Race Test"; } }

        #endregion

        public override void SetBaseMChessOptionsForTestExecution(AppTasks.RunMChessBasedTestTask runTestTask, MChessOptions opts)
        {
            base.SetBaseMChessOptionsForTestExecution(runTestTask, opts);

            // Enable data race detection
            opts.EnableRaceDetection = true;
            opts.MaxExecs = 20;
        }

    }
}
