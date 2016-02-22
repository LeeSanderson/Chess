/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    internal class QuiesceCommand : Command
    {
        internal QuiesceCommand(bool interactive) : this(null,interactive)
        {
        }
        internal QuiesceCommand(Command followup, bool interactive)
            : base(interactive)
        {
            this.followup = followup;
        }

        Command followup;

        protected override bool PerformExecute(Model model)
        {
            model.controller.AddQuiesceCommand(followup);
            return true;
        }
    }
}
