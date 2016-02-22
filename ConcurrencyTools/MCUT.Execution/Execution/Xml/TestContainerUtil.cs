/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using System.Xml.Linq;
using System.IO;
using System.Reflection;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.Execution.Xml
{
    public static class TestContainerUtil
    {

        public static XDocument ReadTestContainerInSeperateProcess(string assemblyLocation)
        {
            string assyXml;
            string errMsg;

            assemblyLocation = Path.GetFullPath(assemblyLocation);
            string assyFolderPath = Path.GetDirectoryName(assemblyLocation);

            if (!Directory.Exists(assyFolderPath))
            {
                assyXml = TestAssemblyReader.CreateXTestAssemblyPlaceholder(assemblyLocation).ToString();
                errMsg = null;
            }
            else
            {
                using (Process p = new Process())
                {
                    p.StartInfo = new ProcessStartInfo(MCutToolsUtil.MCutExecutableFilePath) {
                        WorkingDirectory = assyFolderPath,
                        Arguments = String.Format("getTestListFromAssembly \"{0}\"", Path.GetFileName(assemblyLocation)),
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    };
                    p.Start();

                    assyXml = p.StandardOutput.ReadToEnd();
                    errMsg = p.StandardError.ReadToEnd();
                    p.WaitForExit();
                }
            }

            if (String.IsNullOrWhiteSpace(errMsg))
                return XDocument.Parse(assyXml, LoadOptions.SetLineInfo);
            else
                return new XDocument(XNames.CreateXError(errMsg));
        }

        // load document
        /// <summary>
        /// Loads the test container file.
        /// Always returns a non-null value, or else an exception is thrown.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="recursively"></param>
        /// <returns></returns>
        public static XElement LoadTestContainer(string filename, bool recursively)
        {
            // Normalize the path.
            string filePath = Path.GetFullPath(filename);

            XDocument doc;
            switch (Path.GetExtension(filePath).ToLower())
            {
                case ".exe":
                case ".dll":
                    doc = ReadTestContainerInSeperateProcess(filePath);
                    if (doc.Root.Name == XNames.Error)
                    {
                        System.Diagnostics.Trace.WriteLine(doc.Root, "Error reading test assembly");
                        throw new Exception((string)doc.Root.Element(XNames.ErrorMessage));
                    }
                    break;

                case ".csproj":
                    if (!File.Exists(filePath))
                        throw new Exception("can not load test project. \"" + filePath + "\" does not exist.");

                    // Create a new project entity xml with just the information we know.
                    doc = new XDocument(
                        new XElement(XNames.TestProject
                            , new XAttribute(XNames.ALocation, filePath)
                        // Loaded after attaching to a session
                        //, MSBuild.MSBuildProject.ParseProjectFileToXElement(filePath)
                            ));
                    break;

                default:
                    if (!File.Exists(filePath))
                        throw new Exception("can not load testlist. \"" + filePath + "\" does not exist.");
                    doc = XDocument.Load(filePath, LoadOptions.SetLineInfo);
                    break;
            }

            // Validate the test container xml
            try
            {
                UnitTestingSchemaUtil.ValidateTestListXml(doc);
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid test container \"" + filePath + "\"." + ex.Message);
            }


            // detach from the document
            XElement xroot = doc.Root;
            xroot.Remove();

            // For testlists that we load from the file system, allow them to be reloaded
            if (xroot.Name == XNames.Testlist)
                xroot.SetAttributeValue(XNames.ALocation, filePath); // Always update just in case the file was moved from the current value

            // replace @ character with directory
            string folderPath = Path.GetDirectoryName(filePath);
            foreach (XElement x in xroot.Descendants())
            {
                if (x.Name == XNames.Executable
                    || x.Name == XNames.WorkingDirectory
                    || x.Name == XNames.Shellline
                    || x.Name == XNames.Arg
                    )
                {
                    string v = x.Value;
                    if (v.Contains('@'))
                        x.Value = Path.GetFullPath(v.Replace("@", folderPath));
                }
                else if (x.Name == XNames.Include
                    || x.Name == XNames.TestProject
                    )
                {
                    XAttribute xloc = x.Attribute(XNames.ALocation);
                    string v = xloc.Value;
                    if (v.Contains('@'))
                        xloc.Value = Path.GetFullPath(v.Replace("@", folderPath));
                }

                foreach (XAttribute a in x.Attributes().Where(a =>
                    a.Name == XNames.AObservationFile
                    || a.Name == XNames.AProject
                    ))
                {
                    string v = a.Value;
                    if (v.Contains('@'))
                        a.Value = Path.GetFullPath(v.Replace("@", folderPath));
                }
            }

            if (recursively)
            {
                // replace <include location="filename"/> with contents
                XElement[] includes = xroot.Descendants(XNames.Include).ToArray();
                foreach (XElement xinclude in includes)
                {
                    // We've already fully resolved the location attribute to it's full path above
                    string loc = (string)xinclude.Attribute(XNames.ALocation);
                    try
                    {
                        var xcontainer = LoadTestContainer(loc, recursively);

                        MergeInSettingsFromInclude(xcontainer, xinclude);

                        xinclude.ReplaceWith(xcontainer);
                    }
                    catch (Exception ex)
                    {
                        xinclude.Add(XConcurrencyNames.CreateXError("Error trying to load included test container: " + ex.Message));
                    }
                }
            }

            return xroot;
        }

        public static void MergeInSettingsFromInclude(XElement xtestContainer, XElement xinclude)
        {
            // Copy the GoldenObservationFilesFolderPath setting to the assembly
            if (xtestContainer.Name == XNames.TestAssembly)
            {
                var goldenObs = (string)xinclude.Element(XNames.GoldenObservationFilesFolderPath);
                if (!String.IsNullOrEmpty(goldenObs))
                {
                    xtestContainer.AddFirst(new XElement(XNames.GoldenObservationFilesFolderPath, goldenObs));
                }
            }
        }

    }
}
