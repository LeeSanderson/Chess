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
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution;
using System.Diagnostics;
using Microsoft.Concurrency.TestTools.Execution.AppTasks;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    class BuildAndRunCommand : StagedCommand
    {

        /// <summary>
        /// Build and run all items in the session, using default parameters.
        /// </summary>
        internal BuildAndRunCommand()
            : this(null, false, false)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="buildonly"></param>
        /// <param name="interactive"></param>
        internal BuildAndRunCommand(XElement node, bool buildonly, bool interactive)
            : base(interactive)
        {
            this.node = node;
            this.buildonly = buildonly;
        }

        private XElement node;
        private bool buildonly;

        public bool Rebuild { get; set; }

        protected override IEnumerable<bool> ExecuteStages(Model model)
        {
            // if node was not specified, use entire session
            if (node == null)
            {
                node = model.session.Entity.DataElement;
                System.Diagnostics.Debug.Assert(node != null);
            }

            // Get the entity being acted on
            EntityBase targetEntity = node.GetEntity();

            // determine ancestors that need building
            var preReqBuilds = (from ancestor in targetEntity.AncestorsAndSelf()
                                where ancestor is IBuildableEntity || ancestor is IDefinesBuild
                                let b = (ancestor as IBuildableEntity) ?? ((IDefinesBuild)ancestor).BuildEntity
                                where b != null && b.IsBuildable
                                select b)
                                .Reverse()  // When running the builds, we want to run them from the top-down
                                .ToArray();

            // build each pre requisite in order
            foreach (var build in preReqBuilds)
            {
                System.Diagnostics.Debug.Assert(build != null);
                BuildCommand buildCmd = new BuildCommand(build, Rebuild, interactive);
                //Debug.WriteLine("BuildAndRunCommand spawning BuildCommand: " + build.DisplayName);
                buildCmd.Execute(model);
                yield return true;

                // Next stage: Wait until the build finishes
                while (!buildCmd.Task.IsComplete)
                    yield return true;
                if (buildCmd.Task.Status == AppTaskStatus.Error || buildCmd.Task.ExitCode != 0)
                {
                    SetError("Build failed.");
                    yield break;
                }
            }

            // build at each container level child in parallel
            foreach (bool stageSuccessful in BuildChildren(model, targetEntity))
            {
                if (stageSuccessful)
                    yield return true;
                else
                {
                    SetError("Build failed.");
                    yield break;
                }
            }

            // for each test, launch a task
            if (!buildonly)
            {
                // Lets just wait for any other queued commands to run
                // This adds it to the FollowupCommands. Followup cmds are only executing during
                // a controller.TimerTick.
                yield return true;

                // Need to wait for all the refresh files to happen so we add it to the quiesce list.
                Debug.WriteLine("BuildAndRunCommand Quiescing RunAllTestsCommand");
                model.controller.AddQuiesceCommand(new RunAllMCutTestsCommand(targetEntity, null, interactive));
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetEntity"></param>
        /// <returns>An enumeration where a true value indicates to wait for the next stage and a false indicates to break the whole build.</returns>
        private IEnumerable<bool> BuildChildren(Model model, EntityBase targetEntity)
        {
            var groups = (from g in targetEntity.GetChildEntities().OfType<TestGroupingEntity>()
                          let defBld = g as IDefinesBuild
                          let bld = (g as IBuildableEntity) ?? (defBld == null ? null : defBld.BuildEntity)
                          select new {
                              Entity = g,
                              Build = bld,
                              RunCmd = (bld != null && bld.IsBuildable) ? new BuildCommand(bld, Rebuild, interactive) : null,
                          })
                          .ToList();

            while (groups.Count != 0)
            {
                // traverse as far as possible
                for (int i = 0; i < groups.Count; i++)
                {
                    var grp = groups[i];
                    bool isGroupDone = true;
                    if (grp.RunCmd != null)
                    {
                        // Be sure to start it
                        if (grp.RunCmd.Task == null)
                        {
                            //Debug.WriteLine("BuildAndRunCommand spawning BuildCommand: " + grp.Build.DisplayName);
                            grp.RunCmd.Execute(model);
                        }

                        isGroupDone = grp.RunCmd.Task.IsComplete;
                        if (isGroupDone && grp.RunCmd.Task.ExitCode != 0)
                            yield return false;
                    }

                    if (isGroupDone)
                    {
                        // Remove the group and append it's children onto the end of the list
                        groups.RemoveAt(i--);
                        groups.AddRange(from g in grp.Entity.GetChildEntities().OfType<TestGroupingEntity>()
                                        let defBld = g as IDefinesBuild
                                        let bld = (g as IBuildableEntity) ?? (defBld == null ? null : defBld.BuildEntity)
                                        select new {
                                            Entity = g,
                                            Build = bld,
                                            RunCmd = (bld != null && bld.IsBuildable) ? new BuildCommand(bld, Rebuild, interactive) : null,
                                        });
                    }
                }

                // if we're still waiting on builds, then yield so the ui may be responsive
                yield return true;
            }
        }

    }
}
