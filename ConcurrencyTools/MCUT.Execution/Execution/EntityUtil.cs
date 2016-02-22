using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Utility class for entity classes.
    /// </summary>
    public static class EntityUtil
    {

        #region Find Entity Helpers

        /// <summary>
        /// Creates a Function that, given the equivalent root entity, will find the entity.
        /// </summary>
        /// <param name="rootEntity">
        /// The root entity to start a search from. This should be the same equivalent root
        /// that the search will start from. For example, the session or tests container being
        /// refreshed.
        /// </param>
        /// <param name="entityToFind">The entity to get found. This would be from the current entity tree structure.</param>
        /// <param name="findClosestAncestor">
        /// If true, the function returned will only return either the entity to be being searched
        /// for or the closest matching ancestor to that node, defaulting to the rootEntity.</param>
        /// <returns>A function that when given an entity equivalent to the <paramref name="rootEntity"/> will return an entity to be found.</returns>
        public static Func<EntityBase, EntityBase> CreateFindEquivalentEntityFunc(EntityBase rootEntity, EntityBase entityToFind, bool findClosestAncestor)
        {
            if (entityToFind == null) throw new ArgumentNullException("entityToFind");

            // Start with a function that returns the same entity
            Func<EntityBase, EntityBase> findFunc = new Func<EntityBase, EntityBase>(e => e);

            EntityBase oldAncestor = entityToFind;
            while (oldAncestor != null && oldAncestor != rootEntity)
            {
                var findInAncestor = GetEquivalentEntityFunc(oldAncestor, out oldAncestor);
                if (findInAncestor == null)
                {
                    // No search function could be created to find the exact entity.
                    if (findClosestAncestor)
                        continue;   // Then try finding the closest ancestor.
                    else
                        return null;    // Can't find the exact entity, so stop trying
                }

                var findInDescendant = findFunc; // Needed for the closure on the next line
                findFunc = new Func<EntityBase, EntityBase>(a => {
                    var descendant = findInAncestor(a);

                    // If we're allowed to return the closes ancestor,
                    // then return the ancestor if we couldn't find the descendant.
                    if (descendant == null && findClosestAncestor)
                        return a;

                    return findInDescendant(descendant);
                }); // the newFindFunc should already handle nulls
            }

            return findFunc;
        }

        private static Func<EntityBase, EntityBase> GetEquivalentEntityFunc(EntityBase oldEntity, out EntityBase oldAncestor)
        {
            Type oldEntityType = oldEntity.GetType();

            if (oldEntity is TestListEntity)
            {
                oldAncestor = oldEntity.Parent;
                var oldTestlist = (TestListEntity)oldEntity;
                var oldTestlistSourceFilePath = oldTestlist.SourceFilePath;
                if (String.IsNullOrEmpty(oldTestlistSourceFilePath))
                {
                    // Than we will have to require the name to match
                    var oldTestlistName = oldTestlist.DisplayName;
                    return new Func<EntityBase, TestListEntity>(p => {
                        // Only valid parent for a testlist is another testlist (ignoring sessions)
                        if (!(p is TestListEntity || p is ISessionEntity))
                            return null;

                        return p
                            .EntitiesOfType<TestListEntity>()
                            .Where(tl => String.IsNullOrEmpty(tl.SourceFilePath) && tl.DisplayName.Equals(oldTestlistName, StringComparison.OrdinalIgnoreCase))
                            .SingleOrDefault();
                    });
                }
                else
                {
                    // Just need to match on the sourcefilepath than
                    return new Func<EntityBase, TestListEntity>(p => {
                        // Only valid parent for a testlist is another testlist (ignoring sessions)
                        if (!(p is TestListEntity || p is ISessionEntity))
                            return null;

                        return p
                            .EntitiesOfType<TestListEntity>()
                            .Where(tl => oldTestlistSourceFilePath.Equals(tl.SourceFilePath, StringComparison.OrdinalIgnoreCase))
                            .SingleOrDefault();
                    });
                }
            }

            if (oldEntity is TestProjectEntity)
            {
                oldAncestor = oldEntity.Parent;
                var oldProj = (TestProjectEntity)oldEntity;
                var oldProjSourceFilePath = oldProj.SourceFilePath;
                if (String.IsNullOrEmpty(oldProjSourceFilePath))
                    return null;    // The source path is a required property

                return new Func<EntityBase, TestProjectEntity>(p => {
                    // Only valid parent for a test project is a test list or session
                    if (!(p is TestListEntity || p is ISessionEntity))
                        return null;

                    return p
                        .EntitiesOfType<TestProjectEntity>()
                        .Where(ta => ta.SourceFilePath.Equals(oldProjSourceFilePath, StringComparison.OrdinalIgnoreCase))
                        .SingleOrDefault();
                });
            }

            if (oldEntity is TestAssemblyEntity)
            {
                oldAncestor = oldEntity.Parent;
                var oldAssy = (TestAssemblyEntity)oldEntity;
                var oldAssySourceFilePath = oldAssy.SourceFilePath;
                if (String.IsNullOrEmpty(oldAssySourceFilePath))
                    return null;    // The source path is a required property

                return new Func<EntityBase, TestAssemblyEntity>(p => {
                    // Only valid parent for a test assembly is a test list or test project
                    if (!(p is TestListEntity || p is TestProjectEntity || p is ISessionEntity))
                        return null;

                    return p
                        .EntitiesOfType<TestAssemblyEntity>()
                        .Where(ta => ta.SourceFilePath.Equals(oldAssySourceFilePath, StringComparison.OrdinalIgnoreCase))
                        .SingleOrDefault();
                });
            }

            if (oldEntity is TestClassEntity)
            {
                var oldTestClass = (TestClassEntity)oldEntity;
                oldAncestor = oldTestClass.OwningAssembly;

                var oldTestClassFullName = oldTestClass.ClassFullName;
                return new Func<EntityBase, TestClassEntity>(p => {
                    // Given a test method, try to find the old entity's equivalent
                    var testAssy = p as TestAssemblyEntity;
                    if (testAssy == null) return null;

                    // Just need to match the entity type since only one of each type is permitted
                    return testAssy
                        .EntitiesOfType<TestClassEntity>()
                        .Where(tc => tc.ClassFullName == oldTestClassFullName)
                        .SingleOrDefault();
                });
            }

            if (oldEntity is TestMethodEntity)
            {
                var oldTestMethod = (TestMethodEntity)oldEntity;
                oldAncestor = oldTestMethod.OwningClass;

                var oldTestMethodName = oldTestMethod.MethodName;
                var oldTestMethodDisplayName = oldTestMethod.DisplayName;
                return new Func<EntityBase, TestMethodEntity>(p => {
                    // Given a test method, try to find the old entity's equivalent
                    var testClass = p as TestClassEntity;
                    if (testClass == null) return null;

                    // Just need to match the entity type since only one of each type is permitted
                    return testClass
                        .EntitiesOfType<TestMethodEntity>()
                        // Test the MethodName first because it's faster
                        // the TestMethodEntity.DisplayName also takes in to account method parameters etc.
                        .Where(tm => tm.MethodName == oldTestMethodName && tm.DisplayName == oldTestMethodDisplayName)
                        .SingleOrDefault();
                });
            }

            if (oldEntity is TestEntity)
            {
                //var oldTest = (TestEntity)oldEntity;
                oldAncestor = oldEntity.Parent;

                return new Func<EntityBase, TestEntity>(p => {
                    // Given a test method, try to find the old entity's equivalent
                    var parent = p as TestMethodEntity;
                    if (parent == null) return null;

                    // Just need to match the entity type since only one of each type is permitted
                    return (TestEntity)parent.GetChildEntities()
                        .SingleOrDefault(c => c.GetType() == oldEntityType);
                });
            }

            if (oldEntity is TaskRunEntity)
            {
                oldAncestor = oldEntity.Parent;
                var run = (TaskRunEntity)oldEntity;
                int oldTaskID = run.TaskID;

                return new Func<EntityBase, TaskRunEntity>(p => {
                    if (p == null) return null;

                    // Just need to match the entity type since only one of each type is permitted
                    return p.EntitiesOfType<TaskRunEntity>()
                        .SingleOrDefault(r => r.TaskID == oldTaskID);
                });
            }

            if (oldEntity is TestResultEntity)
            {
                oldAncestor = oldEntity.Parent;
                var oldResult = (TestResultEntity)oldEntity;

                return new Func<EntityBase, TestResultEntity>(p => {
                    var run = p as TestRunEntity;
                    if (run == null) return null;

                    return run.Result;
                });
            }

            //if (oldEntity is Chess.ChessResultEntity)
            //{
            //    oldAncestor = oldEntity.Parent;
            //    var oldChessResult = (Chess.ChessResultEntity)oldEntity;

            //    return new Func<EntityBase, Chess.ChessResultEntity>(p => {
            //        var testResult = p as TestResultEntity;
            //        if (testResult == null) return null;

            //        return testResult.;
            //    });
            //}

            oldAncestor = oldEntity.Parent;
            return null;
        }

        #endregion

        #region Result Type utils

        /// <summary>
        /// Looks at the first char of the label to determine the result type from mchess.
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static MChessResultType ParseMChessResultType(string label)
        {
            switch (label[0])
            {
                case 'N': return MChessResultType.Notification;
                case 'W': return MChessResultType.Warning;
                case 'E': return MChessResultType.Error;
                case 'R': return MChessResultType.Race;
                default: throw new NotImplementedException("The mchess result label is not recognized: " + label[0]);
            }
        }

        public static MChessResultActionType ParseMChessResultActionType(string name)
        {
            switch (name)
            {
                case XChessNames.Actions.View: return MChessResultActionType.View;
                case XChessNames.Actions.Repeat: return MChessResultActionType.Repeat;
                case XChessNames.Actions.Continue: return MChessResultActionType.Continue;
                case XChessNames.Actions.Repro: return MChessResultActionType.Repro;
                case XChessNames.Actions.ReproLastSchedule: return MChessResultActionType.ReproLastSchedule;
                default: throw new NotImplementedException("Action name not recognized: " + name);
            }
        }

        #endregion

        public static IEnumerable<TestResultEntity> FindAllTestResultsIn(EntityBase entity)
        {
            return entity
                .DataElement
                .DescendantsAndSelf(XTestResultNames.TestResult)
                .SelectEntities<TestResultEntity>()
                ;
        }

    }
}
