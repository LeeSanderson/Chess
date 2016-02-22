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
using System.Threading.Tasks;
using System.IO;

namespace Microsoft.Concurrency.TestTools.Alpaca.AActions.MChessActions
{
    /// <summary>
    /// Action that runs all tests contained within the current entity (including self).
    /// </summary>
    internal class AViewConcurrencyExplorerAction : AAction
    {

        bool _isRace;

        internal AViewConcurrencyExplorerAction()
            : base("View with ConcurrencyExplorer")
        {
        }

        protected override void OnBindToContext()
        {
            base.OnBindToContext();

            Applicable &= Context.TestRun != null;
            if (!Applicable) return;

            XElement xaction = null;
            if (Context.ChessResult != null)
                xaction = Context.ChessResult.GetChessXActionMatchingStartOf(Text);
            // Disabled for Runs since unless it was a repro, there's not much benefit if a trace was run
            //if (xaction == null)
            //    xaction = Context.TestRun.GetChessXActionMatchingStartOf(Text);

            // Lets see if we're allowed this action
            Applicable &= xaction != null;
            if (!Applicable) return;

            _isRace = xaction.Attribute(XChessNames.ARace) != null
                || (Context.TestRun != null && Context.TestRun.DisplayName.StartsWith("Repro Race"))
                ;

            if (_isRace)
                this.Text = Text.Replace("View", "View Race");

            Enabled &= !Context.TestRun.HasTestSourceChanged;
        }

        public override void Go()
        {
            string traceFilePath = Path.Combine(Context.TestRun.TaskFolderPath, "trace");
            TextReader reader = new StreamReader(traceFilePath);

            // we're not actually submitting a job here
            Task.Factory.StartNew(() => {
                Microsoft.ConcurrencyExplorer.ConcurrencyExplorer ce =
                    new Microsoft.ConcurrencyExplorer.ConcurrencyExplorer(reader, _isRace);
                ce.start_controllers();
                ce.join_controllers();
            }, TaskCreationOptions.LongRunning).ContinueWith(_ => reader.Close());
        }

    }
}
