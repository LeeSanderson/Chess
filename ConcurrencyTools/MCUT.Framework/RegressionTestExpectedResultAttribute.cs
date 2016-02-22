using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>
    /// Specifies the expected regression test result.
    /// </summary>
    /// <remarks>
    /// This is a special expected result test attribute because it applies to all tests defined on a method.
    /// This assertion applies after a test case has run and it's accociated expected result specification
    /// has been asserted. It allows for performing regression tests on other regression test features or tests
    /// that don't have specific expected result capability.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RegressionTestExpectedResultAttribute : Attribute
    {

        /// <summary>Marks a method as a data race test method.</summary>
        /// <param name="expResultTypes">The valid results expected from this test. The test will pass when any of the specified result types occur.</param>
        public RegressionTestExpectedResultAttribute(params TestResultType[] expResultTypes)
        {
            ExpectedResultTypes = expResultTypes;
        }

        /// <summary>The result expected for this regression test.</summary>
        public TestResultType[] ExpectedResultTypes { get; private set; }

    }
}