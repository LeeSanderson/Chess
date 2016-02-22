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
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using Microsoft.Concurrency.TestTools.Alpaca.AActions;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    /// <summary>
    /// Spawns <see cref="RunMCutTestCaseCommand"/>s, one for each of the cross product of all
    /// the contexts and testArgs.
    /// </summary>
    internal class RunAllMCutTestCasesCommand : Command
    {
        private MCutTestEntity _test;
        private RunMCutTestOptions _runOptions;
        MCutTestRunType _runType;

        internal RunAllMCutTestCasesCommand(MCutTestEntity test, MCutTestRunType runType, RunMCutTestOptions runOptions, bool interactive)
            : base(interactive)
        {
            _test = test;
            _runOptions = runOptions;
            _runType = runType;
        }

        public override string DisplayName { get { return "Spawn test cases for " + _test.DisplayName; } }

        protected override bool PerformExecute(Model model)
        {
            // TODO: Allow filtering to a specific test case

            var testContexts = _test.GetContexts();
            var allArgs = _test.TestSource.AllArgs;

            bool isFirst = true;
            string error = null;
            foreach (var ctx in testContexts)
            {
                if (_test.TestSource.HasParameters)
                {
                    if (allArgs.Length == 0)
                        error = "No arguments specified for test with parameters.";
                    else
                    {
                        foreach (var args in allArgs)
                        {
                            error = ExecuteTestCase(_test, ctx, args, isFirst);
                            isFirst = false;
                        }
                    }
                }
                else
                {
                    error = ExecuteTestCase(_test, ctx, null, isFirst);
                    isFirst = false;
                }

                // Detect errors in spawning test case runs.
                if (error != null)
                {
                    SetError(error);
                    return true;
                }
            }

            return true;
        }

        private string ExecuteTestCase(MCutTestEntity test, TestContextEntityBase context, TestArgs args, bool selectRun)
        {
            var cmd = new RunMCutTestCaseCommand(test, _runType, context, args, _runOptions, interactive);
            cmd.SelectRun = selectRun;
            System.Diagnostics.Debug.WriteLine("   Executing sub-command: " + cmd.DisplayName);
            cmd.Execute((Model)this._test.Model);
            return cmd.GetError();
        }

    }

}
