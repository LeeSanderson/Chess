/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Utility class for composing a command line.
    /// </summary>
    public static class CommandlineUtil
    {

        /// <summary>
        /// Composes a command line using the specified executable and arguments.
        /// </summary>
        /// <param name="executable"></param>
        /// <param name="args">List of non-escaped raw arguments. Each arg will be quote escaped if it contains space or quotes itself.</param>
        /// <returns></returns>
        public static string ComposeCommandline(string executable, IEnumerable<string> args)
        {
            // build and return command line
            StringBuilder commandline = new StringBuilder();
            commandline.Append(QuoteCommandLinePart(executable, isArg: false));

            string cmdArgs = ComposeCommandlineArguments(args);
            if (cmdArgs != null)
            {
                commandline.Append(" ");
                commandline.Append(cmdArgs);
            }

            return commandline.ToString();
        }

        public static string ComposeCommandlineArguments(IEnumerable<string> args)
        {
            if (args == null || !args.Any())
                return null;

            StringBuilder sb = new StringBuilder();
            foreach (string arg in args)
            {
                sb.Append(" ");
                sb.Append(QuoteCommandLinePart(arg));
            }

            return sb.ToString().Trim();
        }

        /// <summary>
        /// Escapes a command line executable or argument with quotes if it's needed.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="isArg"></param>
        /// <returns></returns>
        public static string QuoteCommandLinePart(string part, bool isArg = true)
        {
            if (part.Contains(' ') && !part.Contains('"'))
            {
                // The Windows command line parser will turn the following quoted arg
                // "foo\bar\" into "foo\bar\"". i.e. The trailing '\' char escapes the ending quote.
                // We need to detect this special case for when the user wants that '\' at the end
                if (part.EndsWith("\\"))
                    part = part + "\\";

                part = "\"" + part + "\"";
            }

            return part;
        }

    }
}
