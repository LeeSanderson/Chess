using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Execution
{
    public enum MCutTestRunType
    {
        RunTest = 0,

        /// <summary>Indicates the rerunning of a test.</summary>
        ReRunTest,

        /// <summary>Indicates the run is a 'Repro' of an MChess test schedule.</summary>
        Repro,

        /// <summary>
        /// Indicates the run is a continuation of a previous run.
        /// e.g. For an MChess test that was stopped due to errors or a limiting constraint
        /// was reached (i.e. max schedule, max time, etc).
        /// </summary>
        Continue,
    }
}
