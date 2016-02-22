/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using Microsoft.Concurrency.TestTools.Execution;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    class Session
    {
        private Model model;
        private string _filePath;
        private string _backupFilePath;

        // change flag
        bool session_changed = false;

        public readonly SessionEntity Entity;

        internal Session(Model model, string sessionFilePath, string backupFilename)
        {
            this.model = model;

            _filePath = sessionFilePath;
            string folderPath = Path.GetDirectoryName(_filePath);
            _backupFilePath = Path.Combine(folderPath, backupFilename);

            if (File.Exists(_filePath))
            {
                //if (new FileInfo(_filePath).IsReadOnly)
                //    Console.Error.WriteLine("session file (in current directory) must be writable");
                model.xdocument = XDocument.Load(_filePath);
            }
            else
            {
                model.xdocument = new XDocument(
                    new XElement(XSessionNames.Session
                        , XNames.CreateXmlnsAttribute(true)
                        , XSessionNames.CreateXmlnsAttribute(false)
                        , XChessNames.CreateXmlnsAttribute(false)
                        ));
                model.xdocument.Save(_filePath);
            }

            //var backupFile = new FileInfo(_backupFilePath);
            //if(backupFile.Exists && backupFile.IsReadOnly)
            //    Console.Error.WriteLine("session backup file (" + backupFilename + " in current directory) must be writable");

            ValidateSessionXml();

            // Setup entities for existing elements before given a chance for the UI to respond.
            XElement xsession = model.xdocument.Root;
            if (xsession.Name != XSessionNames.Session)
                throw new Exception("Invalid session file.");
            Entity = (SessionEntity)model.EntityBuilder.CreateEntityAndBindToElement(xsession);
            Entity.InitializeSessionEntity(_filePath);
            Entity.LoadChildren(true);
        }

        internal void MarkSessionChanged()
        {
            lock (this)
                session_changed = true;
        }

        internal void AutoSave()
        {
            lock (this) // guard against reentrancy - not that it should happen anyway
            {
                // autosave
                if (session_changed)
                {
                    // move current session to backup
                    if (File.Exists(_filePath))
                    {
                        try
                        {
                            File.Copy(_filePath, _backupFilePath, true);
                        }
                        catch (IOException e)
                        {
                            MessageBox.Show("Could not save to session backup file (" + Path.GetFileName(_backupFilePath) + "). Exception: \n" + e);
                        }
                    }

                    // save session
                    try
                    {
                        model.xdocument.Save(_filePath);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Could not save to session file (" + Path.GetFileName(_filePath) + "). Exception: \n" + e);
                    }

                    ValidateSessionXml();

                    // reset flag
                    session_changed = false;
                }
            }
        }

        public void ValidateSessionXml()
        {
            // Validate the session state xml for our sake
            System.Diagnostics.Trace.WriteLine("ValidateSessionXml...");
            XSessionUtil.ValidateSessionXml(model.xdocument, new System.Xml.Schema.ValidationEventHandler((obj, e) => {
                System.Diagnostics.Trace.WriteLine(e.Message, "   ");
            }));
            System.Diagnostics.Trace.WriteLine("   Finished.");
        }

        // for debugging
        internal void DumpToConsole()
        {
            System.Console.WriteLine(model.xdocument.ToString());
        }

    }

}
