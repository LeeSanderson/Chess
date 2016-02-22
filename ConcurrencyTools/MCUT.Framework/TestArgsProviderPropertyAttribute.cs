using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>
    /// Identifies a static property from which test method arguments may be retrieved.
    /// The property must be a public and static property with a type of IEnumerable&lt;object&gt;.
    /// Furthermore, this property needs to be able to produce the arguments by only instantiating
    /// the test class and calling the property's get value method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestArgsProviderPropertyAttribute : Attribute, ITestMethodArgsProvider
    {

        private string propertyName;

        public TestArgsProviderPropertyAttribute(string propertyName)
        {
            this.propertyName = propertyName;
        }

        #region ITestMethodArgsProvider Implementation

        IEnumerable<object[]> ITestMethodArgsProvider.GetArgs(MethodInfo testMethod)
        {
            return Util.GetStaticPropertyValue<IEnumerable<object[]>>(testMethod.ReflectedType, propertyName);
        }

        #endregion

    }
}