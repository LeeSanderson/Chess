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
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using System.Diagnostics;
using Microsoft.Concurrency.TestTools.Alpaca.AActions;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    class CommandController
    {

        private Model model;

        internal CommandController(Model model)
        {
            this.model = model;
        }

        // we set this if program is closing, to avoid getting timer callbacks right after closing
        internal bool closing;

        // maintain three lists of pending commands
        private List<Command> commands = new List<Command>();
        private List<Command> followups = new List<Command>();
        private List<Command> quiesces = new List<Command>();

        internal void Init(IEnumerable<Command> batchcommands)
        {
            foreach (Command c in batchcommands)
                AddNewCommand(c);

            RefreshTaskStates();

            System.Windows.Forms.Application.Idle += new EventHandler(Application_Idle);
        }

        private bool _isProcessingTasks;
        internal void RefreshTaskStates()
        {
            // It's possible this can be called via the Application_Idle or TimerTick events
            // The side effect is that if we pop up a blocking dialog box, from one event handler
            // the other handler can still run and cause more tasks to be processed.
            if (_isProcessingTasks)
                return;

            try
            {
                _isProcessingTasks = true;
                model.tasksController.ProcessTasks();
            }
            finally
            {
                _isProcessingTasks = false;
            }
        }



        #region Add*Command()

        /// <summary>add a command, to be executed ASAP</summary>
        /// <param name="command"></param>
        internal void AddNewCommand(Command command)
        {
            if (!command.CheckRedundancy(commands))
                commands.Add(command);
        }

        /// <summary> add a command, to be executed on the next timer tick</summary>
        internal void AddFollowupCommand(Command command)
        {
            if (!command.CheckRedundancy(followups))
                followups.Add(command);
        }

        /// <summary> add a command, to be executed on quiesce</summary>
        internal void AddQuiesceCommand(Command command)
        {
            if (!command.CheckRedundancy(quiesces))
                quiesces.Add(command);
        }

        #endregion

        #region Execute commands runtime

        void Application_Idle(object sender, EventArgs e)
        {
            // execute commands
            ExecuteCommands();

            // autosave
            model.session.AutoSave();

            // close form 
            if (closing)
            {
                if (model.mainForm != null)
                    model.mainForm.Close();
            }
        }

        private void ExecuteList(string listName, List<Command> list)
        {
            Dictionary<string, bool> errorset = new Dictionary<string, bool>(); // set of errors

            foreach (Command c in list)
            {
                //Debug.WriteLine("Controller executing command in {0}: {1}", listName, c.DisplayName);
                bool done = c.Execute(model);
                if (!done)
                {
                    if (c is QuiesceCommand)
                    {
                        AddQuiesceCommand(c);
                    }
                    else
                    {
                        AddFollowupCommand(c);
                    }
                }
                if (!c.Successful() && c.IsInteractive() && !errorset.ContainsKey(c.GetError()))
                    errorset.Add(c.GetError(), true);
            }

            foreach (string s in errorset.Keys)
                MessageBox.Show(s);
        }

        private void ExecuteCommands()
        {
            if (commands.Count == 0)
                return;

            model.OnBeginUpdate();

            try
            {
                while (commands.Count > 0)
                {
                    List<Command> list = commands;
                    commands = new List<Command>();
                    ExecuteList("commands", list);
                }

                RefreshTaskStates(); // more responsive if we do this right away.

                CheckForQuiescence();
            }
            finally
            {
                model.DoEndUpdate();
            }
        }

        bool _isTimerTickExecuting;
        internal void TimerTick()
        {
            if (model.controller.closing || _isTimerTickExecuting)
                return;

            _isTimerTickExecuting = true;
            try
            {
                model.OnBeginUpdate();
                RefreshTaskStates();
                CheckForQuiescence();
                ExecuteFollowups();
            }
            finally
            {
                model.DoEndUpdate();
                _isTimerTickExecuting = false;
            }
        }

        private void ExecuteFollowups()
        {
            if (followups.Count == 0)
                return;

            List<Command> list = followups;
            followups = new List<Command>();
            ExecuteList("followup commands", list);

            RefreshTaskStates(); // more responsive if we do this right away.
        }

        /// <summary>
        /// </summary>
        private void CheckForQuiescence()
        {
            if (quiesces.Count() > 0 && commands.Count() == 0 && followups.Count() == 0 && model.runs.AllRunsComplete())
            {
                List<Command> list = quiesces;
                quiesces = new List<Command>();
                ExecuteList("quiesce commands", list);
            }
        }

        #endregion

    }
}
