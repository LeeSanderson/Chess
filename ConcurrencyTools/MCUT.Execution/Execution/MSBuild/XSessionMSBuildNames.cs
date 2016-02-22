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
    /// XNames for MSBuild elements in an MCut Session.
    /// </summary>
    public static class XSessionMSBuildNames
    {
        public static readonly XNamespace SessionNS = XSessionNames.SessionNS;

        // Well-defined elements & attributes
        public static readonly XName Project = SessionNS + "msbuildProject";
        public static readonly XName AProjectFilePath = "location";
        public static readonly XName ADefaultConfiguration = "defaultConfiguration";
        public static readonly XName ADefaultPlatform = "defaultPlatform";
        public static readonly XName AOutputType = "outputType";
        public static readonly XName AAssemblyName = "assemblyName";

        public static readonly XName ProjectConfiguration = SessionNS + "projectConfiguration";
        public static readonly XName AConfiguration = "configuration";
        public static readonly XName APlatform = "platform";
        public static readonly XName AOutputFolderPath = "outputFolderPath";
    }
}
