/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ExtendedReflection.CommandLine;
using Microsoft.ExtendedReflection.Monitoring;
using System.Collections.Specialized;
using System.Collections;
using System.Diagnostics;
using Microsoft.ExtendedReflection.Utilities;
using Microsoft.ExtendedReflection.Collections;
using Microsoft.ExtendedReflection.Utilities.Safe.Diagnostics;
using Microsoft.ManagedChess.Launcher;
using Microsoft.ManagedChess.EREngine;
using System.Reflection;
using System.IO;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.ManagedChess.Console
{
    /// <summary> 
    /// Entry point class for the user.
    /// </summary>
    /// <remarks>
    /// This class provides facilities to set up the environment,
    /// launch the application under test, etc.
    /// </remarks>

    internal static class UserMain
    {
        /// <summary>
        /// main method
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static int Main(string[] args)
        {
            try {
                // parse options
                LauncherOptions options = new LauncherOptions();
                if (!CommandLineParser.ParseArgumentsWithUsage("mchess", args, options))
                    return (int)ChessExitCode.ChessInvalidTest;
                if (!options.Validate())
                    return (int)ChessExitCode.ChessInvalidTest;

                // Since ChessTestClass has a default, lets remove it if a Concurrency Unit Test was specified.
                if (!String.IsNullOrEmpty(options.UnitTestName))
                    options.ChessTestClass = null;

                // capture xml command line
                options.XmlCommandline = XmlCommandline.XmlifyCommandline(args);

                // handle special option /addtesttolist:filename:listname:...:listname:testname
                if (!String.IsNullOrEmpty(options.AddTestToList))
                {
                    // add test to testlist
                    XmlCommandline.AddTestToList(options.AddTestToList, args);
                    // do not run test... just return.
                    return (int)ChessExitCode.Success;
                }
                
                // specific set up for ManagedChess
                var old_filename = options.FileName;
                options.FileName =  Assembly.GetAssembly(typeof(Microsoft.ManagedChess.Base.Program)).Location;
                var old_args = options.Arguments;
                options.Arguments = new string[old_args.Length+1];
                options.Arguments[0] = old_filename;
                for (int i = 0; i < old_args.Length; i++)
                {
                    options.Arguments[i + 1] = old_args[i];
                }
                return new MCLauncher(options).Execute();

            } catch (Exception e) {
                System.Console.WriteLine(e.Message);
                return (int)ChessExitCode.ChessFailure;
            }
        }

  
    }
}
