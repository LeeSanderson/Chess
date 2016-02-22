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
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    internal class Engine : Plugins.IPluginEngine
    {
        internal Engine(PluginEngineEntity plugin)
        {
            _plugin = plugin;
            model = (Model)plugin.Model;
            pluginconstructor = Type.GetType(plugin.PluginTypeName).GetConstructor(new Type[0]);
        }

        private PluginEngineEntity _plugin;
        private Model model;
        private System.Reflection.ConstructorInfo pluginconstructor;

        // create engine prototype (does not contain id or options yet)
        internal static XElement GetPrototype(string runName, Type plugin)
        {
            XElement xengine = new XElement(XSessionNames.Pluginengine
                , new XAttribute(XNames.AName, runName)
                , new XElement(XSessionNames.Plugintype, plugin.FullName)
                );
            xengine.Add(new XElement(XSessionNames.Pluginstate));
            return xengine;
        }

        //internal string GetFullName()
        //{
        //    string s = _plugin.TaskName;
        //    if (_plugin.Parent != null)
        //    {
        //        if (_plugin.Parent is PluginEngineEntity)
        //            s = model.engines.GetEngine((PluginEngineEntity)_plugin.Parent).GetFullName() + "/" + s;
        //        else if (_plugin.Parent is TestEntity)
        //            s = model.tests.GetFullName(_plugin.Parent) + " : " + s;
        //    }
        //    return s;
        //}


        private Plugins.Plugin CreateNewPluginInstance()
        {
            Object o = pluginconstructor.Invoke(null);
            return (Plugins.Plugin)o;
        }

        private Plugins.Plugin.Parameters GetPluginParameters()
        {
            Plugins.Plugin.Parameters p;
            p.xtest = _plugin.OwningTest.DataElement;
            p.xpluginstate = _plugin.DataElement.Element(XSessionNames.Pluginstate);
            p.xargs = _plugin.DataElement.Elements(XNames.Arg).ToArray();
            p.xchessargs = _plugin.DataElement.Elements(XNames.Chessarg).ToArray();
            p.engine = this;
            return p;
        }

        internal void Start()
        {
            CreateNewPluginInstance().Start(GetPluginParameters());
        }

        internal void NewResults(RunEntity run, XElement xresults)
        {
            CreateNewPluginInstance().ProcessResults(GetPluginParameters(), run, xresults);
        }

        // interface to plugins
        public void LaunchPlugin(XElement xrunPrototype)
        {
            throw new NotImplementedException("Launching plugins is not implemented.");
            //TODO: Need to uncomment these lines and implement a seperate way to run a plugin
            //xrunPrototype.Add(new XElement(XNames.Engineid, id));
            //model.controller.AddNewCommand(new RunCommand(Aspects.RunType.Plugin, xelement, xrunPrototype, false));
        }

    }
}
