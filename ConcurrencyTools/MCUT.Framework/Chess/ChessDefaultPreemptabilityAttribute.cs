
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace Microsoft.Concurrency.TestTools.UnitTesting.Chess
{
    // TODO: JAM - Create unit tests for the ChessDefaultPreemptabilityAttribute attribute.
    /// <summary>
    /// Indicates that Chess should enable or disable preemptions by default.
    /// Use of <see cref="ChessTogglePreemptabilityAttribute"/> can help to specify
    /// the opposite of this default.
    /// Without specifying this attribute Chess has preemptions enabled.
    /// </summary>
    /// <remarks>
    /// Setting of this attribute will force the developer to redefine any
    /// <see cref="ChessTogglePreemptabilityAttribute"/>s that were defined higher in
    /// the test assembly hierarchy (i.e. assembly, test class, test method).
    /// </remarks>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ChessDefaultPreemptabilityAttribute : Attribute
    {

        /// <summary>
        /// Creates a new instance indicating whether preemptions should be enabled by default.
        /// </summary>
        public ChessDefaultPreemptabilityAttribute(bool enabled)
        {
            Enabled = enabled;
        }

        /// <summary>Indicates whether Chess should enable preemptions by default.</summary>
        public bool Enabled { get; private set; }

        internal XElement ToTestlistXml()
        {
            return new XElement(XChessNames.DefaultPreemptability, Enabled);
        }

    }
}