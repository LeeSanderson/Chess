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
using Microsoft.Concurrency.TestTools.Alpaca.Views;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Chess;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    public partial class ChessResultList : UserControl
    {

        private Model _model;
        private TestRunEntity _testRun;
        private Dictionary<string, DataGridViewRow> rowmap = new Dictionary<string, DataGridViewRow>();
        private Dictionary<string, ChessResultEntity> resultmap = new Dictionary<string, ChessResultEntity>();

        public ChessResultList()
        {
            InitializeComponent();
        }

        internal void Init(Model model)
        {
            this._model = model;
            model.EntityChanged += new ModelEntityEventHandler<EntityChangeEventArgs>(model_EntityChanged);
            model.SelectionUpdated += new ModelEventHandler<Selection.State, Selection.State>(model_SelectionUpdateEvt);
        }

        void model_EntityChanged(EntityBase entity, EntityChangeEventArgs e)
        {
            bool doRefresh = false;

            var result = entity as TestResultEntity;
            if (result != null && e.EntityChange == EntityChange.Add)
            {
                var run = result.OwningTestRun;
                doRefresh = e.EntityChange == EntityChange.Add
                    && run == _testRun
                    && run.HasResult
                    ;
            }

            if (doRefresh)
                SetRun(_testRun);
        }

        void model_SelectionUpdateEvt(Selection.State previous, Selection.State current)
        {
            if (previous.SelectedEntity != null && previous.SelectedEntity is ChessResultEntity)
                rowmap[previous.chessResult.Label].DefaultCellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
            if (current.sender != this)
            {
                if (current.run != previous.run)
                    SetRun(current.run);
            }
            if (current.sender != this && current.SelectedEntity != null && current.SelectedEntity is ChessResultEntity)
                rowmap[current.chessResult.Label].DefaultCellStyle.BackColor = SystemColors.ControlLight;
        }

        private void SetRun(TaskRunEntity run)
        {
            // Clear everything first
            suspend_selection_notify = true;
            dataGridView1.Rows.Clear();
            rowmap.Clear();
            resultmap.Clear();

            TestRunEntity testRun = run as TestRunEntity;

            if (testRun != null && testRun.Result != null)
            {
                var results = testRun.Result.GetChessResults();
                bool clearSel = false;
                foreach (var result in results)
                {
                    string label = result.Label;
                    resultmap[label] = result;
                    //bool hasschedule = result.DataElement.Element(XNames.Schedule) != null;
                    //bool hastrace = run.Entity.TaskHandle.GetTraceFile() != null;
                    int newrow = dataGridView1.Rows.Add(result.Label, result.Description);
                    DataGridViewRow row = dataGridView1.Rows[newrow];
                    rowmap[label] = row;
                    row.Cells[1].Style.ForeColor = result.ResultType.GetDisplayColor();
                }
                if (clearSel)
                    dataGridView1.ClearSelection();
            }

            _testRun = testRun;
            suspend_selection_notify = false;
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left && e.ColumnIndex == 2) || e.Button == MouseButtons.Right)
            {
                int index = e.RowIndex;
                DataGridViewRow row = dataGridView1.Rows[index];
                row.Selected = true;
                string label = row.Cells[0].Value.ToString();
                ChessResultEntity result = resultmap[label];
                PopupMenu.ShowContextMenu(contextMenuStrip1, result);
            }
        }

        private void dataGridView1_Leave(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }

        private bool suspend_selection_notify = false;

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (suspend_selection_notify)
                return;
            DataGridViewSelectedRowCollection selection = dataGridView1.SelectedRows;
            if (selection.Count == 1)
            {
                DataGridViewRow row = selection[0];
                _model.selection.Update(this, resultmap[row.Cells[0].Value.ToString()]);
            }
            else if (selection.Count > 1)
                _model.selection.ClearSelection(this);
        }

    }
}
