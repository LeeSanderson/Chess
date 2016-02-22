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
using Microsoft.Concurrency.TestTools.Execution.AppTasks;
using Microsoft.Concurrency.TestTools.Alpaca.AActions;
using Microsoft.Concurrency.TestTools.Alpaca.AActions.MChessActions;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.Alpaca
{

    internal class RunMCutTestCaseCommand : SpawnAppTaskCommand<RunMCutTestCaseAppTask>
    {

        private Func<RunMCutTestCaseAppTask> _createTaskFunc;
        private TestRunOptions _runOptions;

        #region Constructors

        internal RunMCutTestCaseCommand(MCutTestEntity test, MCutTestRunType runType, TestContextEntityBase context, TestArgs args, RunMCutTestOptions runOptions, bool interactive)
            : base(interactive)
        {
            _runOptions = runOptions;
            _createTaskFunc = () => {
                var task = (RunMCutTestCaseAppTask)test.CreateRunTestTask(runType);
                task.ParentEntity = test;
                task.TestContext = context;
                task.TestArgs = args;
                return task;
            };
        }

        internal RunMCutTestCaseCommand(MCutTestCaseRunEntity prevTestRun, MCutTestRunType runType, RunMCutTestOptions runOptions, bool interactive)
            : base(interactive)
        //: this(testRun.OwningTest
        //, testRun.DataElement.Element(XTestResultNames.TestResult)
        //, testRun.OwningTest.CreateXTestCase(testRun.TestCase, prototype)
        //, prototype, interactive)
        {
            _runOptions = runOptions;
            _createTaskFunc = () => {
                var task = (RunMCutTestCaseAppTask)prevTestRun.OwningTest.CreateRunTestTask(runType);
                task.ParentEntity = prevTestRun.Result;
                task.PreviousRun = prevTestRun;
                return task;
            };
        }

        #endregion

        public bool AutoOpenConcurrencyExplorer { get; set; }

        public override string DisplayName
        {
            get
            {
                if (Task == null)
                    return String.Format("Run test case.");
                else if (Task.PreviousRun == null)
                    return String.Format("Run Test Case: test={0}, context='{1}', args={{{2}}}"
                        , Task.Test
                        , Task.TestContext
                        , UnitTestsUtil.GetTestArgsDisplayText(Task.Test.OwningTestMethod, Task.TestArgs, false)
                        );
                else
                    return String.Format("Rerun Test Case: {0}", Task.PreviousRun.TestCase);
            }
        }

        protected override RunMCutTestCaseAppTask CreateTask()
        {
            var task = _createTaskFunc();

            if (_runOptions != null && task is IAcceptsRunTestOptions)
                ((IAcceptsRunTestOptions)task).Accept(_runOptions);

            if (AutoOpenConcurrencyExplorer)
                task.ContinueWith(t => {
                    if (t.Status == AppTaskStatus.Complete)
                    {
                        // TODO: Need to test this with deadlocks too
                        var chessResult = Views.EntityViewUtil.GetChessResults(task.Run.Result)
                            .Where(cres => cres.DataElement
                                .Elements(XChessNames.Action)
                                .Any(xa => ((string)xa.Attribute(XNames.AName)).StartsWith("View")))
                            .First();
                        var action = new AViewConcurrencyExplorerAction();
                        action.BindToContext(new AActionContext(chessResult));
                        if (action.Applicable && action.Actionable && action.Enabled)
                            action.Go();
                    }
                });

            return task;
        }

    }

}
