/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Microsoft.ConcurrencyExplorer
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "MultiThread")]
    internal partial class MultiThreadView : Form, IThreadView
    {
        private Font original;
        private List<OneThreadView> threads;

        private Dictionary<int, int> tid_to_index;
        private Dictionary<int, int> index_to_tid;
        private Dictionary<int, int> column_to_index;
        private Dictionary<int, int> index_to_column;
        private Dictionary<int, string> column_to_name;
        
        private List<TaskVarOp> tvo_list;
        private Dictionary<int, int> seqno_to_row;

        private int[] row_to_item;
        private int[] item_to_row;
        private ListViewItem[] orig_items;

        private int[][] pc_list;
        private Formatter formatter;
        private GuiController guicontrol;
        private int num_threads = 0;

        internal MultiThreadView(Formatter formatter, GuiController controller)
        {
            this.formatter = formatter;
            this.guicontrol = controller;
            this.threads = new List<OneThreadView>();
            this.tvo_list = new List<TaskVarOp>();
            this.tid_to_index = new Dictionary<int, int>();
            this.index_to_tid = new Dictionary<int, int>();
            this.column_to_index = new Dictionary<int, int>();
            this.index_to_column = new Dictionary<int, int>();
            this.seqno_to_row = new Dictionary<int, int>();
            this.column_to_name = new Dictionary<int, string>();

            InitializeComponent();
            listView2.Items.Clear();
            listView2.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(listView2_ItemSelectionChanged);
            listView2.CheckBoxes = true;
       }

       public void Initialize()
       {
            OneThreadView otv;
            var threads_panel = this.splitContainer2.Panel2;
            threads_panel.Controls.Clear();
            var threadlist = this.listView1;
            threadlist.Items.Clear();

            if (num_threads == 0)
            {
                return;
            }

            SplitContainer splitcontainer = new SplitContainer();

            if (num_threads != 1)
            {
                threads_panel.Controls.Add(splitcontainer);
                splitcontainer.Orientation = Orientation.Vertical;
                splitcontainer.Dock = System.Windows.Forms.DockStyle.Fill;
                splitcontainer.Location = new System.Drawing.Point(0, 0);
            }

            for (int i = 0; i < num_threads; i++)
            {
                otv = new OneThreadView();
                threads.Add(otv);

                if (num_threads == 1)
                {
                    threads_panel.Controls.Add(otv);
                    otv.Dock = DockStyle.Fill;
                    otv.AutoSize = true;
                    otv.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                }

                otv.SetTitle(column_to_name[index_to_column[i]]);

                otv.BackColor = Palette.getTaskColor(index_to_tid[i]);
                otv.ForeColor = Color.White;

                var row = new ListViewItem(column_to_name[index_to_column[i]]);
                row.Checked = true;
                row.ForeColor = Color.White;
                row.BackColor = Palette.getTaskColor(index_to_tid[i]);
                row.SubItems.Add(index_to_tid[i].ToString());
                row.SubItems.Add("file: none");
                row.SubItems.Add("proc: none");
                row.SubItems.Add("op: none");
                listView1.Items.Add(row);

                otv.Dock = DockStyle.Fill;
                otv.AutoSize = true;
                otv.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                if (num_threads != 1)
                {
                    if (i == num_threads - 1)
                        splitcontainer.Panel2.Controls.Add(otv);
                    else
                    {
                        splitcontainer.Panel1.Controls.Add(otv);
                        if (i < num_threads - 2)
                        {
                            // create new split container
                            var newsplitcontainer = new SplitContainer();
                            newsplitcontainer.Orientation = Orientation.Vertical;
                            newsplitcontainer.Dock = System.Windows.Forms.DockStyle.Fill;
                            newsplitcontainer.Location = new System.Drawing.Point(0, 0);
                            splitcontainer.Panel2.Controls.Add(newsplitcontainer);
                            splitcontainer = newsplitcontainer;
                        }
                    }
                }
            }

            pc_list = new int[tvo_list.Count][];
            var curr = new int[num_threads];
            for (int i = 0; i < num_threads; i++) 
                curr[i] = -1;
            for (int i = tvo_list.Count - 1; i >= 0; i--)
            {
                // track the index in tvo_list of the tid at i
                curr[this.tid_to_index[tvo_list[i].entry.record.tid]] = i;
                pc_list[i] = new int[num_threads];
                curr.CopyTo(pc_list[i], 0);
            }
           
           row_to_item = new int[tvo_list.Count];
           item_to_row = new int[tvo_list.Count];
           for (int i = 0; i < tvo_list.Count; i++)
           {
               row_to_item[i] = i;
               item_to_row[i] = i;
           }
           orig_items = new ListViewItem[listView2.Items.Count];
           listView2.Items.CopyTo(orig_items,0);

           initialized = true;

           show_all_pcs(0);

           // TODO: this is fragile. If we do this too early then toggles are sent to the model from listView2
           listView2.ItemChecked += new ItemCheckedEventHandler(listView2_ItemChecked);
           listView1.ItemChecked += new ItemCheckedEventHandler(listView1_ItemChecked);

        }
       private bool initialized = false;

       #region IThreadView Members

       private void display_file_line(TaskVarOp tvo, bool selected)
        {
            int i = tid_to_index[tvo.entry.record.tid];
            var record = tvo.entry.record;
            lock (record)
            {
                if (record.stack != null && record.stack.Count > 0)
                {
                    var subitems = listView1.Items[i].SubItems;
                    var format = formatter.GetFormat(tvo.entry, true, false);
                    subitems[2].Text = record.tid.ToString();
                    if (record.stack[0].file != null)
                    {
                        subitems[1].Text = record.stack[0].file.Name;
                    }
                    if (record.stack[0].proc != null)
                    {
                        subitems[3].Text = record.stack[0].proc;
                    }
                    subitems[4].Text = format.text;
                }
            }
            threads[i].GotoFileLine(tvo.entry, selected);
        }

        public void SetName(int i, string s)
        {
            // Console.WriteLine("SN: {0} {1}", i, s);
            column_to_name[i] = s;
        }

        public void NewColumn()
        {

        }

        private void check_for_tid(int i, int tid)
        {
            if (!tid_to_index.ContainsKey(tid))
            {
                // Console.WriteLine("NCE: {0} {1} {2}", num_threads, i, tid);
                tid_to_index.Add(tid, num_threads);
                index_to_tid.Add(num_threads, tid);
                column_to_index.Add(i, num_threads);
                index_to_column.Add(num_threads, i);
                num_threads++;
            }
        }

        public bool NewColumnEntry(int index, Entry entry)
        {
            var tvo = new TaskVarOp(index, entry);
            check_for_tid(index, entry.record.tid);

            seqno_to_row[entry.seqno] = tvo_list.Count;
            tvo_list.Add(tvo);

            if (tvo.entry.status.Equals("p"))
            {
                // preemption
                var newitem = new ListViewItem("Pre");
                newitem.SubItems.Add("emp");
                newitem.SubItems.Add("tion");
                newitem.BackColor = Color.Orange;
                listView2.Items.Add(newitem);
            }
            else
            {
                var record = tvo.entry.record;
                lock (record)
                {
                    var format = formatter.GetFormat(tvo.entry,true,false);
                    var newitem = new ListViewItem(record.tid.ToString());
                    newitem.BackColor = format.threadColor;
                    newitem.ForeColor = format.textColor;
                    if (format.fatBorder)
                    {
                        newitem.ForeColor = format.lineColor;
                    }
                    newitem.SubItems.Add(format.text);
                    newitem.SubItems.Add(format.objs);
                    listView2.Items.Add(newitem);
                }
            }
            return true;
        }

        public void InvisibleEntry(int tid)
        {
            // NOP for this view
            // Console.WriteLine("IE: {0}", i);
        }

        public void SetSelection(int entry)
        {
            // Console.WriteLine("SS: {0}", entry);
            try
            {
                show_all_pcs(seqno_to_row[entry]);
            }
            catch (KeyNotFoundException)
            {

            }
        }

        #endregion

        // selecting trace element
        private void show_all_pcs(int index)
        {
            if (!initialized)
                return;

            if (row_to_item[index] >= 0)
            {
                int tid_at_index = tvo_list[index].entry.record.tid;
                // update PC of all threads
                for (int i = 0; i < num_threads; i++)
                {
                    if (pc_list[index][i] != -1) // && (last_pc == null || last_pc[i] != pc_list[index][i] || last_tid == index_to_tid[i]))
                        this.display_file_line(tvo_list[pc_list[index][i]], i == tid_to_index[tid_at_index]);
                }
            }
        }

        private void listView2_ItemSelectionChanged(object sender, EventArgs e)
        {
            if (!initialized)
                return;

            ListView lv = (ListView)sender;
            if (lv.SelectedIndices.Count > 0)
            {
                var entry = lv.SelectedIndices[0];
                // notify the rest of the world
                guicontrol.SetSelection(tvo_list[item_to_row[entry]].entry.seqno);
            }
        }

        // checking trace element
        private void listView2_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!initialized)
                return;

            // Console.WriteLine("Toggle: {0}", e.Item.Index);
            guicontrol.ToggleMark(tvo_list[item_to_row[e.Item.Index]].entry.seqno);
        }

        // selecting threads
        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!initialized)
                return;

            int i = e.Item.Index;
            if (threads!= null && i<threads.Count && threads[i].Parent is Panel)
            {
                var panel = (Panel)threads[i].Parent;
                var sc = (SplitContainer)panel.Parent;
                // TODO: Logic isn't quite right here
                if (sc.Panel1 == panel)
                {
                    sc.Panel1Collapsed = !e.Item.Checked;
                }
                else
                {
                    sc.Panel2Collapsed = !e.Item.Checked;
                }
            }

            // project the trace
            listView2.BeginUpdate();
            listView2.Items.Clear();
            int projection_cnt = 0;
            for (int j = 0; j < tvo_list.Count; j++)
            {
                int tid = tvo_list[j].entry.record.tid;
                if (listView1.Items[tid_to_index[tid]].Checked)
                {
                    row_to_item[j] = projection_cnt;
                    item_to_row[projection_cnt] = j;
                    listView2.Items.Add(orig_items[j]);
                    projection_cnt++;
                }
                else
                {
                    row_to_item[j] = -1;
                }
            }
            listView2.EndUpdate();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!initialized)
                return;

            guicontrol.ShutDown();
        }

        private void courier10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!initialized)
                return;

            if (original == null)
                original = this.Font;
            this.Font = new Font("Courier New", 10);
            this.Invalidate();
        }

        private void courier11ToolStripMenuItem_Click(object sender, EventArgs e)
        {
           if (!initialized)
                return;

            if (original == null)
                original = this.Font;
            this.Font = new Font("Courier New", 11);
            this.Invalidate();
        }

        private void courier12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
           if (!initialized)
                return;

            if (original == null)
                original = this.Font;
            this.Font = new Font("Courier New", 12);
            this.Invalidate();
        }

        private void originalToolStripMenuItem_Click(object sender, EventArgs e)
        {
           if (!initialized)
                return;

            if (original != null)
            {
                this.Font = original;
                this.Invalidate();
            }
        }
    }
}
