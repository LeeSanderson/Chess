using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>
    /// Identifies a static property to use to get the <see cref="ITestContext"/> instances to use.
    /// When applied to a class, this becomes the default property to use for all tests in the class.
    /// When applied to a method, this overrides any class-specified contexts.
    /// The property specified should be of the type <see cref="IEnumerable&lt;ITestContext&gt;"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ContextsPropertyAttribute : Attribute, ITestContextsProviderAttribute
    {

        string propertyName;

        /// <summary></summary>
        /// <param name="propertyName">The name of the property. This property should be of the type IEnumerable&lt;ITestCaseContext&gt;.</param>
        public ContextsPropertyAttribute(string propertyName)
        {
            this.propertyName = propertyName;
        }

        #region ITestContextsProvider Implementation

        IEnumerable<ITestContext> ITestContextsProviderAttribute.GetContexts(Type testClass)
        {
            return Util.GetStaticPropertyValue<IEnumerable<ITestContext>>(testClass, propertyName);
        }

        IEnumerable<ITestContext> ITestContextsProviderAttribute.GetContexts(MethodInfo testMethod)
        {
            return Util.GetStaticPropertyValue<IEnumerable<ITestContext>>(testMethod.ReflectedType, propertyName);
        }

        #endregion

    }
}