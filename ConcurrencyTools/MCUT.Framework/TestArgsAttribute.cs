using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>
    /// Specifies a single set of arguments for a test method.
    /// Apply multiple of these attributes to manually specify different sets of arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestArgsAttribute : Attribute, ITestMethodArgsProvider
    {

        private object[] _data;

        // What's the point of this? If there are no parameters to the method, there's no point in even supplying an empty args.
        //public InlineDataAttribute()
        //{
        //    this.Data = new object[] { };
        //}

        /// <summary>Specifies the arguments for a single test case.</summary>
        /// <param name="data">List of arguments. Be sure the types are correct.</param>
        public TestArgsAttribute(params object[] data)
        {
            _data = data;
        }

        #region ITestMethodArgsProvider Implementation

        IEnumerable<object[]> ITestMethodArgsProvider.GetArgs(MethodInfo testMethod)
        {
            yield return _data;
        }

        #endregion

    }
}