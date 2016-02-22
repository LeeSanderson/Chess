/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Alpaca.Views;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    internal partial class TaskList : UserControl
    {

        private Model model;

        private BindingSource _tasksBindingSource;
        private PropertyBindingList<BoundRunItem> _tasks;
        private Dictionary<int, BoundRunItem> _tasksByID = new Dictionary<int, BoundRunItem>();

        private List<DataGridViewRow> _selectedRows = new List<DataGridViewRow>();

        public TaskList()
        {
            InitializeComponent();

            _tasks = new PropertyBindingList<BoundRunItem>();
            _tasksBindingSource = new BindingSource();
            _tasksBindingSource.DataSource = _tasks;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = _tasksBindingSource;
        }

        public Color InactiveSelectionBackColor { get { return SystemColors.Control; } }

        internal void Init(Model model)
        {
            this.model = model;
            model.SessionInitialized += new ModelEventHandler(model_NewSessionEvt);
            model.EntityChanged += new ModelEntityEventHandler<EntityChangeEventArgs>(model_EntityChanged);
            model.SelectionUpdated += new ModelEventHandler<Selection.State, Selection.State>(model_SelectionUpdateEvt);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            foreach (DataGridViewRow row in dataGridView1.Rows)
                row.DefaultCellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
        }

        private void model_NewSessionEvt()
        {
            AddRuns(model.session.Entity.DescendantRuns());

            suppress_selection_notify = true;
            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
            dataGridView1.ClearSelection();
            suppress_selection_notify = false;
        }

        void model_EntityChanged(EntityBase entity, EntityChangeEventArgs e)
        {
            if (e.EntityChange == EntityChange.Add)
            {
                if (entity is TestResultEntity)
                {
                    int taskID = ((TestResultEntity)entity).OwningTestRun.TaskID;
                    BoundRunItem runItem;
                    if (_tasksByID.TryGetValue(taskID, out runItem))
                        runItem.Update();
                }

                AddRuns(entity.DescendantRunsAndSelf());
            }
            else if (e.EntityChange == EntityChange.Remove)
                RemoveRuns(entity.DescendantRunsAndSelf());
        }

        void model_SelectionUpdateEvt(Selection.State previous, Selection.State current)
        {
            if (suppress_selection_notify)
                return;

            suppress_selection_notify = true;
            try
            {
                // Reset the previously selected row's color
                foreach (var row in _selectedRows)
                {
                    row.DefaultCellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
                    row.Selected = false;
                }
                _selectedRows.Clear();

                // Set the selected row's color
                if (current.sender != this && current.run != null)
                {
                    int taskID = current.run.TaskID;
                    BoundRunItem runItem;
                    if (_tasksByID.TryGetValue(taskID, out runItem))
                    {
                        var row = dataGridView1.Rows[_tasksBindingSource.Find("TaskID", taskID)];
                        row.DefaultCellStyle.BackColor = InactiveSelectionBackColor;
                        _selectedRows.Add(row);

                        // Unfortunately, there's no way to auto-scroll an arbitrary row into view.
                    }
                }
            }
            finally
            {
                suppress_selection_notify = false;
            }
        }

        private void AddRuns(IEnumerable<TaskRunEntity> runs)
        {
            suppress_selection_notify = true;
            foreach (var run in runs)
            {
                var item = new BoundRunItem(run);
                _tasksByID.Add(item.TaskID, item);
                _tasks.Add(item);
            }
            dataGridView1.ClearSelection();
            suppress_selection_notify = false;
        }

        private void RemoveRuns(IEnumerable<TaskRunEntity> runs)
        {
            suppress_selection_notify = true;
            foreach (var run in runs)
            {
                var taskID = run.TaskID;
                if (_tasksByID.ContainsKey(taskID))
                {
                    var item = _tasksByID[taskID];
                    _tasksByID.Remove(taskID);
                    _tasks.Remove(item);
                }
            }
            dataGridView1.ClearSelection();
            suppress_selection_notify = false;
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            var selectedEntities = from DataGridViewRow row in dataGridView1.SelectedRows
                                   let runItem = (BoundRunItem)row.DataBoundItem
                                   select runItem.Run;
            model.controller.AddNewCommand(new DeleteCommand(selectedEntities, true, true, false));
            e.Cancel = true; // we ALWAYS cancel here so the datagridview does not remove it twice
        }

        private bool suppress_selection_notify = false;

        private void dataGridView1_Enter(object sender, EventArgs e)
        {
            // Highlight the selected rows
            suppress_selection_notify = true;
            foreach (var row in _selectedRows)
                row.Selected = true;
            suppress_selection_notify = false;
        }

        private void dataGridView1_Leave(object sender, EventArgs e)
        {
            // De-emphisize the selected rows
            suppress_selection_notify = true;
            foreach (var row in _selectedRows)
                row.Selected = false;
            suppress_selection_notify = false;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (suppress_selection_notify)
                return;

            suppress_selection_notify = true;

            try
            {
                // Reset the back color for the previously selected rows
                foreach (var row in _selectedRows)
                    row.DefaultCellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
                _selectedRows.Clear();

                DataGridViewSelectedRowCollection selection = dataGridView1.SelectedRows;
                if (selection.Count == 1)
                {
                    DataGridViewRow row = selection[0];
                    _selectedRows.Add(row);
                    int taskID = (int)row.Cells[0].Value;
                    TaskRunEntity run = model.runs[taskID];
                    model.selection.Update(this, run);
                }
                else if (selection.Count > 1)
                {
                    List<TaskRunEntity> selectedEntities = new List<TaskRunEntity>();
                    foreach (DataGridViewRow row in selection)
                    {
                        _selectedRows.Add(row);
                        int taskID = (int)row.Cells[0].Value;
                        TaskRunEntity run = model.runs[taskID];
                        selectedEntities.Add(run);
                    }
                    model.selection.Update(this, selectedEntities);
                }

                foreach (var row in _selectedRows)
                    row.DefaultCellStyle.BackColor = InactiveSelectionBackColor;
            }
            finally
            {
                suppress_selection_notify = false;
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // If clicking the header row, than we won't have a data row.
            if (e.RowIndex == -1)
                return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            int taskID = (int)row.Cells[0].Value;
            TaskRunEntity run = model.runs[taskID];
            //DataGridViewCell cell = row.Cells[e.ColumnIndex];

            if (e.Button == MouseButtons.Left)
            {
                // If the entity is already highlighted 
                // but not in the model.selection context, then force
                // it to be selected
                if (row.Selected && model.selection.current.SelectedEntity != run)
                {
                    suppress_selection_notify = true;
                    try
                    {
                        model.selection.Update(this, run);
                    }
                    finally
                    {
                        suppress_selection_notify = false;
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                PopupMenu.ShowContextMenu(contextMenuStrip1, run);
            }
        }

    }
}
