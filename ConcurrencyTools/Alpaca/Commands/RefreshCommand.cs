/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Windows.Forms;
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    /// <summary>
    /// The command to execute when wanting to refresh an entity that has a source file.
    /// </summary>
    internal class RefreshCommand : Command
    {

        private EntityBase _entity;
        private IHasTestContainerSourceFile _hsf;
        private bool _confirmWithUser;

        internal RefreshCommand(EntityBase entity, bool interactive, bool confirmWithUser)
            : base(interactive)
        {
            if (!(entity is IHasTestContainerSourceFile))
                throw new ArgumentException("Entity doesn't implement IHasSourceFile.", "entity");

            _entity = entity;
            _hsf = (IHasTestContainerSourceFile)entity;
            if (!_hsf.SupportsRefresh)
                throw new ArgumentException("The entity doesn't support refreshing.", "entity");

            _confirmWithUser = confirmWithUser;
        }

        internal override bool CheckRedundancy(List<Command> commandqueue)
        {
            if (!_entity.GetSessionProperty_DetectChanges())
                return true;

            return commandqueue
                .OfType<RefreshCommand>()
                .Any(cmd => cmd._entity == this._entity);
        }

        protected override bool PerformExecute(Model model)
        {
            if (!_entity.GetSessionProperty_DetectChanges()) return true;

            // First, ask if we should refresh first
            if (interactive && _confirmWithUser)
            {
                string msg = String.Format(@"The source file ""{0}"" has changed.

Would you like to refresh it?", _hsf.SourceFilePath);
                string caption = "Source File Changed";
                if (MessageBox.Show(msg, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                    return true;

                // In case it got changed while waiting on the user
                if (!_entity.GetSessionProperty_DetectChanges()) return true;
            }

            try
            {
                var loader = new TestContainerLoader(_hsf.SourceFilePath) {
                    Model = model,
                    LoadRecursiveIncludes = true,
                    RegisterWithSession = false,
                };

                if (!loader.Load())
                {
                    SetError("Error: failed to refresh test container.\n" + loader.LoadError.Message);
                    return true;    // true indicates no followups
                }

                var newEntity = loader.TestContainer;
                if (newEntity.DataElement.Name != _entity.DataElement.Name)
                    throw new ArgumentException("The new data element has a different XName than the original entity.");

                // Figure out which entity to select when done
                Func<EntityBase, EntityBase> selectAncestorFunc = null;
                var prevSelectedEntity = model.selection.current.SelectedEntity;
                if (prevSelectedEntity != null && _entity.Contains(prevSelectedEntity))
                {
                    selectAncestorFunc = EntityUtil.CreateFindEquivalentEntityFunc(model.session.Entity, prevSelectedEntity, true);
                    model.selection.Update(null, _entity.Parent);
                }

                // Create a temporary xml element for while merging because we don't want to raise entity changed
                // events for every run added, just one at the very end.
                var ph = PlaceholderEntity.CreatePlaceholder(model, "Loading tests container...");
                _entity.DataElement.ReplaceWith(ph.DataElement);

                MergeSessionState(newEntity, _entity);

                // Need to make a copy so we aren't bound anymore
                ph.DataElement.ReplaceWith(newEntity.DataElement);
                _entity.SetSessionProperty_DetectChanges(false); // Turn this off here to prevent trying to update it again

                // Select the old selected node within the new sub-tree
                if (selectAncestorFunc != null)
                {
                    var entityToSelect = selectAncestorFunc(model.session.Entity);
                    if (entityToSelect != null)
                        //model.controller.AddFollowupCommand(new SelectEntityCommand(this, entityToSelect));
                        model.selection.Update(this, entityToSelect);
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex, "RefreshCommand.Execute threw exception");
                SetError("Failed to refresh:\n" + ex.Message);
            }

            return true;
        }

        private void MergeSessionState(EntityBase newEntity, EntityBase oldEntity)
        {
            // Find entities with any attributes using the session namespace
            var oldEntitiesWithSessionAttributes = oldEntity.DescendantsAndSelf()
                .Where(e => e.DataElement.Attributes()
                    .Where(attr => attr.Name.Namespace == XSessionNames.SessionNS)
                    .Any()
                    );

            // Copy old session attributes to the new entity elements
            foreach (var olde in oldEntitiesWithSessionAttributes)
            {
                EntityBase newe = FindNewEquivalent(newEntity, oldEntity, olde);
                if (newe != null)
                {
                    // Create copies of the old attributes
                    var newAttributes = olde.DataElement.Attributes()
                        .Where(attr => attr.Name.Namespace == XSessionNames.SessionNS)
                        .Select(attr => new XAttribute(attr));
                    newe.DataElement.Add(newAttributes);
                }
            }

            //
            MergeInOldRuns(newEntity, oldEntity);
        }

        private void MergeInOldRuns(EntityBase newEntity, EntityBase oldEntity)
        {
            // First, find all the old runs that aren't descendents of a run
            var oldRuns = oldEntity
                .DescendantRunsAndSelf()
                .Where(r => !r.Ancestors().Any(a => a is TaskRunEntity))
                ;
            if (!oldRuns.Any())
                return;

            // And add the runs for self
            foreach (var oldRun in oldRuns.ToList())
            {
                EntityBase equivalentParent = FindNewEquivalent(newEntity, oldEntity, oldRun.Parent);
                if (equivalentParent != null)
                {
                    oldRun.DataElement.Remove();
                    equivalentParent.DataElement.Add(oldRun.DataElement);
                    oldRun.Parent = equivalentParent;
                }
            }

            // Make sure all test runs are disabled
            foreach (var run in newEntity.DescendantRunsAndSelf().OfType<TestRunEntity>())
            {
                run.HasTestSourceChanged = true;
            }
        }

        private EntityBase FindNewEquivalent(EntityBase newTopEntity, EntityBase oldTopEntity, EntityBase oldEntity)
        {
            if (oldEntity == null)
                return null;

            if (oldEntity == oldTopEntity)
                return newTopEntity;

            var findFunc = EntityUtil.CreateFindEquivalentEntityFunc(oldTopEntity, oldEntity, false);

            return findFunc == null ? null : findFunc(newTopEntity);
        }

    }
}

