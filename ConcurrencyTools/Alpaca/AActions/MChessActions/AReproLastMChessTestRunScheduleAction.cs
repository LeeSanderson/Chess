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
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.Alpaca.AActions.MChessActions
{
    /// <summary>
    /// Action that reruns the last schedule of a previous MChess test run.
    /// </summary>
    internal class AReproLastMChessTestRunScheduleAction : ARerunMChessTestAction
    {

        internal AReproLastMChessTestRunScheduleAction()
            : base("Repro Last Schedule")
        {
        }

        protected override MCutTestRunType RunType { get { return MCutTestRunType.Repro; } }

        protected override void OnBindToContext()
        {
            base.OnBindToContext();
            if (!Applicable) return;

            XElement xaction = Context.MCutTestRun.GetChessXActionMatchingStartOf(Text);
            Applicable &= xaction != null
                && Context.LastXSchedule != null
                // Don't allow repro of a repro
                && Context.MCutTestRun.RunType != MCutTestRunType.Repro
                ;
            if (!Applicable) return;

            // Need to add the last schedule from the TestRun
            var opts = new MChessOptions();
            opts.EnableRepro = true;
            opts.XSchedule = Context.LastXSchedule;

            // Merge with existing options
            opts.MergeWith(MChessOptions);
            MChessOptions = opts;
        }

    }
}
