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
    /// Marks a method as a non-Chess test (i.e. Unless the <see cref="Chess.ChessTestMethodAttribute"/> attribute
    /// is also applied, mchess and Alpaca will not see the method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class UnitTestMethodAttribute : TestTypeAttributeBase
    {

        /// <summary>Marks a method as a non-chess test method.</summary>
        public UnitTestMethodAttribute() { }

        protected override XElement CreateTestTypeXml(MethodInfo testMethod)
        {
            var methodAttrs = testMethod.GetCustomAttributes(true);

            XElement xtest = new XElement(XNames.UnitTest);

            return xtest;
        }

    }
}