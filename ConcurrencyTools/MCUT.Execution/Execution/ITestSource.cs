using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Defines the interface of an entity that is the source of a test.
    /// </summary>
    public interface ITestSource
    {
        /// <summary>
        /// Indicates whether this test source has parameters.
        /// i.e. Whether args may be passed in to it.
        /// </summary>
        bool HasParameters { get; }

        /// <summary>
        /// Defines all the parameters this test source takes.
        /// If none are defined, returns an empty array.
        /// </summary>
        TestMethodParameter[] Parameters { get; }

        /// <summary>
        /// Gets all the <see cref="TestArgs"/> defined for this instance.
        /// If none are defined, returns an empty array.
        /// </summary>
        TestArgs[] AllArgs { get; }

        /// <summary>
        /// Creates an <see cref="XElement"/> instance defining the test source for insertion
        /// in a test case xml element.
        /// </summary>
        /// <returns></returns>
        XElement ToXTestCaseSource();

        /// <summary>
        /// Gets the <see cref="ExpectedRegressionTestResultEntity"/> decribing the expected result
        /// info for when the test is run in regression mode.
        /// </summary>
        /// <returns></returns>
        ExpectedRegressionTestResultEntity GetExpectedRegressionTestResult();

    }
}
