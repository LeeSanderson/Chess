using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Execution
{
    public interface IMSBuildSettings
    {
        string ProjectFullPath { get; }
        string Configuration { get; }
        string Platform { get; }

        /// <summary>
        /// A semi-colon separated list of key=value pairs.
        /// </summary>
        string OtherProperties { get; }
    }
}
