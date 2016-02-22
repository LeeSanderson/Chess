using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Represents a single test case for a CUT.
    /// </summary>
    public class ManagedTestCase
    {

        public ManagedTestCase(MethodInfo testMethod, ITestContext context, object[] arguments)
        {
            if (testMethod == null) throw new ArgumentNullException("testMethod");
            if (arguments == null) arguments = new object[0];

            Method = testMethod;
            Context = context;
            Arguments = arguments;

            var methodParams = Method.GetParameters();
            if (methodParams.Length != arguments.Length)
            {
                throw new ArgumentException(
                    string.Format("Wrong number of arguments. Expected:{0} Actual:{1} for {2}.{3}", methodParams.Length, arguments.Length, Method.ReflectedType.Name, Method.Name)
                );
            }

            var expExAttr = (ExpectedExceptionAttribute)Method.GetCustomAttributes(typeof(ExpectedExceptionAttribute), true).SingleOrDefault();
            if (expExAttr != null)
            {
                ExpectedExceptionType = expExAttr.ExceptionType;
                ExpectedExceptionMessage = expExAttr.ExpectedMessage;
            }
        }

        public MethodInfo Method { get; private set; }
        public ITestContext Context { get; private set; }
        public object[] Arguments { get; private set; }

        public Type ExpectedExceptionType { get; private set; }
        public string ExpectedExceptionMessage { get; private set; }

        public string UnitTestName { get { return UnitTestsUtil.GetUnitTestName(Method); } }
        public string DisplayNameWithArgs { get { return UnitTestsUtil.GetUnitTestCaseDisplayName(Method, Arguments); } }

        public XElement XTestCase { get; set; }


        /// <summary>
        /// Creates a new instance of the class containing the test method
        /// and optionally, sets the context property.
        /// </summary>
        public object CreateNewTestObject()
        {
            ConstructorInfo ctorInfo = Method.ReflectedType.GetConstructor(Type.EmptyTypes);
            if (ctorInfo == null)
            {
                throw new InvalidOperationException(
                    String.Format(
                        "Can't find a public empty .ctor: {0}.{1}",
                        Method.ReflectedType.Name, Method.Name
                    )
                );
            }

            object testObj = ctorInfo.Invoke(null);

            // If the test object declares the context, then lets set it
            if (Context != null)
            {
                Type ctxType = Context.GetType();
                PropertyInfo contextProp = Method.ReflectedType.GetProperty(ctxType.Name, BindingFlags.Public | BindingFlags.Instance);
                if (contextProp != null && contextProp.PropertyType == ctxType)
                    contextProp.SetValue(testObj, Context, null);
            }

            return testObj;
        }

    }
}
