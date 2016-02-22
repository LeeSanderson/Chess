using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>
    /// Marks a method as a data race detection method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DataRaceTestMethodAttribute : TestTypeAttributeBase
    {

        /// <summary>Marks a method as a data race test method.</summary>
        public DataRaceTestMethodAttribute() { }

        protected override XElement CreateTestTypeXml(MethodInfo testMethod)
        {
            return new XElement(XNames.DataRaceTest
                );
        }

    }
}