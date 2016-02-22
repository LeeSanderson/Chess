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
    class ObservationTestController : TestTypeController
    {

        public ObservationTestController() : base(typeof(ObservationTestEntity)) { }

        internal override IEnumerable<AAction> CreateTestActions(AActionContext context)
        {
            // See if the observation file has been created
            var obsTest = (ObservationTestEntity)context.Test;

            // Run Test
            yield return new ARunAllMCutTestCasesAction("Run Observation Test") {
            };

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
