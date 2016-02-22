/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿namespace Microsoft.Concurrency.TestTools.Alpaca
{
    partial class BottomTabs
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
            this.tabcontrol1 = new System.Windows.Forms.TabControl();
            this.tabpgArguments = new System.Windows.Forms.TabPage();
            this.invocationdetails = new System.Windows.Forms.TextBox();
            this.tabpgTaskresults = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabpgRunError = new System.Windows.Forms.TabPage();
            this.txtTestRunErrors = new System.Windows.Forms.TextBox();
            this.resultList1 = new Microsoft.Concurrency.TestTools.Alpaca.ChessResultList();
            this.stdOutputStreamBox = new Microsoft.Concurrency.TestTools.Alpaca.StreamBox();
            this.stdErrorStreamBox = new Microsoft.Concurrency.TestTools.Alpaca.StreamBox();
            this.tabcontrol1.SuspendLayout();
            this.tabpgArguments.SuspendLayout();
            this.tabpgTaskresults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabpgRunError.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabcontrol1
            // 
            this.tabcontrol1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabcontrol1.Controls.Add(this.tabpgArguments);
            this.tabcontrol1.Controls.Add(this.tabpgTaskresults);
            this.tabcontrol1.Controls.Add(this.tabpgRunError);
            this.tabcontrol1.Location = new System.Drawing.Point(0, 0);
            this.tabcontrol1.Name = "tabcontrol1";
            this.tabcontrol1.SelectedIndex = 0;
            this.tabcontrol1.Size = new System.Drawing.Size(784, 383);
            this.tabcontrol1.TabIndex = 1;
            // 
            // tabpgArguments
            // 
            this.tabpgArguments.Controls.Add(this.invocationdetails);
            this.tabpgArguments.Location = new System.Drawing.Point(4, 22);
            this.tabpgArguments.Name = "tabpgArguments";
            this.tabpgArguments.Padding = new System.Windows.Forms.Padding(3);
            this.tabpgArguments.Size = new System.Drawing.Size(776, 357);
            this.tabpgArguments.TabIndex = 0;
            this.tabpgArguments.Text = "Arguments";
            this.tabpgArguments.UseVisualStyleBackColor = true;
            // 
            // invocationdetails
            // 
            this.invocationdetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.invocationdetails.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.invocationdetails.Location = new System.Drawing.Point(3, 6);
            this.invocationdetails.Multiline = true;
            this.invocationdetails.Name = "invocationdetails";
            this.invocationdetails.ReadOnly = true;
            this.invocationdetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.invocationdetails.Size = new System.Drawing.Size(767, 345);
            this.invocationdetails.TabIndex = 1;
            // 
            // tabpgTaskresults
            // 
            this.tabpgTaskresults.Controls.Add(this.splitContainer1);
            this.tabpgTaskresults.Location = new System.Drawing.Point(4, 22);
            this.tabpgTaskresults.Name = "tabpgTaskresults";
            this.tabpgTaskresults.Padding = new System.Windows.Forms.Padding(3);
            this.tabpgTaskresults.Size = new System.Drawing.Size(776, 357);
            this.tabpgTaskresults.TabIndex = 3;
            this.tabpgTaskresults.Text = "Task Results";
            this.tabpgTaskresults.UseVisualStyleBackColor = true;
            this.tabpgTaskresults.Enter += new System.EventHandler(this.taskresults_Enter);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.resultList1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(770, 351);
            this.splitContainer1.SplitterDistance = 298;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.stdOutputStreamBox);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.stdErrorStreamBox);
            this.splitContainer2.Size = new System.Drawing.Size(468, 351);
            this.splitContainer2.SplitterDistance = 156;
            this.splitContainer2.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Interval = 7000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tabpgRunError
            // 
            this.tabpgRunError.Controls.Add(this.txtTestRunErrors);
            this.tabpgRunError.Location = new System.Drawing.Point(4, 22);
            this.tabpgRunError.Name = "tabpgRunError";
            this.tabpgRunError.Padding = new System.Windows.Forms.Padding(3);
            this.tabpgRunError.Size = new System.Drawing.Size(776, 357);
            this.tabpgRunError.TabIndex = 4;
            this.tabpgRunError.Text = "Error";
            this.tabpgRunError.UseVisualStyleBackColor = true;
            // 
            // txtTestRunErrors
            // 
            this.txtTestRunErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTestRunErrors.Location = new System.Drawing.Point(3, 3);
            this.txtTestRunErrors.Multiline = true;
            this.txtTestRunErrors.Name = "txtTestRunErrors";
            this.txtTestRunErrors.ReadOnly = true;
            this.txtTestRunErrors.Size = new System.Drawing.Size(770, 351);
            this.txtTestRunErrors.TabIndex = 0;
            // 
            // resultList1
            // 
            this.resultList1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.resultList1.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.resultList1.Location = new System.Drawing.Point(4, 10);
            this.resultList1.Name = "resultList1";
            this.resultList1.Size = new System.Drawing.Size(291, 337);
            this.resultList1.TabIndex = 0;
            // 
            // stdOutputStreamBox
            // 
            this.stdOutputStreamBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.stdOutputStreamBox.FilePath = null;
            this.stdOutputStreamBox.Location = new System.Drawing.Point(4, 4);
            this.stdOutputStreamBox.Name = "stdOutputStreamBox";
            this.stdOutputStreamBox.Size = new System.Drawing.Size(461, 149);
            this.stdOutputStreamBox.TabIndex = 0;
            this.stdOutputStreamBox.Title = "Standard Out";
            // 
            // stdErrorStreamBox
            // 
            this.stdErrorStreamBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.stdErrorStreamBox.FilePath = null;
            this.stdErrorStreamBox.Location = new System.Drawing.Point(4, 3);
            this.stdErrorStreamBox.Name = "stdErrorStreamBox";
            this.stdErrorStreamBox.Size = new System.Drawing.Size(461, 188);
            this.stdErrorStreamBox.TabIndex = 0;
            this.stdErrorStreamBox.Title = "Standard Error";
            // 
            // BottomTabs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabcontrol1);
            this.Name = "BottomTabs";
            this.Size = new System.Drawing.Size(784, 383);
            this.tabcontrol1.ResumeLayout(false);
            this.tabpgArguments.ResumeLayout(false);
            this.tabpgArguments.PerformLayout();
            this.tabpgTaskresults.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabpgRunError.ResumeLayout(false);
            this.tabpgRunError.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabcontrol1;
        private System.Windows.Forms.TabPage tabpgArguments;
        private System.Windows.Forms.TabPage tabpgTaskresults;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private Alpaca.StreamBox stdOutputStreamBox;
        private Alpaca.StreamBox stdErrorStreamBox;
        private Alpaca.ChessResultList resultList1;
        private System.Windows.Forms.TextBox invocationdetails;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabPage tabpgRunError;
        private System.Windows.Forms.TextBox txtTestRunErrors;

    }
}
