using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace Microsoft.Concurrency.TestTools.UnitTesting.Chess
{
    // TODO: JAM - Create unit tests for the ChessInstrumentAssemblyAttribute attribute.
    /// <summary>
    /// Instructs Chess to instrument an additional assembly other than the one containing the test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ChessInstrumentAssemblyAttribute : Attribute
    {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="assembly">The short or full name of the assembly.</param>
        public ChessInstrumentAssemblyAttribute(string assembly)
        {
            Assembly = assembly;
        }

        /// <summary>The name of the assembly.</summary>
        public string Assembly { get; private set; }

        /// <summary>
        /// When true, it will exclude the assembly from instrumentation.
        /// e.g. When applied to a method, if the assembly was defined as being instrumented by the owning class,
        /// then this would remove it from the list of assemblies to instrument by chess for this method.
        /// </summary>
        public bool Exclude { get; set; }

        internal XElement ToTestlistXml()
        {
            return new XElement(XChessNames.InstrumentAssembly
                , Exclude ? new XAttribute("exclude", Exclude) : null
                , Assembly
                );
        }

    }
}