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
using System.Windows.Forms;
using System.IO;
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Backends;
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Alpaca.Backends
{
    class ClusterTask : Task
    {
        // construction time
        private ClusterBackend backend;

        // submit time
        private int jobnumber;

        public ClusterTask(ClusterBackend backend, RunEntity runEntity)
            : base(backend, runEntity)
        {
            this.backend = backend;

            if (submitted)
            {
                jobnumber = (int)Run.DataElement.Element(XSessionNames.Job);
                backend.AddPollJob(jobnumber);
                backend.AddTask(this);
                State = HpcInterface.Taskstate.Unknown;
            }
            else
                State = HpcInterface.Taskstate.Unknown;
        }

        internal HpcInterface.Taskstate State { get; set; }

        protected override string MakeTaskID()
        {
            return "s" + jobnumber.ToString("d7") + "t" + TaskState.TaskIndex.ToString("d7");
        }

        protected override void OnSubmit()
        {
            base.OnSubmit();
            jobnumber = backend.GetJob();
            Run.DataElement.Add(new XElement(XSessionNames.Job, jobnumber.ToString()));
            backend.GetShell().AddTask(jobnumber, GetTaskIndex(), Run.ScriptFolderPath, GetStartScriptPath());
            backend.AddTask(this);
            UpdateStatus();
        }

        internal int GetJob()
        {
            return jobnumber;
        }

        public override int? GetExitCode()
        {
            return null;
        }

        protected override TaskStatus ComputeRunningTaskStatus()
        {
            switch (State)
            {
                case HpcInterface.Taskstate.Canceled:
                case HpcInterface.Taskstate.Failed:
                case HpcInterface.Taskstate.Finished:
                    return TaskStatus.Complete;
                case HpcInterface.Taskstate.Configuring:
                case HpcInterface.Taskstate.Queued:
                case HpcInterface.Taskstate.Submitted:
                    return TaskStatus.Waiting;
                default:
                    return TaskStatus.Running;
            }
        }
    }
}
