using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    // Implements the default settings used in the regression tests in the old ManagedTest folder
    // using the golden files.
    [ContextsProperty("StandardChessContexts")]
    public class MChessRegressionTestBase
    {
        /// <summary>
        /// The 3 standard chess contexts that were previously specifies using the
        /// ManagedTest\cmdfile file.
        /// These contexts represent a different MaxPreemptions setting (1-3) and set the
        /// ChessTestContext.ExpectedResultKey value to "csb1", "csb2", "csb3" as appropriate.
        /// </summary>
        public static IEnumerable<ITestContext> StandardChessContexts
        {
            // From cmdfile
            // csb1 : ..\bin\mchess.exe /volatile /p:notime=true /p:show_hbexecs=true /p:nopopups=true /csb:1
            // csb2 : ..\bin\mchess.exe /volatile /p:notime=true /p:show_hbexecs=true /p:nopopups=true /csb:2
            // csb3 : ..\bin\mchess.exe /volatile /p:notime=true /p:show_hbexecs=true /p:nopopups=true /csb:3
            get
            {
                yield return new ChessTestContext("csb1", null, "csb1", new MChessOptions { MaxPreemptions = 1 });
                yield return new ChessTestContext("csb2", null, "csb2", new MChessOptions { MaxPreemptions = 2 });
                yield return new ChessTestContext("csb3", null, "csb3", new MChessOptions { MaxPreemptions = 3 });
            }
        }
    }
}
