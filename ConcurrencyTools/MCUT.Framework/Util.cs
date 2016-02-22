using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>
    /// Helper class for this assembly. 
    /// </summary>
    internal static class Util
    {

        /// <summary>
        /// Gets the value of the static property on the specified class.
        /// </summary>
        internal static T GetStaticPropertyValue<T>(Type testClass, string propertyName) where T : class
        {
            PropertyInfo propInfo = testClass.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (propInfo == null)
                throw new InvalidUnitTestConfigurationException(string.Format("Could not find the public static property {0} on {1}.", propertyName, testClass.FullName));

            if (!typeof(T).IsAssignableFrom(propInfo.PropertyType))
                throw new InvalidUnitTestConfigurationException(string.Format("Property {0}.{1} does not return a value that can be assigned to the type {2}", testClass.FullName, propertyName, typeof(T).FullName));

            return (T)propInfo.GetValue(null, null);
        }

    } // class

} // namespace
