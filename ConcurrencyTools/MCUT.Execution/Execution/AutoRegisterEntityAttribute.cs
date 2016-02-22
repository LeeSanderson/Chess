using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Declares that this entity should be auto registered with the EntityBuilderBase class.
    /// Using this attribute requires you to declare the following field:<br />
    /// public static readonly XName EntityXName = ...;<br />
    /// As with all entities, it should also have a public constructor that accepts an XElement.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AutoRegisterEntityAttribute : Attribute
    {

        private const string FieldName = "EntityXName";

        internal XName GetEntityXName(Type entityType)
        {
            var entityXNameField = entityType.GetField(FieldName);
            if (entityXNameField == null
                //|| !entityXNameField.IsPublic
                || !entityXNameField.IsStatic
                || entityXNameField.FieldType != typeof(XName)
                || !entityXNameField.IsInitOnly
                )
                throw new Exception(String.Format("The entity {0} must declare the field 'public static readonly XName {1};'.", entityType.Name, FieldName));

            return (XName)entityXNameField.GetValue(null);
        }

    }
}
