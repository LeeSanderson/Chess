using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Interface for when an entity defines test contexts.
    /// </summary>
    public interface IDefinesTestContexts
    {
        /// <summary>
        /// Gets all contexts available to this test instance.
        /// </summary>
        IEnumerable<TestContextEntityBase> GetContexts();
    }
}
