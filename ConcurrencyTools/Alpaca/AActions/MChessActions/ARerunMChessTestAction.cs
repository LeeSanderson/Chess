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
    /// Action that reruns a previous test run.
    /// These actions are only Applicable on TestResultEntity or TestRunEntities that have results.
    /// </summary>
    internal class ARerunMChessTestAction : ARerunTestAction<MChessTestRunOptions>
    {

        internal ARerunMChessTestAction(string text)
            : base(text)
        {
        }

        public MChessOptions MChessOptions
        {
            get { return RunOptions.MChessOptions; }
            set { RunOptions.MChessOptions = value; }
        }

    }
}
