using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    public interface IAcceptsRunTestOptions
    {
        void Accept(IRunTestOptions runOptions);
    }
}
