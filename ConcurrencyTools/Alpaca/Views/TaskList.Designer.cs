/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿namespace Microsoft.Concurrency.TestTools.Alpaca
{
    partial class TaskList
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TaskIDCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TaskNameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StatusCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TestResultCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TaskIDCol,
            this.TaskNameCol,
            this.StatusCol,
            this.TestResultCol});
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 18;
            this.dataGridView1.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.Size = new System.Drawing.Size(428, 240);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseClick);
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            this.dataGridView1.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dataGridView1_UserDeletingRow);
            this.dataGridView1.Enter += new System.EventHandler(this.dataGridView1_Enter);
            this.dataGridView1.Leave += new System.EventHandler(this.dataGridView1_Leave);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // TaskIDCol
            // 
            this.TaskIDCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.TaskIDCol.DataPropertyName = "TaskID";
            this.TaskIDCol.FillWeight = 20F;
            this.TaskIDCol.HeaderText = "#";
            this.TaskIDCol.MinimumWidth = 20;
            this.TaskIDCol.Name = "TaskIDCol";
            this.TaskIDCol.ReadOnly = true;
            this.TaskIDCol.Width = 39;
            // 
            // TaskNameCol
            // 
            this.TaskNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.TaskNameCol.DataPropertyName = "DisplayText";
            this.TaskNameCol.FillWeight = 117.5258F;
            this.TaskNameCol.HeaderText = "Task";
            this.TaskNameCol.MinimumWidth = 80;
            this.TaskNameCol.Name = "TaskNameCol";
            this.TaskNameCol.ReadOnly = true;
            // 
            // StatusCol
            // 
            this.StatusCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.StatusCol.DataPropertyName = "TaskStatus";
            this.StatusCol.HeaderText = "Status";
            this.StatusCol.MinimumWidth = 60;
            this.StatusCol.Name = "StatusCol";
            this.StatusCol.ReadOnly = true;
            this.StatusCol.Width = 60;
            // 
            // TestResultCol
            // 
            this.TestResultCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.TestResultCol.DataPropertyName = "TestResult";
            this.TestResultCol.HeaderText = "Result";
            this.TestResultCol.MinimumWidth = 60;
            this.TestResultCol.Name = "TestResultCol";
            this.TestResultCol.ReadOnly = true;
            this.TestResultCol.Width = 60;
            // 
            // TaskList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.Controls.Add(this.dataGridView1);
            this.Name = "TaskList";
            this.Size = new System.Drawing.Size(428, 240);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.DataGridViewTextBoxColumn TaskIDCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn TaskNameCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn StatusCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn TestResultCol;
    }
}
