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
using System.IO;
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using Microsoft.Concurrency.TestTools.Alpaca.Actions;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Alpaca.AActions;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    internal static class PopupMenu
    {
        internal static void Init(Model model)
        {
            PopupMenu.model = model;
        }

        private static Model model;

        internal static void ShowContextMenu(ContextMenuStrip strip, EntityBase entity)
        {
            strip.Items.Clear();
            BuildContextMenu(strip, entity);
            if (strip.Items.Count > 0)
            {
                strip.Show(Control.MousePosition.X, Control.MousePosition.Y);
            }
        }

        /// <summary>
        /// Adds a separator only if the previous item is NOT a separator.
        /// </summary>
        /// <param name="items"></param>
        internal static void AddSeparatorSmartly(ToolStripItemCollection items)
        {
            int numitems = items.Count;
            if (numitems != 0 && !(items[numitems - 1] is ToolStripSeparator))
                items.Add("-"); // The "-" in this overload will internally create a ToolStripSeparator instance.
        }

        internal static void BuildContextMenu(ContextMenuStrip strip, EntityBase entity)
        {
            AActionContext acontext = new AActionContext(entity);
            AddActions(strip.Items, acontext.Actions);
            AddSeparatorSmartly(strip.Items);



            //System.Diagnostics.Debug.WriteLine(entity.GetType(), "PopupMenu.BuildContextMenu");
            string itemTypeName = entity.MenuItemTypeName();

            var runEntity = entity as TaskRunEntity;
            var testProj = entity as TestProjectEntity;

            // Clear tasks
            if (entity.CanDeleteTasks())
            {
                strip.Items.Add(new ThunkMenuItem("Clear Tasks", () => {
                    model.controller.AddNewCommand(new DeleteCommand(entity, true, true, false));
                }));
            }
            // observation file
            if (entity.DataElement.Attribute(XNames.AObservationFile) != null)
            {
                string obsFile = entity.DataElement.Attribute(XNames.AObservationFile).Value;

                AddSeparatorSmartly(strip.Items);
                strip.Items.Add(new ThunkMenuItem("Open Observation File", () => {
                    model.controller.AddNewCommand(BrowseCommand.OpenNotepad(obsFile));
                }));
            }
            // Delete generated observation files
            if (entity.CanDeleteObservationFiles())
            {
                strip.Items.Add(new ThunkMenuItem("Delete Observation Files", () => {
                    model.controller.AddNewCommand(new DeleteObservationFilesCommand(entity));
                }) {
                    Enabled = entity.HasObservationFilesToDelete()
                });
            }

            // add test/group specific section
            AddSeparatorSmartly(strip.Items);

            // Delete self
            if (entity.IsDeleteable())
            {
                strip.Items.Add(new ThunkMenuItem("Delete " + itemTypeName, () => {
                    model.controller.AddNewCommand(new DeleteCommand(entity, entity.DeletesTasksOnDelete(), true, false));
                }));
            }

        }

        private static DropDownMenuItem CreateMenuItem_RefreshTestContainer(IHasTestContainerSourceFile hsf)
        {
            var entity = (EntityBase)hsf;
            return new ThunkMenuItem("Refresh " + entity.MenuItemTypeName(), () => {
                model.controller.AddNewCommand(new RefreshCommand(entity, true, false));
            });
        }

        //private static DropDownMenuItem CreateMenuItem_OpenSourceFileLocation(IHasTestContainerSourceFile hsf)
        //{
        //    var entity = (EntityBase)hsf;
        //    return new ThunkMenuItem(String.Format("Open {0} Location", entity.MenuItemTypeName()), () => {
        //        model.controller.AddNewCommand(BrowseCommand.OpenExplorer(hsf.SourceFilePath, true));
        //    });
        //}

        //private static DropDownMenuItem CreateMenuItem_OpenDirectory(string folderPath)
        //{
        //    return new ThunkMenuItem("Open Directory", () => {
        //        model.controller.AddNewCommand(BrowseCommand.OpenExplorer(folderPath, false));
        //    });
        //}

        //private static DropDownMenuItem CreateMenuItem_ShowXmlInConsole(XElement el)
        //{
        //    return new ThunkMenuItem("Show XML In Console", () => {
        //        System.Console.WriteLine(el.ToString());
        //    });
        //}

        //private static DropDownMenuItem CreateMenuItem_OpenCmdShell(string workingDir)
        //{
        //    return new ThunkMenuItem("Open Command Shell", () => {
        //        model.controller.AddNewCommand(BrowseCommand.OpenCmdShell(workingDir));
        //    });
        //}

        private static void AddActions(ToolStripItemCollection stripItems, IEnumerable<AAction> actions)
        {
            if (actions == null)
                return;

            foreach (var action in actions)
            {
                if (!action.Applicable)
                    continue;

                if (action is SeparatorFauxAction)
                    AddSeparatorSmartly(stripItems);
                else
                {
                    var menuItem = new AActionMenuItem(action);
                    AddActions(menuItem.DropDownItems, action.Children);
                    stripItems.Add(menuItem);
                }
            }
        }

    }

    class DropDownMenuItem : ToolStripMenuItem
    {

        /// <summary>Gets the <see cref="ContextMenuStrip"/> that this item is part of.</summary>
        public ToolStripDropDown DropDownMenu { get { return this.Parent as ToolStripDropDown; } }

        public virtual bool IsActionable { get { return true; } }

        protected sealed override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (!IsActionable)
                return;

            if (DropDownMenu != null)
                DropDownMenu.Close(ToolStripDropDownCloseReason.ItemClicked);

            try
            {
                OnDoAction();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex, "Error running action - " + this.Text);
                MessageBox.Show("Error running action:" + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected virtual void OnDoAction() { }

    }

    class ThunkMenuItem : DropDownMenuItem
    {

        internal ThunkMenuItem(string text, System.Action action)
        {
            this.Text = text;
            this.action = action;
        }

        System.Action action;

        public override bool IsActionable { get { return action != null; } }

        protected override void OnDoAction()
        {
            this.action();
        }

    }

}
