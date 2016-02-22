using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Chess;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Alpaca.AActions;
using Microsoft.Concurrency.TestTools.Alpaca.AActions.MChessActions;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    class ObservationGeneratorController : TestTypeController
    {

        public ObservationGeneratorController() : base(typeof(ObservationGeneratorEntity)) { }

        internal override IEnumerable<AAction> CreateTestActions(AActionContext context)
        {
            // Run Test
            yield return new ARunAllMCutTestCasesAction("Run Observation Generator");

            if (context.MCutTestRun != null)
            {
                // View w/Concurrency Explorer
                yield return new AViewConcurrencyExplorerAction();

                // Repro Chess Result
                if (context.ChessResult != null)
                {
                    yield return new SeparatorFauxAction();
                    foreach (var a in MChessTestController.CreateChessResultActions())
                        yield return a;
                }
            }
        }

    }
}
