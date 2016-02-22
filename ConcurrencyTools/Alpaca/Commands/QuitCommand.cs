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
    internal class QuitCommand : Command
    {
        internal QuitCommand(bool interactive)
            : base(interactive)
        {
        }

        protected override bool PerformExecute(Model model)
        {
            model.mainForm.Close();
            return true;
        }
    }
}
