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
    internal class ARunAllObservationGeneratorsAction : ARunTestActionBase
    {

        internal ARunAllObservationGeneratorsAction(string text = "Run All Observation Generators")
            : base(text)
        {
        }

        protected override void OnBindToContext()
        {
            base.OnBindToContext();

            Applicable &= !(Target is ObservationGeneratorEntity)
                && Target.Descendants<ObservationGeneratorEntity>().Any();
        }

        public override void Go()
        {
            var generators = Target.Descendants<ObservationGeneratorEntity>();

            foreach (var obsGen in generators)
            {
                var cmd = new RunAllMCutTestCasesCommand(obsGen, MCutTestRunType.RunTest, RunOptions, true);
                Context.Model.controller.AddNewCommand(cmd);
            }
        }

    }
}
