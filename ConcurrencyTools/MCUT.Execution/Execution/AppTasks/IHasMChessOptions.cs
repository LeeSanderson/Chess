using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    public interface IHasMChessOptions
    {
        MChessOptions MChessOptions { get; set; }
    }
}
