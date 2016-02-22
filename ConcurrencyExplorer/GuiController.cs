/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Microsoft.ConcurrencyExplorer
{
    internal class GuiController : IController
    {
        internal GuiController(bool multipleExecutions, bool racedisplay, IModel model)
        {
            this.multipleExecutions = multipleExecutions;
            this.racedisplay = racedisplay;
            this.model = model;
            controllerthread = new Thread(new ThreadStart(run));
            controllerthread.Name = "guicontroller";
        }

        private bool multipleExecutions;
        private bool racedisplay;
        private IModel model;
        private Thread controllerthread;
        private List<IView> views = new List<IView>();
        private Queue<Command> commands = new Queue<Command>();

        // controller interface

        public void Start()
        {
            controllerthread.Start();
        }

        public void Join()
        {
            controllerthread.Join();
        }

        internal bool isDone()
        {
            return is_done;
        }

        private volatile bool is_done;

        // command interface : called by views

        internal void ViewHasClosed(IView view)
        {
            //System.Console.WriteLine("view has closed: " + view.ToString());
            lock (this)
            {
                views.Remove(view);
                Monitor.Pulse(this);
            }
        }

        internal void SetSelection(int entry)
        {
            lock (model) {
                model.SetSelection(entry);
            }
            // System.Console.WriteLine("selection = " + entry);
        }

       internal void ToggleMark(int entry)
        {
            lock (model) {
                model.ToggleMark(entry);
            }
            // System.Console.WriteLine("toggle = " + entry);
        }


       internal  delegate void Command();

        internal void QueueCommand(Command command)
        {
            lock (this)
            {
                commands.Enqueue(command);
                Monitor.Pulse(this);
            }
        }

        internal void CreateFilteredView(IFilter eventfilter, Thunk initialAction, bool oldDisplayStyle)
        {
            StandardView view = new StandardView(this, model, eventfilter, multipleExecutions, oldDisplayStyle);
            lock (this)
            {
                views.Add(view);
            }
            view.SetInitialAction(initialAction);
            view.StartView();
        }

       internal void CreateCommandPanel()
        {
            IView view = new CommandPanel(this, model, multipleExecutions, racedisplay);
            lock (this)
            {
                views.Add(view);
            }
            view.StartView();
        }

       internal bool shutdown;

       internal void ShutDown()
       {
           lock (this)
           {
               foreach (IView view in views)
               {
                   view.CloseView();
               }
           }
           shutdown = true;
       }

       private void run()
       {
           // create the command panel
           CreateCommandPanel();
           // create a standard view
          // CreateFilteredView(new TrivialFilter());

           // wait for commands to execute, until all views are gone
           Command c;
           do
           {
               c = null;
               lock (this)
               {
                   while (views.Count > 0 && commands.Count == 0)
                       Monitor.Wait(this);
                   if (commands.Count > 0)
                       c = commands.Dequeue();
               }
               if (c != null)
               {
                   // execute command
                   c();
               }
           } while (c != null);

           is_done = true;
           //System.Console.WriteLine("controller done: " + this.ToString());

           if (shutdown)
           {
             //  System.Environment.Exit(1);
           }
       }
    }
}
