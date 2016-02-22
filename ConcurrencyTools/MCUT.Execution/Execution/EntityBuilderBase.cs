using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.Execution.Chess;
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// The base implementation for an entity builder.
    /// </summary>
    public abstract class EntityBuilderBase
    {

        private static Dictionary<XName, Func<XElement, EntityBase>> _entityByXName;

        public EntityBuilderBase(IEntityModel model)
        {
            if (model == null) throw new ArgumentNullException();

            Model = model;

            _entityByXName = new Dictionary<XName, Func<XElement, EntityBase>>();

            OnRegisterKnownEntities();
        }

        protected virtual void OnRegisterKnownEntities()
        {
            RegisterXEntityFactory(XConcurrencyNames.Error, x => {
                if (String.IsNullOrEmpty((string)x.Attribute(XConcurrencyNames.AErrorExceptionType)))
                    return new ErrorEntity(x);
                else
                    return new ExceptionErrorEntity(x);
            });

            // Do auto registration
            var executionAssy = System.Reflection.Assembly.GetExecutingAssembly();
            AutoRegisterEntitiesInAssembly(executionAssy);
        }

        public void AutoRegisterEntitiesInAssembly(System.Reflection.Assembly executionAssy)
        {
            var entitiesToAutoRegister = from t in executionAssy.GetTypes()
                                         where Attribute.IsDefined(t, typeof(AutoRegisterEntityAttribute))
                                         let autoRegAttr = (AutoRegisterEntityAttribute)Attribute.GetCustomAttribute(t, typeof(AutoRegisterEntityAttribute))
                                         let ctor = t.GetConstructor(new[] { typeof(XElement) })
                                         select new {
                                             EntityType = t,
                                             XName = autoRegAttr.GetEntityXName(t),
                                             EntityCtor = ctor
                                         };
            foreach (var autoReg in entitiesToAutoRegister)
            {
                if (!typeof(EntityBase).IsAssignableFrom(autoReg.EntityType))
                    throw new Exception(String.Format("The type {0} doesn't inherit from EntityBase.", autoReg.EntityType.Name));
                if (!typeof(EntityBase).IsAssignableFrom(autoReg.EntityType))
                    throw new Exception(String.Format("The entity type {0} must not be abstract to use the AutoRegisterEntityAttribute.", autoReg.EntityType.Name));
                if (autoReg.EntityCtor == null)
                    throw new Exception(String.Format("The entity type {0} doesn't have a public constructor that accepts an XElement.", autoReg.EntityType.Name));

                var ctor = autoReg.EntityCtor;  // Need to make sure this doesn't change for our delegate on the next iteration (i.e. capturing issues)
                RegisterXEntityFactory(autoReg.XName, x => (EntityBase)ctor.Invoke(new[] { x }));
            }
        }

        public void RegisterXEntityFactory(XName xname, Func<XElement, EntityBase> factoryMethod)
        {
            if (_entityByXName.ContainsKey(xname))
                throw new ArgumentException(String.Format("The XName '{0}' is already registered with the entity factory.", xname), "xname");

            _entityByXName.Add(xname, factoryMethod);
        }

        /// <summary>
        /// Removes the registration of the factory method for elements of the specified XName.
        /// </summary>
        /// <param name="xname"></param>
        public void UnregisterXEntityFactory(XName xname)
        {
            _entityByXName.Remove(xname);
        }

        protected IEntityModel Model { get; private set; }

        /// <summary>Determines whether the specified element is associated with a concrete entity.</summary>
        /// <returns>true if the <see cref="XName"/> has a factory method registered for creating an entity for it.</returns>
        public bool IsEntity(XName xname)
        {
            return _entityByXName.ContainsKey(xname);
        }

        /// <summary>
        /// Ensures that the entity for an element is created and bound to the element
        /// and that it's Parent property is set (if one exists).
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <param name="el"></param>
        /// <returns></returns>
        public TEntity EnsureEntityBound<TEntity>(XElement el)
            where TEntity : EntityBase
        {
            var e = el.Annotation<TEntity>();
            if (e == null)
            {
                e = (TEntity)CreateEntityAndBindToElement(el);
                SetParentEntity(e);
            }
            return e;
        }

        /// <summary>
        /// Creates the entity instance for the element and binds the entity
        /// to the element via an XElement Annotation.
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public EntityBase CreateEntityAndBindToElement(XElement el)
        {
            EntityBase entity = CreateEntity(el);
            BindEntityToElement(entity, el);

            return entity;
        }

        private void BindEntityToElement(EntityBase entity, XElement el)
        {
            // Bind the entity to the element's annotation list
            System.Diagnostics.Debug.Assert(el.Annotation<EntityBase>() == null, "The element is already bound to an entity.");
            el.AddAnnotation(entity);
        }

        public void SetParentEntity(EntityBase entity)
        {
            // Find the first ancestor element that also is an entity
            XElement xparent = entity.DataElement.Parent;
            while (xparent != null && !IsEntity(xparent.Name))
                xparent = xparent.Parent;
            if (xparent != null)
            {
                EntityBase parentEntity = xparent.Annotation<EntityBase>();

                //// Make sure the parent's entity exists
                System.Diagnostics.Debug.Assert(parentEntity != null, "The parent entity wasn't created.");

                entity.Parent = parentEntity;
            }
        }

        private EntityBase CreateEntity(XElement el)
        {
            Func<XElement, EntityBase> factory;
            if (!_entityByXName.TryGetValue(el.Name, out factory))
                throw new KeyNotFoundException("The XElement " + el.Name.LocalName + " does not have an entity associated with it.");

            var entity = factory(el);
            entity.BindToModel(Model);
            return entity;
        }

    }
}
