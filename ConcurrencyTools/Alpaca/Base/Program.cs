/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Windows.Forms;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    public class Program
    {

        private static readonly string usage = String.Format(@"Usage:
  {0} [/import (<testlist> | <testAssembly>)]

    /import - Imports the testlist or .net assembly into the session.
", Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location));

        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string sessionFilename = "session.xml";
            string sessionBackupFilename = "session.backup.xml";

            string errmsg;
            bool showUI;
            IEnumerable<Command> batchcommands = GetBatchCommands(args, out errmsg, out showUI);
            if (errmsg != null)
            {
                Console.Error.WriteLine(errmsg);
            }
            else if (showUI)
            {
                Model model = new Model();
                model.StartSession(Path.GetFullPath(sessionFilename), sessionBackupFilename, batchcommands);

                Application.Run(model.mainForm);
            }
        }

        private static IEnumerable<Command> GetBatchCommands(string[] args, out string errormsg, out bool showUI)
        {
            showUI = true;
            List<Command> commands = new List<Command>();
            errormsg = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "/import" && (++i < args.Length))
                {
                    // We want this to be interactive because we want to be notified when the list can't be loaded, i.e. errors out.
                    string containerPath = args[i];
                    commands.Add(new CustomCommand("Load test container from command-line", true, model => {
                        model.session.Entity.AddTestContainer(containerPath);
                    }));
                }
                else
                    errormsg = usage;
            }

            return commands;
        }
    }
}
