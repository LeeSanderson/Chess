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
using Microsoft.Concurrency.TestTools.Execution;
using System.IO;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    internal class DeleteCommand : Command
    {
        private List<EntityBase> _sourceEntities;

        private bool _runsonly;
        private bool _resetcounter;

        /// <summary>
        /// </summary>
        /// <param name="entities">Set to null to indicate to remove all entities from the model's session.</param>
        /// <param name="runsonly"></param>
        /// <param name="interactive"></param>
        /// <param name="resetcounter"></param>
        internal DeleteCommand(EntityBase entity, bool runsonly, bool interactive, bool resetcounter)
            : this(new[] { entity }, runsonly, interactive, resetcounter)
        {
            if (entity == null) throw new ArgumentNullException("entity");
        }

        /// <summary>Creates a command that deletes from the session root.</summary>
        /// <param name="runsonly"></param>
        /// <param name="interactive"></param>
        /// <param name="resetcounter"></param>
        internal DeleteCommand(bool runsonly, bool interactive, bool resetcounter)
            : this(new EntityBase[0], runsonly, interactive, resetcounter)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="entities">Set to null to indicate to remove all entities from the model's session.</param>
        /// <param name="runsonly"></param>
        /// <param name="interactive"></param>
        /// <param name="resetcounter"></param>
        internal DeleteCommand(IEnumerable<EntityBase> entities, bool runsonly, bool interactive, bool resetcounter)
            : base(interactive)
        {
            _sourceEntities = entities.ToList();
            _runsonly = runsonly;
            _resetcounter = resetcounter;
        }

        protected override bool PerformExecute(Model model)
        {
            // Figure out which entity to select when done
            Func<EntityBase, EntityBase> selectAncestorFunc = null;
            if (_sourceEntities.Count != 0 && _sourceEntities[0].DataElement.Document != null)
            {
                var srcEntity = _sourceEntities[0];
                selectAncestorFunc = EntityUtil.CreateFindEquivalentEntityFunc(model.session.Entity, srcEntity, true);
            }

            // if nodes are not specified, use all
            if (_sourceEntities.Count == 0)
                _sourceEntities.AddRange(model.session.Entity.GetChildEntities());

            // Since we don't know the order that each of the source entities will be in (i.e. document order)
            // we delete each one separately
            List<string> runsNotDeletedErrors = new List<string>();
            foreach (var srcEntity in _sourceEntities)
            {
                var errors = DeleteRuns(srcEntity);
                if (errors.Count != 0)
                {
                    runsNotDeletedErrors.AddRange(errors);
                    continue;
                }

                //// delete empty engines
                //var emptyengines = srcEntity
                //    .DescendantsAndSelf<PluginEngineEntity>()
                //    .Where(p => p.DataElement.Document != null && !p.DescendantRunsAndSelf().Any())
                //    ;
                //foreach (var pluginEntity in emptyengines)
                //    pluginEntity.Delete();

                // delete nodes that are still in document and have no runs
                if (!_runsonly)
                    if (srcEntity.DataElement.Document != null && !srcEntity.DescendantRunsAndSelf().Any())
                        srcEntity.Delete();

            }

            foreach (var delRunErr in runsNotDeletedErrors)
                SetError(delRunErr);

            // bail out if interactive, and we couldn't delete all nodes
            if (!Successful() && interactive)
                return true;

            if (Successful() && _resetcounter)
            {
                model.session.Entity.RuntimeState.ResetTaskCounter();
                model.tasksController.DeleteAllTaskFolders();
            }

            // Select the parent node of the first entity that was deleted
            if (selectAncestorFunc != null)
            {
                var entityToSelect = selectAncestorFunc(model.session.Entity);
                if (entityToSelect != null)
                    model.selection.Update(null, entityToSelect);
            }

            return Successful(); // if we were not successful we will try again
        }

        private List<string> DeleteRuns(EntityBase srcEntity)
        {
            List<string> runsNotDeletedErrors = new List<string>();
            if (srcEntity.DataElement.Document != null)
            {
                // Get the runs in reverse order so nested runs are deleted first
                var runs = srcEntity.DescendantRunsAndSelf().Reverse();

                foreach (var run in runs)
                {
                    // first, see if there are dependent tasks. This would happen if a child run 
                    // couldn't be deleted from a previous iteration (we traverse in reverse doc order)
                    if (run.DescendantRuns().Any())
                        continue;

                    // if task is not complete, try to cancel it
                    if (!run.Task.IsComplete)
                    {
                        if (!run.Task.Cancel())
                        {
                            runsNotDeletedErrors.Add(String.Format("Cannot delete task {0}, probably because its process is still running.", run.TaskID));
                            continue;
                        }
                    }

                    // finally, delete run, it will take care of deleting its task
                    run.Delete();
                }
            }

            return runsNotDeletedErrors;
        }

        internal override bool CheckRedundancy(List<Command> commands)
        {
            foreach (Command command in commands)
            {
                DeleteCommand other = command as DeleteCommand;
                if (other != null && (other.interactive == this.interactive) && (other._runsonly == this._runsonly))
                {
                    foreach (var entity in _sourceEntities)
                        other._sourceEntities.Add(entity);
                    return true;
                }
            }
            return false;
        }
    }
}
