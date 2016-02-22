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
    class DataRaceTestController : TestTypeController
    {

        public DataRaceTestController() : base(typeof(DataRaceTestEntity)) { }

        internal override IEnumerable<AAction> CreateTestActions(AActionContext context)
        {
            // NOTE: For data race tests, we're not giving the user as many of the options as a normal MChess test
            // If users want certain options available, then we can add back as we go.

            // Run Test
            yield return new ARunAllMCutTestCasesAction("Run Data Race Test");

            if (context.MCutTestRun != null)
            {
                // View w/Concurrency Explorer
                yield return new AViewConcurrencyExplorerAction();

                // Repeat Test
                yield return new ARerunMChessTestAction("Repeat Test") {
                    ChildrenIfApplicable = new[]{
                        MChessTestController.CreateMChessOptionsModifier_WithBounds(),
                        //MChessTestController.CreateMChessOptionsModifier_WithTracing(false),
                        MChessTestController.CreateMChessOptionsModifier_WithDebugger(),
                        MChessTestController.CreateMChessOptionsModifier_WithLogging(),
                    }
                };

                // Continue Test
                yield return new AContinueMChessTestRunAction() {
                    ChildrenIfApplicable = new[]{
                        MChessTestController.CreateMChessOptionsModifier_WithBounds(),
                        //MChessTestController.CreateMChessOptionsModifier_WithTracing(true),
                    }
                };

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
