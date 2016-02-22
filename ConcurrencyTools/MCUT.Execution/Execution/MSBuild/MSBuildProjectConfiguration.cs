using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using System.Text.RegularExpressions;

namespace Microsoft.Concurrency.TestTools.Execution.MSBuild
{
    /// <summary>
    /// Represents the settings for a particular msbuild configuration of a Test Project.
    /// </summary>
    [DebuggerDisplay("{Key}")]
    public class MSBuildProjectConfiguration : XElementDataObject
    {

        public static readonly Regex PropGrpConditionRegex = new Regex(
            @"^\s*'\$\(Configuration\)\|\$\(Platform\)'\s*==\s*'(?<config>\w+)\|(?<platform>\w+)'\s*$"
            , RegexOptions.Compiled | RegexOptions.ExplicitCapture
            );

        public static string CreateConfigurationKey(string configuration, string platform)
        {
            return String.Concat(configuration, '|', platform);
        }

        internal static XElement ParseXProjectConfigurationFromMSBuild(XElement xpropGrp)
        {
            if (xpropGrp == null)
                throw new ArgumentNullException("xpropGrp");
            string grpCond = (string)xpropGrp.Attribute(XMSBuild03Names.ACondition);
            if (String.IsNullOrWhiteSpace(grpCond))
                throw new ArgumentException("The Condition attribute is not defined.", "xpropGrp");
            if (!PropGrpConditionRegex.IsMatch(grpCond))
                throw new ArgumentException("The property group's Condition attribute is not in the format expected for a project configuration.", "xpropGrp");

            var match = PropGrpConditionRegex.Match(grpCond);
            var outFolderPath = (string)xpropGrp.Element(XMSBuild03Names.Prop_OutputPath);
            if (String.IsNullOrWhiteSpace(outFolderPath))
                throw new ArgumentException("The OutputPath property was not specified.");

            return new XElement(XSessionMSBuildNames.ProjectConfiguration
                , new XAttribute(XSessionMSBuildNames.AConfiguration, match.Groups["config"].Value)
                , new XAttribute(XSessionMSBuildNames.APlatform, match.Groups["platform"].Value)
                , new XAttribute(XSessionMSBuildNames.AOutputFolderPath, outFolderPath)
                );
        }

        internal MSBuildProjectConfiguration(XElement xel)
            : base(xel)
        {
            if (xel.Name != XSessionMSBuildNames.ProjectConfiguration)
                throw new ArgumentException("Invalid element.");
        }

        public string Configuration { get { return DataElement.Attribute(XSessionMSBuildNames.AConfiguration).Value; } }
        public string Platform { get { return DataElement.Attribute(XSessionMSBuildNames.APlatform).Value; } }

        public string Key { get { return CreateConfigurationKey(Configuration, Platform); } }

        /// <summary>Gets the path relative to the project file to the output for this configuration.</summary>
        public string OutputFolderPath { get { return DataElement.Attribute(XSessionMSBuildNames.AOutputFolderPath).Value; } }

    }
}
