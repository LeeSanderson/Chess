using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Execution
{
    public abstract class EntityBase : PseudoDependencyObject, IEntity
    {

        #region Constructors

        protected EntityBase(XElement el)
        {
            this.DataElement = el;
        }

        #endregion

        #region Properties

        /// <summary>The Model instance in which this entity exists.</summary>
        public IEntityModel Model { get; private set; }

        /// <summary>The source element from which this entity represents.</summary>
        public XElement DataElement { get; private set; }

        /// <summary>Gets the parent entity.</summary>
        public EntityBase Parent { get; set; }

        public virtual string DisplayName { get { return null; } }

        public virtual bool RaisesEntityChangedEvents { get { return true; } }

        #endregion

        public void BindToModel(IEntityModel model)
        {
            this.Model = model;
        }

        #region Children management

        protected abstract IEnumerable<XElement> GetChildEntityElements();

        public IEnumerable<EntityBase> GetChildEntities()
        {
            foreach (var childEl in GetChildEntityElements())
            {
                if (Model.EntityBuilder.IsEntity(childEl.Name))
                {
                    EntityBase childEntity = childEl.Annotation<EntityBase>();

                    // Make sure the child's entity is created here.
                    if (childEntity == null)
                        childEntity = Model.EntityBuilder.CreateEntityAndBindToElement(childEl);
                    if (childEntity.Parent == null)
                        childEntity.Parent = this;

                    yield return childEntity;
                }
            }
        }

        // TODO: Make internal once the usage of this method is moved into this assembly
        public void LoadChildren(bool recursive)
        {
            // Need to actually iterate thru all the entries to make sure
            // we've created them all.
            foreach (var child in GetChildEntities())
            {
                if (recursive)
                    child.LoadChildren(true);
            }

            OnChildrenLoaded();
        }

        protected virtual void OnChildrenLoaded()
        {
        }

        protected TEntity AddEntity<TEntity>(XElement xentity)
            where TEntity : EntityBase
        {
            // Create the entity instance
            var entity = xentity.Annotation<TEntity>();
            if (entity == null)
                entity = (TEntity)Model.EntityBuilder.CreateEntityAndBindToElement(xentity);

            // Make sure the entity doesn't have a parent
            if (entity.Parent != null)
                throw new ArgumentException("The element's existing bound entity should not have a parent set when using this method.", "xentity");

            DataElement.Add(xentity);
            if (entity.Parent == null)  // Because this may already get set during Session.EntityChanged event
                Model.EntityBuilder.SetParentEntity(entity);
            entity.LoadChildren(true);

            return entity;
        }

        protected void AddEntity<TEntity>(TEntity entity)
            where TEntity : EntityBase
        {
            AddEntity<TEntity>(entity.DataElement);
        }

        protected void ReplaceWithEntity(XElement xentity)
        {
            // Create the entity instance
            var entity = xentity.Annotation<EntityBase>();
            if (entity == null)
                entity = Model.EntityBuilder.CreateEntityAndBindToElement(xentity);

            // Make sure the entity doesn't have a parent
            if (entity.Parent != null)
                throw new ArgumentException("The element's existing bound entity should not have a parent set when using this method.", "xentity");

            DataElement.ReplaceWith(xentity);
            if (entity.Parent == null)  // Because this may already get set during Session.EntityChanged event
                Model.EntityBuilder.SetParentEntity(entity);
            entity.LoadChildren(true);
        }

        protected void ReplaceWithEntity(EntityBase entity)
        {
            ReplaceWithEntity(entity.DataElement);
        }

        #endregion

        #region Enity hierarchy navigation

        public IEnumerable<EntityBase> AncestorsAndSelf()
        {
            return new EntityBase[] { this }
                .Union(Ancestors());
        }

        public IEnumerable<EntityBase> Ancestors()
        {
            EntityBase e = Parent;
            while (e != null)
            {
                yield return e;
                e = e.Parent;
            }
        }

        public IEnumerable<EntityBase> DescendantsAndSelf()
        {
            return new EntityBase[] { this }
                .Union(Descendants());
        }

        public IEnumerable<EntityBase> Descendants()
        {
            return GetChildEntities()
                .SelectMany(x => x.DescendantsAndSelf());
        }

        public IEnumerable<T> Descendants<T>()
            //where TEntity : EntityBase
        {
            return GetChildEntities()
                .SelectMany(x => x.DescendantsAndSelf())
                .OfType<T>();
        }

        /// <summary>Gets all the descendant tests contained within this entity (not including self).</summary>
        public virtual IEnumerable<TestEntity> Tests()
        {
            yield break;
        }

        /// <summary>
        /// Gets all the tests under this entity, including self.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TestEntity> TestsAndSelf()
        {
            // We don't know that we're a test, so just return the child tests.
            return Tests();
        }

        /// <summary>Determines whether this entity is a test or contains any tests.</summary>
        public bool IsTestOrHasTests()
        {
            return TestsAndSelf().Any();
        }

        /// <summary>Determines whether this entity is a test or contains any tests.</summary>
        public bool IsTestOrHasTestsOfType<T>() where T : TestEntity
        {
            return TestsAndSelf().OfType<T>().Any();
        }

        protected virtual IEnumerable<XElement> DescendantXRuns()
        {
            yield break;
        }

        /// <summary>Gets the descendant <see cref="TaskRunEntity"/> instances below this entity.</summary>
        public virtual IEnumerable<TaskRunEntity> DescendantRuns()
        {
            return DescendantXRuns()
                .SelectEntities<TaskRunEntity>();
        }

        public virtual IEnumerable<TaskRunEntity> DescendantRunsAndSelf()
        {
            return DescendantRuns();
        }

        #endregion

        /// <summary>
        /// Gets the details to display in the UI pertaining to invoking the entity.
        /// </summary>
        /// <returns></returns>
        public virtual string GetInvocationDetails()
        {
            return null;
        }

        /// <summary>
        /// Utility method for creating a copy of the DataElement associated with this entity.
        /// </summary>
        /// <param name="srcEl"></param>
        /// <param name="removeAllRuns"></param>
        /// <param name="excludeChildrenWithNames"></param>
        /// <returns></returns>
        protected static XElement CopyDataElement(XElement srcEl, bool removeAllRuns, params XName[] excludeChildrenWithNames)
        {
            XElement copy = new XElement(srcEl);

            // Remove children to exclude first
            if (excludeChildrenWithNames != null && excludeChildrenWithNames.Length != 0)
            {
                var superfluous = copy.Elements(excludeChildrenWithNames)
                    .ToList(); // Need to actualize so we don't change the underlying collection when removing them
                foreach (var x in superfluous)
                    x.Remove();
            }

            // Remove all runs if requested
            if (removeAllRuns)
            {
                var runNodes = copy.Descendants()//(XSessionNames.Run)
                    .Where(xel => TaskRunEntity.TaskRunXNames.Contains(xel.Name))
                    .Reverse()  // So we know we won't have to worry about nested Runs
                    .ToArray();
                foreach (var runNode in runNodes)
                    runNode.Remove();
            }

            return copy;
        }

        public virtual void Dispose()
        {
            // Recursively dispose of the child entities
            foreach (var child in GetChildEntities())
                child.Dispose();
        }

        /// <summary>Deletes this entity from the entity tree.</summary>
        public virtual void Delete()
        {
            // Dispose of any resources this element or it's child entities have open first
            Dispose();
            DataElement.Remove();
            Parent = null;
        }

        ///// <summary>
        ///// Determines if this entity is essentially equivalent to another entity as it pertains
        ///// to the merging of an entity hierarchy.
        ///// The base implementation returns false.
        ///// </summary>
        ///// <param name="other"></param>
        ///// <returns></returns>
        //public virtual bool IsMergeEquivalent(EntityBase other)
        //{
        //    return false;
        //}

        public override string ToString()
        {
            return DisplayName;
        }

    }
}
