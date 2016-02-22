/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Microsoft.ConcurrencyExplorer
{
    internal enum EventAttributeEnum
    {
        VAR_OP = 1,
        VAR_ID = 2,
        STATUS = 3,
        INSTR_METHOD = 4,
        THREADNAME = 5,
        STACKTRACE = 6,
        DISPLAY_BOXED = 7,
        HBSTAMP = 8,
        EVT_SID = 9,
        ENABLE = 10,
        DISABLE = 11,
        LAST_ATTR = 12,
    }

    internal class EventDB : IModel
    {
        private Dictionary<string, FileData> _fileDataCache;

        internal EventDB()
        {
            executions = new Dictionary<int, ExecutionData>();
            entries = new Dictionary<int, Entry>();
            enableinfo = new Dictionary<long, EnableInfo>();
            _fileDataCache = new Dictionary<string, FileData>();
        }

        public int GetNumberEntries()
        {
            return entries.Count;
        }

        public bool IsComplete()
        {
            return is_complete;
        }

        public Entry GetEntry(int entryID)
        {
            return entries[entryID];
        }

        public int GetNumberExecutions()
        {
            return cur_exec;
        }

        public IEnumerable<int> GetThreads(int exec)
        {
            return executions[exec].threads.Keys;
        }

        public string GetThreadName(int exec, int thread)
        {
            return executions[exec].threads[thread].name;
        }

        public int GetNumberEvents(int exec, int thread)
        {
            return executions[exec].threads[thread].max_nr;
        }

        public int GetFirstEntry(int exec)
        {
            return executions[exec].sequence[0].seqno;
        }

        public int GetNumberEntries(int exec)
        {
            return executions[exec].sequence.Count;
        }

        public bool HasRace(int exec)
        {
            return executions[exec].racing.Count == 2;
        }
        public EventRecord[] GetRacingEntries(int exec)
        {
            return executions[exec].racing.ToArray();
        }
        public Entry[] GetMarkedEntries()
        {
            return marked.ToArray();
        }

        // enable info
        internal static void GetEnableChange(Entry e, out int[] enable, out int[] disable)
        {
            if (e.status == "c")
            {
                if (e.record.enables != null)
                    enable = e.record.enables.ToArray();
                else
                    enable = null;
                if (e.record.disables != null)
                    disable = e.record.disables.ToArray();
                else
                    disable = null;
            }
            else if (e.status == "b")
            {
                enable = null;
                disable = new int[] { e.record.tid };
            }
            else
            {
                enable = null;
                disable = null;
            }
        }

        private void ComputeEnabledInfo(int exec)  // called under model lock
        {
            // compute info for execution
            int[] tids = GetThreads(exec).ToArray();
            int num_entries = GetNumberEntries(exec);

            bool[] enabled = new bool[tids.Length];
            enabled[0] = true;
            int[] enables, disables;
            int start = GetFirstEntry(exec);
            int end = start + num_entries;
            for (int en = start; en < end; en++)
            {
                Entry e = GetEntry(en);
                GetEnableChange(e, out enables, out disables);
                lock (e)
                {
                    e.enableinfo = new EnableInfo[tids.Length];
                    for (int t = 0; t < tids.Length; t++)
                        e.enableinfo[t] = (enabled[t] ? EnableInfo.Enabled : EnableInfo.Disabled);
                    if (enables != null)
                        foreach (int tid in enables)
                        {
                            for (int t = 0; t < tids.Length; t++)
                                if (tids[t] == tid)
                                {
                                    if (!enabled[t])
                                    {
                                        enabled[t] = true;
                                        e.enableinfo[t] = EnableInfo.Enable;
                                    }
                                    else
                                    {
                                        e.enableinfo[t] = EnableInfo.Error;
                                        //System.Diagnostics.Debug.Assert(false);
                                    }
                                }
                        }
                    if (disables != null)
                        foreach (int tid in disables)
                        {
                            for (int t = 0; t < tids.Length; t++)
                                if (tids[t] == tid)
                                {
                                    if (enabled[t])
                                    {
                                        enabled[t] = false;
                                        e.enableinfo[t] = EnableInfo.Disable;
                                    }
                                    else
                                    {
                                        e.enableinfo[t] = EnableInfo.Error;
                                        //System.Diagnostics.Debug.Assert(false);
                                    }
                                }
                        }
                }
            }

        }

        // stored executions and entries
        private Dictionary<int, ExecutionData> executions;
        private Dictionary<int, Entry> entries;
        private Dictionary<long, EnableInfo> enableinfo;

        private bool is_complete;

        // currently active execution
        private int cur_exec = 0;
        private ExecutionData cur_execdata;

        public void SetComplete()
        {
            if (cur_exec != 0 && cur_execdata.last_entry != null)
            {
                if (cur_execdata.last_entry != null)
                    cur_execdata.last_entry.last_in_block = true;
                // compute enabled info
                ComputeEnabledInfo(cur_exec);
            }

            is_complete = true;

            // notify observers
            if (Complete != null)
                Complete();
        }

        public void StartNewExecution()
        {
            if (cur_exec != 0)
            {
                if (cur_execdata.last_entry != null)
                    cur_execdata.last_entry.last_in_block = true;
                // compute enabled info
                ComputeEnabledInfo(cur_exec);
            }

            cur_exec++;
            cur_execdata = new ExecutionData();
            executions[cur_exec] = cur_execdata;

            // notify observers
            if (NewExecution != null)
                NewExecution(cur_exec);
        }

        // create entry, or create or modify EventRecord
        public void ProcessTuple(int tid, int nr, int attr, String value)
        {
            // make sure thread is known
            ThreadData t;
            if (!cur_execdata.threads.ContainsKey(tid))
                t = cur_execdata.threads[tid] = new ThreadData(tid);
            else
                t = cur_execdata.threads[tid];

            // update max nr
            if (nr > t.max_nr)
                t.max_nr = nr;

            // find or create event record
            EventRecordImpl rec;
            if (t.events.ContainsKey(nr))
            {
                rec = t.events[nr];
            }
            else
            {
                rec = new EventRecordImpl(this, cur_exec, tid, nr);
                t.events[nr] = rec;
            }

            // update the attribute
            EventAttributeEnum attribute = (EventAttributeEnum)attr;
            rec.set_attribute(attribute, value);

            if (attribute == EventAttributeEnum.STATUS)
            {
                int seqno = entries.Count + 1;
                Entry entry = new Entry(rec, value, seqno);
                entries[seqno] = entry;
                cur_execdata.sequence.Add(entry);
                rec.entries.Add(entry);
                // default timestamp to last timestamp
                if (rec.hbStamp == null)
                    rec.hbStamp = t.last_stamp;
                // update first/last in block
                if (cur_execdata.last_thread != rec.tid)
                {
                    entry.first_in_block = true;
                    if (cur_execdata.last_entry != null)
                        cur_execdata.last_entry.last_in_block = true;
                }
                cur_execdata.last_thread = rec.tid;
                cur_execdata.last_entry = entry;
                // notify observers
                if (NewEntry != null)
                    NewEntry(entry);
                // set selection to first entry if not set yet
                if (seqno == 1 && selection == -1)
                    SetSelection(1);
            }
            else if (attribute == EventAttributeEnum.THREADNAME)
            {
                if (t.name != value)
                {
                    t.name = value;
                    // notify observers
                    if (ThreadnameUpdate != null)
                    {
                        ThreadnameUpdate(cur_exec, tid, value);
                    }
                }
            }
            else
            {
                if (attribute == EventAttributeEnum.DISPLAY_BOXED)
                {
                    cur_execdata.racing.Add(rec);
                }
                if (attribute == EventAttributeEnum.HBSTAMP)
                {
                    t.last_stamp = rec.hbStamp;
                }
                // notify observers
                if (EntryUpdate != null)
                    foreach (Entry entry in rec.entries)
                        EntryUpdate(entry);
            }
        }


        // selection
        private int selection = -1;
        public void SetSelection(int entry)
        {
            if (entry != selection)
            {
                selection = entry;
                // notify observers
                if (SelectionUpdate != null)
                    SelectionUpdate(entry);
            }
        }
        public int GetSelection()
        {
            return selection;
        }

        // mark
        List<Entry> marked = new List<Entry>();
        public void ToggleMark(int e)
        {
            Entry entry = GetEntry(e);
            entry.marked = !entry.marked;
            if (entry.marked)
                marked.Add(entry);
            else
                marked.Remove(entry);
            // notify observers
            if (SelectionUpdate != null)
                SelectionUpdate(e);
        }

        // events
        public event NewExecutionHandler NewExecution;
        public event NewEntryHandler NewEntry;
        public event EntryUpdateHandler EntryUpdate;
        public event CompleteHandler Complete;
        public event ThreadnameUpdateHandler ThreadnameUpdate;
        public event SelectionUpdateHandler SelectionUpdate;

        internal FileData GetFileFromCache(string filename)
        {
            FileData fileData = null;
            if (!String.IsNullOrEmpty(filename))
            {
                lock (_fileDataCache)
                {
                    if (!_fileDataCache.TryGetValue(filename, out fileData) || fileData.LastModified != File.GetLastWriteTime(filename))
                    {
                        if (File.Exists(filename))
                        {
                            fileData = new FileData(filename);
                            _fileDataCache[filename] = fileData;
                        }
                    }
                }
            }

            return fileData;
        }
    }

    internal class ThreadData
    {
        internal Dictionary<int, EventRecordImpl> events = new Dictionary<int, EventRecordImpl>();
        internal int tid;
        internal string name;
        internal int max_nr;
        internal Timestamp last_stamp;
        internal ThreadData(int tid)
        {
            this.tid = tid;
            name = "Task " + tid;
        }
    }

    internal class ExecutionData
    {
        internal Dictionary<int, ThreadData> threads = new Dictionary<int, ThreadData>();
        internal List<Entry> sequence = new List<Entry>();
        internal List<EventRecord> racing = new List<EventRecord>();
        internal int last_thread = -1;
        internal Entry last_entry = null;
    }

    internal class EventRecordImpl : EventRecord
    {

        EventDB _ownerDB;

        internal EventRecordImpl(EventDB ownerDB, int exec, int tid, int nr)
            : base(exec, tid, nr)
        {
            _ownerDB = ownerDB;
            entries = new List<Entry>();
        }

        internal void set_attribute(EventAttributeEnum attr, String val)
        {
            System.Diagnostics.Debug.Assert(val != null);
            switch (attr)
            {
                case EventAttributeEnum.VAR_OP:
                    mop = val;
                    if (val == "DATA_READ" || val == "DATA_WRITE")
                        isData = true;
                    break;
                case EventAttributeEnum.VAR_ID:
                    int varid = Int32.Parse(val);
                    vars[varid] = varid;
                    break;
                case EventAttributeEnum.INSTR_METHOD:
                    name = val;
                    break;
                case EventAttributeEnum.STACKTRACE:
                    parseStacktrace(val);
                    break;
                case EventAttributeEnum.DISPLAY_BOXED:
                    boxColor = val;
                    break;
                case EventAttributeEnum.HBSTAMP:
                    parseTimestamp(val);
                    break;
                case EventAttributeEnum.EVT_SID:
                    sid = Int32.Parse(val);
                    break;
                case EventAttributeEnum.ENABLE:
                    if (enables == null)
                        enables = new List<int>();
                    enables.Add(Int32.Parse(val));
                    break;
                case EventAttributeEnum.DISABLE:
                    if (disables == null)
                        disables = new List<int>();
                    disables.Add(Int32.Parse(val));
                    break;
                default:
                    break;
            }

            // suppress stack trace for TASK_START events
            if (mop == "TASK_BEGIN" && stack != null && stack.Count != 0)
                stack = new List<StackFrame>();
        }

        private int zero = Convert.ToInt32('0');
        private int nine = Convert.ToInt32('9');
        private void parseTimestamp(String val)
        {
            List<int> elts = new List<int>();
            int pos = 0;
            pos++; // skip '['
            while (pos < val.Length && val[pos] != ']')
            {
                int number = 0;
                while (pos < val.Length && zero <= val[pos] && nine >= val[pos])
                {
                    number = (10 * number) + (val[pos] - zero);
                    pos++;
                }
                elts.Add(number);

                pos++; // skip ' '
            }
            hbStamp = new Timestamp(elts);
        }

        private void parseStacktrace(String val)
        {
            string[] split = val.Split(new Char[] { '|' });
            int pos = 0;
            stack = new List<StackFrame>();
            while (pos + 3 <= split.Length)
            {
                string proc = split[pos++];
                string file = split[pos++];
                string line = split[pos++];
                FileData f = _ownerDB.GetFileFromCache(file);
                stack.Add(new StackFrame(proc, f, Int32.Parse(line), file));
            }
        }

    }


}
