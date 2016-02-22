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
    [DebuggerDisplay("AssemblyName={AssemblyName}")]
    public class MSBuildProject : XElementDataObject
    {

        #region Static Members

        public static XElement ParseProjectFileToXElement(string projectFile)
        {
            if (!File.Exists(projectFile))
                return null;

            switch (Path.GetExtension(projectFile))
            {
                case ".csproj": return ParseCSharpProjectFileToXElement(projectFile);
                default: throw new NotImplementedException("Cannot parse project file of type: " + Path.GetExtension(projectFile));
            }
        }

        private static XElement ParseCSharpProjectFileToXElement(string projectFile)
        {
            XDocument xdoc = XDocument.Load(projectFile);
            if (xdoc.Root.Name.Namespace != XMSBuild03Names.MSBuildNS)
                throw new InvalidOperationException(String.Format(
                    "Cannot parse cs project file because it doesn't use a recognized namespace. Project file namespace: {0}; Supported namespace: {1}"
                    , xdoc.Root.Name.Namespace
                    , XMSBuild03Names.MSBuildNS
                    ));
            if (xdoc.Root.Name != XMSBuild03Names.Project)
                throw new InvalidOperationException("Invalid project file: The root element should be " + XMSBuild03Names.Project);

            XElement xproj = xdoc.Root;
            var defXProps = xproj
                .Elements(XMSBuild03Names.PropertyGroup)
                // no condition on the property group == default properties
                .Where(x => x.Attribute(XMSBuild03Names.ACondition) == null)
                ;

            var defConfig = (string)defXProps.Elements(XMSBuild03Names.Prop_Configuration).Single();
            var defPlatform = (string)defXProps.Elements(XMSBuild03Names.Prop_Platform).Single();
            var outputType = defXProps.Elements(XMSBuild03Names.Prop_OutputType).Single().ParseXmlEnum<MSBuildProjectOutputType>().Value;
            var assyName = defXProps.Elements(XMSBuild03Names.Prop_AssemblyName).Single().Value;
            var xsessionProj = new XElement(XSessionMSBuildNames.Project
                , new XAttribute(XSessionMSBuildNames.AProjectFilePath, Path.GetFullPath(projectFile))
                , new XAttribute(XSessionMSBuildNames.ADefaultConfiguration, defConfig)
                , new XAttribute(XSessionMSBuildNames.ADefaultPlatform, defPlatform)
                , new XAttribute(XSessionMSBuildNames.AOutputType, outputType)
                , new XAttribute(XSessionMSBuildNames.AAssemblyName, assyName)
                // Parse the available configurations
                , from xpropGrp in xproj.Elements(XMSBuild03Names.PropertyGroup)
                  let grpCond = (string)xpropGrp.Attribute(XMSBuild03Names.ACondition)
                  where !String.IsNullOrWhiteSpace(grpCond)
                  where MSBuildProjectConfiguration.PropGrpConditionRegex.IsMatch(grpCond)
                  select MSBuildProjectConfiguration.ParseXProjectConfigurationFromMSBuild(xpropGrp)
                );

            return xsessionProj;
        }

        #endregion

        private MSBuildProjectConfiguration[] _configurations;

        public MSBuildProject(XElement xel)
            : base(xel)
        {
            // Since each instance is mutable, we create the objects for the configurations right now.
            _configurations = DataElement
                .Elements(XSessionMSBuildNames.ProjectConfiguration)
                .Select(x => new MSBuildProjectConfiguration(x))
                .ToArray();
        }

        /// <summary>Gets the full path to the project file.</summary>
        public string ProjectFilePath { get { return DataElement.Attribute(XSessionMSBuildNames.AProjectFilePath).Value; } }
        public string ProjectFolderPath { get { return Path.GetDirectoryName(ProjectFilePath); } }

        public string DefaultConfiguration { get { return DataElement.Attribute(XSessionMSBuildNames.ADefaultConfiguration).Value; } }
        public string DefaultPlatform { get { return DataElement.Attribute(XSessionMSBuildNames.ADefaultPlatform).Value; } }
        public string DefaultConfigurationKey { get { return MSBuildProjectConfiguration.CreateConfigurationKey(DefaultConfiguration, DefaultPlatform); } }

        public MSBuildProjectOutputType OutputType { get { return DataElement.Attribute(XSessionMSBuildNames.AOutputType).ParseXmlEnum<MSBuildProjectOutputType>().Value; } }
        public string AssemblyName { get { return DataElement.Attribute(XSessionMSBuildNames.AAssemblyName).Value; } }
        public string OutputFilename { get { return AssemblyName + GetOutputFileExtension(); } }

        private string GetOutputFileExtension()
        {
            switch (OutputType)
            {
                case MSBuildProjectOutputType.Library: return ".dll";
                case MSBuildProjectOutputType.Exe: return ".exe";

                case MSBuildProjectOutputType.Module: //return ".dll";
                case MSBuildProjectOutputType.Winexe: //return ".exe";
                default: throw new NotImplementedException("Unhandled output type: " + OutputType);
            }
        }

        public IEnumerable<MSBuildProjectConfiguration> Configurations { get { return _configurations; } }

    }
}
