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
    /// just uses the default settings of MChess for exploring all schedules.
    /// </summary>
    [AutoRegisterEntity]
    public class ScheduleTestEntity : MChessBasedTestEntity
    {

        public static readonly XName EntityXName = XNames.ScheduleTest;

        #region Constructors

        public ScheduleTestEntity(XElement el)
            : base(el)
        {
        }

        #endregion

        #region Properties

        public override string TestTypeDisplayName { get { return "Schedule Test"; } }

        #endregion

    }
}
