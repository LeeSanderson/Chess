/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using Microsoft.Concurrency.TestTools.Execution.Backends;
using Microsoft.Concurrency.TestTools.Execution;

namespace Microsoft.Concurrency.TestTools.Alpaca.Backends
{
    class ClusterBackend : IBackend
    {
        /*NOTE 
         *  this class is not functional at this point of time. 
         *  The plan is to connect with HPC Pack SDK interface at some point. */

        public ClusterBackend(string headnode)
        {
            this.headnode = headnode;
            hpcinterface = new HpcInterface(headnode);
        }

        public bool IsChoosable { get { throw new NotImplementedException("TODO: ClusterBackend.IsChoosable needs to be implemented."); } }

        public string Identifier { get { return "Cluster " + headnode.ToString(); } }

        public ClusterTask CreateTask(RunEntity run)
        {
            ClusterTask task = new ClusterTask(this, run);
            task.UpdateStatus();
            return task;
        }
        ITaskHandle IBackend.CreateTask(RunEntity run) { return CreateTask(run); }

        string headnode;
        HpcInterface hpcinterface;

        private Dictionary<int, int> polljobs = new Dictionary<int, int>();
        private Dictionary<int, ClusterTask> tasks = new Dictionary<int, ClusterTask>();

        int job = -1;

        internal int GetJob()
        {
            if (job == -1)
                BeginSubmission();
            System.Diagnostics.Debug.Assert(job != -1);
            return job;
        }

        internal HpcInterface GetShell()
        {
            return hpcinterface;
        }

        internal void AddTask(ClusterTask t)
        {
            tasks.Add(t.GetTaskIndex(), t);
        }

        internal void AddPollJob(int jobnumber)
        {
            polljobs.Add(jobnumber, 0);
        }

        internal void BeginSubmission()
        {
            System.Diagnostics.Debug.Assert(job == -1);
            job = hpcinterface.NewJob();
        }

        public void EndSubmission()
        {
            if (job != -1)
            {
                hpcinterface.SubmitJob(job);
                polljobs.Add(job, 0);
                job = -1;
            }
        }

        public void Init()
        {
        }

        public void Update()
        {
            List<int> to_be_removed = new List<int>();
            List<int> taskindex = new List<int>();
            List<HpcInterface.Taskstate> taskstate = new List<HpcInterface.Taskstate>();

            foreach (int job in polljobs.Keys)
            {
                taskindex.Clear();
                taskstate.Clear();
                hpcinterface.GetTasks(job, taskindex, taskstate);

                bool found_running_task = false;
                for (int i = 0; i < taskindex.Count; i++)
                {
                    ClusterTask task;
                    if (tasks.TryGetValue(taskindex[i], out task))
                    {
                        task.State = taskstate[i];
                        task.UpdateStatus();
                        found_running_task = (task.GetStatus() != TaskStatus.Complete);
                    }
                }

                if (!found_running_task)
                {
                    to_be_removed.Add(job);
                }
            }

            foreach (int job in to_be_removed)
                polljobs.Remove(job);

        }

    }
}
