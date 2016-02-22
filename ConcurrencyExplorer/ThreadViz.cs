/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.IO;

namespace Microsoft.ConcurrencyExplorer
{
    internal class ThreadViz : Form, IThreadView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private StateBrowser sb;
        private List<TaskVarOp> tvo_list;

        internal ThreadViz(Formatter formatter, GuiController controller)
        {
            tvo_list = new List<TaskVarOp>();
            sb = new StateBrowser(
                    delegate(TaskVarOp tvo) {},
                    this,
                    formatter,
                    controller
                    );

            this.Controls.Add(sb);

            // their arrangement
            sb.Dock = DockStyle.Fill;

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            Show();
        }

        public void Initialize()
        {
            int num_threads = sb.SetFocus(0, false);
            sb.SetReplay();
            if (num_threads > 0)
            {
                tvo_list = new List<TaskVarOp>();
                for (int i = 0; i < num_threads; i++)
                {
                    tvo_list.Add(null);
                }
            }
        }

        public void NewColumn()
        {
            sb.NewColumn();
        }

        public bool NewColumnEntry(int tid, Entry entry)
        {
            return sb.NewColumnEntry(new TaskVarOp(tid, entry));
        }

        public void InvisibleEntry(int tid)
        {
            sb.InvisibleEntry(tid);
        }

        public void SetSelection(int entry)
        {
            sb.SetSelection(entry);
        }

        public void SetName(int i, string s)
        {
            //
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        // #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // Form1
            // 
            AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            ClientSize = new System.Drawing.Size(1000, 800);
            Text = "Concurrency Explorer";
            OnResize(EventArgs.Empty);
        }
        // #endregion
    }
}