using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ExpectedExceptionAttribute : Attribute
    {
        /// <summary>
        /// Indicates an exception of the specified type is expected to be thrown from the test method.
        /// </summary>
        /// <param name="exceptionType">This type should derive from Exception.</param>
        public ExpectedExceptionAttribute(Type exceptionType)
            : this(exceptionType, null)
        {
        }
        /// <summary>
        /// Indicates an exception of the specified type is expected to be thrown from the test method.
        /// </summary>
        /// <param name="exceptionType">This type should derive from Exception.</param>
        /// <param name="noExceptionMessage">The message to display when no exception at all was thrown by the unit test.</param>
        public ExpectedExceptionAttribute(Type exceptionType, string noExceptionMessage)
        {
            if (exceptionType == null)
                throw new ArgumentNullException("exceptionType");
            if (!typeof(Exception).IsAssignableFrom(exceptionType))
                throw new ArgumentException(exceptionType.Name + " does not inherit from Exception.", "exceptionType");

            ExceptionType = exceptionType;
        }

        public Type ExceptionType { get; private set; }
        public string NoExceptionMessage { get; private set; }

        /// <summary>
        /// When specified, not null, indicates the exact Exception.Message that should be on the expected thrown exception.
        /// </summary>
        public string ExpectedMessage { get; set; }

    }


} // namespace
