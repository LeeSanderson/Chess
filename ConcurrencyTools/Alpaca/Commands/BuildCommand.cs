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
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using System.Threading.Tasks;
using Microsoft.Concurrency.TestTools.Execution.AppTasks;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    /// <summary>Represents a run command specifically for builds.</summary>
    internal class BuildCommand : SpawnAppTaskCommand<BuildAppTaskBase>
    {

        IBuildableEntity _build;
        bool _rebuild;

        internal BuildCommand(IBuildableEntity build, bool rebuild, bool interactive)
            : base(interactive)
        {
            _build = build;
            _rebuild = rebuild;
        }

        protected override BuildAppTaskBase CreateTask()
        {
            var task = _build.CreateBuildTask();
            if (_build.SupportsRebuild)
            {
                // If the entity supports it, then the task it created should also support it
                task.IsRebuild = _rebuild;
            }

            return task;
        }

    }

}
