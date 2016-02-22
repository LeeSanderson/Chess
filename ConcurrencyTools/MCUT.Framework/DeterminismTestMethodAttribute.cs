using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DeterminismTestMethodAttribute : TestTypeAttributeBase
    {

        public DeterminismTestMethodAttribute() { }

        protected override XElement CreateTestTypeXml(MethodInfo testMethod)
        {
            return new XElement(XNames.DeterminismTest);
        }

    }
}