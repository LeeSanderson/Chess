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
    internal interface IModel
    {
        // query interface (called by views)

        int GetNumberEntries();
        Entry GetEntry(int entry);
        
        int GetNumberExecutions();
        IEnumerable<int> GetThreads(int exec);
        string GetThreadName(int exec, int thread);
        int GetNumberEvents(int exec, int thread);
        int GetFirstEntry(int exec);
        int GetNumberEntries(int exec);

        bool HasRace(int exec);
        EventRecord[] GetRacingEntries(int exec);
        Entry[] GetMarkedEntries();

        bool IsComplete();

        int GetSelection(); // returns -1 if none selected

        // mutator interface (called by controllers)
        void StartNewExecution();
        void ProcessTuple(int tid, int nr, int attr, string val);
        void SetComplete();

        void SetSelection(int entry);
        void ToggleMark(int entry);

        // observer interfaces
        event NewExecutionHandler NewExecution;
        event NewEntryHandler NewEntry;
        event EntryUpdateHandler EntryUpdate;
        event CompleteHandler Complete;
        event ThreadnameUpdateHandler ThreadnameUpdate;
        event SelectionUpdateHandler SelectionUpdate;
        
        // intended locking scheme:
        // - all random access and update methods ASSUME that calling thread holds model lock 
        // - all calls to event handlers GUARANTEE that calling thread holds model lock
        //   (therefore, observer callbacks must guarantee timely return)
        // - all accesses to eventrecord fields (except for accesses that target constant field only) 
        //   must grab EventRecord lock.
    }
    
    delegate void NewEntryHandler(Entry entry);
    delegate void NewExecutionHandler(int exec);
    delegate void EntryUpdateHandler(Entry entry);
    delegate void CompleteHandler();
    delegate void ThreadnameUpdateHandler(int exec, int tid, string name);
    delegate void SelectionUpdateHandler(int entry);

    internal enum EnableInfo { Enabled, Disabled, Enable, Disable, Error };

    internal class Entry
    {
        internal Entry(EventRecord r, string s, int sn)
        {
            record = r;
            status = s;
            seqno = sn;
        }
        // these fields are constant after construction and can be read without lock.
        internal EventRecord record;
        internal string status;
        internal int seqno;

        // can be accessed without lock
        internal volatile bool marked;
        internal volatile bool first_in_block, last_in_block;

        // need to be accessed under lock
        internal EnableInfo[] enableinfo;

        public override string  ToString()
        {
            return "status=" + status + " " + record.ToString();
        }

    }
    
   internal class EventRecord
    {
        internal EventRecord(int exec, int tid, int nr)
        {
            this.exec = exec;
            this.tid = tid;
            this.nr = nr;
        }
        // these fields are constant after construction and can be read without lock.
        internal int exec;
        internal int tid;
        internal int nr;

        // all following fields must be accessed under EventRecord.lock

        // fields corresponding to attributes
        internal SortedDictionary<int,int> vars = new SortedDictionary<int,int>(); //used like a set
        internal string mop;
        internal string name;
        internal string boxColor;
        internal List<StackFrame> stack;
        internal bool isData;
        internal Timestamp hbStamp;
        internal int sid = -1;
        internal List<int> enables;
        internal List<int> disables;

        // the entries linked to this record
        internal List<Entry> entries;

        public override string ToString()
        {
            string info = "(E " + exec + ") " + tid + "." + nr + " mop=" + mop + " name=" + name + " pc=";
            if (stack != null)
            for(int i = 0; i < stack.Count; i++)
                info = info + "\n" + stack[i].proc + ":" + stack[i].origname + "(" + stack[i].line + ")";
            return info;
        }
    }

    internal class Timestamp
    {
        private int[] vec;
        internal int get(int tid)
        {
            if (tid - 1 < vec.Length)
                return vec[tid - 1];
            else
                return 0;
        }
        internal Timestamp(List<int> elts)
        {
            vec = elts.ToArray();
        }
        internal bool lessThanOrEqual(Timestamp ts)
        {
            int pos = 0;
            while (pos < vec.Length)
            {
                if (vec[pos] > ts.get(pos))
                    return false;
                pos++;
            }
            return true;
        }
        internal String format(int numt)
        {
            StringBuilder b = new StringBuilder();
            b.Append("[");
            for (int i = 1; i <= numt; ++i)
            {
                if (i != 1)
                    b.Append(" ");
                b.Append(get(i).ToString());
            }
            b.Append("]");
            return b.ToString();
        }
    }

    internal interface IIndexConverter
    {
        // convert indices; returns -1 if not in domain
        int ToModelExec(int exec);
        int FromModelExec(int exec);
        int ToModelThread(int exec, int thread);
        int FromModelThread(int exec, int thread);
    }
}
