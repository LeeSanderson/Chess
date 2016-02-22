using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using System.Diagnostics;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Represents the base implementation of a run that performs a build.
    /// </summary>
    [AutoRegisterEntity]
    public class IncludeEntity : EntityBase
    {

        public static readonly XName EntityXName = XNames.Include;

        #region Static Members

        /// <summary>Creates an include element for a test list.</summary>
        public static XElement CreateXInclude(string location)
        {
            return new XElement(XNames.Include
                , new XAttribute(XNames.ALocation, location)
                );
        }

        #endregion

        #region Constructors

        public IncludeEntity(XElement el)
            : base(el)
        {
        }

        #endregion

        /// <summary>The location of the test container to include.</summary>
        public string TestContainerLocation { get { return (string)DataElement.Attribute(XNames.ALocation); } }

        /// <summary>Indicates whether there was a load error for this include.</summary>
        public bool HasLoadError { get { return DataElement.Elements(XConcurrencyNames.Error).Any(); } }

        /// <summary>Gets the load error.</summary>
        public ErrorEntity LoadError { get { return this.EntityOfType<ErrorEntity>(); } }

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            return DataElement.Elements(XConcurrencyNames.Error);
        }

        /// <summary>
        /// Tries to load the source test container.
        /// If successful, this entity is replaced with the loaded test container.
        /// Otherwise, the LoadError property is set, indicating the reason.
        /// </summary>
        /// <returns></returns>
        public bool TryLoad(bool loadDescendentIncludes = true, bool registerWithSession = true)
        {
            // Remove our previous error
            // It's not in the try-catch because any error is due to other code, not due to a loading failure.
            if (LoadError != null)
                LoadError.DataElement.Remove();

            var loader = new TestContainerLoader(this) {
                LoadRecursiveIncludes = loadDescendentIncludes,
                RegisterWithSession = registerWithSession,
            };

            if (loader.Load())
            {
                TestContainerUtil.MergeInSettingsFromInclude(loader.TestContainer.DataElement, DataElement);

                this.ReplaceWithEntity(loader.TestContainer);
                return true;
            }
            else
            {
                this.AddEntity<ErrorEntity>(loader.LoadError);
                return false;
            }
        }

    }
}
