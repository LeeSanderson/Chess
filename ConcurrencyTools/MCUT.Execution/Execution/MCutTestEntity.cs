using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution.Chess;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Represents the base implementation for all tests that run using mcut.
    /// </summary>
    /// <remarks>
    /// A Unit Test here means a test that is a method in an assembly.
    /// </remarks>
    public abstract class MCutTestEntity : TestEntity
    {

        /// <summary>
        /// Internal because we want inheritors to actually use <see cref="MCutTestEntity&lt;TContext, TExpResult&gt;"/>.
        /// </summary>
        internal MCutTestEntity(XElement el)
            : base(el)
        {
        }

        /// <summary>Gets the UI friendly name of this test type.</summary>
        public abstract string TestTypeDisplayName { get; }

        public override string DisplayName { get { return TestTypeDisplayName; } }

        public TestMethodEntity OwningTestMethod { get { return (TestMethodEntity)base.Parent; } }
        public override ITestSource TestSource { get { return OwningTestMethod; } }

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            return DataElement
                .Elements(XSessionNames.MCutTestRun);
        }

        protected override IEnumerable<XElement> DescendantXRuns()
        {
            return DataElement.Descendants(XSessionNames.MCutTestRun);
        }

        //public override IEnumerable<TestContextEntityBase> GetContexts()
        //{
        //    return new[] { new NullTestContextEntity() { } };
        //}

        public override IEnumerable<TestArgs> GetTestArgsSets()
        {
            return OwningTestMethod.AllArgs;
        }

        /// <summary>When implemented in a derived class, creates a new instance that can run this test.</summary>
        public abstract AppTasks.RunMCutTestCaseAppTask CreateRunTestTask(MCutTestRunType runType);

    }

    public abstract class MCutTestEntityBase<TContext> : MCutTestEntity
        where TContext : TestContextEntityBase
    {

        private Dictionary<string, TContext> _contexts;
        private TContext _defaultContext;

        protected MCutTestEntityBase(XElement el)
            : base(el)
        {
        }

        #region Properties

        protected abstract XName ContextXName { get; }

        #endregion

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            // Need to also allow the ContextXName elements too
            return DataElement
                .Elements(ContextXName, XSessionNames.MCutTestRun);
        }

        protected override void OnChildrenLoaded()
        {
            base.OnChildrenLoaded();

            _contexts = DataElement.Elements(ContextXName)
                .SelectEntities<TContext>()
                .ToDictionary(ctx => ctx.Name); // If duplicate keys exist, an error will be thrown
        }

        /// <summary>
        /// Gets an enumeration of all the contexts defined for this test.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<TestContextEntityBase> GetContexts()
        {
            System.Diagnostics.Debug.Assert(_contexts != null, "OnChildrenLoaded has not run yet.");

            // First, get contexts from self
            if (_contexts.Count != 0)
                return _contexts.Values;    // Already known to be filtered to the type we want

            // If none defined on test itself, then use those from the parent entity
            IEnumerable<TestContextEntityBase> contexts = null;
            EntityBase ancestor = this.Parent;
            while (ancestor != null)
            {
                IDefinesTestContexts defContexts = ancestor as IDefinesTestContexts;
                if (defContexts != null)
                {
                    contexts = defContexts.GetContexts().OfType<TContext>();
                    if (contexts.Any())
                        return contexts;
                }
                ancestor = ancestor.Parent;
            }

            // If none defined at all, then return a default context
            if (_defaultContext == null)
                _defaultContext = CreateDefaultContext();
            return new TContext[] { _defaultContext };
        }

        protected abstract TContext CreateDefaultContext();

    }
}
