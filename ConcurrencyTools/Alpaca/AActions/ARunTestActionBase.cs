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
    internal abstract class ARunTestActionBase : ARunTestActionBase<RunMCutTestOptions>
    {
        internal ARunTestActionBase(string text) : base(text) { }
    }

    /// <summary>
    /// Action that runs all the test cases specified within the current test entity.
    /// </summary>
    internal abstract class ARunTestActionBase<TOpts> : AAction
        where TOpts : RunMCutTestOptions, new()
    {

        internal ARunTestActionBase(string text)
            : base(text)
        {
            RunOptions = new TOpts();
        }

        protected override void OnBindToContext()
        {
            base.OnBindToContext();

            if (RunOptions != null)
            {
                // Set the options from the context first
                Context.SetRunOptionsFromUI(this.RunOptions);

                RunOptions.BindToContext(Context);
            }
        }

        /// <summary>
        /// Gets the options to use on top of the default behavior of the run test task.
        /// </summary>
        public TOpts RunOptions { get; private set; }

        public override bool AcceptsModifier(AActionModifier modifier)
        {
            return base.AcceptsModifier(modifier)
                || (RunOptions != null && RunOptions.AcceptsModifier(modifier));
        }

        public override void ApplyModifier(AActionModifier modifier)
        {
            base.ApplyModifier(modifier);

            if (RunOptions != null)
                RunOptions.ApplyModifier(modifier);
        }

    }
}
