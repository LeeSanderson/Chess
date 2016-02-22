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
    /// Action that runs all the test cases specified within the current test entity.
    /// </summary>
    internal class ARunAllMCutTestCasesAction : ARunAllMCutTestCasesAction<RunMCutTestOptions>
    {
        internal ARunAllMCutTestCasesAction(string text) : base(text) { }
    }

    /// <summary>
    /// Action that runs all the test cases specified within the current test entity.
    /// </summary>
    internal class ARunAllMCutTestCasesAction<TOpts> : ARunTestActionBase<TOpts>
        where TOpts : RunMCutTestOptions, new()
    {

        internal ARunAllMCutTestCasesAction(string text)
            : base(text)
        {
        }

        protected MCutTestEntity Test { get { return (MCutTestEntity)this.Target; } }

        protected override void OnBindToContext()
        {
            base.OnBindToContext();

            Applicable &= Target is MCutTestEntity;
        }

        public override void Go()
        {
            var cmd = new RunAllMCutTestCasesCommand(Test, MCutTestRunType.RunTest, RunOptions, true);
            Context.Model.controller.AddNewCommand(cmd);
        }

    }
}
