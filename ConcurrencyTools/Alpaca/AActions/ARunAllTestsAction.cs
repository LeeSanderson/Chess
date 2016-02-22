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
    /// Action that runs all tests contained within the current entity (including self).
    /// </summary>
    internal class ARunAllTestsAction : ARunTestActionBase
    {

        internal ARunAllTestsAction(string text = "Run All Tests")
            : base(text)
        {
        }

        protected override void OnBindToContext()
        {
            base.OnBindToContext();

            Applicable &= Target.IsTestOrHasTests()
                && !(Target is TestEntity);
        }

        public override void Go()
        {
            Context.Model.controller.AddNewCommand(new RunAllMCutTestsCommand(Target, RunOptions, true));
        }

    }
}
