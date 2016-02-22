/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Execution.Chess
{

    internal static class PreemptionVarsUtil
    {
        //      <racingvars>a/d/d/4 dksljkd/kdf/2 ??/1</racingvars>

        internal static string[] FindPreemptionVars(this XElement xelement)
        {
            Dictionary<string,bool> varset = new Dictionary<string,bool>();
            FindPreemptionVars(xelement, varset);
            return varset.Keys.ToArray();
        }

        internal static void FindPreemptionVars(XElement xelement, Dictionary<string,bool> varset)
        {
            if (xelement.Parent != null)
                FindPreemptionVars(xelement.Parent, varset);
            XElement x = xelement.Element(XSessionNames.PreemptionVars);
            if (x != null)
                FindPreemptionVars(x.Value, varset);
        }

        internal static void FindPreemptionVars(string str, Dictionary<string,bool> varset)
        {
            foreach (string s in str.Split(','))
                varset[s] = true;
        }
    }

}
