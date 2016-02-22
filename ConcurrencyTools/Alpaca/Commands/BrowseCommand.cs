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
using System.IO;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    internal class BrowseCommand : Command
    {

        enum CmdApp { OpenNotepad, CmdShell, Explorer }

        internal static BrowseCommand OpenCmdShell(string dir)
        {
            return new BrowseCommand(CmdApp.CmdShell, dir);
        }

        internal static BrowseCommand OpenNotepad(string filename)
        {
            return new BrowseCommand(CmdApp.OpenNotepad, filename);
        }

        internal static BrowseCommand OpenExplorer(string path, bool select)
        {
            return new BrowseCommand(CmdApp.Explorer, path, select);
        }


        CmdApp _cmdApp;
        string _path;
        bool _selectInExplorer;

        private BrowseCommand(CmdApp cmd, string path, bool select = false)
            : base(true)
        {
            _cmdApp = cmd;
            _path = path;
            _selectInExplorer = select;
        }


        protected override bool PerformExecute(Model model)
        {
            string path = Path.GetFullPath(_path);

            System.Diagnostics.Process p = null;
            switch (_cmdApp)
            {
                case CmdApp.OpenNotepad:
                    if (!File.Exists(path))
                    {
                        SetError("File couldn't be found: " + Environment.NewLine + path);
                        break;
                    }

                    p = new System.Diagnostics.Process();
                    p.StartInfo.FileName = "notepad";
                    p.StartInfo.Arguments = path;
                    p.StartInfo.UseShellExecute = false;
                    break;

                case CmdApp.CmdShell:
                    if (!Directory.Exists(path))
                    {
                        SetError("Directory couldn't be found: " + Environment.NewLine + path);
                        break;
                    }

                    p = new System.Diagnostics.Process();
                    p.StartInfo.FileName = "cmd";
                    p.StartInfo.Arguments = "/K \"PUSHD " + path + "\"";
                    p.StartInfo.UseShellExecute = true;
                    break;

                case CmdApp.Explorer:
                    if (!File.Exists(path) && !Directory.Exists(path))
                    {
                        SetError("File or directory couldn't be found: " + Environment.NewLine + path);
                        break;
                    }

                    p = new System.Diagnostics.Process();
                    p.StartInfo.FileName = "explorer";
                    if (_selectInExplorer)
                        p.StartInfo.Arguments = String.Format("/select,\"{0}\"", path);
                    else
                        p.StartInfo.Arguments = path;
                    p.StartInfo.UseShellExecute = false;
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (p != null)
                p.Start();

            return true;
        }

        internal override bool CheckRedundancy(List<Command> commandqueue)
        {
            return false;
        }
    }

}
