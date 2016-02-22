/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;

[assembly: CLSCompliant(true)]
namespace Microsoft.ConcurrencyExplorer
{
    internal interface ISimpleChessMonitor
    {
        void NewExecution();
        void ProcessTuple(int tid, int nr, int attr, String value);
        void WaitForGUI();
    }

    public class ConcurrencyExplorer : ISimpleChessMonitor
    {
        private List<IController> controllers = new List<IController>();
        private EventController eventcontroller;
        private GuiController guicontroller;

        public ConcurrencyExplorer(TextReader stream, bool racedisplay)
        {
            // make model
            IModel model = new EventDB();

            // get first line to determine type of trace file
            bool recordAndReplay = false;
            string marker = stream.ReadLine();
            if (marker == "m")
                recordAndReplay = false;
            else if (marker == "s")
                recordAndReplay = true;
            else
            {
                System.Console.WriteLine("the specified file is not a valid trace file.");
                System.Environment.Exit(0);
            }

            // make GUI controller
            controllers.Add(guicontroller = new GuiController(!recordAndReplay, racedisplay, model));

            // make stream or event controller
            if (stream != null)
            {
                controllers.Add(new StreamController(stream, model));
            }
            else
            {
                controllers.Add(eventcontroller = new EventController(model));
            }
        }

        private bool started = false;

        public void start_controllers()
        {
            // start all controllers
            foreach (IController c in controllers)
            {
                c.Start();
            }
            started = true;
        }

        public void join_controllers()
        {
            // wait for controllers to finish
            foreach (IController c in controllers)
            {
                c.Join();
            }
        }

        // start on first call to NewExecution; delegate ISimpleChessMonitor calls
        public void NewExecution()
        {
            if (!started)
                start_controllers();
            eventcontroller.NewExecution();
        }
        public void ProcessTuple(int tid, int nr, int attr, String value)
        {
            System.Diagnostics.Debug.Assert(started);
            eventcontroller.ProcessTuple(tid, nr, attr, value);
        }
        public void WaitForGUI()
        {
            if (started)
            {
                eventcontroller.Complete();
                join_controllers();
            }
        }

        [STAThread]
        static void Main(String[] args)
        {
            String filename = null;
            bool racedisplay = false;
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i] == "/b")
                {
                    System.Diagnostics.Debugger.Break();
                    //System.Console.WriteLine("setting debug break");
                }
                else if (args[i] == "/r")
                {
                    racedisplay = true;
                    //System.Console.WriteLine("setting racedisplay");
                }
                else
                {
                    filename = args[i];
                }
            }
            TextReader reader;
            if (filename != null)
            {
                try
                {
                    reader = new StreamReader(filename);
                }
                catch (System.IO.FileNotFoundException e)
                {
                    System.Console.WriteLine("Could not find file " + filename);
                    System.Console.WriteLine(e.Message);
                    reader = null;
                }
                catch (System.IO.DirectoryNotFoundException e)
                {
                    System.Console.WriteLine("Could not find directory for file " + filename);
                    System.Console.WriteLine(e.Message);
                    reader = null;
                }
                catch (System.IO.IOException e)
                {
                    System.Console.WriteLine("Could not open file " + filename + " because of an I/O failure");
                    System.Console.WriteLine(e.Message);
                    reader = null;
                }
            }
            else
            {
                // default: read from standard in
                reader = System.Console.In;
            }
            if (reader != null)
            {
                ConcurrencyExplorer ce = new ConcurrencyExplorer(reader, racedisplay);
                ce.start_controllers();
                ce.join_controllers();
            }
        }
    }
}
