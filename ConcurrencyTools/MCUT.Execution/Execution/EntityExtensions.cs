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
    /// Defines extension methods for entity classes.
    /// </summary>
    public static class EntityExtensions
    {

        /// <summary>
        /// Gets the first child entity of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T EntityOfType<T>(this EntityBase entity) where T : EntityBase
        {
            if (entity == null) return null;
            return entity.GetChildEntities().OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets the child entities of the specified type.
        /// </summary>
        public static IEnumerable<T> EntitiesOfType<T>(this EntityBase entity) where T : EntityBase
        {
            if (entity == null) return Enumerable.Empty<T>();
            return entity.GetChildEntities().OfType<T>();
        }

        public static IEnumerable<XElement> SelectDataElements(this IEnumerable<EntityBase> entities)
        {
            if (entities == null)
                return null;
            else
                return entities.Select(e => e.DataElement);
        }

        /// <summary>Gets the entity associated with this element.</summary>
        /// <returns>The entity bound to the element. Otherwise, null.</returns>
        public static EntityBase GetEntity(this XElement x)
        {
            return x.Annotation<EntityBase>();
        }

        /// <summary>
        /// Selects all the <see cref="EntityBase"/> instances from the specified elements, if they exist.
        /// WARNING: This will return null instances for any element w/o an entity annotation.
        /// </summary>
        /// <param name="xelements"></param>
        /// <returns></returns>
        public static IEnumerable<EntityBase> SelectEntities(this IEnumerable<XElement> xelements)
        {
            return xelements.Select(GetEntity);
        }

        /// <summary>
        /// Selects all the entities of type <typeparamref name="TEntity"/> from the specified elements, if they exist.
        /// </summary>
        /// <param name="xelements"></param>
        /// <returns></returns>
        public static IEnumerable<TEntity> SelectEntities<TEntity>(this IEnumerable<XElement> xelements)
            where TEntity : EntityBase
        {
            return xelements
                .Select(x => x.Annotation<EntityBase>())
                //.Where(e => e != null)    // Looks like the OfType already excludes nulls
                .OfType<TEntity>();
        }

        #region Result Type utils

        /// <summary>
        /// Gets the label used for this type of result.
        /// </summary>
        /// <param name="resultType"></param>
        /// <returns></returns>
        public static string ToLabel(this MChessResultType resultType)
        {
            switch (resultType)
            {
                case MChessResultType.Notification: return "N";
                case MChessResultType.Warning: return "W";
                case MChessResultType.Error: return "E";
                case MChessResultType.Race: return "R";
                default: throw new NotImplementedException("The mchess result label is not recognized: " + resultType);
            }
        }

        #endregion

        /// <summary>
        /// Determines whether this entity contains the other entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool Contains(this EntityBase entity, EntityBase other)
        {
            return other != null && Contains(entity, other.DataElement);
        }

        /// <summary>
        /// Determines whether this entity contains is or contains the xelement.
        /// </summary>
        public static bool Contains(this EntityBase entity, XElement other)
        {
            return entity != null && other != null
                && other.AncestorsAndSelf()
                .Contains(entity.DataElement)
                ;
        }

        /// <summary>
        /// WARNING: This navigates the Entity heirarchy which is an intrinsically slow process compared to Linq to Xml
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to return</typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static IEnumerable<TEntity> DescendantsAndSelf<TEntity>(this EntityBase entity)
            where TEntity : EntityBase
        {
            return entity.DescendantsAndSelf().OfType<TEntity>();
        }

        /// <summary>
        /// WARNING: This navigates the Entity heirarchy which is an intrinsically slow process compared to Linq to Xml
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to return</typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<TEntity> DescendantsAndSelf<TEntity>(this IEnumerable<EntityBase> source)
            where TEntity : EntityBase
        {
            return source.SelectMany(e => e.DescendantsAndSelf<TEntity>());
        }

        public static XElement GetChessXActionMatchingStartOf(this TestRunEntity testRun, string actionText)
        {
            return testRun.DataElement
                .Elements(XTestResultNames.TestResult)
                .Elements(XChessNames.ChessResults)
                .Elements(XChessNames.Action)   // We want the actions just under the 'results' element, not from within any 'result'
                .Where(x => actionText.StartsWith((string)x.Attribute(XNames.AName)))
                .SingleOrDefault();
        }

        public static XElement GetChessXActionMatchingStartOf(this Chess.ChessResultEntity chessResult, string actionText)
        {
            return chessResult.DataElement
                .Elements(XChessNames.Action)
                .Where(x => actionText.StartsWith((string)x.Attribute(XNames.AName)))
                .SingleOrDefault();
        }

        /// <summary>
        /// Gets the last chess schedule executed by the run.
        /// </summary>
        /// <param name="testRun"></param>
        /// <returns></returns>
        public static XElement GetLastXSchedule(this TestRunEntity testRun)
        {
            if (testRun == null || testRun.Result == null) return null;
            return testRun.Result.DataElement
                .Elements(XChessNames.ChessResults)
                .Elements(XChessNames.Schedule)
                .LastOrDefault();
        }

        #region Session Properties

        public static bool GetSessionProperty_DetectChanges<T>(this T entity)
            where T : EntityBase
        {
            return (bool?)entity.DataElement.Attribute(XSessionNames.ADetectChanges) ?? true;
        }

        public static void SetSessionProperty_DetectChanges<T>(this T entity, bool value)
            where T : EntityBase
        {
            entity.DataElement.SetAttributeValue(XSessionNames.ADetectChanges, value);
        }

        #endregion

    }
}
