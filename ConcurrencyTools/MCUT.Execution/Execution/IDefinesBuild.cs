using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Execution
{
    public interface IDefinesBuild
    {
        BuildEntity BuildEntity { get; }
        string DisplayName { get; }
    }
}
