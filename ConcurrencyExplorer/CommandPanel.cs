/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Linq;

namespace Microsoft.ConcurrencyExplorer
{
    internal partial class CommandPanel : Form, IView
    {
        private GuiController controller;
        private IModel model;
        private Thread guithread;
        private bool multipleExecutions = false;

        internal CommandPanel(GuiController controller, IModel model, bool multipleExecutions, bool racedisplay)
        {
            this.controller = controller;
            this.model = model;

            // create thread
            guithread = new Thread(new ThreadStart(RunGuiThread));
            guithread.IsBackground = true;
            guithread.SetApartmentState(ApartmentState.STA);
            guithread.Name = "gui-cmdpanel";

            InitializeComponent();

            this.FormClosing += (FormClosingEventHandler)HandleFormClosing;

            if (multipleExecutions)
            {
                this.multipleExecutions = true;
                olddisplaystyle.Checked = true;
                olddisplaystyle.Enabled = false;
            }
            this.racedisplay = racedisplay;
            
        }
        private bool racedisplay;

        public void StartView()
        {
            guithread.Start();
        }

        public void CloseView()
        {
            closing = true;
            BeginInvoke(new Thunk(delegate() { Close(); }));
        }

        // this is the entry point for the gui thread
        private void RunGuiThread()
        {
            // need timer to finish initialization
            timer1.Start();

            // run form
            System.Windows.Forms.Application.Run(this);

            //System.Console.WriteLine("cmdpanel thread done");

            // unregister observers
            lock (model)
            {
                model.NewEntry -= new NewEntryHandler(HandleNewEntry);
                model.NewExecution -= new NewExecutionHandler(HandleNewExecution);
                model.SelectionUpdate -= new SelectionUpdateHandler(HandleSelectionUpdate);
                model.ThreadnameUpdate -= new ThreadnameUpdateHandler(HandleThreadnameUpdate);
            }

            // notify controller
            controller.ViewHasClosed(this);
        }

        volatile bool closing;

        bool registered = false;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!registered)
            {
                lock (model)
                {
                    // register as observer for model events
                    model.NewEntry += new NewEntryHandler(HandleNewEntry);
                    model.NewExecution += new NewExecutionHandler(HandleNewExecution);
                    model.SelectionUpdate += new SelectionUpdateHandler(HandleSelectionUpdate);
                    model.ThreadnameUpdate += new ThreadnameUpdateHandler(HandleThreadnameUpdate);
                }
                registered = true;
            }

            bool completedjustnow = false;

            lock (model)
            {
                // update
                UpdateText();
                UpdateThreads();
                completedjustnow = model.IsComplete();
            }

            if (completedjustnow)
            {
                HandleCompletion();
                timer1.Stop();
            }
        }

        public void HandleCompletion()
        {
            if (multipleExecutions)
                return; // no default view

            if (racedisplay)
            {
               // System.Console.WriteLine("opening default view: race display");
                OpenRaceView(this);
            }
            else
            {
                //System.Console.WriteLine("opening default view: all events");
                IFilter filter = makeThreadFilter();
                controller.QueueCommand(new GuiController.Command(delegate()
                {
                    controller.CreateFilteredView(filter, null, olddisplaystyle.Checked);
                }));
            }
        }

        void HandleSelectionUpdate(int entry)
        {
            if (!closing)
            {
                UpdateText();
                UpdateThreads();
            }
        }
        void HandleNewEntry(Entry entry)
        {
            if (!closing)
            {
                UpdateText();
            }
        }
        void HandleNewExecution(int exec)
        {
            if (!closing)
            {
                UpdateText();
                UpdateThreads();
            }
        }
        void HandleThreadnameUpdate(int exec, int tid, string name)
        {
            if (!closing)
            {
                UpdateThreads();
            }
        }

        public void HandleFormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closing = true;
        }
        
        volatile int cursel = -1; // currently selected entry, or -1 if none
        volatile int curex = -1;  // currently selected execution, or -1 if none

        private void UpdateText()
        {
            string str_ex = "Execution: ";
            string str_tr = "Thread: ";
            string str_en = "Entry: ";
            string str_vc = "VC: ";
            string str_sid = "sid: ";
            string str_enables = "enables: ";
            string str_disables = "disables: ";
            bool has_race;

           // get the data we need (note that we are holding model lock already)
            cursel = model.GetSelection();
            if (cursel != -1)
            {
                Entry e = model.GetEntry(cursel);
                curex = e.record.exec;
                str_ex += e.record.exec + "/" + model.GetNumberExecutions();
                str_tr += e.record.tid + "/" + model.GetThreads(e.record.exec).Count();
                //+ "(" + model.getThreadName(e.record.exec, e.record.tid) + ")";
                str_en += (e.seqno - model.GetFirstEntry(e.record.exec) + 1)
                          + "/" + model.GetNumberEntries(e.record.exec);
                if (e.status == "c" && e.record.hbStamp != null)
                    str_vc = "VC: " + e.record.hbStamp.format(model.GetThreads(e.record.exec).Count());
                has_race = model.HasRace(e.record.exec);
                if (e.status == "c" && e.record.sid >= 0)
                    str_sid += (e.record.sid);
                int[] enables, disables;
                EventDB.GetEnableChange(e, out enables, out disables);
                if (enables != null)
                    foreach (int t in enables)
                        str_enables += t + " ";
                if (disables != null)
                    foreach (int t in disables)
                        str_disables += t + " ";
             }
            else
            {
                curex = -1;
                has_race = false;
            }

            BeginInvoke(new Thunk(delegate()
            {
                label1.Text = str_ex;
                label2.Text = str_tr;
                label3.Text = str_en;
                label4.Text = str_vc;
                label5.Text = str_sid;
                label6.Text = str_enables;
                label7.Text = str_disables;
                button2.Enabled = (cursel != -1);
                button3.Enabled = has_race;
            }));
        }

        private List<CheckBox> boxes = new List<CheckBox>();

        private void UpdateThreads()
        {

            List<string> threadnames = new List<string>();
            cursel = model.GetSelection();
            if (cursel != -1)
            {
                Entry e = model.GetEntry(cursel);
                foreach (int tid in model.GetThreads(e.record.exec))
                    threadnames.Add(model.GetThreadName(e.record.exec, tid));
            }

            BeginInvoke(new Thunk(delegate()
            {
                while (boxes.Count < threadnames.Count)
                {
                    CheckBox box = new CheckBox();
                    box.Checked = true;
                    box.Location = new Point(0, 0 + 17 * boxes.Count);
                    box.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
                    box.AutoSize = true;
                    panel1.Controls.Add(box);
                    boxes.Add(box);
                }
                for (int pos = 0; pos < threadnames.Count; pos++)
                {
                    CheckBox box = boxes[pos];
                    box.Text = threadnames[pos];
                    if (!box.Visible)
                    {
                        box.Visible = true;
                    };
                }
                for (int pos = threadnames.Count; pos < boxes.Count; pos++)
                {
                    boxes[pos].Visible = false;
                }
            }));
        }

        private IFilter makeThreadFilter()
        {
            List<bool> checks = new List<bool>();
            foreach (CheckBox box in boxes)
                if (box.Visible)
                    checks.Add(box.Checked);
            return new ThreadFilter(checks.ToArray());
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            IFilter filter = makeThreadFilter();
            controller.QueueCommand(new GuiController.Command(delegate() 
                                   {
                                      controller.CreateFilteredView(filter, null, olddisplaystyle.Checked); 
                                   }));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            controller.QueueCommand(new GuiController.Command(delegate () 
                                   {
                                      controller.ShutDown(); 
                                   }));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IFilter filter = makeThreadFilter();
            List<int> varlist = new List<int>();
            lock (model)
            {
                foreach (Entry entry in model.GetMarkedEntries())
                    varlist.AddRange(entry.record.vars.Keys);
            }
            if (varlist.Count == 0)
           {
            MessageBox.Show((Control)sender, "No marked entries found. Use <SPACE> key to mark entries.", "", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1, (MessageBoxOptions)0);
            }
            else
            {
                lock (model)
                {
                    filter = new IntersectionFilter(filter, new VarFilter(varlist.ToArray()));
                }
                controller.QueueCommand(new GuiController.Command(delegate()
                                       {
                                           controller.CreateFilteredView(filter, null, olddisplaystyle.Checked);
                                       }));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (cursel == -1)
                MessageBox.Show((Control)sender, "Please select and execution first.", "",
                            MessageBoxButtons.OK, MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1, (MessageBoxOptions)0);
            else
            {
                OpenRaceView(sender);
            }
        }

        private void OpenRaceView(object sender)
        {
            RaceFilter filter;
            IFilter tfilter = makeThreadFilter();
            lock (model)
            {
                filter = new RaceFilter(model, curex);
            }
            if (String.IsNullOrEmpty(filter.getError()))
                controller.QueueCommand(new GuiController.Command(delegate()
                {
                    controller.CreateFilteredView(
                      new IntersectionFilter(tfilter, filter),
                      delegate()
                      {
                          int firstsel = filter.getFirstSelection();
                          if (firstsel != -1)
                              lock (model)
                              {
                                  model.SetSelection(firstsel);
                              }
                          int secondsel = filter.getSecondSelection();
                          if (secondsel != -1)
                              lock (model)
                              {
                                  model.SetSelection(secondsel);
                              }
                      },
                      olddisplaystyle.Checked);
                }));
            else
            {
                    MessageBox.Show((Control)sender, filter.getError(), "",
                                MessageBoxButtons.OK, MessageBoxIcon.Information,
                                MessageBoxDefaultButton.Button1, (MessageBoxOptions)0);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            IFilter filter = makeThreadFilter();
            lock (model)
            {
                filter = new IntersectionFilter(new PreemptionFilter(), filter);
            }
            controller.QueueCommand(new GuiController.Command(delegate()
                {
                    controller.CreateFilteredView(filter, null, olddisplaystyle.Checked);
                }));

        }

        private void ExportTrace()
        {

            System.Console.WriteLine("s");
            System.Console.WriteLine("#");
            // process event DB
            lock (model)
            {
                int[] tids = model.GetThreads(1).ToArray();
                int[] nr = new int[tids.Length];
                List<StackFrame>[] stacks = new List<StackFrame>[tids.Length];
                for (int i = 0; i < tids.Length; i++)
                    stacks[i] = new List<StackFrame>();

                for (int ei = 0; ei < model.GetNumberEntries(1); ei++)
                {
                    Entry entry = model.GetEntry(model.GetFirstEntry(1) + ei);
                    for (int thread = 0; thread < tids.Length; thread++)
                        if (tids[thread] == entry.record.tid)
                        {

                            // filter events we don't want to display
                            if (entry.record.mop == "TASK_BEGIN")
                                continue;

                            if (entry.record.stack != null && entry.record.stack.Count > 0)
                            {

                                // compare stacks
                                List<StackFrame> ns = entry.record.stack;
                                List<StackFrame> os = stacks[thread];
                                int ni = ns.Count - 1;
                                int oi = os.Count - 1;
                                while (ni >= 0 && oi >= 0 && ns[ni].Equals(os[oi]))
                                {
                                    ni--; oi--;
                                }

                                // filter duplicates
                                if (ni == -1 && oi == -1)
                                    continue;

                                int ri = oi;  // return index
                                int ci = ni;  // call index
                                if (ri >= 0 && ci >= 0 && os[ri].proc == ns[ci].proc)
                                {
                                    ri--;
                                    ci--;
                                }
                                for (int i = 0; i <= ri; i++)
                                    InsertEntry(os, i, entry.record.tid, ++nr[thread], true);
                                for (int i = ci; i >= 0; i--)
                                    InsertEntry(ns, i, entry.record.tid, ++nr[thread], false);

                                // remember this entry
                                stacks[thread] = entry.record.stack;
                            }

                            // echo entry
                            DumpEntry(entry.record, entry.status, ++nr[thread]);
                        }
                }

            }
        }

        private void InsertEntry(List<StackFrame> stack, int index, int tid, int nr, bool isReturn)
        {
            if (index + 1 >= stack.Count)
                return;

            List<StackFrame> newstack = new List<StackFrame>();
            for (int i = index + 1; i < stack.Count; i++)
                newstack.Add(stack[i]);
            EventRecord rec = new EventRecord(1, tid, nr);
            rec.vars = new SortedDictionary<int, int>();
            rec.mop = (isReturn ? "RETURN" : "CALL");
            rec.name = null;
            rec.stack = newstack;
            DumpEntry(rec, "C", nr);
        }

        private void DumpEntry(EventRecord record, string status, int newnr)
        {
            DumpTuple(record.tid, newnr, EventAttributeEnum.VAR_OP, record.mop);
            foreach(int varid in record.vars.Keys)
               DumpTuple(record.tid, newnr, EventAttributeEnum.VAR_ID, varid.ToString());
            if (record.name != null)
                DumpTuple(record.tid, newnr, EventAttributeEnum.INSTR_METHOD, record.name);
            DumpTuple(record.tid, newnr, EventAttributeEnum.THREADNAME, model.GetThreadName(1, record.tid));
            if (record.stack != null)
                DumpTuple(record.tid, newnr, EventAttributeEnum.STACKTRACE, StringifyStacktrace(record.stack));
            DumpTuple(record.tid, newnr, EventAttributeEnum.STATUS, status);
        }

        private string StringifyStacktrace(List<StackFrame> stack)
        {
            StringBuilder b = new StringBuilder();
            foreach (StackFrame frame in stack)
            {
                b.Append(frame.proc);
                b.Append("|");
                string filename = frame.origname;
                filename = filename.Replace("c:\\Users\\sburckha\\home\\sd\\chess\\dev\\main\\Installer\\Samples\\Managed\\BankAccount",
                                 "C:\\Temp\\CHESSSamples\\BankAccount"); // for Kael
                b.Append(filename);
                b.Append("|");
                b.Append(frame.line);
                b.Append("|");
            }
            return b.ToString();
        }

        private void DumpTuple(int tid, int newnr, EventAttributeEnum attr, string val)
        {
            System.Console.WriteLine(tid.ToString() + " " + newnr.ToString() + " " + (int)attr + " " + val.Length + " " + val);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ExportTrace();
        }

    }
}
