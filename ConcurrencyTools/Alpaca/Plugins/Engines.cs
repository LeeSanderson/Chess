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
    class Engines
    {

        private Model model;
        private Dictionary<int, Engine> _engines;

        public Engines(Model model)
        {
            this.model = model;
            _engines = new Dictionary<int, Engine>();

            model.SessionInitialized += new Handler(model_NewSessionEvt);
            model.EntityChanged += new ModelEntityEventHandler<EntityChangeEventArgs>(model_EntityChanged);
        }

        internal Engine this[int engineID]
        {
            get { return _engines[engineID]; }
        }

        private void model_NewSessionEvt()
        {
            EnginesAdded(model.session.Entity.DescendantsAndSelf<PluginEngineEntity>());
        }

        void model_EntityChanged(EntityBase entity, EntityChangeEventArgs e)
        {
            if (e.EntityChange == EntityChange.Add)
                EnginesAdded(entity.DescendantsAndSelf<PluginEngineEntity>());
            else if (e.EntityChange == EntityChange.Remove)
                EnginesRemoved(entity.DescendantsAndSelf<PluginEngineEntity>());
        }

        private void EnginesAdded(IEnumerable<PluginEngineEntity> plugins)
        {
            foreach (var plugin in plugins)
            {
                Engine e = BindEngine(plugin);
                _engines.Add(plugin.EngineID, e);
            }
        }

        private void EnginesRemoved(IEnumerable<PluginEngineEntity> plugins)
        {
            foreach (var plugin in plugins)
            {
                _engines.Remove(plugin.EngineID);
            }
        }

        /// <summary>
        /// Gets the engine associated with the plugin.
        /// The engine is created if not bound to the enitty.
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        internal Engine BindEngine(PluginEngineEntity plugin)
        {
            Engine engine = plugin.DataElement.Annotation<Engine>();
            if (engine == null)
            {
                engine = new Engine(plugin);
                plugin.DataElement.AddAnnotation(engine);
            }
            return engine;
        }

        internal void CreateAndStart(XElement xcontainer, XElement prototype)
        {
            // add id to parameters
            int engineID = model.session.Entity.RuntimeState.GetNextEngineID();
            prototype.Add(new XElement(XSessionNames.Engineid, engineID));

            // link into session, start
            xcontainer.Add(prototype);
            var entity = (PluginEngineEntity)prototype.GetEntity();
            System.Diagnostics.Debug.Assert(entity != null, "Adding to the doc tree should've created the entity.");
            Engine engine = BindEngine(entity);
            engine.Start();
        }

    }
}
