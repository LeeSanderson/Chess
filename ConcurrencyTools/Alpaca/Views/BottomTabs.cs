/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.AppTasks;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    public partial class BottomTabs : UserControl
    {
        public BottomTabs()
        {
            InitializeComponent();
            stdOutputStreamBox.Title = "Standard Out";
            stdErrorStreamBox.Title = "Standard Error";
            tabcontrol1.SelectedTab = tabpgTaskresults;
        }

        private Model model;
        internal void Init(Model model)
        {
            this.model = model;
            resultList1.Init(model);
            model.EntityChanged += new ModelEntityEventHandler<EntityChangeEventArgs>(model_EntityChanged);
            model.SelectionUpdated += new ModelEventHandler<Selection.State, Selection.State>(model_SelectionUpdateEvt);

            timer1.Enabled = true;
        }

        private TaskRunEntity current_run;

        void model_EntityChanged(EntityBase entity, EntityChangeEventArgs e)
        {
            var run = entity as TestRunEntity;
            if (run == null)
                return;

            if (e.EntityChange == EntityChange.Modified && run == current_run)
                RefreshRun();
        }

        void model_SelectionUpdateEvt(Selection.State previous, Selection.State current)
        {
            if (current.run != previous.run)
                ChangeRun(current.run);
            if (current.SelectedEntity != previous.SelectedEntity)
                ChangeInvocation(current.SelectedEntity);
        }

        void ChangeRun(TaskRunEntity run)
        {
            string outfile = null;
            string errfile = null;
            if (run != null)
            {
                var scriptTask = run.Task as AppScriptProcessTask;
                if (scriptTask != null)
                {
                    outfile = scriptTask.OutputFilePath;
                    errfile = scriptTask.ErrorFilePath;
                }
            }

            stdOutputStreamBox.FilePath = outfile;
            stdErrorStreamBox.FilePath = errfile;
            if (tabcontrol1.SelectedTab == tabpgTaskresults)
                RefreshRun();
            current_run = run;
            BindErrorTab();
        }

        void BindErrorTab()
        {
            txtTestRunErrors.Clear();

            var testRun = current_run as TestRunEntity;
            if (testRun != null && testRun.HasResult)
            {
                if (testRun.Result.Error != null)
                {
                    var exError = testRun.Result.Error as ExceptionErrorEntity;
                    if (exError != null)
                        AppendExErrorToText(exError);
                    else
                        AppendErrorToText(testRun.Result.Error);
                }
                else if (testRun.Result.ResultType == UnitTesting.TestResultType.Error)
                    txtTestRunErrors.AppendText(testRun.Result.Message);
            }

            // Determine if we should display the tab
            if (txtTestRunErrors.Text.Length == 0)
            {
                if (tabcontrol1.TabPages.Contains(tabpgRunError))
                {
                    if (tabcontrol1.SelectedTab == tabpgRunError)
                        tabcontrol1.SelectedTab = tabpgTaskresults;
                    tabcontrol1.TabPages.Remove(tabpgRunError);
                }
                return;
            }
            else
            {
                // Workaround: While Windows uses \n as carriage return chars, the TextBox control requires the linux style: \r\n
                txtTestRunErrors.Text = System.Text.RegularExpressions.Regex.Replace(txtTestRunErrors.Text, "(\r\n)|((?=[^\r])\n)|(\r(?=[^\n]))", "\r\n");

                if (!tabcontrol1.TabPages.Contains(tabpgRunError))
                    tabcontrol1.TabPages.Add(tabpgRunError);
            }
        }

        private void AppendExErrorToText(ExceptionErrorEntity exError)
        {
            txtTestRunErrors.AppendText(String.Format("An exception of type {0} was thrown: {1}\nStack Trace:\n{2}\n"
                , exError.ExceptionTypeName, exError.Message, exError.StackTrace));

            if (exError.HasInnerErrors)
            {
                txtTestRunErrors.AppendText("\nInner Exceptions:\n");
                foreach (var innerEx in exError.EntitiesOfType<ExceptionErrorEntity>())
                {
                    AppendExErrorToText(innerEx);
                }
                txtTestRunErrors.AppendText(">>> End Inner Exceptions\n");
            }
        }

        private void AppendErrorToText(ErrorEntity error)
        {
            txtTestRunErrors.AppendText(String.Format("Error: {0}\n", error.Message));

            if (error.HasInnerErrors)
            {
                txtTestRunErrors.AppendText("\nInner Errors:\n");
                foreach (var innerError in error.EntitiesOfType<ErrorEntity>())
                {
                    AppendErrorToText(innerError);
                }
                txtTestRunErrors.AppendText(">>> End Inner Errors\n");
            }
        }

        void ChangeInvocation(EntityBase entity)
        {
            if (entity == null)
                invocationdetails.Text = String.Empty;
            else
                invocationdetails.Text = entity.GetInvocationDetails() ?? String.Empty;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!model.controller.closing)
            {
                if (tabcontrol1.SelectedTab == tabpgTaskresults
                    && current_run != null
                    && !current_run.Task.IsComplete
                    )
                {
                    RefreshRun();
                }
            }
        }

        private void taskresults_Enter(object sender, EventArgs e)
        {
            RefreshRun();
        }

        private void RefreshRun()
        {
            BindErrorTab();
            stdOutputStreamBox.RefreshFile();
            stdErrorStreamBox.RefreshFile();
        }
    }
}
