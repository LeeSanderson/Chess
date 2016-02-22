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
using System.Diagnostics;
using Microsoft.Concurrency.TestTools.Alpaca.Views;
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Alpaca.Actions;
using Microsoft.Concurrency.TestTools.Execution;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    internal enum XTreeNodeVisibility { Normal, Transparent, Invisible };

    internal partial class Explorer : UserControl
    {

        static readonly Microsoft.Concurrency.TestTools.Execution.TypedPseudoDependencyProperty<TreeNode> EntityTreeNodeProperty = new Microsoft.Concurrency.TestTools.Execution.TypedPseudoDependencyProperty<TreeNode>("TreeNode", null);

        private Model model;
        private Font buildgroupfont;

        public Explorer()
        {
            InitializeComponent();
            buildgroupfont = new Font(treeView1.Font, FontStyle.Underline);
            treeView1.ShowNodeToolTips = true;
        }

        internal void Init(Model model)
        {
            this.model = model;
            model.SessionInitialized += new ModelEventHandler(model_NewSessionEvt);

            model.EntityChanged += new ModelEntityEventHandler<EntityChangeEventArgs>(model_EntityChanged);

            model.BeginUpdate += new ModelEventHandler(BeginUpdate);
            model.EndUpdate += new ModelEventHandler(EndUpdate);
            model.SelectionUpdated += new ModelEventHandler<Selection.State, Selection.State>(model_SelectionUpdated);
        }


        private void model_NewSessionEvt()
        {
            Clear();
            Create();
        }

        void model_EntityChanged(EntityBase entity, EntityChangeEventArgs e)
        {
            switch (e.EntityChange)
            {
                case EntityChange.Add:
                case EntityChange.Remove:   // ... the parent should already be there from when it was added
                    Debug.Assert(entity.Parent != null, "The parent entity should be there even if the entity has been removed.");
                    UpdateEntity(entity.Parent);
                    break;
                case EntityChange.Modified:
                    UpdateEntity(entity);
                    break;
                default:
                    throw new NotImplementedException("EntityChange is not handled: " + e.EntityChange);
            }
        }


        private void BeginUpdate()
        {
            treeView1.BeginUpdate();
        }
        private void EndUpdate()
        {
            treeView1.EndUpdate();
        }

        private EntityBase GetSelectedEntity()
        {
            XTreeNode treenode = (XTreeNode)treeView1.SelectedNode;
            if (treenode != null)
                return treenode.DataEntity;
            else
                return null;
        }




        private void Clear()
        {
            if (model.xdocument != null)
            {
                foreach (XElement x in model.xdocument.Descendants())
                {
                    XTreeNode treenode = x.Annotation<XTreeNode>();
                    if (treenode != null)
                    {
                        treenode.Remove();
                        x.RemoveAnnotations<XTreeNode>();
                    }
                }
            }
        }

        private void Create()
        {
            AddChildTreeNodes(treeView1.Nodes, model.session.Entity);
        }


        private void AddChildTreeNodes(TreeNodeCollection parentNodes, EntityBase parentEntity)
        {
            Debug.Assert(parentNodes != null);
            Debug.Assert(parentEntity != null);

            foreach (var childEntity in parentEntity.GetChildEntities())
            {
                TreeNodeCollection nodes = parentNodes;
                if (childEntity.TreeNodeVisibility() == XTreeNodeVisibility.Normal)
                {
                    UpdateEntity(childEntity);
                    Debug.Assert(childEntity.GetValue(EntityTreeNodeProperty) != null, "BaseEntity.TreeNode should have been set during the UpdateNode call.");
                    parentNodes.Add(childEntity.GetValue(EntityTreeNodeProperty));
                }
                else if (childEntity.TreeNodeVisibility() == XTreeNodeVisibility.Transparent)
                {
                    // Flatten the entity hierarchy
                    AddChildTreeNodes(parentNodes, childEntity);
                }
            }
        }

        /// <summary>Update the tree node bound to an entity. If no tree node is bound, then a new one is bound.</summary>
        /// <param name="xelement">The entity to be updated.</param>
        private void UpdateEntity(EntityBase entity)
        {
            EntityBase visibleEntity = entity.GetVisibleAncestor();

            if (visibleEntity == null)
            {
                // Then the top would be the session element
                ClearTreeNodes(treeView1.Nodes);
                AddChildTreeNodes(treeView1.Nodes, model.session.Entity);
            }
            else
            {
                TreeNode entityNode = visibleEntity.GetValue(EntityTreeNodeProperty);
                if (entityNode == null)
                {
                    entityNode = new XTreeNode(visibleEntity);
                    visibleEntity.SetValue(EntityTreeNodeProperty, entityNode);
                }
                else
                {
                    Debug.Assert(visibleEntity.DataElement.Annotation<XTreeNode>() == entityNode, "Somehow the element's tree node annotation got out of sync with the entity.");

                    // A tree node instance already exists, we'll need to rebind
                    ClearTreeNodes(entityNode.Nodes);
                }

                BindTreeNode(visibleEntity);
                AddChildTreeNodes(entityNode.Nodes, visibleEntity);
            }
        }

        private void ClearTreeNodes(TreeNodeCollection nodes)
        {
            // TODO fix memory leak, should remove annotations
            nodes.Clear();
        }

        private void BindTreeNode(EntityBase entity)
        {
            var treeNode = entity.GetValue(EntityTreeNodeProperty);

            var run = entity as TaskRunEntity;
            if (run != null)
                treeNode.Text = String.Format("({0}) {1}", run.TaskID, run.DisplayName);
            else
                treeNode.Text = entity.DisplayName ?? entity.MenuItemTypeName();
            treeNode.ForeColor = entity.TreeNodeForeColor();

            // Format entities that define builds
            var entityDefinesBuild = entity as IDefinesBuild;
            if (entityDefinesBuild != null)
                treeNode.NodeFont = entityDefinesBuild.BuildEntity == null ? null : buildgroupfont;
        }

        private bool suspend_selection_notify;

        void model_SelectionUpdated(Selection.State previous, Selection.State current)
        {
            suspend_selection_notify = true;
            if (current.sender != this && current.SelectedEntity != previous.SelectedEntity)
            {
                var visibleEntity = current.SelectedEntity == null ? null : current.SelectedEntity.GetVisibleAncestor();
                if (visibleEntity != null)
                {
                    Debug.Assert(visibleEntity.GetValue(EntityTreeNodeProperty) != null, "TreeNode has not been set.");
                    treeView1.SelectedNode = visibleEntity.GetValue(EntityTreeNodeProperty);
                    if (treeView1.SelectedNode != null)
                        treeView1.SelectedNode.EnsureVisible();
                }
                else
                    treeView1.SelectedNode = null;
            }
            suspend_selection_notify = false;
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                var selection = GetSelectedEntity();
                if (selection != null)
                    model.controller.AddNewCommand(new DeleteCommand(selection, false, true, false));
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (suspend_selection_notify)
                return;

            var selection = GetSelectedEntity();
            if (selection != null)
                model.selection.Update(this, selection);
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            XTreeNode node = (XTreeNode)e.Node;

            if (e.Button == MouseButtons.Left)
            {
                // If the entity is already selected in the tree
                // but not in the model.selection context, then force
                // it to be selected
                if (node == treeView1.SelectedNode && model.selection.current.SelectedEntity != node.DataEntity)
                    model.selection.Update(this, node.DataEntity);
            }
            else if (e.Button == MouseButtons.Right)
            {
                PopupMenu.ShowContextMenu(contextMenuStrip1, node.DataEntity);
            }
        }

    }
}
