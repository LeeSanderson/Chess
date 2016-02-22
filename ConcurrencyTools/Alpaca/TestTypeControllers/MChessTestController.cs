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
using Microsoft.Concurrency.TestTools.Execution.AppTasks;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    class MChessTestController : TestTypeController
    {

        public MChessTestController() : base(typeof(MChessTestEntity)) { }

        internal override IEnumerable<AAction> CreateTestActions(AActionContext context)
        {
            // Run Test
            yield return new ARunAllMCutTestCasesAction<MChessTestRunOptions>("Run Chess Test") {
                ChildrenIfApplicable = CreateRunChessTestActions(true),
            };

            if (context.MCutTestRun != null)
            {
                // View w/Concurrency Explorer
                yield return new AViewConcurrencyExplorerAction();

                // Repeat Test
                yield return new ARerunMChessTestAction("Repeat Test") {
                    ChildrenIfApplicable = new[]{
                        CreateMChessOptionsModifier_WithBounds(),
                        //CreateMChessOptionsModifier_WithTracing(false),
                        CreateMChessOptionsModifier_WithDebugger(),
                        CreateMChessOptionsModifier_WithLogging(),
                        new AMChessOptionsModifier("With Instrumentation Diagnosis", new MChessOptions { PrintDiagnosticInformation = true}),
                    }
                };

                // Continue Test
                yield return new AContinueMChessTestRunAction() {
                    ChildrenIfApplicable = new[]{
                        CreateMChessOptionsModifier_WithBounds(),
                        //CreateMChessOptionsModifier_WithTracing(true),
                    }
                };

                // Repro Last Schedule
                yield return new AReproLastMChessTestRunScheduleAction() {
                    ChildrenIfApplicable = new[]{
                        CreateMChessOptionsModifier_WithTracing(false),
                        CreateMChessOptionsModifier_WithDebugger(),
                        CreateMChessOptionsModifier_WithLogging(),
                    }
                };

                // Repro Chess Result
                if (context.ChessResult != null)
                {
                    yield return new SeparatorFauxAction();

                    foreach (var a in CreateChessResultActions())
                        yield return a;
                }
            }
        }

        public static IEnumerable<AAction> CreateChessResultActions()
        {
            yield return new AReproChessResultAction("Repro and view in Concurrency Explorer") {
                AutoOpenConcurrencyExplorer = true
            };

            //yield return new AReproChessResultAction("Repro with Tracing");    // Tracing is on by default

            yield return new AReproChessResultAction("Repro with Debugger") {
                Actionable = false,
                ChildrenIfApplicable = CreateChessDebugActions()
            };

            yield return new AReproChessResultAction("Repro with Logging") {
                MChessOptions = new MChessOptions { EnableLogging = true }
            };
        }

        public static IEnumerable<AAction> CreateRunChessTestActions(bool includeRaceAndAutomicityChecking)
        {
            foreach (var a in CreateChessBoundsActions())
                yield return a;

            yield return new SeparatorFauxAction();

            if (includeRaceAndAutomicityChecking)
            {
                yield return new AMChessOptionsModifier("With Race Detection", new MChessOptions { EnableRaceDetection = true }, CreateChessBoundsActions());
                yield return new AMChessOptionsModifier("With Atomicity Checking", new MChessOptions { EnableAtomicityChecking = true }, CreateChessBoundsActions());
            }
            yield return new AMChessOptionsModifier("Without Preempting Volatiles", new MChessOptions { PreemptVolatiles = false }, CreateChessBoundsActions());
            yield return new AMChessOptionsModifier("Preempting All Accesses", new MChessOptions { PreemptAllAccesses = true }, CreateChessBoundsActions());
        }

        internal static AMChessOptionsModifier CreateMChessOptionsModifier_WithBounds()
        {
            return new AMChessOptionsModifier("With Bounds", null, CreateChessBoundsActions()) {
                Actionable = false
            };
        }

        internal static AMChessOptionsModifier CreateMChessOptionsModifier_WithDebugger()
        {
            return new AMChessOptionsModifier("With Debugger"
                , null
                , CreateChessDebugActions()) {
                    Actionable = false
                };
        }

        internal static AMChessOptionsModifier CreateMChessOptionsModifier_WithLogging()
        {
            return new AMChessOptionsModifier("With Logging"
                , new MChessOptions { EnableLogging = true }
                );
        }

        internal static AMChessOptionsModifier CreateMChessOptionsModifier_WithTracing(bool withBounds)
        {
            return new AMChessOptionsModifier("With Tracing"
                , new MChessOptions { TraceAllSchedules = true }
                , withBounds ? CreateChessBoundsActions() : null
                );
        }

        public static IEnumerable<AAction> CreateChessDebugActions()
        {
            yield return new AMChessOptionsModifier("Break At Beginning", new MChessOptions { Break = MChessBreak.Start });
            yield return new AMChessOptionsModifier("Break Before+After Context Switches", new MChessOptions { Break = MChessBreak.ContextSwitch | MChessBreak.AfterContextSwitch });
            yield return new AMChessOptionsModifier("Break Before+After Preemptions", new MChessOptions { Break = MChessBreak.Preemption | MChessBreak.AfterPreemption });
            yield return new AMChessOptionsModifier("Break At Deadlock", new MChessOptions { Break = MChessBreak.Deadlock });
            yield return new AMChessOptionsModifier("Break At Timeout", new MChessOptions { Break = MChessBreak.Timeout });
        }

        public static IEnumerable<AAction> CreateChessBoundsActions()
        {
            yield return new AMChessOptionsModifier("Zero Preemptions", new MChessOptions { MaxPreemptions = 0 }, CreateChessBoundsActions_2ndLevel());
            yield return new AMChessOptionsModifier("One Preemption", new MChessOptions { MaxPreemptions = 1 }, CreateChessBoundsActions_2ndLevel());
            yield return new AMChessOptionsModifier("Two Preemptions", new MChessOptions { MaxPreemptions = 2 }, CreateChessBoundsActions_2ndLevel());
            yield return new AMChessOptionsModifier("Three Preemptions", new MChessOptions { MaxPreemptions = 3 }, CreateChessBoundsActions_2ndLevel());
            yield return new AMChessOptionsModifier("Unlimited Preemptions", new MChessOptions { MaxPreemptions = 100000 }, CreateChessBoundsActions_2ndLevel());
        }

        private static IEnumerable<AAction> CreateChessBoundsActions_2ndLevel()
        {
            yield return new AMChessOptionsModifier("5 Schedules", new MChessOptions { MaxExecs = 5 });
            yield return new AMChessOptionsModifier("10 Schedules", new MChessOptions { MaxExecs = 10 });
            yield return new AMChessOptionsModifier("100 Schedules", new MChessOptions { MaxExecs = 100 });
            yield return new AMChessOptionsModifier("1000 Schedules", new MChessOptions { MaxExecs = 1000 });
            yield return new AMChessOptionsModifier("10000 Schedules", new MChessOptions { MaxExecs = 10000 });
            yield return new AMChessOptionsModifier("100000 Schedules", new MChessOptions { MaxExecs = 100000 });
            yield return new AMChessOptionsModifier("Unlimited Schedules", new MChessOptions { MaxExecs = 0 });
            yield return new SeparatorFauxAction();
            yield return new AMChessOptionsModifier("5 Seconds", new MChessOptions { MaxChessTime = 5 });
            yield return new AMChessOptionsModifier("10 Seconds", new MChessOptions { MaxChessTime = 10 });
            yield return new AMChessOptionsModifier("30 Seconds", new MChessOptions { MaxChessTime = 30 });
            yield return new AMChessOptionsModifier("1 Minute", new MChessOptions { MaxChessTime = 60 });
            yield return new AMChessOptionsModifier("5 Minutes", new MChessOptions { MaxChessTime = 300 });
            yield return new AMChessOptionsModifier("10 Minutes", new MChessOptions { MaxChessTime = 600 });
            yield return new AMChessOptionsModifier("30 Minutes", new MChessOptions { MaxChessTime = 1800 });
            yield return new AMChessOptionsModifier("1 Hour", new MChessOptions { MaxChessTime = 3600 });
            yield return new AMChessOptionsModifier("2 Hours", new MChessOptions { MaxChessTime = 7200 });
            yield return new AMChessOptionsModifier("3 Hours", new MChessOptions { MaxChessTime = 10800 });
            yield return new AMChessOptionsModifier("Unlimited Time", new MChessOptions { MaxChessTime = 0 });
        }

    }
}
