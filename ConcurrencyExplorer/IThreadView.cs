/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Microsoft.ConcurrencyExplorer
{
    delegate void DisplayLocationDel(TaskVarOp tvo);

    internal class FileData
    {
        static private int file_cnt = 0;

        /// <summary>
        /// Gets the last modified time for this file data when it was created.
        /// This helps caching of files to make sure we always display the latest file state
        /// even if the user changes it.
        /// </summary>
        public DateTime LastModified { get; private set; }
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string[] Lines { get; private set; }
        private int[] _lineOffsetsWithoutLineTerminatingChars;

        internal FileData(string fn)
        {
            Id = System.Threading.Interlocked.Increment(ref file_cnt);
            Name = fn;
            LastModified = File.GetLastWriteTime(Name);

            Lines = File.ReadAllLines(Name);  // This version won't include any line terminating marks

            // Pre-compute offsets w/o line terminating characters
            var offsets = new int[Lines.Length];
            int sum = 0;
            for (int i = 0; i < Lines.Length; i++)
            {
                offsets[i] = sum;
                sum += Lines[i].Length;
            }
            _lineOffsetsWithoutLineTerminatingChars = offsets;
        }

        /// <summary>
        /// Computes the character offset to the beginning of the line if
        /// using the specified lineTerminator.
        /// </summary>
        /// <param name="lineIdx">The index of the line to compute the char offset for.</param>
        /// <param name="lineTerminator">
        /// The line terminator string. This will allow for computation to be usage independent
        /// since some WinForms controls don't like \n chars and instead require \r\n.
        /// </param>
        /// <returns></returns>
        public int GetLineOffset(int lineIdx, string lineTerminator)
        {
            return _lineOffsetsWithoutLineTerminatingChars[lineIdx] + lineTerminator.Length * lineIdx;
        }

    }

    internal struct StackFrame
    {
        internal StackFrame(string p, FileData f, int l, string o) { proc = p; file = f; line = l; origname = o; }
        internal string proc;
        internal FileData file;
        internal int line;
        internal string origname;
        // public override bool Equals(object obj)
        //{
        //     StackFrame other = obj as StackFrame;
        //     return (other.proc == proc && other.file.Equals(file) && other.line == line);
        /// }
        // public override 
    }

    internal class TaskVarOp
    {
        internal TaskVarOp(int t, Entry e)
        {
            tid = t; entry = e;
        }
        internal int tid;
        internal Entry entry;
    }

    interface IThreadView
    {
        void Initialize();
        void NewColumn();
        void SetName(int i, string s);
        bool NewColumnEntry(int tid, Entry entry);
        void InvisibleEntry(int tid);
        void SetSelection(int entry);
    }
}


