/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Microsoft.ConcurrencyExplorer
{
    internal class StandardView : IView, IDisposable, IIndexConverter
    {
        private GuiController controller;
        private IModel model;

        private IFilter filter;

        private IThreadView threadviz;
        private Thread guithread;
        private Formatter formatter;
        private bool multipleExecutions;
        private bool oldDisplayStyle;
        private System.Windows.Forms.Timer timer;

        private const int TimerInterval = 1000;

        internal StandardView(GuiController controller, IModel model, IFilter filter, bool multipleExecutions, bool oldDisplayStyle)
        {
            this.controller = controller;
            this.model = model;
            this.filter = filter;
            this.multipleExecutions = multipleExecutions;
            this.oldDisplayStyle = oldDisplayStyle || multipleExecutions;
            formatter = new Formatter(this.oldDisplayStyle, this);

            // create thread
            guithread = new Thread(new ThreadStart(RunGuiThread));
            guithread.IsBackground = true;
            guithread.SetApartmentState(ApartmentState.STA);
            guithread.Name = "gui-view";
        }

        public void StartView()
        {
            // start thread
            guithread.Start();
        }

        public void CloseView()
        {
            // can not close directly (only GUI thread may do that).
            // set this flag.. GUI thread will check it periodically and close form
            needs_closing = true;
        }

        public void Dispose()
        {
            formatter.Dispose();
            timer.Dispose();
            GC.SuppressFinalize(this);
        }

        // this is the entry point for the gui thread
        private void RunGuiThread()
        {
            threadviz =
                (!oldDisplayStyle) ? 
                (new MultiThreadView(formatter, controller) as IThreadView) :
                (new ThreadViz(formatter, controller) as IThreadView);

            // be notified of form closing
            (threadviz as Form).FormClosing += new FormClosingEventHandler(HandleFormClosing);

            // need timer callback to process updates
            timer = new System.Windows.Forms.Timer();
            timer.Interval = TimerInterval;
            timer.Tick += new System.EventHandler(HandleTimerTick);
            timer.Start();

            // register as observer for model events
            lock (model)
            {
                model.Complete += new CompleteHandler(HandleComplete);
                model.EntryUpdate += new EntryUpdateHandler(HandleUpdate);
                model.NewEntry += new NewEntryHandler(HandleNewEntry);
                model.ThreadnameUpdate += new ThreadnameUpdateHandler(HandleThreadnameUpdate);
                model.SelectionUpdate += new SelectionUpdateHandler(HandleSelectionUpdate);
            }

            // run form
            System.Windows.Forms.Application.Run((Form)threadviz);

            //System.Console.WriteLine("guithread done");

            // unregister observers
            lock (model)
            {
                model.Complete -= new CompleteHandler(HandleComplete);
                model.EntryUpdate -= new EntryUpdateHandler(HandleUpdate);
                model.NewEntry -= new NewEntryHandler(HandleNewEntry);
                model.ThreadnameUpdate -= new ThreadnameUpdateHandler(HandleThreadnameUpdate);
                model.SelectionUpdate -= new SelectionUpdateHandler(HandleSelectionUpdate);
            }

            // notify controller
            controller.ViewHasClosed(this);
        }

        private Thunk initialAction = null;
        internal void SetInitialAction(Thunk thunk)
        {
            initialAction = thunk;
        }

        // process pending jobs for GUI thread (closing, updates)
        private void HandleTimerTick(Object sender, EventArgs eargs)
        {
            if (!closed)
            {
                if (needs_closing)
                {
                    actual_close(true);
                    needs_closing = false;
                }
                else if (needs_update)
                {
                    ProcessUpdates();
                    needs_update = false;
                }

                if (initialAction != null)
                {
                    initialAction();
                    initialAction = null;
                }

            }
        }

        private void HandleFormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!closed)
            {
                actual_close(false);
            }
        }

        // shut down the GUI elements
        private void actual_close(bool closeviz)
        {
            closed = true;

            // disable timer
            timer.Enabled = false;

            // truly close the view now (if it's not already closing)
            if (closeviz)
                (threadviz as Form).Close();
        }

        // record observer calls

        private void HandleComplete()
        {
            needs_update = true;
        }

        private void HandleNewEntry(Entry entry)
        {
            needs_update = true;
        }

        private void HandleUpdate(Entry entry)
        {
            needs_update = true;
        }

        private void HandleThreadnameUpdate(int exec, int tid, string name)
        {
            needs_update = true;
            update_threadnames = true;
        }

        // this one we want more speedy... invoke on the spot
        void HandleSelectionUpdate(int entry)
        {
            if (entry != -1)
            {
                // use async 'BeginInvoke' rather than 'Invoke' to avoid deadlock
                // side effect: view may sometimes display stale selection
                (threadviz as Form).BeginInvoke(new Thunk(delegate
                    {
                        threadviz.SetSelection(entry);
                    }));
            }
        }
        
    
        // use flag once closed
        private bool closed = false;

        // stored info about pending updates
        private volatile bool needs_closing = false;
        private volatile bool needs_update = true;

        // current state of processing
        private int last_entry_processed = 0;
        private int current_exec = 0;
        private bool is_complete = false;
        private bool update_threadnames = true;

        private void ProcessUpdates()
        {
            lock (model)
            {
                // process entries
                int most_recent_entry = model.GetNumberEntries();
                while (last_entry_processed < most_recent_entry)
                {
                    int execindex;
                    int tindex;
                    Entry entry = model.GetEntry(++last_entry_processed);
                    CatchUpExecutions(entry.record.exec);
                    if (!getExecIndex(entry.record.exec, out execindex))
                        continue;
                    if (!getThreadIndex(current_exec, entry.record.tid, out tindex))
                        continue;
                    if (filter.show_entry(entry))
                        threadviz.NewColumnEntry(tindex, entry);
                    else
                        threadviz.InvisibleEntry(tindex);
                }

                if (update_threadnames)
                {
                    int execindex;
                    if (getExecIndex(current_exec, out execindex))
                    {
                        UpdateThreadnames();
                    }
                    update_threadnames = false;
                }

                if (!is_complete && model.IsComplete())
                {
                    CatchUpExecutions(model.GetNumberExecutions());
                    if (!multipleExecutions)
                    {
                        threadviz.Initialize();
                        UpdateThreadnames();
                    }
                    is_complete = true;
                }
                
                // set selection
                int selected_entry = model.GetSelection();
                if (selected_entry != -1)
                {
                    threadviz.SetSelection(selected_entry);
                }
            }
            // redraw (TODO: redraw only changed portions of view)
            (threadviz as Form).Invalidate();
        }

        private void CatchUpExecutions(int to)
        {
            int execindex;
            while (current_exec < to)
            {
                if (current_exec != 0 && update_threadnames)
                    UpdateThreadnames();
                current_exec++;
                if (getExecIndex(current_exec, out execindex))
                    threadviz.NewColumn();
            }
        }


        private void UpdateThreadnames()
        {
            foreach (int tid in model.GetThreads(current_exec))
            {
                int tindex;
                if (!getThreadIndex(current_exec, tid, out tindex))
                    continue;
                threadviz.SetName(tindex, model.GetThreadName(current_exec, tid));
            }
        }
        
        // maintain maps for converting model indexes of executions and threads
        private class ExecInfo
        {
            internal int exec;
            internal int index;
            internal Dictionary<int, int> tmap;
            internal List<int> ts;
            internal ExecInfo(int exec, int index)
            {
                this.exec = exec;
                this.index = index;
                tmap = new Dictionary<int, int>();
                ts = new List<int>();
            }
        }
        private Dictionary<int, ExecInfo> execmap = new Dictionary<int,ExecInfo>();
        private List<ExecInfo> execs = new List<ExecInfo>();

        internal bool getExecIndex(int modelindex, out int index)
        {
            ExecInfo info;
            if (execmap.TryGetValue(modelindex, out info))
            {
                index = info.index;
                return true;
            }
            else
            {
                if (filter.show_execution(modelindex))
                {
                    index = execs.Count + 1;
                    ExecInfo ei = new ExecInfo(modelindex, index);
                    execmap[modelindex] = ei;
                    execs.Add(ei);
                    return true;
                }
                else
                {
                    index = 0;
                    return false;
                }
            }
        }
        internal bool getThreadIndex(int modelindex, int thread, out int index)
        {
            ExecInfo info = execmap[modelindex];
            if (info.tmap.TryGetValue(thread, out index))
                return true;
            else
            {
                if (filter.show_thread(modelindex, thread))
                {
                    index = info.tmap.Count + 1;
                    info.tmap[thread] = index;
                    info.ts.Add(thread);
                    return true;
                }
                else
                {
                    index = 0;
                    return false;
                }
            }
        }

        public int ToModelExec(int exec)
        {
            if (exec < 1 || exec > execs.Count)
                return -1;
            return execs[exec-1].exec;
        }


        public int ToModelThread(int exec, int thread)
        {
            if (exec < 1 || exec > execs.Count)
                return -1;
            ExecInfo info = execs[exec - 1];
            if (thread < 1 || thread > info.ts.Count)
                return -1;
            return info.ts[thread-1];
        }

        public int FromModelExec(int exec)
        {
            ExecInfo info;
            if (!execmap.TryGetValue(exec, out info))
                return -1;
            return info.index;
        }
        public int FromModelThread(int exec, int thread)
        {
           ExecInfo info;
            if (!execmap.TryGetValue(exec, out info))
                return -1;
            int tindex;
            if (!getThreadIndex(info.index, thread, out tindex))
                return -1;
            return tindex;
        }

    }
}
