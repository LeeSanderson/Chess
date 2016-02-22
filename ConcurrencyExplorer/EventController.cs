/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Microsoft.ConcurrencyExplorer
{

    internal class EventController : IController
    {
        internal EventController(IModel model)
        {
            this.model = model;
        }

        private IModel model;

        public void Start()
        {
            // this controller does not need a thread!
            // the "life" of this controller is CHESS
        }

        public void Join()
        {
           // need not wait here
        }

        public void NewExecution()
        {
            lock (model)
            {
                model.StartNewExecution();
            }
        }

        public void ProcessTuple(int tid, int nr, int attr, String value)
        {
            lock (model)
            {
                model.ProcessTuple(tid, nr, attr, value);
            }
        }

        public void Complete()
        {
            lock (model)
            {
                model.SetComplete();
            }
        }
    }
}
