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
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Chess;
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Alpaca.Plugins
{
    // define plugin for iterative race reduction
    // (this class is not directly instantiated; see ActionContext.cs for how to make it appear as a menu item  )

    internal class RaceReduction : Plugin
    {
        internal override void Start(Parameters parameters)
        {
            // init state
            InitCounter(parameters.xpluginstate);
            InitVarlist(parameters.xpluginstate);

            // launch first iteration
            LaunchNextIteration(parameters);
        }

        internal override void ProcessResults(Parameters parameters, RunEntity run, XElement xresults)
        {
            bool need_another_iteration = false;
            List<string> curlist = GetVarlist(parameters.xpluginstate);

            // extract the preemption variables from results, then check if there are any new ones
            IEnumerable<XElement> pvs = xresults.Descendants(XSessionNames.PreemptionVars);
            if (pvs.Count() > 0)
            {
                foreach (string s in pvs.Single().Value.Split(new char[] { ',' }))
                    if (!curlist.Contains(s))
                    {
                        need_another_iteration = true;
                        curlist.Add(s);
                    }
            }

            if (need_another_iteration)
            {
                SetVarlist(parameters.xpluginstate, curlist);
                LaunchNextIteration(parameters);
            }
        }

        private void LaunchNextIteration(Parameters parameters)
        {
            // increment the counter
            int count = GetCounter(parameters.xpluginstate) + 1;
            SetCounter(parameters.xpluginstate, count);

            // figure out name
            string commadelimitedlist = string.Join(",", GetVarlist(parameters.xpluginstate).ToArray());
            string name = "Run " + count;
            if (!string.IsNullOrEmpty(commadelimitedlist))
                name = name + " [" + commadelimitedlist + "]";

            // prepare a new run
            XElement xrun = TestRunEntity.CreateRunXPrototype(name);

            // add chess parameters for the run (selected by user from context menu)
            foreach (XElement xchessarg in parameters.xchessargs)
                xrun.Add(xchessarg);

            // turn on race detection
            xrun.Add(new ChessArgsModifier("/detectraces").ToXml());

            // add preemption variable parameter
            if (!string.IsNullOrEmpty(commadelimitedlist))
                xrun.Add(new XElement(XSessionNames.PreemptionVars, commadelimitedlist));

            // launch the run
            parameters.engine.LaunchPlugin(xrun);
        }


        // plugin state variable : counter 
        // e.g. <counter>23</counter>

        private void InitCounter(XElement xpluginstate)
        {
            xpluginstate.Add(new XElement(XSessionNames.Counter, 0));
        }
        private int GetCounter(XElement xpluginstate)
        {
            return (int)xpluginstate.Element(XSessionNames.Counter);
        }
        private void SetCounter(XElement xpluginstate, int value)
        {
            xpluginstate.SetElementValue(XSessionNames.Counter, value);
        }

        // plugin state variable : set of preemption variables
        // e.g. <preemptionvars>522,523,527</preemptionvars>

        private void InitVarlist(XElement xpluginstate)
        {
            xpluginstate.Add(new XElement(XSessionNames.PreemptionVars, ""));
        }
        private List<string> GetVarlist(XElement xpluginstate)
        {
            string commaseparatedlist = xpluginstate.Element(XSessionNames.PreemptionVars).Value;
            List<string> list = new List<string>();
            foreach (string s in commaseparatedlist.Split(new char[] { ',' }))
                if (!string.IsNullOrEmpty(s))
                    list.Add(s);
            return list;
        }
        private void SetVarlist(XElement xpluginstate, List<string> list)
        {
            xpluginstate.SetElementValue(XSessionNames.PreemptionVars, string.Join(",", list.ToArray()));
        }
    }
}
