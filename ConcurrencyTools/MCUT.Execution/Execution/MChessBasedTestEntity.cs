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
    /// Represents the base entity for unit test types that run against MChess.
    /// These test types also do not support the use of contexts.
    /// </summary>
    public abstract class MChessBasedTestEntity : MCutTestEntityBase<NullTestContextEntity>, ITestUsesMChess
    {

        #region Constructors

        public MChessBasedTestEntity(XElement el)
            : base(el)
        {
        }

        #endregion

        #region Properties

        protected override XName ContextXName { get { return XNames.NULL; } }

        #endregion

        protected override NullTestContextEntity CreateDefaultContext()
        {
            return NullTestContextEntity.CreateDefaultInstance();
        }

        public override string GetInvocationDetails()
        {
            XElement copy = CopyDataElement(DataElement, true);

            copy.Add(new XComment("The following (if any) are implied via shared elements further up the xml hierarchy."));
            copy.Add(this.OwningTestMethod.GetApplicableChessElementsSharedAccrossTestTypes());

            return copy.ToString();
        }

        public override AppTasks.RunMCutTestCaseAppTask CreateRunTestTask(MCutTestRunType runType)
        {
            return new AppTasks.RunMChessBasedTestTask(this) { RunType = runType };
        }

        /// <summary>
        /// Sets up the base <see cref="MChessOptions"/> instance for a fresh run of this test.
        /// This is the part that usually differentiate tests using MChess from each other.
        /// </summary>
        /// <returns></returns>
        public virtual void SetBaseMChessOptionsForTestExecution(AppTasks.RunMChessBasedTestTask runTestTask, MChessOptions opts)
        {
            // Look to see if the test defines an MChessOptions element
            var xopts = DataElement.Element(XChessNames.MChessOptions);
            if (xopts != null)
            {
                var testOpts = new MChessOptions(xopts);
                opts.MergeWith(testOpts);
            }
        }

    }
}
