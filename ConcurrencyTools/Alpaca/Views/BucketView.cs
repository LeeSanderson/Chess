/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using Microsoft.Concurrency.TestTools.Alpaca.Views;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.Execution;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    internal partial class BucketView : UserControl
    {

        private class TestResultTreeNode : TreeNode
        {
            internal TestResultTreeNode(TestResultEntity result)
            {
                this.result = result;

                // Bind here:
                Text = String.Format("({0}) {1}", result.OwningTestRun.TaskID, result.OwningTestRun.DisplayName);
                //ForeColor = result.ResultType.GetDisplayColor();
                ToolTipText = String.Format("{0}", result.Message);
            }

            internal TestResultEntity result;

        }


        private Model model;

        private Dictionary<TestResultType, TreeNode> _resultTypeNodes;
        static readonly TestResultType[] PreferedDisplayOrder = new[]{
                                                                      TestResultType.Error,
                                                                      TestResultType.Exception,
                                                                      TestResultType.RegressionAssertFailure,
                                                                      TestResultType.ResultAssertFailure,
                                                                      TestResultType.ResultInconclusive,
                                                                      TestResultType.AssertFailure,
                                                                      TestResultType.Inconclusive,
                                                                      TestResultType.DataRace,
                                                                      TestResultType.Deadlock,
                                                                      TestResultType.Livelock,
                                                                      TestResultType.Passed,
                                                                  };

        public BucketView()
        {
            InitializeComponent();

            treeView1.ShowNodeToolTips = true;

            int expEnumValueCount = Enum.GetValues(typeof(TestResultType)).Length;
            _resultTypeNodes = new Dictionary<TestResultType, TreeNode>() {
                {TestResultType.Passed, new TreeNode("Passed")},
                {TestResultType.Error, new TreeNode("Errors")},
                {TestResultType.Exception, new TreeNode("Exceptions")},
                {TestResultType.ResultAssertFailure, new TreeNode("Expected Result Assert Failures")},
                {TestResultType.ResultInconclusive, new TreeNode("Expected Result Inconclusive")},
                {TestResultType.RegressionAssertFailure, new TreeNode("Regression Test Assert Failures")},
                {TestResultType.AssertFailure, new TreeNode("Assert Failures")},
                {TestResultType.Inconclusive, new TreeNode("Inconclusive")},
                {TestResultType.DataRace, new TreeNode("Races")},
                {TestResultType.Deadlock, new TreeNode("Deadlocks")},
                {TestResultType.Livelock, new TreeNode("Livelocks")},
            };
            System.Diagnostics.Debug.Assert(_resultTypeNodes.Count == expEnumValueCount, "Missing some of the values for " + typeof(TestResultType).Name);
            System.Diagnostics.Debug.Assert(PreferedDisplayOrder.Length == expEnumValueCount, "PreferedDisplayOrder doesn't have the same umber of items as the enum " + typeof(TestResultType).Name);

            foreach (var keyItemPair in _resultTypeNodes)
            {
                keyItemPair.Value.ForeColor = keyItemPair.Key.GetDisplayColor();
            }
        }

        internal void Init(Model model)
        {
            this.model = model;

            model.SessionInitialized += new ModelEventHandler(model_SessionInitialized);
            model.EntityChanged += new ModelEntityEventHandler<EntityChangeEventArgs>(model_EntityChanged);

            model.BeginUpdate += new ModelEventHandler(model_BeginUpdate);
            model.EndUpdate += new ModelEventHandler(model_EndUpdate);
            model.SelectionUpdated += new ModelEventHandler<Selection.State, Selection.State>(model_SelectionUpdateEvt);
        }

        void model_SessionInitialized()
        {
            foreach (var result in model.session.Entity.GetAllTestResults())
                AddResultNode(result);
            RebuildTree();
        }

        private void model_BeginUpdate()
        {
            treeView1.BeginUpdate();
        }
        private void model_EndUpdate()
        {
            treeView1.EndUpdate();
        }

        private bool _treeNeedsRebuilding;
        void model_EntityChanged(EntityBase entity, EntityChangeEventArgs e)
        {
            _treeNeedsRebuilding = false;

            if (e.EntityChange == EntityChange.Remove)
            {
                foreach (var result in EntityUtil.FindAllTestResultsIn(entity))
                    RemoveResultNode(result);
            }
            else if (e.EntityChange == EntityChange.Add)
            {
                foreach (var result in EntityUtil.FindAllTestResultsIn(entity))
                    AddResultNode(result);
            }

            if (_treeNeedsRebuilding)
                RebuildTree();
        }

        void model_SelectionUpdateEvt(Selection.State previous, Selection.State current)
        {
            if (current.sender == this)
                return;

            TestResultTreeNode node = null;
            if (current.testResult != null)
            {
                node = current.testResult.DataElement.Annotation<TestResultTreeNode>();
                System.Diagnostics.Debug.Assert(node != null, "The node was selected before there was a test result. How's this possible? We have a race somewhere.");
            }

            suspend_selection_notify = true;
            treeView1.SelectedNode = node;
            if (node != null)
                node.EnsureVisible();
            suspend_selection_notify = false;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (suspend_selection_notify)
                return;

            if (SelectedResultnode != null)
                model.selection.Update(this, SelectedResultnode.result);
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // If not clicking on a result node, then don't do anything special
            var resultNode = e.Node as TestResultTreeNode;
            if (resultNode == null)
                return;

            if (e.Button == MouseButtons.Left)
            {
                // If the result is already selected in the tree
                // but not in the model.selection context, then force
                // it to be selected
                if (resultNode == treeView1.SelectedNode && model.selection.current.SelectedEntity != resultNode.result)
                    model.selection.Update(this, SelectedResultnode.result);
            }
            else if (e.Button == MouseButtons.Right)
            {
                PopupMenu.ShowContextMenu(contextMenuStrip1, resultNode.result);
            }
        }

        private TestResultTreeNode SelectedResultnode { get { return treeView1.SelectedNode as TestResultTreeNode; } }
        private bool suspend_selection_notify = false;

        private void RemoveResultNode(TestResultEntity result)
        {
            TestResultTreeNode node = result.DataElement.Annotation<TestResultTreeNode>();
            if (node != null && node.TreeView != null)
            {
                System.Diagnostics.Debug.Assert(node.TreeView == treeView1);
                _treeNeedsRebuilding = node.Parent.Nodes.Count == 1; // Only need to refresh if by removng us, it'll mean the parent is empty
                node.Remove();
                result.DataElement.RemoveAnnotations<TestResultTreeNode>();
            }
        }

        private void AddResultNode(TestResultEntity result)
        {
            TestResultTreeNode node = result.DataElement.Annotation<TestResultTreeNode>();
            System.Diagnostics.Debug.Assert(node == null);

            node = new TestResultTreeNode(result);
            var resultTypeNode = _resultTypeNodes[result.ResultType];
            result.DataElement.AddAnnotation(node);

            // Allow for sub-grouping if the result type wants it
            TreeNode bucketNode = GetBucketNode(resultTypeNode, result);
            bucketNode.Nodes.Add(node);

            _treeNeedsRebuilding = true;
        }

        private TreeNode GetBucketNode(TreeNode parentNode, TestResultEntity result)
        {
            string bucketName = GetBucketName(result);
            if (bucketName == null)
                return parentNode;

            TreeNode bucketNode = parentNode.Nodes
                .Cast<TreeNode>()
                .Where(n => n.Text == bucketName)
                .FirstOrDefault();
            if (bucketNode == null)
            {
                bucketNode = new TreeNode(bucketName) {
                    ForeColor = parentNode.ForeColor
                };
                parentNode.Nodes.Add(bucketNode);
            }

            return bucketNode;
        }

        private string GetBucketName(TestResultEntity result)
        {
            switch (result.ResultType)
            {
                case TestResultType.Passed:
                case TestResultType.DataRace:
                case TestResultType.Deadlock:
                case TestResultType.Livelock:
                    return null;    // Do not bucket

                case TestResultType.Error:
                case TestResultType.Exception:
                    var exError = result.Error as ExceptionErrorEntity;
                    if (exError != null && exError.IsAggregateException && exError.InnerErrorsCount == 1)
                    {
                        // Get the inner error (should be an exception error)
                        exError = exError.EntityOfType<ExceptionErrorEntity>();
                        if (exError != null)
                            return exError.Message;
                    }

                    // Otherwise, just use the default of the result's message.
                    return result.Message;

                case TestResultType.AssertFailure:
                case TestResultType.ResultAssertFailure:
                    return result.Message;
                case TestResultType.Inconclusive:
                case TestResultType.ResultInconclusive:
                    return result.Message;

                default:
                    throw new NotImplementedException("TestResultType is not implemented: " + result.ResultType);
            }
        }

        private void RebuildTree()
        {
            // Keep the previously selected tree node
            TreeNode prevSelection = treeView1.SelectedNode;
            bool isPrevVisible = prevSelection != null && prevSelection.IsVisible;

            suspend_selection_notify = true;

            // Add them to the tree manually so I don't have to worry about sorting
            treeView1.Nodes.Clear();
            foreach (var resultType in PreferedDisplayOrder)
            {
                TreeNode resultTypeNode = _resultTypeNodes[resultType];

                // First, remove any empty buckets
                for (int i = resultTypeNode.Nodes.Count - 1; i >= 0; i--)
                {
                    TreeNode bucketNode = resultTypeNode.Nodes[i];
                    if (!(bucketNode is TestResultTreeNode) && bucketNode.Nodes.Count == 0)
                        bucketNode.Remove();
                }

                // And if the result node is empty, remove it too
                if (resultTypeNode.Nodes.Count != 0)
                    treeView1.Nodes.Add(resultTypeNode);
            }

            if (prevSelection != null && prevSelection.TreeView == treeView1)
            {
                treeView1.SelectedNode = prevSelection;
                if (isPrevVisible)
                    prevSelection.EnsureVisible();
            }

            suspend_selection_notify = false;
        }

    }
}
