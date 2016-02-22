using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Utility that provides information about the current MCUT tool set.
    /// i.e. locations of executables within the current bin folder of the running application.
    /// </summary>
    public static class MCutToolsUtil
    {

        /// <summary>Gets the full path to the mchess executable for the current tool set.</summary>
        public static string MChessExecutableFilePath { get; private set; }

        /// <summary>Gets the full path to the mcut executable for the current tool set.</summary>
        public static string MCutExecutableFilePath { get; private set; }

        public static string MSBuildExecutableFilename { get { return "msbuild"; } }

        static MCutToolsUtil()
        {
            string binPath = Path.GetFullPath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

            MChessExecutableFilePath = Path.Combine(binPath, "mchess");
            MCutExecutableFilePath = Path.Combine(binPath, "mcut");
        }

        /// <summary>
        /// Gets the path to the vcvarsall.bat file for the currently installed version of Visual Studio.
        /// Works with VS 2008 and 2010.
        /// </summary>
        /// <returns></returns>
        public static string GetVSCmdLineVarsFilePath()
        {
            // Add the msbuild env vars cmd line (Works with VS 2008 and 2010)
            // e.g. "C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\Tools\"
            string vstoolsPath = Environment.GetEnvironmentVariable("VS100COMNTOOLS")
                ?? Environment.GetEnvironmentVariable("VS90COMNTOOLS");
            if (vstoolsPath == null)
                return null;

            string vcvarsPath = Path.GetFullPath(Path.Combine(vstoolsPath, @"..\..\VC\vcvarsall.bat"));

            return vcvarsPath;
        }



    }
}
