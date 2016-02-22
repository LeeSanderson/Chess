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

namespace Microsoft.Concurrency.TestTools.Execution
{
    public abstract class TestRunEntity : TaskRunEntity
    {

        public static readonly XName[] TestRunXNames = new[]{
            XSessionNames.MCutTestRun,
        };

        #region Constructors

        public TestRunEntity(XElement el)
            : base(el)
        {
        }

        #endregion

        public override string DisplayName { get { return "Run Test"; } }

        //public override string FullName
        //{
        //    get { return OwningTest.DisplayName + " : " + DisplayName; }
        //}

        public TestEntity OwningTest { get { return Ancestors().OfType<TestEntity>().First(); } }

        public TestResultEntity Result { get { return this.EntityOfType<TestResultEntity>(); } }
        /// <summary>Returns whether this test run has a Result entity yet.</summary>
        public bool HasResult { get { return Result != null; } }

        /// <summary>
        /// Gets a value indicating whether the test source for this run has changed.
        /// If it has, then some actions are now possibly not valid esp if an assembly has changed
        /// thus chess schedules etc may no longer be valid.
        /// </summary>
        public bool HasTestSourceChanged
        {
            get { return (bool?)DataElement.Attribute(XSessionNames.AHasTestSourceChanged) ?? false; }
            set { DataElement.SetAttributeValue(XSessionNames.AHasTestSourceChanged, value); }
        }

    }
}
