using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>
    /// A base class for attributes that define a test type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public abstract class TestTypeAttributeBase : Attribute, ITestTypeAttribute
    {

        protected abstract XElement CreateTestTypeXml(MethodInfo testMethod);

        XElement ITestTypeAttribute.GetTestTypeXml(MethodInfo testMethod)
        {
            return CreateTestTypeXml(testMethod);
        }

    }
}