
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace Microsoft.Concurrency.TestTools.UnitTesting.Chess
{
    // TODO: JAM - Create unit tests for the ChessDontPreemptAttribute attribute.
    /// <summary>
    /// Toggles whether preemptions should be enabled or disabled on a target
    /// assembly, namespace, type or method. This toggles the setting specified in the
    /// <see cref="ChessDefaultPreemptabilityAttribute"/> at the current declaration
    /// level in the test assembly hierarchy (i.e. assembly, test class, test method).
    /// If no <see cref="ChessDefaultPreemptabilityAttribute"/> is defined, Chess enables
    /// preemptions by default.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ChessTogglePreemptabilityAttribute : Attribute
    {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="targetKind">The type of target: assembly, namespace, type or method.</param>
        /// <param name="targetName">The name of the target to not preempt.</param>
        public ChessTogglePreemptabilityAttribute(PreemptabilityTargetKind targetKind, string targetName)
        {
            if (String.IsNullOrWhiteSpace(targetName))
                throw new ArgumentException("The target's name must be specified.", "targetName");

            TargetKind = targetKind;
            TargetName = targetName;
        }

        /// <summary>The kind of the target.</summary>
        public PreemptabilityTargetKind TargetKind { get; private set; }

        /// <summary>The name of the target to toggle preemptability.</summary>
        public string TargetName { get; private set; }

        internal XElement ToTestlistXml()
        {
            return new XElement(XChessNames.TogglePreemptability
                , new XAttribute("targetKind", TargetKind)
                , TargetName
                );
        }

    }
}