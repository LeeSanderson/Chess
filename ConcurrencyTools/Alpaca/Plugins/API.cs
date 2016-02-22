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

namespace Microsoft.Concurrency.TestTools.Alpaca.Plugins
{
    
    // API for UI to access plugins
    internal abstract class Plugin
    {
        // use this struct to pass parameters into plugin
        // (passed for each call, because plugin implementation is stateless)
        // plugin may (1) read/modify xpluginstate or call methods on UI,
        // but may not modify the other parameters.
        internal struct Parameters
        {
              internal IPluginEngine engine;      // handle to chessboard
              internal XElement xtest;            // the target test
              internal XElement[] xchessargs;     // chess options (as selected by user from context menu)
              internal XElement[] xargs;          // other plugin-specific options (as selected by user from context menu)
              internal XElement xpluginstate;     // encapsulated state of plugin
        }

        // called when user selects this plugin to run on a test
        internal abstract void Start(Parameters parameters); 

        // called whenever results are coming back for a run
        internal abstract void ProcessResults(Parameters parameters, RunEntity run, XElement xresults);  
    }

    // API for plugins to access UI functionality
    internal interface IPluginEngine
    {
        void LaunchPlugin(XElement xrun);
    }
}
