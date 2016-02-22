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
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.AppTasks;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    class Runs
    {
        private Model model;
        private Dictionary<int, TaskRunEntity> _runsByID = new Dictionary<int, TaskRunEntity>();

        internal Runs(Model model)
        {
            this.model = model;

            model.SessionInitialized += new ModelEventHandler(model_NewSessionEvt);
            model.EntityChanged += new ModelEntityEventHandler<EntityChangeEventArgs>(model_EntityChanged);
        }

        /// <summary>
        /// If the run doesn't exist, then null is returned.
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        internal TaskRunEntity this[int taskID]
        {
            get
            {
                TaskRunEntity run;
                _runsByID.TryGetValue(taskID, out run);
                return run;
            }
        }

        public IEnumerable<TaskRunEntity> GetRuns()
        {
            return _runsByID.Values;
        }

        private void model_NewSessionEvt()
        {
            RunsAdded(model.session.Entity.DescendantRuns());
        }

        void model_EntityChanged(EntityBase entity, EntityChangeEventArgs e)
        {
            var runs = entity.DescendantRunsAndSelf();
            if (e.EntityChange == EntityChange.Add)
                RunsAdded(runs);
            else if (e.EntityChange == EntityChange.Remove)
                RunsRemoved(runs);
        }

        private void RunsAdded(IEnumerable<TaskRunEntity> runsToAdd)
        {
            foreach (var r in runsToAdd)
                _runsByID.Add(r.TaskID, r);
        }

        private void RunsRemoved(IEnumerable<TaskRunEntity> runsToRemove)
        {
            foreach (var r in runsToRemove)
                _runsByID.Remove(r.TaskID);
        }

        internal bool AllRunsComplete()
        {
            return _runsByID.Values.All(r => r.Task.IsComplete);
        }

        internal void EnsureTaskHandleCreated(TaskRunEntity run)
        {
            if (run.Task != null)
                return;

            // Re-create the task from the run entity
            AppTask task = AppTaskBuilder.BuildTaskFromRun(run);
            run.Task = task;
        }

    }
}
