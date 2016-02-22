/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ConcurrencyExplorer
{
    internal interface IFilter
    {
        bool show_entry(Entry entry);
        bool show_execution(int exec);
        bool show_thread(int exec, int thread);
    }

    internal class TrivialFilter : IFilter
    {
        public bool show_entry(Entry entry) { return true; }
        public bool show_execution(int exec) { return true; }
        public bool show_thread(int exec, int thread) { return true; }
    }

    internal class IntersectionFilter : IFilter
    {
        private IFilter f1;
        private IFilter f2;
        internal IntersectionFilter(IFilter f1, IFilter f2)
        {
            this.f1 = f1; this.f2 = f2;
        }
        public bool show_entry(Entry entry)
        {
            return f1.show_entry(entry) && f2.show_entry(entry);
        }
        public bool show_execution(int exec)
        {
            return f1.show_execution(exec) && f2.show_execution(exec);
        }
        public bool show_thread(int exec, int thread)
        {
            return f1.show_thread(exec, thread) && f2.show_thread(exec, thread);
        }
    }

    internal class ThreadFilter : IFilter {
        private bool[] mask;
        internal ThreadFilter(bool[] mask)
        {
            this.mask = mask;
        }
        public bool show_entry(Entry entry)
        {
            return show_thread(entry.record.exec, entry.record.tid);
        }
        public bool show_execution(int exec)
        {
            return true;
        }
        public bool show_thread(int exec, int thread)
        {
            return (thread > mask.Length) || mask[thread-1];
        }
    }

    internal class VarFilter : IFilter
    {
        private int[] vars;

        // call this under model lock
        internal VarFilter(int[] vars)
        {
            this.vars = vars;
        }

        public bool show_execution(int exec) { return true; }
        public bool show_thread(int exec, int thread) { return true; }
        public bool show_entry(Entry entry)
        {
            foreach (int var in vars)
            {
                if (entry.record.vars.ContainsKey(var))
                    return true;
            }
            return false;
        }
    }

    internal class PreemptionFilter : IFilter
    {
        // call this under model lock
        internal PreemptionFilter() { }

        public bool show_execution(int exec) { return true; }
        public bool show_thread(int exec, int thread) { return true; }
        public bool show_entry(Entry entry)
        {
            return entry.first_in_block || entry.last_in_block;
        }
    }

 
    internal class RaceFilter : IFilter
    {
        private int exec;
        private int t1;
        private int t2;
        private int nr1;
        private int nr2;
        private Timestamp ts1;
        private Timestamp ts2;

        private int firstsel = -1;
        private int secondsel = -1;

        private string error;

        // call this under model lock
        internal RaceFilter(IModel model, int exec)
        {
            this.exec = exec;
            EventRecord[] recs = model.GetRacingEntries(exec);
            if (recs.Length != 2)
                error = "The selected execution does not contain a race.";
            else
            {
                t1 = recs[0].tid;
                nr1 = recs[0].nr;
                lock (recs[0])
                {
                    ts1 = recs[0].hbStamp;
                }
                t2 = recs[1].tid;
                nr2 = recs[1].nr;
                lock (recs[1])
                {
                    ts2 = recs[1].hbStamp;
                }
                if (ts1 == null || ts2 == null)
                    error = "The selected execution does not contain enough information. Rerun with /ls option.";
                else
                {
                    if (recs[0].entries.Count > 0)
                        firstsel = recs[0].entries[recs[0].entries.Count - 1].seqno;
                    if (recs[1].entries.Count > 0)
                        secondsel = recs[1].entries[recs[1].entries.Count - 1].seqno;
                }
            }
        }
        
        internal int getFirstSelection()
        {
            return firstsel;
        }
        internal int getSecondSelection()
        {
            return secondsel;
        }

        internal string getError()
        {
            return error;
        }

        public bool show_execution(int execution) { return (execution == this.exec); }
        public bool show_thread(int execution, int thread) { return (execution == this.exec); }
        public bool show_entry(Entry entry)
        {
            if (entry.record.exec != exec)
                return false;
            if (entry.record.isData)
                return (entry.record.boxColor != null);
            if (entry.record.hbStamp == null)
                return true;
            
            if (entry.record.tid == t1 && entry.record.nr <= nr1)
                return true;
            if (entry.record.tid == t2 && entry.record.nr <= nr2)
                return true;
            if (entry.record.tid != t1 && entry.record.nr <= ts1.get(entry.record.tid))
                return true;
            if (entry.record.tid != t2 && entry.record.nr <= ts2.get(entry.record.tid))
                return true;
            return false;
        }


    }
}
