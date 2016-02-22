using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using System.Xml.Linq;
using System.Reflection;
using Microsoft.Concurrency.TestTools.Execution;

namespace Microsoft.Concurrency.TestTools.UnitTesting.MCutConsole
{
    public class UnitTestController : TestTypeControllerBase
    {

        public override ITestCaseRunner CreateRunner()
        {
            return new UnitTestCaseRunner(this);
        }

        internal ManagedTestCase CreateManagedTestCase(TestCaseMetadata runningTestCase)
        {
            XElement xmanagedTestMethod = runningTestCase.TestCase.DataElement.Element(XTestCaseNames.ManagedTestMethod);
            if (xmanagedTestMethod == null)
                throw new Exception("Test case xml missing element " + XTestCaseNames.ManagedTestMethod);

            // Get the test assembly
            Assembly testAssembly = Assembly.LoadFrom((string)xmanagedTestMethod.Attribute("assemblyLocation"));

            // Determine the managed test method
            string testName = (string)xmanagedTestMethod.Attribute("fullClassName") + "." + (string)xmanagedTestMethod.Attribute("methodName");
            int argCount = runningTestCase.TestArgs == null ? 0 : runningTestCase.TestArgs.Values.Length;
            MethodInfo testMethod = UnitTestsUtil.FindUnitTestMethodByName(testAssembly, testName, argCount);

            // And the args
            object[] args = argCount == 0 ? null : UnitTestsUtil.ParseCommandLineArguments(testMethod, runningTestCase.TestArgs.Values);

            ITestContext context = CreateTestContext(runningTestCase);

            return new ManagedTestCase(testMethod, context, args);
        }

        protected virtual ITestContext CreateTestContext(TestCaseMetadata runningTestCase)
        {
            var expResult = runningTestCase.TestCase.ExpectedTestResult;
            return new TestContext(runningTestCase.TestCase.ContextName, expResult == null ? null : expResult.Key);
        }

    }
}
