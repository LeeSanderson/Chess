using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.Execution.Xml
{
    /// <summary>Loads a test container file.</summary>
    public class TestContainerLoader
    {

        private string _containerPath;
        private IncludeEntity _include;

        internal TestContainerLoader(IncludeEntity include)
        {
            _include = include;
            _containerPath = include.TestContainerLocation;
            Model = include.Model;
            RegisterWithSession = true;
            LoadRecursiveIncludes = true;
        }

        public TestContainerLoader(string containerFile)
        {
            _containerPath = containerFile;
            RegisterWithSession = true;
            LoadRecursiveIncludes = true;
        }

        public IEntityModel Model { get; set; }
        private ISessionEntity Session { get { return Model.Session; } }

        /// <summary>
        /// Indicates whether the Load method should also register entities with the Session.
        /// The default is true.
        /// </summary>
        public bool RegisterWithSession { get; set; }

        /// <summary>
        /// Indicates whether to recursively load up includes.
        /// The default is true.
        /// </summary>
        public bool LoadRecursiveIncludes { get; set; }

        public ErrorEntity LoadError { get; private set; }
        public TestGroupingEntity TestContainer { get; private set; }

        public bool Load()
        {
            if (Model == null)
                throw new InvalidOperationException("Model not specified.");

            if (!LoadContainerEntityFromFile())
                return false;

            // Recursively try and load as much of the children as possible
            bool includesLoaded;
            do
            {
                includesLoaded = false;

                // Register test project (before assemblies) so any extra includes will be created
                var testProjectsToLoad = TestContainer.DescendantsAndSelf<TestProjectEntity>()
                    .Where(tp => !tp.IsProjectFileLoaded && !tp.HasProjectLoadError)
                    .ToArray(); // Since we'll be changing the entity try when we register
                foreach (var testProj in testProjectsToLoad)
                    testProj.TryLoadProjectFile();

                if (LoadRecursiveIncludes)
                {
                    // Try to load any includes that haven't been loaded as of yet
                    var includes = TestContainer.DescendantsAndSelf<IncludeEntity>()
                        .Where(inc => !inc.HasLoadError)    // From a previous load
                        .ToArray(); // actualize since we're potentially changing the tree
                    foreach (var include in includes)
                    {
                        // Don't want to register w/the session here because we'll do it after we've
                        // recursively loaded as much as we can
                        if (include.TryLoad(loadDescendentIncludes: true, registerWithSession: false))
                            includesLoaded = true;
                    }
                }

                // Keep going until all recursive includes have been given a chance to be loaded
            } while (includesLoaded);

            // Only register entities with the session if indicated to do so
            if (RegisterWithSession)
            {
                // Register test assemblies
                foreach (var testAssy in TestContainer.DescendantsAndSelf<TestAssemblyEntity>())
                    Session.RegisterTestAssembly(testAssy);
            }

            return true;
        }

        private bool LoadContainerEntityFromFile()
        {
            LoadError = null;
            TestContainer = null;

            XElement xtestContainer = null;
            XElement xerror = null;
            try
            {
                xtestContainer = TestContainerUtil.LoadTestContainer(_containerPath, true);

                // Look for an error
                if (xtestContainer.Name == XConcurrencyNames.Error)
                {
                    xerror = xtestContainer;
                    xtestContainer = null;
                }
                else
                {
                    xerror = xtestContainer.Element(XConcurrencyNames.Error);
                    if (xerror != null)
                        xerror.Remove();
                }
            }
            catch (Exception ex)
            {
                xerror = XConcurrencyNames.CreateXError(ex);
            }

            // This is outside of the try-catch because it's updating the xdocument tree only
            // and any errors during events regarding the tree changing will be caught elsewhere.
            if (xerror != null)
            {
                LoadError = (ErrorEntity)Model.EntityBuilder.CreateEntityAndBindToElement(xerror);
                return false;
            }

            TestContainer = (TestGroupingEntity)Model.EntityBuilder.CreateEntityAndBindToElement(xtestContainer);
            TestContainer.LoadChildren(true);
            return true;
        }

    }
}
