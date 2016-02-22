using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Alpaca.Aspects
{
    class PluginEngineEntity : RunEntity
    {

        public PluginEngineEntity(XElement el)
            : base(el)
        {
        }

        new public Model Model { get { return (Model)base.Model; } }

        public override string DisplayName { get { return base.Name ?? "Plugin"; } }
        public override string FullName { get { return DisplayName; } }

        public override bool UsesResults { get { return true; } }

        public int EngineID { get { return (int)DataElement.Element(XSessionNames.Engineid); } }
        public string PluginTypeName { get { return (string)DataElement.Element(XSessionNames.Plugintype); } }


        protected override IEnumerable<XElement> GetChildElements()
        {
            return DataElement.Elements()
                .Where(el =>
                    el.Name == XSessionNames.Pluginengine
                    || el.Name == XSessionNames.Run
                    );
        }


        public TestEntity OwningTest { get { return Ancestors().OfType<TestEntity>().First(); } }
        public override string StartFolderPath { get { return ScriptFolderPath; } }

        /// <param name="testDir">The directory in which the test is being run.</param>
        public override string GetCommandLine()
        {
            throw new NotImplementedException("Running a plugin isn't currently supported.");
            // the test element knows all
            //return OwningTest.GetCommandLine(this);
        }

        public override IEnumerable<string> GetShellLines()
        {
            throw new NotImplementedException("Running a plugin isn't currently supported. Not sure it would be appropriate or correct to use the same script lines for all plugins.");
            // the build element knows all
            //return OwningTest.GetShellLines();
        }

        //public override void CheckForResults()
        //{
        //    XElement xchessResults = TaskState.TaskHandle.GetXmlResults();
        //    if (xchessResults != null)
        //        ProcessTaskResults(xchessResults);
        //}

        internal void ProcessTaskResults(XElement xtaskResults)
        {
            // TODO: Might need to add this element elsewhere once plugins are finished
            DataElement.Add(xtaskResults);

            // if this run was started by an engine, notify it now
            int? xengineid = (int?)DataElement.Element(XSessionNames.Engineid);
            if (xengineid.HasValue)
            {
                Engine engine = Model.engines[xengineid.Value];
                engine.NewResults(this, xtaskResults);
            }
        }

    }
}
