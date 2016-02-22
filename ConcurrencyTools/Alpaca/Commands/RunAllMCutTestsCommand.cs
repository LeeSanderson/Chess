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
    /// Spawns a test run for each test (including self) under the target entity.
    /// </summary>
    internal class RunAllMCutTestsCommand : Command
    {
        private EntityBase _entity;
        RunMCutTestOptions _runOptions;

        internal RunAllMCutTestsCommand(EntityBase entity, RunMCutTestOptions runOptions, bool interactive)
            : base(interactive)
        {
            _entity = entity;
            _runOptions = runOptions;
        }

        /// <summary>
        /// Optional filter on the <see cref="TestEntity"/>s.
        /// </summary>
        public Func<IEnumerable<TestEntity>, IEnumerable<TestEntity>> FilterTests { get; set; }

        protected override bool PerformExecute(Model model)
        {
            var tests = _entity.TestsAndSelf();

            // Apply any filtering
            if (FilterTests != null)
                tests = FilterTests(tests);

            foreach (var test in tests)
            {
                if (test is MCutTestEntity)
                {
                    var cmd = new RunAllMCutTestCasesCommand((MCutTestEntity)test, MCutTestRunType.RunTest, _runOptions, interactive);
                    model.controller.AddNewCommand(cmd);
                }
                else
                    throw new NotImplementedException("Only MCut tests are supported.");
            }

            return true;
        }

    }

}
