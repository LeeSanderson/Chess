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

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    internal abstract class Command
    {

        internal Command(bool interactive)
        {
            this.interactive = interactive;
        }

        protected bool interactive;
        internal bool IsInteractive() { return interactive; }

        public virtual string DisplayName { get { return this.GetType().Name; } }

        // returns true if finished, false if followup required
        /// <summary>
        /// Executes the command and blocks until it is finished.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal bool Execute(Model model)
        {
            try
            {
                return PerformExecute(model);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex, this.GetType().Name + ".PerformExecute threw exception");
                SetError("Error running command:\n" + ex.Message);
                return true;
            }
        }

        /// <summary>
        /// When implemented in a derived class, performs logic for executing this command.
        /// A generic layer of error handling has been implemented around this method.
        /// </summary>
        /// <returns></returns>
        protected abstract bool PerformExecute(Model model);

        // used only for interactive mode
        /// <summary>
        /// Checks a command queue to see if this instance is redundant to a command already in the queue.
        /// Some commands may choose to override this method to merge this command with an existing command
        /// and then return true so this instance doesn't get added to the queue.
        /// </summary>
        /// <param name="commandqueue"></param>
        /// <returns>true if this command is redundant; otherwise, false.</returns>
        internal virtual bool CheckRedundancy(List<Command> commandqueue)
        {
            return false;
        }

        // error status of a command
        private string error;
        protected void SetError(string message) { error = message; }
        internal string GetError() { return error; }

        /// <summary>
        /// Returns a value indicating whether the command executed successfully.
        /// This is determined by whether SetError has been called.
        /// </summary>
        /// <returns></returns>
        internal bool Successful() { return String.IsNullOrEmpty(error); }
    }



}
