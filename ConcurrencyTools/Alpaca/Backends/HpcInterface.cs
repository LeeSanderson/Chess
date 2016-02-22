/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    class HpcInterface
    {
        /*NOTE 
         *  this class is not functional at this point of time. 
         *  The plan is to connect with HPC Pack SDK interface at some point. */

        
        public HpcInterface(string headnode)
        {
            //TODO   
        }


        public enum Taskstate
        {
            Unknown,
            Configuring,
            Submitted,
            Queued,
            Running,
            Finished,
            Failed,
            Canceled
        }

        private string GetJobName(int jobnr)
        {
            //TODO
            return "todo";
        }

        // create job, return id, (or -1 if failed)
        public int NewJob()
        {
            //TODO
            return 0;
        }

        public void AddTask(int jobid, int taskindex, string workdir, string startscript)
        {
            //TODO
        }

        public void SubmitJob(int jobid)
        {
            //TODO
        }

        public void GetTasks(int jobid, 
                                List<int> taskindex, 
                                List<Taskstate> taskstate)
        {
            //TODO            
        }


        
    }
}
