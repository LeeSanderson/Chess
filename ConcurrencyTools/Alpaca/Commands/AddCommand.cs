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

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    /// <summary>
    /// Command for adding a child XElement node to a parent.
    /// </summary>
    class AddCommand : Command
    {
        internal AddCommand(XElement parent, XElement child)
            : base(false)
        {
            this.parent = parent;
            this.child = child;
        }

        private XElement parent;
        private XElement child;

        protected override bool PerformExecute(Model model)
        {
            if (parent.Document != null && child.Document == null) // validates situation
            {
                System.Diagnostics.Debug.Assert(child.Document == null);
                parent.Add(child);
            }
            return true;
        }

        internal override bool CheckRedundancy(List<Command> commandqueue)
        {
            return false;
        }
    }
}
