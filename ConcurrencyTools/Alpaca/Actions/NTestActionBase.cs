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

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    /// <summary>
    /// Base Action class for actions to be performed on test entities or entities containing tests.
    /// </summary>
    internal abstract class NTestActionBase : Action
    {

        internal NTestActionBase(ActionContext context, Action parent, string text)
            : base(context, parent, text)
        {
            Applicable &= context.Entity.IsTestOrHasTests();
        }

    }
}
