using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Chess;

namespace Microsoft.Concurrency.TestTools.Alpaca.Actions
{
    internal static class EntityActionUtil
    {

        public static string MenuItemTypeName(this EntityBase entity)
        {
            // Derived from: TestGroupingEntity
            if (entity is TestGroupingEntity)
            {
                if (entity is TestProjectEntity)
                    return "Test Project";
                if (entity is TestAssemblyEntity)
                    return "Test Assembly";
                if (entity is TestClassEntity)
                    return "Test Class";
                if (entity is TestMethodEntity)
                    return "Test Method";
                if (entity is TestListEntity)
                    return "Test List";
                return "Test Group";
            }

            // Derived from: TestEntity
            if (entity is TestEntity)
            {
                if (entity is MChessTestEntity)
                    return "Chess Test";
                else if (entity is UnitTestEntity)
                    return "Unit Test";
                return "Test";
            }

            if (entity is TaskRunEntity)
            {
                if (entity is BuildRunEntity)
                    return "Build Run";
                if (entity is TestRunEntity)
                {
                    if (entity is MCutTestCaseRunEntity)
                        return "Unit Test Run";
                    return "Test Run";
                }
                return "Task";
            }

            return "item";
        }

        /// <summary>
        /// Determines whether an entity is deletable in the interface.
        /// </summary>
        public static bool IsDeleteable(this EntityBase entity)
        {
            if (entity is TestGroupingEntity)
                return true;
            if (entity is TestEntity)
                return true;
            if (entity is TaskRunEntity)
                return true;

            return false;
        }

        public static bool CanDeleteTasks(this EntityBase entity)
        {
            if (entity is TestGroupingEntity)
                return true;
            if (entity is TestEntity)
                return true;

            return false;
        }

        /// <summary>
        /// Indicates whether the Delete command on this entity only deletes tasks.
        /// </summary>
        public static bool DeletesTasksOnDelete(this EntityBase entity)
        {
            if (entity is TaskRunEntity)
                return true;
            return false;
        }

        public static bool CanDeleteObservationFiles(this EntityBase entity)
        {
            if (entity is TestGroupingEntity)
                return entity.Descendants<ObservationGeneratorEntity>().Any();
            if (entity is ObservationGeneratorEntity)
                return true;

            return false;
        }

        public static bool HasObservationFilesToDelete(this EntityBase entity)
        {
            if (entity is TestGroupingEntity)
                return entity.Descendants<ObservationGeneratorEntity>().Any(g => g.DoObservationFilesExist());
            if (entity is ObservationGeneratorEntity)
                return ((ObservationGeneratorEntity)entity).DoObservationFilesExist();

            return false;
        }

    }
}
