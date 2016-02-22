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

namespace Microsoft.Concurrency.TestTools.Alpaca.AActions.MChessActions
{
    /// <summary>
    /// Action that repros a Chess result.
    /// </summary>
    internal class AReproChessResultAction : ARunTestActionBase<MChessTestRunOptions>
    {

        internal AReproChessResultAction(string text)
            : base(text)
        {
        }

        public XElement XAction { get; private set; }

        public MChessOptions MChessOptions
        {
            get { return RunOptions.MChessOptions; }
            set { RunOptions.MChessOptions = value; }
        }

        public bool AutoOpenConcurrencyExplorer { get; set; }

        protected override void OnBindToContext()
        {
            base.OnBindToContext();

            Applicable &= Context.ChessResult != null;
            if (!Applicable) return;

            XAction = Context.ChessResult.GetChessXActionMatchingStartOf(this.Text);
            Applicable &= XAction != null;
            if (!Applicable) return;

            Enabled &= !Context.TestRun.HasTestSourceChanged;

            Text = Text.Replace("Repro", GetSpecializedReproName());

            var opts = new MChessOptions();
            opts.EnableRepro = true;
            opts.EnableTracing = true;
            opts.TargetRace = (int?)XAction.Attribute(XChessNames.ARace);
            opts.XSchedule = Context.ChessResult.DataElement.Element(XChessNames.Schedule);

            // Merge with existing options
            opts.MergeWith(MChessOptions);
            MChessOptions = opts;
        }

        private string GetSpecializedReproName()
        {
            if (Context.ChessResult.ResultType == MChessResultType.Race)
                return "Repro Race " + Context.ChessResult.Label;

            else if (Context.ChessResult.ResultType == MChessResultType.Error)
            {
                if (Context.ChessResult.Description == "Deadlock")
                    return "Repro " + Context.ChessResult.Description;
                else
                    return "Repro Error";
            }

            return "Repro"; // Shouldn't even be Applicable
        }

        public override void Go()
        {
            var cmd = new RunMCutTestCaseCommand(Context.MCutTestRun, MCutTestRunType.Repro, RunOptions, true) {
                SelectRun = true,
                AutoOpenConcurrencyExplorer = AutoOpenConcurrencyExplorer,
            };

            Context.Model.controller.AddNewCommand(cmd);
        }

    }
}
