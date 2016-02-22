using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>Indicates an entity can have a source file.</summary>
    public interface IHasTestContainerSourceFile
    {
        /// <summary>When specified, is the path to the source.</summary>
        string SourceFilePath { get; }

        bool SupportsRefresh { get; }

    }
}
