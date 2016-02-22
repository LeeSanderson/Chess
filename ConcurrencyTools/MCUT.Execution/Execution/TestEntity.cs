using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Represents the base implementation of a generic test.
    /// This is the base class used for all test entities and elements.
    /// </summary>
    public abstract class TestEntity : EntityBase, IDefinesTestContexts
    {

        public TestEntity(XElement el)
            : base(el)
        {
        }

        #region Properties

        public abstract ITestSource TestSource { get; }

        /// <summary>
        /// Gets a value indicating whether this test type requires a user to interact with it.
        /// </summary>
        public virtual bool RequiresUserInteraction { get { return false; } }

        /// <summary>
        /// Gets a value indicating whether this test can be run.
        /// Generally, this is affected if the test type has prerequisites that are met or not at runtime.
        /// </summary>
        public virtual bool CanRun { get { return true; } }

        #endregion

        public override string GetInvocationDetails()
        {
            return CopyDataElement(DataElement, true)
                .ToString();
        }

        public override IEnumerable<TestEntity> TestsAndSelf()
        {
            yield return this;
        }

        /// <summary>Gets contexts available to this test instance.</summary>
        /// <returns></returns>
        public abstract IEnumerable<TestContextEntityBase> GetContexts();

        /// <summary>
        /// Gets the complete set of TestArgs for this test.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<TestArgs> GetTestArgsSets();

    }
}
