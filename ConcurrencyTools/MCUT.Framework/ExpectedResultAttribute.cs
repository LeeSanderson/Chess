using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>
    /// Indicates the expected result when running a test.
    /// When no key is set, this attribute applies to all test types on a test method
    /// but only for contexts that don't have an ExpectedResultKey set.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class ExpectedResultAttribute : Attribute
    {

        #region Constructors

        public ExpectedResultAttribute(string key, TestResultType result)
        {
            Key = key;
            Result = result;
        }

        public ExpectedResultAttribute(TestResultType result)
        {
            Result = result;
        }

        #endregion

        /// <summary>The programmatic key used to identify this result from a multi-test case scenario.</summary>
        public string Key { get; private set; }

        /// <summary>
        /// The result expected of this test.
        /// </summary>
        public TestResultType Result { get; private set; }

        /// <summary>
        /// The message expected to be reported from mcut.
        /// A default value of null indicates this property is not asserted.
        /// </summary>
        public string Message { get; set; }

    }
}
