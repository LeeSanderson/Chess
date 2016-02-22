using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>Interface needed to mark an Attribute as a provider of unit test contexts.</summary>
    public interface ITestContextsProviderAttribute
    {

        /// <summary>
        /// Gets the default contexts to apply to unit test methods in the specified class
        /// that do not explicitly declare contexts at the method level.
        /// <br/>
        /// This overload is only used when the provider attribute is applied to a class.
        /// </summary>
        /// <param name="testClass">The class which this provider applies to.</param>
        /// <returns>An enumeration of unit test contexts that should be applied to each method.</returns>
        IEnumerable<ITestContext> GetContexts(Type testClass);

        /// <summary>
        /// Gets the contexts to apply to the specified test method.
        /// When used, these contexts will replace any contexts declared at the
        /// test class level.
        /// <br/>
        /// This overload is only used when the provider attribute is applied to a method.
        /// </summary>
        /// <param name="testMethod">The method which this provider applies to.</param>
        /// <returns>An enumeration of unit test contexts that should be applied to the specified method.</returns>
        IEnumerable<ITestContext> GetContexts(MethodInfo testMethod);

    }
}