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
    public static class XMSBuild03Names
    {
        public const string MSBuildNamespaceURI = "http://schemas.microsoft.com/developer/msbuild/2003";
        public static readonly XNamespace MSBuildNS = XNamespace.Get(MSBuildNamespaceURI);

        // Well-defined elements & attributes
        public static readonly XName Project = MSBuildNS + "Project";
        public static readonly XName PropertyGroup = MSBuildNS + "PropertyGroup";
        public static readonly XName ItemGroup = MSBuildNS + "ItemGroup";
        public static readonly XName ACondition = "Condition";


        // Common MSBuild Project property elements
        public static readonly XName Prop_AssemblyName = MSBuildNS + "AssemblyName";
        public static readonly XName Prop_Configuration = MSBuildNS + "Configuration";
        public static readonly XName Prop_Platform = MSBuildNS + "Platform";
        public static readonly XName Prop_OutputType = MSBuildNS + "OutputType";
        public static readonly XName Prop_OutputPath = MSBuildNS + "OutputPath";
    }
}
