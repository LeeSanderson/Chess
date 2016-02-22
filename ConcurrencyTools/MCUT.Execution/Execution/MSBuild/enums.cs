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
    public enum MSBuildProjectOutputType
    {
        /// <summary>The output file type is a .net library (extension: '.dll').</summary>
        Library,
        /// <summary>The output file type is a .net executable (extension: '.exe').</summary>
        Exe,
        Module,
        Winexe
    }

    public static class MSBuildConfigurationNames
    {
        public const string Debug = "Debug";
        public const string Release = "Release";
    }

    public static class MSBuildPlatformNames
    {
        public const string AnyCPU = "AnyCPU";
        public const string x86 = "x86";
        public const string Win32 = "Win32";
    }
}
