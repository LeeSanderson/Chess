using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using System.Drawing;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.Execution.Chess;
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Alpaca.Views
{
    internal static class EntityViewUtil
    {

        public static XTreeNodeVisibility TreeNodeVisibility(this EntityBase entity)
        {
            if (entity is PlaceholderEntity)
                return XTreeNodeVisibility.Normal;
            if (entity is SessionEntity)
                return XTreeNodeVisibility.Transparent;
            if (entity is BuildEntity)
                return XTreeNodeVisibility.Transparent;

            if (entity is TestAssemblyEntity)
            {
                // If a test assembly is part of a project, then we don't want to display its node.
                if (((TestAssemblyEntity)entity).OwningProject != null)
                    return XTreeNodeVisibility.Transparent;
            }

            if (entity is TestGroupingEntity)
                return XTreeNodeVisibility.Normal;

            if (entity is TestEntity)
            {
                //return entity.Parent is TestMethodEntity ? XTreeNodeVisibility.Transparent : XTreeNodeVisibility.Normal;
                return XTreeNodeVisibility.Normal;
            }
            if (entity is TestResultEntity)
                return XTreeNodeVisibility.Transparent;
            if (entity is ChessResultEntity)
                return XTreeNodeVisibility.Transparent;

            if (entity is TaskRunEntity)
            {
                //if (entity is PluginEngineEntity)
                //{
                //    throw new NotImplementedException("Running plugin entities is not currently implemented.");
                //    // Looks like some plugins build, while others run. Need to see when this is the case so we can implement this property.
                //    //// This is the same as the TaskRunEntity
                //    //bool isBuildEntity = Model.runs.GetRun(DataElement).IsBuild();
                //    //return isBuildEntity ? XTreeNodeVisibility.Invisible : XTreeNodeVisibility.Normal;
                //}

                if (entity is BuildRunEntity)
                    return XTreeNodeVisibility.Invisible;

                return XTreeNodeVisibility.Normal;
            }

            return XTreeNodeVisibility.Invisible;
        }

        public static Color TreeNodeForeColor(this EntityBase entity)
        {
            if (entity is TestEntity || entity is ITestSource)
                return Color.DarkBlue;
            if (entity is TestGroupingEntity)
                return Color.DarkMagenta;
            if (entity is ChessResultEntity)
                return Color.DarkRed;

            //if (entity is PluginEngineEntity)
            //    return Color.Brown;

            if (entity is PlaceholderEntity)
                return Color.DarkGreen;

            if (entity is TestRunEntity && ((TestRunEntity)entity).HasTestSourceChanged)
                return Color.Gray;

            return Color.Black;
        }

        /// <summary>
        /// Gets the entity up the Parent tree that is visible in the tree.
        /// This entity's visibility is also included.
        /// </summary>
        /// <returns></returns>
        public static EntityBase GetVisibleAncestor(this EntityBase entity)
        {
            while (entity != null && entity.TreeNodeVisibility() != XTreeNodeVisibility.Normal)
                entity = entity.Parent;

            return entity;
        }

        #region Result Type utils

        /// <summary>Gets the color to use when displaying the specified type of results.</summary>
        /// <returns></returns>
        internal static Color GetDisplayColor(this MChessResultType resultType)
        {
            switch (resultType)
            {
                case MChessResultType.Notification: return Color.DarkGreen;
                case MChessResultType.Warning: return Color.DarkBlue;
                case MChessResultType.Error: return Color.Red;
                case MChessResultType.Race: return Color.FromArgb(192, 0, 0);
                default: throw new NotImplementedException("The mchess result label is not recognized: " + resultType);
            }
        }

        /// <summary>Gets the color to use when displaying the specified type of results.</summary>
        /// <returns></returns>
        internal static Color GetDisplayColor(this TestResultType resultType)
        {
            switch (resultType)
            {
                case TestResultType.Passed:
                    return Color.DarkGreen;
                case TestResultType.Error:
                case TestResultType.AssertFailure:
                case TestResultType.ResultAssertFailure:
                case TestResultType.Exception:
                case TestResultType.RegressionAssertFailure:
                    return Color.Red;
                case TestResultType.Inconclusive:
                case TestResultType.ResultInconclusive:
                    return Color.Wheat;
                case TestResultType.DataRace:
                case TestResultType.Deadlock:
                case TestResultType.Livelock:
                    return Color.FromArgb(192, 0, 0);
                default: throw new NotImplementedException("The mchess result label is not recognized: " + resultType);
            }
        }

        #endregion

        /// <summary>Gets all the results in this session instance.</summary>
        /// <returns></returns>
        public static IEnumerable<TestResultEntity> GetAllTestResults(this SessionEntity session)
        {
            return session.Descendants<TestResultEntity>();
            //.DataElement
            //.Descendants(XSessionNames.MCutTestRun)
            //.Elements(XTestResultNames.TestResult)
            //.SelectEntities<TestResultEntity>()
            //;
        }

        //internal static IEnumerable<ChessResultEntity> GetChessResults(this TestRunEntity source)
        //{
        //    return source.DataElement
        //        .Elements(XTestResultNames.TestResult)
        //        .Elements(XChessNames.ChessResults)
        //        .Elements(XChessNames.Result)
        //        .SelectEntities<ChessResultEntity>()
        //        ;
        //}

        internal static IEnumerable<ChessResultEntity> GetChessResults(this TestResultEntity source)
        {
            return source.DataElement
                .Elements(XChessNames.ChessResults)
                .Elements(XChessNames.Result)
                .SelectEntities<ChessResultEntity>()
                ;
        }

        /// <summary>
        /// Gets the ChessResults that are non-informational only. i.e. not warnings or messages.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static IEnumerable<ChessResultEntity> GetNonInformationalChessResults(this TestResultEntity source)
        {
            return source.DataElement
                .Elements(XChessNames.ChessResults)
                .Elements(XChessNames.Result)
                .SelectEntities<ChessResultEntity>()
                .Where(cr=>cr.ResultType != MChessResultType.Notification && cr.ResultType!= MChessResultType.Warning)
                ;
        }

    }
}
