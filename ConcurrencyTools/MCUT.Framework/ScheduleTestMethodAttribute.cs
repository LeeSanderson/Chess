using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>
    /// Marks a method as an execution schedule method.
    /// The default just asserts no errors are detected.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ScheduleTestMethodAttribute : TestTypeAttributeBase
    {

        /// <summary>Marks a method as an execution schedule test method.</summary>
        public ScheduleTestMethodAttribute() { }

        #region MChessOptions

        internal int? _MaxPreemptions;
        /// <summary>
        /// Indicates the maximum number of preemptions to insert into the scheduler.
        /// A value of 0 indicates no preemptions. (Default is 2)
        /// Don't set this property if you want to use the default number of preemptions used by mchess.
        /// </summary>
        public int MaxPreemptions
        {
            get { return _MaxPreemptions ?? 0; }
            set { _MaxPreemptions = value; }
        }

        internal int? _MaxSchedules;
        /// <summary>
        /// The maximum number of schedules to run.
        /// The default is unlimited schedules.
        /// </summary>
        public int MaxSchedules
        {
            get { return _MaxSchedules ?? 0; }
            set { _MaxSchedules = value; }
        }

        internal int? _MaxRunTime;
        /// <summary>
        /// Indicates the maximum time (in seconds) to allow this test to run.
        /// This means that all schedule explorations must be done within the time specified.
        /// At that time, schedule exploration stops and the test will pass.
        /// The default is an unlimited amount of time.
        /// </summary>
        public int MaxRunTime
        {
            get { return _MaxRunTime ?? 0; }
            set { _MaxRunTime = value; }
        }

        internal bool? _PreemptAllAccesses;
        /// <summary>
        /// Specifies whether to preempt on all data accesses.
        /// Matches the /pa and /preemptaccesses command line arguments for mchess.
        /// </summary>
        public bool PreemptAllAccesses
        {
            get { return _PreemptAllAccesses ?? false; }
            set { _PreemptAllAccesses = value; }
        }

        #endregion

        protected override XElement CreateTestTypeXml(MethodInfo testMethod)
        {
            MChessOptions mchessOpts = new MChessOptions();
            if (_MaxSchedules.HasValue) mchessOpts.MaxExecs = MaxSchedules;
            if (_MaxRunTime.HasValue) mchessOpts.MaxChessTime = MaxRunTime;
            if (_MaxPreemptions.HasValue) mchessOpts.MaxPreemptions = MaxPreemptions;
            if (_PreemptAllAccesses.HasValue) mchessOpts.PreemptAllAccesses = PreemptAllAccesses;
            var xopts = mchessOpts.ToXElement();

            return new XElement(XNames.ScheduleTest
                , xopts.HasAttributes || xopts.HasElements ? xopts : null
                );
        }

    }
}