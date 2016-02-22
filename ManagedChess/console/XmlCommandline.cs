/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace Microsoft.ManagedChess.Console
{
    class XmlCommandline
    {
        // create a formatted string representing xml contents of commandline, to be included in results.xml (by ResultsPrinter.cpp)
        internal static string XmlifyCommandline(string[] args)
        {
            string indent = "          ";
            StringBuilder b = new StringBuilder();
            b.Append(indent);
            b.Append("  <executable>");
            b.Append(GetMchessExe());
            b.Append("</executable>\n");
            b.Append(indent);
            b.Append("  <startdir>" + Environment.CurrentDirectory + "</startdir>\n");
            foreach (string s in args)
            {
                b.Append(indent);
                b.Append("  <carg>");
                foreach (char c in s)
                {
                    switch (c)
                    {
                        case '<': b.Append("&lt;"); break;
                        case '&': b.Append("&amp;"); break;
                        case '>': b.Append("&gt;"); break;
                        default: b.Append(c); break;
                    }
                }
                b.Append("</carg>\n");
            }
            return b.ToString();
        }

        // get the name of the mchess executable
        public static string GetMchessExe()
        {
            System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
            return process.MainModule.FileName;
        }

        public const string ns = "{http://research.microsoft.com/chess}";

        // add elements representing xml commandline, to be used by AddTestToList functionality
        private static void GetXArgs(string[] args, XElement container)
        {
            container.Add(new XElement(ns + "startdir", Environment.CurrentDirectory));
            container.Add(new XElement(ns + "executable", GetMchessExe()));
            foreach (string s in args)
                if (!(s.StartsWith("/addtesttolist") || s.StartsWith("/at")))
                    container.Add(new XElement(ns + "carg", s));
        }
        
        
        // add test spec to testlist
        internal static void AddTestToList(string spec, string[] args)
        {
            // extract names from spec
            string[] names = spec.Split(":".ToCharArray());
            if (names.Length < 3)
               throw new Exception("Incorrect argument format for addtesttolist. Expected format is filename:listname:...:listname:testname");
            string filename = names[0];
            string testname = names[names.Length-1];
            string[] listnames = new string[names.Length-2];
            for (int i = 1; i < names.Length-1; i++)
                listnames[i-1] = names[i];

            // get/create file and top list
            FileInfo file = new FileInfo(filename);
            XDocument doc = null;
            XElement curlist = null;
            if (!file.Exists)
            {
                curlist = new XElement(ns + "testlist", new XAttribute("name", listnames[0]));
                doc = new XDocument(curlist);
            }
            else {
                doc = XDocument.Load(filename);
                curlist = doc.Element(ns + "testlist");
                if (curlist == null)
                {
                    curlist = new XElement(ns + "testlist", new XAttribute("name", listnames[0]));
                    doc.Add(curlist);
                }
                else
                {
                    XAttribute curname = curlist.Attribute("name");
                    if (curname.Value != listnames[0])
                        throw new Exception("can not add test to file because name of testlist does not match");
                }
            }
            // find/create nested lists
            for (int j = 1; j < listnames.Length; j++)
            {
                IEnumerable<XElement> matchinglists = curlist.Elements(ns + "testlist")
                          .Where(x => x.Attribute("name") != null && x.Attribute("name").Value == listnames[j]);
                XElement childlist = null;
                if (matchinglists.Count() != 0)
                {
                    childlist = matchinglists.Single();
                }
                else
                {
                    childlist = new XElement(ns + "testlist", new XAttribute("name", listnames[j]));
                    XElement childtest = curlist.Element(ns + "test");
                    if (childtest == null)
                        curlist.Add(childlist);
                    else // need precaution to add testlist before test (schema requires that order)
                        childtest.AddBeforeSelf(childlist);
                }
                curlist = childlist;
            }
            // check uniqueness of name
            if (curlist.Elements(ns + "test")
                       .Where(x => x.Attribute("name") != null && x.Attribute("name").Value == testname)
                       .Count() != 0)
                throw new Exception("can not add test to file because a test with the name \"" + testname + "\" already exists.");
            // add test
            XElement test = new XElement(ns + "test", new XAttribute("name", testname));
            GetXArgs(args, test);
            curlist.Add(test);
            // save file
            doc.Save(filename);
        }
    }
}
