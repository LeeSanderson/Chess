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
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Chess;
using System.Diagnostics;
using Microsoft.Concurrency.TestTools.Execution.AppTasks;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    internal class Selection
    {

        internal class State
        {
            internal Object sender;

            internal EntityBase SelectedEntity;
            internal EntityBase[] SelectedEntities = new EntityBase[0];

            internal BuildEntity build;

            internal TestEntity test;
            internal TestMethodEntity testMethod;
            //internal TestGroupingEntity group;  // Used to help stop recursion up so the build option isn't set on a run or results node
            internal TaskRunEntity run;
            internal TestResultEntity testResult;
            internal ChessResultEntity chessResult;
        }

        private Model model;
        internal State current = new State();

        internal Selection(Model model)
        {
            this.model = model;
            model.EntityChanged += new ModelEntityEventHandler<EntityChangeEventArgs>(model_EntityChanged);
        }

        void model_EntityChanged(EntityBase entity, EntityChangeEventArgs e)
        {
            if (e.XChange == XObjectChange.Remove && current.SelectedEntity != null)
            {
                // If the entity being removed is under the selected entity, de-select
                if (current.SelectedEntity.Contains(entity))
                    ClearSelection(this);
            }

            // if multiple entities are currently selected, then deselect anytime we add/remove entities
            if ((e.EntityChange == EntityChange.Remove || e.EntityChange == EntityChange.Add)
                && current.SelectedEntities.Length > 1    // 
                )
            {
                ClearSelection(this);
            }
        }

        internal void ClearSelection(Object sender)
        {
            Update(sender, (EntityBase)null);
        }

        internal void Update(Object sender, EntityBase entity)
        {
            //if (entity == null)
            //    System.Diagnostics.Debug.WriteLine("Selection.Update: entity=null");
            //else
            //{
            //    RunEntity run = entity as RunEntity;
            //    System.Diagnostics.Debug.WriteLine("Selection.Update: entity={0}, task={1}", entity.GetType().Name, run == null ? "" : run.TaskState.TaskIndex.ToString());
            //}

            State previous = current;
            current = new State();
            current.sender = sender;
            current.SelectedEntity = entity;
            current.SelectedEntities = new[] { entity };

            FindContext(current.SelectedEntity);

            model.SelectionUpdateNotify(previous, current);
        }

        internal void Update(Object sender, IEnumerable<EntityBase> selectedEntities)
        {
            State previous = current;
            current = new State();
            current.sender = sender;
            current.SelectedEntity = null;
            current.SelectedEntities = selectedEntities.ToArray();
            model.SelectionUpdateNotify(previous, current);
        }

        /// <summary>
        /// Populates context data up the hierarchy based on properties available at
        /// each level
        /// </summary>
        /// <param name="entity"></param>
        private void FindContext(EntityBase entity)
        {
            if (entity == null)
                return;

            if (current.test == null && entity is TestEntity)
                current.test = (TestEntity)entity;

            if (current.testMethod == null && entity is TestMethodEntity)
                current.testMethod = (TestMethodEntity)entity;

            if (current.build == null && entity is IDefinesBuild)
                current.build = ((IDefinesBuild)entity).BuildEntity;

            //if (current.group == null && entity is TestGroupingEntity)
            //    current.group = (TestGroupingEntity)entity;

            if (current.run == null && entity is TaskRunEntity)
            {
                current.run = (TaskRunEntity)entity;

                // If we started at a run, then we should auto-select the test result so it'll also show in the GUI
                if (current.testResult == null && current.run is TestRunEntity)
                    current.testResult = ((TestRunEntity)current.run).Result;
            }

            if (current.testResult == null && entity is TestResultEntity)
                current.testResult = (TestResultEntity)entity;
            if (current.chessResult == null && entity is ChessResultEntity)
                current.chessResult = (ChessResultEntity)entity;

            FindContext(entity.Parent);
        }

        private AppTask _selectTaskOnSetup;

        /// <summary>
        /// Notifies the selection that we should select the run for this task
        /// when it gets setup.
        /// Only the latest task set via here will get auto-selected. If the user
        /// changes the selection before the task has been given time to be setup
        /// then the user selection takes precedence over this.
        /// </summary>
        /// <param name="task">
        /// The task to select.
        /// The type of the task must implement ITaskDefinesARun so we can select its run when it's setup.
        /// If it doesn't, than this method does nothing.
        /// </param>
        internal void SelectRunOnTaskSetup(AppTask task)
        {
            // We can only select tasks that define a run entity.
            if (!(task is ITaskDefinesARun))
                return;

            // See if it's already setup, if it is then select it now.
            if (task.Status >= AppTaskStatus.Setup)
            {
                Update(task, ((ITaskDefinesARun)task).Run);
                return;
            }

            // If there's a task waiting already, then replace it; only the latest gets selected
            if (_selectTaskOnSetup != null)
                _selectTaskOnSetup.TaskSetup -= new AppTaskEventHandler(selectTaskOnSetup_TaskSetup);

            _selectTaskOnSetup = task;
            if (task == null)
                return;

            _selectTaskOnSetup.TaskSetup += new AppTaskEventHandler(selectTaskOnSetup_TaskSetup);
        }

        void selectTaskOnSetup_TaskSetup(AppTask task, EventArgs e)
        {
            Debug.Assert(task is ITaskDefinesARun, "Shouldn't have been able to register this handler unless the task implemented ITaskDefinesARun.");

            // remove the handler
            task.TaskSetup -= new AppTaskEventHandler(selectTaskOnSetup_TaskSetup);

            // Ok then, select the run
            if (task == _selectTaskOnSetup)
                Update(task, ((ITaskDefinesARun)task).Run);
        }


    }
}

