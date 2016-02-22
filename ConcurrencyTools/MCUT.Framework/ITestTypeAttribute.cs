using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Reflection;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>Makes the attribute as a marker for a test class.</summary>
    public interface ITestTypeAttribute
    {
        XElement GetTestTypeXml(MethodInfo method);
    }
}
