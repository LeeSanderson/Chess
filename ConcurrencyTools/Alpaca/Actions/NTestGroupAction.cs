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
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.Execution;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    /// <summary>
    /// Base Action class for actions to be performed on test entities or entities containing tests.
    /// </summary>
    internal abstract class NTestGroupAction : NTestActionBase
    {

        internal NTestGroupAction(ActionContext context, Action parent, string text)
            : base(context, parent, text)
        {
            // But not a test itself
            Applicable &= !(context.Entity is TestEntity);
        }

        /// <summary>
        /// Performs the action by (1) first, getting the xml prototype data for the action
        /// and then (2) call <see cref="PerformActionOnTest"/> for each test in the group
        /// (i.e. Entity.TestsAndSelf).
        /// </summary>
        internal override void Go()
        {
            XElement xprototype = CreatePrototype();
            foreach (var test in Context.Entity.TestsAndSelf())
                PerformActionOnTest(test, xprototype);
        }

        /// <summary>
        /// Use this to create any xml needed to perform the action. This element
        /// is then passed to each of the tests via the <see cref="PerformActionOnTest"/> method.
        /// </summary>
        /// <returns></returns>
        protected abstract XElement CreatePrototype();

        /// <summary>
        /// Perform the actual action on the specified test.
        /// </summary>
        /// <param name="test"></param>
        /// <param name="xprototype"></param>
        protected abstract void PerformActionOnTest(TestEntity test, XElement xprototype);

    }
}
