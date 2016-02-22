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
    /// Action that continues a previous MChess test run.
    /// </summary>
    internal class AContinueMChessTestRunAction : ARerunMChessTestAction
    {

        internal AContinueMChessTestRunAction()
            : base("Continue Test")
        {
        }

        protected override void OnBindToContext()
        {
            base.OnBindToContext();
            if (!Applicable) return;

            XElement xaction = Context.TestRun.GetChessXActionMatchingStartOf(Text);
            Applicable &= xaction != null && Context.LastXSchedule != null;
            if (!Applicable) return;

            // Need to add the last schedule from the TestRun
            var opts = new MChessOptions();
            opts.ContinueFromLastSchedule = true;
            opts.XSchedule = Context.LastXSchedule;

            // Merge with existing options
            opts.MergeWith(MChessOptions);
            MChessOptions = opts;
        }

    }
}
