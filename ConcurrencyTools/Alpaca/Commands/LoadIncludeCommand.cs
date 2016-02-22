/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Windows.Forms;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    internal class LoadIncludeCommand : Command
    {

        internal LoadIncludeCommand(IncludeEntity include, bool interactive)
            : base(interactive)
        {
            if (include == null)
                throw new ArgumentNullException("include");

            IncludeEntity = include;
        }

        public IncludeEntity IncludeEntity { get; private set; }

        internal override bool CheckRedundancy(List<Command> commandqueue)
        {
            return commandqueue
                .OfType<LoadIncludeCommand>()
                .Any(cmd => cmd.IncludeEntity == this.IncludeEntity);
        }

        protected override bool PerformExecute(Model model)
        {
            // We register with the session because this command is to process an include, not to
            // refresh one. Therefore, any registered entities should get registered as new ones.
            if (!IncludeEntity.TryLoad(true, true))
                SetError("Error: failed to import test container.\n" + IncludeEntity.LoadError.Message);

            return true;
        }
    }
}

