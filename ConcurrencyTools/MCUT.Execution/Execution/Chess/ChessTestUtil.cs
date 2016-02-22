using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using System.Xml.Linq;

namespace Microsoft.Concurrency.TestTools.Execution.Chess
{
    public static class ChessTestUtil
    {

        public static IEnumerable<string> GetAssembliesToInstrument(TestMethodEntity testMethod)
        {
            // Get assembly level attributes first
            IEnumerable<string> assemblies = from x in testMethod.OwningClass.OwningAssembly.DataElement.Elements(XChessNames.InstrumentAssembly)
                                             where !XmlUtil.ParseXmlBool(x.Attribute(XNames.AExclude))
                                             select x.Value;

            // Get and filter by class level attributes
            var xassies = testMethod.OwningClass.DataElement.Elements(XChessNames.InstrumentAssembly)
                .Select(x => new {
                    Name = x.Value,
                    Exclude = XmlUtil.ParseXmlBool(x.Attribute(XNames.AExclude))
                });
            assemblies = assemblies
                .Except(from x in xassies where x.Exclude select x.Name)
                .Union(from x in xassies where !x.Exclude select x.Name)
                ;

            // Filter and add from method level attributes
            xassies = testMethod.DataElement.Elements(XChessNames.InstrumentAssembly)
                .Select(x => new {
                    Name = x.Value,
                    Exclude = XmlUtil.ParseXmlBool(x.Attribute(XNames.AExclude))
                });
            assemblies = assemblies
                .Except(from x in xassies where x.Exclude select x.Name)
                .Union(from x in xassies where !x.Exclude select x.Name)
                ;

            // Done
            return assemblies;
        }

        public static void SetPreemptionOptions(MChessOptions opts, TestMethodEntity testMethod)
        {
            bool? defaultPreemptability;
            var xtogglePreempts = GetTogglePreemptionElements(testMethod, out defaultPreemptability);

            if (defaultPreemptability.HasValue)
            {
                // Since the default for Chess is to enable preemptions, the flip setting is the opposite of it
                opts.FlipPreemptionSense = !defaultPreemptability.Value;

                // And clear out the existing options since setting the sense should also reset these other settings
                opts.DontPreemptAssemblies = null;
                opts.DontPreemptNamespaces = null;
                opts.DontPreemptTypes = null;
                opts.DontPreemptMethods = null;
            }

            if (xtogglePreempts.Any())
            {
                opts.SetDontPreempts(xtogglePreempts);
            }
        }

        public static IEnumerable<XElement> GetTogglePreemptionElements(TestMethodEntity testMethod
            , out bool? defaultPreemptability
            )
        {
            defaultPreemptability = null; // Indicate it's not been specified

            // Start at the method and go up until we hit a FlipPreemptionSense element
            var xtogglePreempts = new List<XElement>();

            // Test method level
            xtogglePreempts.AddRange(testMethod.DataElement.Elements(XChessNames.TogglePreemptability));
            var xdefPreempt = testMethod.DataElement.Element(XChessNames.DefaultPreemptability);
            if (xdefPreempt != null)
            {
                defaultPreemptability = XmlUtil.ParseXmlBool(xdefPreempt);
                return xtogglePreempts;  // Once we hit a flip setting, stop traversing up
            }

            // Class level
            xtogglePreempts.AddRange(testMethod.OwningClass.DataElement.Elements(XChessNames.TogglePreemptability));
            xdefPreempt = testMethod.OwningClass.DataElement.Element(XChessNames.DefaultPreemptability);
            if (xdefPreempt != null)
            {
                defaultPreemptability = XmlUtil.ParseXmlBool(xdefPreempt);
                return xtogglePreempts;  // Once we hit a flip setting, stop traversing up
            }

            // Assembly level
            xtogglePreempts.AddRange(testMethod.OwningClass.OwningAssembly.DataElement.Elements(XChessNames.TogglePreemptability));
            xdefPreempt = testMethod.OwningClass.OwningAssembly.DataElement.Element(XChessNames.DefaultPreemptability);
            if (xdefPreempt != null)
                defaultPreemptability = XmlUtil.ParseXmlBool(xdefPreempt);

            // And we're done
            return xtogglePreempts;
        }

    }
}
