using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Concurrency.TestTools.UnitTesting;

namespace Microsoft.Concurrency.TestTools.Execution
{
    public static class UnitTestsUtil
    {

        /// <summary>
        /// Gets the full name of the specified test method.
        /// i.e. full class name and method name.
        /// </summary>
        public static string GetTestMethodFullName(TestMethodEntity method)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(method.OwningClass.ClassFullName);
            sb.Append('.');
            sb.Append(method.MethodName);
            return sb.ToString();
        }

        /// <summary>Gets the display name for a unit test method</summary>
        /// <param name="method"></param>
        /// <param name="showParams"></param>
        /// <returns></returns>
        public static string GetUnitTestName(MethodInfo method)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(method.Name);
            sb.Append('(');
            sb.Append(GetTestParamsFullDisplayText(method));
            sb.Append(')');

            return sb.ToString();
        }

        /// <summary>Gets the display name for a unit test method</summary>
        /// <param name="method"></param>
        /// <param name="showParams"></param>
        /// <returns></returns>
        public static string GetUnitTestCaseDisplayName(MethodInfo method, object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(method.Name);
            sb.Append('(');
            sb.Append(GetTestArgsDisplayText(method, args, true));
            sb.Append(')');

            return sb.ToString();
        }

        public static string GetUnitTestCaseDisplayName(TestMethodEntity method, TestArgs args)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(method.MethodName);
            sb.Append('(');
            sb.Append(GetTestArgsDisplayText(method, args, true));
            sb.Append(')');

            return sb.ToString();
        }

        /// <summary>
        /// Gets the display text for the parameters of a unit test method.
        /// Format: type name[(, type name)*]
        /// </summary>
        public static string GetTestParamsFullDisplayText(MethodInfo method)
        {
            var methodParams = method.GetParameters();
            if (methodParams.Length == 0)
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < methodParams.Length; i++)
            {
                if (i > 0)
                    sb.Append(", ");

                var param = methodParams[i];

                sb.Append(param.ParameterType.Name);
                sb.Append(' ');
                sb.Append(param.Name);
            }

            return sb.ToString();
        }

        public static string GetTestArgsDisplayText(MethodInfo method, object[] args, bool withParamNames)
        {
            var methodParams = method.GetParameters();
            if (methodParams.Length == 0)
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < methodParams.Length && i < args.Length; i++)
            {
                if (i > 0)
                    sb.Append(", ");

                var param = methodParams[i];
                object arg = args[i];

                if (withParamNames)
                {
                    sb.Append(param.Name);
                    sb.Append('=');
                }
                AppendTestCaseArgument(sb, arg);
            }

            return sb.ToString();
        }

        public static string GetTestArgsDisplayText(TestMethodEntity method, TestArgs args, bool withParamNames)
        {
            var methodParams = method.Parameters;
            if (methodParams == null || methodParams.Length == 0)
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < methodParams.Length && i < args.Values.Length; i++)
            {
                if (i > 0)
                    sb.Append(", ");

                var param = methodParams[i];
                string arg = args.Values[i];

                if (withParamNames)
                {
                    sb.Append(param.Name);
                    sb.Append('=');
                }
                AppendTestCaseArgument(sb, param.TypeName, arg);
            }

            return sb.ToString();
        }

        /// <summary>Appends the displayable string representation of the argument value.</summary>
        /// <param name="sb"></param>
        /// <param name="paramTypeName">The name of the type for the parameter for this arg.</param>
        /// <param name="arg">The argument value (already converted to a string).</param>
        private static void AppendTestCaseArgument(StringBuilder sb, string paramTypeName, string arg)
        {
            if (arg == null || arg == "null")
            {
                sb.Append("null");
                return;
            }

            // Try to get the type
            Type paramType = Type.GetType(paramTypeName, false);


            if (paramType == typeof(char))
            {
                sb.Append('\'');
                sb.Append(arg);
                sb.Append('\'');
            }
            else if (paramType.IsArray)
            {
                // Lets just assume the arg is already formatted for display
                sb.Append(arg);

                //var arr = arg as Array;
                //sb.Append("{");
                //for (int i = 0; i < arr.Length; i++)
                //{
                //    if (i != 0)
                //        sb.Append(',');
                //    sb.Append(arr.GetValue(i));
                //}
                //sb.Append("}");
            }
            else if (paramType.IsAssignableFrom(typeof(string)))
            {
                sb.Append('"');
                sb.Append(arg);
                sb.Append('"');
            }
            else
            {
                sb.Append(arg);
            }
        }

        /// <summary>Appends the displayable string representation of the argument value.</summary>
        /// <param name="sb"></param>
        /// <param name="arg">The argument value.</param>
        private static void AppendTestCaseArgument(StringBuilder sb, object arg)
        {
            if (arg == null)
            {
                sb.Append("null");
                return;
            }

            Type argType = arg.GetType();

            if (argType == typeof(char))
            {
                sb.Append('\'');
                sb.Append(arg);
                sb.Append('\'');
            }
            else if (argType.IsArray)
            {
                var arr = arg as Array;
                sb.Append("{");
                for (int i = 0; i < arr.Length; i++)
                {
                    if (i != 0)
                        sb.Append(',');
                    AppendTestCaseArgument(sb, arr.GetValue(i));
                }
                sb.Append("}");
            }
            else if (argType.IsAssignableFrom(typeof(string)))
            {
                sb.Append('"');
                sb.Append(arg);
                sb.Append('"');
            }
            else
            {
                sb.Append(arg);
            }
        }

        public static ManagedTestCase CreateUnitTestCase(MethodInfo method, ITestContext context, string[] cmdLineArgs)
        {
            object[] args = ParseCommandLineArguments(method, cmdLineArgs);
            return new ManagedTestCase(method, context, args);
        }

        /// <summary>
        /// Find the test method with the specified name and number of arguments.
        /// </summary>
        /// <param name="argCount">The number of arguments being supplied.</param>
        public static MethodInfo FindUnitTestMethodByName(Assembly assembly, string unitTestName, int argCount)
        {
            int idx = unitTestName.LastIndexOf('.');
            if (idx < 1 || idx == unitTestName.Length - 1)
                throw new ArgumentException("Invalid test name: " + unitTestName);

            string typeName = unitTestName.Substring(0, idx);
            string methodName = unitTestName.Substring(idx + 1);

            // Find the class test
            Type testClassType = FindType(assembly, typeName);
            if (testClassType == null)
                throw new Exception(String.Format("The class {0} could not be found, or is ambiguous within the test assembly.", testClassType.Name));
            if (!testClassType.IsClass)
                throw new Exception(String.Format("The test type {0} is not a class.", testClassType.Name));
            if (testClassType.IsNested)
            {
                string fullNestedName = testClassType.FullName;
                int lastIdx = fullNestedName.LastIndexOf('.');
                fullNestedName = lastIdx == -1 ? fullNestedName : fullNestedName.Substring(lastIdx + 1);

                if (!testClassType.IsNestedPublic)
                    throw new Exception(String.Format("The nested test type {0} is not public.", fullNestedName));
            }
            else if (!testClassType.IsPublic)
                throw new Exception(String.Format("The test type {0} is not public.", testClassType.Name));

            // Find the methods that match by name
            var methods = testClassType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name == methodName);
            if (methods.Count() == 0)
                throw new Exception("The unit test method '" + methodName + "' could not be found.");

            // Filter to the ones that can take the number of arguments that are being supplied.
            methods = methods
                .Where(m => {
                    var mparams = m.GetParameters();
                    bool hasParamsAry = mparams.Length > 0 && Attribute.IsDefined(mparams.Last(), typeof(ParamArrayAttribute));
                    if (hasParamsAry)
                        return argCount >= (mparams.Length - 1);
                    else
                        return argCount == mparams.Length;
                });
            int cnt = methods.Count();
            if (cnt == 0)
                throw new Exception(String.Format("The unit test method '{0}' could not be found that accepts {1} arguments.", methodName, argCount));
            if (cnt > 1)
                throw new Exception(String.Format("More than one unit test method with name '{0}' could be found that accepts {1} arguments.", methodName, argCount));

            return methods.Single();
        }

        /// <summary>
        /// Finds a type matching the specified name within the specified assembly.
        /// The type does not have to be public.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="typeName">The name or full name or partial full name.</param>
        /// <returns></returns>
        public static Type FindType(this Assembly assembly, string typeName)
        {
            // Try to get the type using it's name first
            Type typ = assembly.GetType(typeName, false, true);
            if (typ == null)
            {
                // Try to find the type via namespace etc.
                string typeNamespace = "";
                string className = typeName;
                int idx = typeName.LastIndexOf('.');
                if (idx != -1)
                {
                    typeNamespace = typeName.Substring(0, idx);
                    typeName = typeName.Substring(idx + 1);
                }

                typ = assembly.GetTypes()
                    .Where(t => t.Name == className
                        && (typeNamespace.Length == 0
                        || ("." + t.Namespace).EndsWith("." + typeNamespace)   // Trick for matching partial namespaces but full sub-namespaces
                        ))
                    .SingleOrDefault();
            }

            return typ;
        }


        /// <summary>
        /// Takes the given arguments as strings and converts them into the appropriate
        /// type for each parameter specified by the method.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object[] ParseCommandLineArguments(MethodInfo method, IList<string> args)
        {
            ParameterInfo[] methodParams = method.GetParameters();

            //we needed some extra logic if the last argument is a "params" argument.
            //because then the user can pass in as many arguments as he wants.
            bool hasParamsArray = methodParams.Length > 0 && Attribute.IsDefined(methodParams.Last(), typeof(ParamArrayAttribute));
            int minArgsLength = methodParams.Length + (hasParamsArray ? -1 : 0);

            if (args.Count < minArgsLength)
            {
                throw new Exception(string.Format("Argument Mismatch. There are fewer arguments than expected. Expected={0}, Actual={1}, TestMethod='{2}'",
                    minArgsLength, args.Count, method.Name));
            }

            //if the last parameter is not params, it must be exactly minArgsLength
            if (!hasParamsArray && args.Count > minArgsLength)
            {
                throw new Exception(string.Format("Argument Mismatch. There are more arguments than expected. Expected={0}, Actual={1}, TestMethod='{2}'",
                    methodParams.Length, args.Count, method.Name
                ));
            }

            object[] result = new object[methodParams.Length];
            // Convert the non-paramsArray args first
            for (int i = 0; i < minArgsLength; ++i)
                result[i] = ConvertToType(args[i], methodParams[i].ParameterType);

            if (hasParamsArray)
            {
                //determine the type of each element of the params array.
                Type paramsType = methodParams[minArgsLength].ParameterType.GetElementType();

                //create an array of that type.
                Array paramsArray = Array.CreateInstance(paramsType, args.Count - minArgsLength);

                //fill that array
                for (int i = 0; i < paramsArray.Length; i++)
                    paramsArray.SetValue(ConvertToType(args[minArgsLength + i], paramsType), i);

                result[minArgsLength] = paramsArray;
            }

            return result;
        }

        private static object ConvertToType(string text, Type targetType)
        {
            if (string.Compare(text.Trim(), "null", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return null;
            }
            else if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, text);
            }
            else
            {
                return Convert.ChangeType(text, targetType);
            }
        }


    }
}
