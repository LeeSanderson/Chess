using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Chess;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using System.Xml.Linq;

namespace Microsoft.Concurrency.TestTools.Alpaca.AActions
{
    class AActionContext
    {

        private List<AAction> _actions;

        public AActionContext(EntityBase target)
        {
            Target = target;
            _actions = new List<AAction>();

            FindFullContext();
            CreateActions();
        }

        /// <summary>Gets the top action nodes for this context.</summary>
        public IEnumerable<AAction> Actions { get { return _actions; } }

        public EntityBase Target { get; private set; }
        public Model Model { get { return (Model)Target.Model; } }

        public TestEntity Test { get; private set; }
        public TestRunEntity TestRun { get; private set; }
        public MCutTestCaseRunEntity MCutTestRun { get; private set; }
        public TestResultEntity TestResult { get; private set; }

        public ChessResultEntity ChessResult { get; private set; }

        public XElement LastXSchedule { get; private set; }

        private void FindFullContext()
        {
            if (Target is ChessResultEntity)
            {
                ChessResult = (ChessResultEntity)Target;

                // NOTE: The old actions would only set the TestRun/TestResult contexts if the result is an error/notification, but not for warnings.
                TestResult = ChessResult.OwningTestResult;
                TestRun = ChessResult.OwningTestRun;
                Test = TestRun.OwningTest;
            }
            else if (Target is TestResultEntity)
            {
                TestResult = (TestResultEntity)Target;
                TestRun = TestResult.OwningTestRun;
                Test = TestRun.OwningTest;

                // Find children we know about
                // None
            }
            else if (Target is TestRunEntity)
            {
                TestRun = (TestRunEntity)Target;
                Test = TestRun.OwningTest;

                // Find children we know about
                TestResult = TestRun.Result;
            }
            else if (Target is TestEntity)
            {
                Test = (TestEntity)Target;
            }

            LastXSchedule = TestRun == null ? null : TestRun.GetLastXSchedule();
            MCutTestRun = TestRun as MCutTestCaseRunEntity;

            // If there's only one bad chess result, then allow the UI to expose it's functionality
            // w/o the user needing to first select the chess result.
            if (ChessResult == null && TestResult != null)
            {
                var chessResults = Views.EntityViewUtil.GetNonInformationalChessResults(TestResult);
                if (chessResults.Count() == 1)
                    ChessResult = chessResults.First(); ;
            }
        }

        private void AddAction(AAction action)
        {
            if (action != null)
            {
                _actions.Add(action);
                action.BindToContext(this);
            }
        }

        private void AddActions(IEnumerable<AAction> actions)
        {
            if (actions != null)
            {
                foreach (var action in actions)
                    AddAction(action);
            }
        }

        private void AddSeparatorAction()
        {
            AddAction(new SeparatorFauxAction());
        }

        private void CreateActions()
        {
            AddSeparatorAction();

            //** New action types
            // Run...
            AddAction(new ARunAllTestsAction());
            AddAction(new ARunAllObservationGeneratorsAction());

            // Get the test type and it's controller
            if (this.Test != null)
            {
                TestTypeController testTypeController = TestTypeController.GetController(Test);
                if (testTypeController != null)
                    AddActions(testTypeController.CreateTestActions(this));

                //** The following is here for referential purposes so when we implement this
                // functionality, we'll know where to start.
                //MakePreemptionBoundSubmenu(new RunPluginOnTestAction(this, null, "Run Race Reduction", typeof(Plugins.RaceReduction), null));

                //// TODO: Re-implement RCAction. Need to know when this shows up.
                //////** ...
                ////// Observations...
                ////MakeEnumObsSubmenu(new RCAction(this, null, "Enumerate Observations", new MChessOptions { EnumerateObservations = true }));
                ////MakeCheckObsSubmenu(new RCAction(this, null, "Check Observations", new MChessOptions { CheckObservations = true }));
            }

            // Build...
            AddSeparatorAction();
            AddAction(new ABuildAction("Build"));
            AddAction(new ABuildAction("Rebuild") { Rebuild = true });
            AddAction(new ABuildAction("Build And Run") { RunAfterBuild = true });

        }

        //private void MakeEnumObsSubmenu(Action parent)
        //{
        //    MakeBoundsSubmenu(new NMChessOptionsModifier(parent, "Serial Interleavings Only", new MChessOptions { ObservationMode = MChessObservationMode.SerialInterleavings }));
        //    MakeBoundsSubmenu(new NMChessOptionsModifier(parent, "Coarse Interleavings Only", new MChessOptions { ObservationMode = MChessObservationMode.CoarseInterleavings }));
        //    MakeBoundsSubmenu(new NMChessOptionsModifier(parent, "All Interleavings", new MChessOptions { ObservationMode = MChessObservationMode.AllInterleavings }));
        //}
        //private void MakeCheckObsSubmenu(Action parent)
        //{
        //    MakeRunSubmenu(new NMChessOptionsModifier(parent, "Linearizability", new MChessOptions { ObservationMode = MChessObservationMode.Linearizability }), false);
        //    //    MakeRunSubmenu(item.MakeSubaction("Linearizability (weak)", new string[] { "/observationmode:lin" }), false);
        //    MakeRunSubmenu(new NMChessOptionsModifier(parent, "Sequential Consistency", new MChessOptions { ObservationMode = MChessObservationMode.SequentialConsistency }), false);
        //    //    MakeRunSubmenu(item.MakeSubaction("Sequential Consistency (weak)", new string[] { "/observationmode:SC" }), false);
        //}

        internal void SetRunOptionsFromUI(RunMCutTestOptions runOpts)
        {
            // For now, we will set this true for all test types.
            // This is required for Performance Tests to function properly.
            runOpts.RunInteractively = true;

            runOpts.EnableRegressionTestAsserts = Model.session.Entity.RuntimeState.EnableRegressionTestingMode;
            runOpts.UseGoldenObservationFile = Model.session.Entity.RuntimeState.UseGoldenObservationFiles;
        }

    }
}
