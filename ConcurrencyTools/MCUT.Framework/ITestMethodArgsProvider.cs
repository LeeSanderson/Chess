using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>Marks an Attribute as a provider of arguments to the test method the attribute is applied to.</summary>
    public interface ITestMethodArgsProvider
    {

        IEnumerable<object[]> GetArgs(MethodInfo testMethod);

    }
}