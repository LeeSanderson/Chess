using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    /// <summary>
    /// The interface (currently just a marker) that identifies an AppTask as a task that runs a test.
    /// </summary>
    public interface IRunTestTask : ITaskDefinesARun
    {
    }
}
