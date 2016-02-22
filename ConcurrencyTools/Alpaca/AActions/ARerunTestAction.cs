/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.Alpaca.AActions
{
    /// <summary>
    /// Action that reruns a previous test run.
    /// These actions are only Applicable on TestResultEntity or TestRunEntities that have results.
    /// </summary>
    internal class ARerunTestAction : ARerunTestAction<RunMCutTestOptions>
    {
        internal ARerunTestAction(string text) : base(text) { }
    }

    /// <summary>
    /// Action that reruns a previous test run.
    /// These actions are only Applicable on TestResultEntity or TestRunEntities that have results.
    /// </summary>
    internal class ARerunTestAction<TOpts> : ARunTestActionBase<TOpts>
        where TOpts : RunMCutTestOptions, new()
    {

        internal ARerunTestAction(string text)
            : base(text)
        {
        }

        protected virtual MCutTestRunType RunType { get { return MCutTestRunType.ReRunTest; } }

        protected override void OnBindToContext()
        {
            base.OnBindToContext();

            Applicable &= Context.MCutTestRun != null // We're only supporting MCut tests
                && (Target is TestResultEntity
                    || (Target is TestRunEntity && Context.MCutTestRun.HasResult)
                    );
            if (!Applicable) return;

            Enabled &= !Context.TestRun.HasTestSourceChanged;
        }

        public override void Go()
        {
            var cmd = new RunMCutTestCaseCommand(Context.MCutTestRun, RunType, RunOptions, true) {
                SelectRun = true
            };

            Context.Model.controller.AddNewCommand(cmd);
        }

    }
}
