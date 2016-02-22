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
    internal abstract class StagedCommand : Command
    {

        private IEnumerator<bool> myiterator; // the bool value is ignored

        internal StagedCommand(bool interactive)
            : base(interactive)
        {
        }

        protected override bool PerformExecute(Model model)
        {
            if (myiterator == null)
                myiterator = ExecuteStages(model).GetEnumerator();

            bool done = !myiterator.MoveNext();
            return done;
        }

        protected abstract IEnumerable<bool> ExecuteStages(Model model);

        internal override bool CheckRedundancy(List<Command> commandqueue)
        {
            return false;
        }

    }
}
