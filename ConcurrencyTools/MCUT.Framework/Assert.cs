using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    // NOTE: The AreEqual(IEnumerable) overloads were removed. Instead, create a special class mirroring the VSTesting Fwk's  CollectionAssert.


    /// <summary>The result of a particular assert.</summary>
    internal enum AssertResult
    {
        Pass,
        Fail,
        Inconclusive
    }

    /// <summary>
    /// Interface that defines the assert policy to use
    /// </summary>
    /// <remarks>The assert policy interface is used to define how the runtime should behave when an
    /// assertion failure occurs. By default, the assertion policy in use it to throw an <see cref="ConcurrencyUnitTestException"/> exception
    /// when a failure occurs. Usually the default behavior is sufficent however for specialized test
    /// environment such as STM a different assert policy (assert behavior) is needed</remarks>
    internal interface IAssertPolicy
    {

        /// <summary>
        /// Set up things to do. For example, create a logger.
        /// </summary>
        void OnRunBegin();

        /// <summary>
        /// Cleanup, disposing objects, etc.
        /// </summary>
        void OnRunEnd();

        /// <summary>
        /// How to handle an assert failure. Depending on your requirements you could
        /// simply throw an exception, or log the failure, etc.
        /// </summary>
        /// <param name="result">The result of the assert.</param>
        /// <param name="text">The text that describes the result.</param>
        void HandleResult(AssertResult result, string text);
    }

    /// <summary>
    /// Facitlity to provide for assertion of Expected and Actual values.
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCode]
    public static class Assert
    {

        #region AssertPolicy classes

        /// <summary>
        /// Defines the default assertion policy that throws an <see cref="ConcurrencyUnitTestException"/> exception on assert failure
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
        private class ThrowingPolicy : IAssertPolicy
        {
            public void OnRunBegin() { }
            public void OnRunEnd() { }

            public void HandleResult(AssertResult result, string message)
            {
                switch (result)
                {
                    case AssertResult.Pass:
                        break;  // Do nothing
                    case AssertResult.Fail:
                        throw new AssertFailedException(message);
                    case AssertResult.Inconclusive:
                        throw new AssertInconclusiveException(message);
                }
            }
        }

        /// <summary>
        /// Class Member that stores the current assertion policy
        /// </summary>
        // / <remarks>Assign a new class that implements the IAssertPolicy to change the default assertion behavior.</remarks>
        internal static readonly IAssertPolicy Policy = new ThrowingPolicy();

        #endregion

        /// <summary>
        /// Method to print the assertion message with all information
        /// </summary>
        /// <param name="didPass">flag to check whether assertion passed or failed</param>
        /// <param name="assertionName">Type of assertion</param>
        /// <param name="message">Message to print</param>
        /// <param name="parameters">args to pass to the String.Format</param>
        /// <remarks>If parameters is null we will not use String.Format. Instead the message is printed
        /// directly</remarks>
        static void HandleResult(bool didPass, string assertionName, string message, params object[] parameters)
        {
            HandleResult(didPass ? AssertResult.Pass : AssertResult.Fail, assertionName, message, parameters);
        }

        /// <summary>
        /// Method to print the assertion message with all information
        /// </summary>
        /// <param name="result">The result of the assertion.</param>
        /// <param name="assertionName">Type of assertion</param>
        /// <param name="message">Message to print</param>
        /// <param name="parameters">args to pass to the String.Format</param>
        /// <remarks>If parameters is null we will not use String.Format. Instead the message is printed
        /// directly</remarks>
        static void HandleResult(AssertResult result, string assertionName, string message, params object[] parameters)
        {
            string text1 = string.Empty;
            if (!string.IsNullOrEmpty(message))
            {
                text1 = (parameters == null) ? message : string.Format(CultureInfo.CurrentCulture, message, parameters);
            }
            text1 = string.Format("{0}|{1}", assertionName, text1);

            Policy.HandleResult(result, text1);
        }

        #region Pass/Fail/Inconclusive

        /// <summary>
        /// Method to print passing of test case
        /// </summary>
        /// <param name="message">Format of the String message to print</param>
        /// <param name="args">Args to pass to String.Format</param>
        public static void Pass(string message = null, params object[] args)
        {
            HandleResult(true, "Assert.Pass", message, args);
        }

        /// <summary>
        /// Method to print Failure of test case
        /// </summary>
        /// <param name="message">Format of the String message to print</param>
        /// <param name="args">Args to pass to String.Format</param>
        public static void Fail(string message = null, params object[] args)
        {
            HandleResult(false, "Assert.Fail", message, args);
        }

        /// <summary>
        /// Indicates that the assertion can not be verified.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Inconclusive(string message = null, params object[] args)
        {
            HandleResult(AssertResult.Inconclusive, "Assert.Inconclusive", message, args);
        }

        #endregion

        #region IsTrue/False()

        /// <summary>
        /// Method to check whether the condition is true
        /// </summary>
        /// <param name="actual">Condition to check</param>
        /// <param name="format">Format of the String message to print</param>
        /// <param name="args">Args to pass to String.Format</param>
        public static void IsTrue(bool actual, string format = null, params object[] args)
        {
            bool result = actual;
            string txt = string.Format(result ? "[PASS] Expected:[True]. {0}" : "[FAIL] Expected:[True]. {0}", format);
            HandleResult(result, "Assert.True", txt, args);
        }

        /// <summary>
        /// Method to check whether the condition is false
        /// </summary>
        /// <param name="actual">Condition to check</param>
        /// <param name="format">Format of the String message to print</param>
        /// <param name="args">Args to pass to String.Format</param>
        public static void IsFalse(bool actual, string format = null, params object[] args)
        {
            bool result = !actual;
            string txt = string.Format(result ? "[PASS] Expected:[False]. {0}" : "[FAIL] Expected:[False]. {0}", format);
            HandleResult(result, "Assert.False", txt, args);
        }

        #endregion

        #region Is[Not]Null()

        /// <summary>
        /// Method to print when actual object is null
        /// </summary>
        /// <param name="actual">Object to test for null</param>
        /// <param name="format">Format of the String message to print</param>
        /// <param name="args">Args to pass to String.Format</param>
        public static void IsNull<T>(T actual, string format = null, params object[] args)
        {
            bool result = actual == null;
            string txt = string.Format(result ? "[PASS] Expected:[null]. {0}" : "[FAIL] Expected:[null]. {0}", format);
            HandleResult(result, "Assert.IsNull", txt, args);
        }

        /// <summary>
        /// Method to print when actual object is not null
        /// </summary>
        /// <param name="actual">Object to test for null</param>
        /// <param name="format">Format of the String message to print</param>
        /// <param name="args">Args to pass to String.Format</param>
        public static void IsNotNull<T>(T actual, string format = null, params object[] args)
        {
            bool result = actual != null;
            string txt = string.Format(result ? "[PASS] Expected:[!null]. {0}" : "[FAIL] Expected:[!null]. {0}", format);
            HandleResult(result, "Assert.IsNotNull", txt, args);
        }

        #endregion

        #region AreEqual()

        /// <summary>
        /// Method to formulate the message to display upon equal operation on two objects
        /// </summary>
        /// <param name="didPass">flag to determine whether to print pass or fail</param>
        /// <param name="message">Message to display</param>
        /// <param name="expected">Expected object</param>
        /// <param name="actual">Actual object</param>
        /// <returns>Returns the message string to print</returns>
        /// <remarks>If the objects are of different type, the return string also prints the type of the objects</remarks>
        static string FormatMsg_AreEqual(bool didPass, string message, object expected, object actual)
        {
            if (actual != null && expected != null && !actual.GetType().Equals(expected.GetType()))
            {
                // The value types are different
                return string.Format(didPass ? "[PASS] Expected:[{1}], Actual:[{3} ({4})]. {0}" : "[FAIL] Expected:[{1} ({2})], Actual:[{3} ({4})]. {0}",
                    message, expected, expected.GetType().FullName, actual, actual.GetType().FullName);
            }
            else
            {
                String actualObjectString = (actual == null) ? "(null)" : actual.ToString();
                String expectedObjectString = (expected == null) ? "(null)" : expected.ToString();

                return string.Format(didPass ? "[PASS] Expected:[{1}]. {0}" : "[FAIL] Expected:[{1}], Actual:[{2}]. {0}", message, expectedObjectString, actualObjectString);
            }
        }

        #region Overloads

        public static void AreEqual(object expected, object actual)
        {
            AreEqual(expected, actual, null);
        }

        public static void AreEqual(string expected, string actual)
        {
            AreEqual(expected, actual, false, null);
        }

        public static void AreEqual(string expected, string actual, bool ignoreCase)
        {
            AreEqual(expected, actual, ignoreCase, null);
        }

        public static void AreEqual(string expected, string actual, string message, params object[] parameters)
        {
            AreEqual(expected, actual, false, message, parameters);
        }

        public static void AreEqual<T>(T expected, T actual)
        {
            AreEqual<T>(expected, actual, null);
        }

        #endregion

        /// <summary>
        /// Method to check if expected equals actual
        /// </summary>
        /// <typeparam name="T">Type of values to compare</typeparam>
        /// <param name="expected">Expected value</param>
        /// <param name="actual">Actual value</param>
        /// <param name="message">Message to display (in String.Format format) if comparison fails</param>
        /// <param name="parameters">The array of objects to be passed to String.Format</param>
        public static void AreEqual<T>(T expected, T actual, string message = null, params object[] parameters)
        {
            bool passed = Object.Equals(expected, actual);
            string assertResult = FormatMsg_AreEqual(passed, message ?? string.Empty, expected, actual);
            HandleResult(passed, "Assert.AreEqual", assertResult, parameters);
        }

        public static void AreEqual(object expected, object actual, string message, params object[] parameters)
        {
            bool passed = Object.Equals(expected, actual);
            string assertResult = FormatMsg_AreEqual(passed, message ?? string.Empty, expected, actual);
            HandleResult(passed, "Assert.AreEqual", assertResult, parameters);
        }

        public static void AreEqual(string expected, string actual, bool ignoreCase, string message, params object[] parameters)
        {
            bool passed = String.Equals(expected, actual, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
            string assertResult = FormatMsg_AreEqual(passed, message ?? string.Empty, expected, actual);
            HandleResult(passed, "Assert.AreEqual", assertResult, parameters);
        }

        #endregion

        #region AreNotEqual()

        /// <summary>
        /// Method to formulate the message to display upon equal operation on two objects
        /// </summary>
        /// <param name="didPass">flag to determine whether to print pass or fail</param>
        /// <param name="message">Message to display</param>
        /// <param name="notExpected">Expected object</param>
        /// <param name="actual">Actual object</param>
        /// <returns>Returns the message string to print</returns>
        /// <remarks>If the objects are of different type, the return string also prints the type of the objects</remarks>
        static string FormatMsg_AreNotEqual(bool didPass, string message, object notExpected, object actual)
        {
            if (actual != null && notExpected != null && !actual.GetType().Equals(notExpected.GetType()))
            {
                // The value types are different
                return string.Format(didPass ? "[PASS] Not Expected:[{1}], Actual:[{3} ({4})]. {0}" : "[FAIL] Not Expected:[{1} ({2})], Actual:[{3} ({4})]. {0}",
                    message, notExpected, notExpected.GetType().FullName, actual, actual.GetType().FullName);
            }
            else
            {
                String actualObjectString = (actual == null) ? "(null)" : actual.ToString();
                String expectedObjectString = (notExpected == null) ? "(null)" : notExpected.ToString();

                return string.Format(didPass ? "[PASS] Not Expected:[{1}]. {0}" : "[FAIL] Not Expected:[{1}], Actual:[{2}]. {0}", message, expectedObjectString, actualObjectString);
            }
        }

        #region Overloads

        public static void AreNotEqual(object notExpected, object actual)
        {
            AreNotEqual(notExpected, actual, null);
        }

        public static void AreNotEqual(string notExpected, string actual)
        {
            AreNotEqual(notExpected, actual, false, null);
        }

        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase)
        {
            AreNotEqual(notExpected, actual, ignoreCase, null);
        }

        public static void AreNotEqual(string notExpected, string actual, string message, params object[] parameters)
        {
            AreNotEqual(notExpected, actual, false, message, parameters);
        }

        public static void AreNotEqual<T>(T notExpected, T actual)
        {
            AreNotEqual<T>(notExpected, actual, null);
        }

        #endregion

        public static void AreNotEqual(object notExpected, object actual, string message, params object[] parameters)
        {
            bool passed = !Object.Equals(notExpected, actual);
            string assertResult = FormatMsg_AreNotEqual(passed, message ?? string.Empty, notExpected, actual);
            HandleResult(passed, "Assert.AreNotEqual", assertResult, parameters);
        }

        /// <summary>
        /// Method to check if expected equals actual
        /// </summary>
        /// <typeparam name="T">Type of values to compare</typeparam>
        /// <param name="expected">Expected value</param>
        /// <param name="actual">Actual value</param>
        /// <param name="message">Message to display (in String.Format format) if comparison fails</param>
        /// <param name="parameters">The array of objects to be passed to String.Format</param>
        public static void AreNotEqual<T>(T expected, T actual, string message = null, params object[] parameters)
        {
            bool passed = !Object.Equals(expected, actual);
            string assertResult = FormatMsg_AreNotEqual(passed, message ?? string.Empty, expected, actual);
            HandleResult(passed, "Assert.AreNotEqual", assertResult, parameters);
        }

        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, string message, params object[] parameters)
        {
            bool passed = !String.Equals(notExpected, actual, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
            string assertResult = FormatMsg_AreNotEqual(passed, message ?? string.Empty, notExpected, actual);
            HandleResult(passed, "Assert.AreNotEqual", assertResult, parameters);
        }

        #endregion

        #region AreSame()

        /// <summary>
        /// Method to formulate the message to display upon two objects not being the same instance.
        /// </summary>
        /// <param name="didPass">flag to determine whether to print pass or fail</param>
        /// <param name="message">Message to display</param>
        /// <param name="expected">Expected object</param>
        /// <param name="actual">Actual object</param>
        /// <returns>Returns the message string to print</returns>
        /// <remarks>If the objects are of different type, the return string also prints the type of the objects</remarks>
        static string FormatMsg_AreSame(bool didPass, string message, object expected, object actual)
        {
            if (actual != null && expected != null && !actual.GetType().Equals(expected.GetType()))
            {
                // The value types are different
                return string.Format(didPass ? "[PASS] Objects are the same instance. {0}" : "[FAIL] Objects are not the same instance. {0}",
                    message, expected, expected.GetType().FullName, actual, actual.GetType().FullName);
            }
            else
            {
                String actualObjectString = (actual == null) ? "(null)" : actual.ToString();
                String expectedObjectString = (expected == null) ? "(null)" : expected.ToString();

                return string.Format(didPass ? "[PASS] Objects are the same instance. {0}" : "[FAIL] Objects are not the same instance. {0}", message, expectedObjectString, actualObjectString);
            }
        }

        #region Overloads

        public static void AreSame(object expected, object actual)
        {
            AreSame(expected, actual, null);
        }

        #endregion

        /// <summary>
        /// Method to check if two objects refer to the same instance.
        /// </summary>
        /// <param name="expected">Expected object</param>
        /// <param name="actual">Actual object</param>
        /// <param name="message">Message to display (in String.Format format) if comparison fails</param>
        /// <param name="parameters">The array of objects to be passed to String.Format</param>
        public static void AreSame(object expected, object actual, string message, params object[] parameters)
        {
            bool passed = Object.ReferenceEquals(expected, actual);
            string assertResult = FormatMsg_AreSame(passed, message ?? string.Empty, expected, actual);
            HandleResult(passed, "Assert.AreSame", assertResult, parameters);
        }

        #endregion

        #region Throws()

        /// <summary>
        /// Asserts that an action throws a certain exception
        /// </summary>
        public static void Throws(Type t, Action action)
        {
            //precondition checks
            if (null == t)
            {
                throw new ArgumentNullException("Argument t cannot be null");
            }

            if (!typeof(Exception).IsAssignableFrom(t))
            {
                throw new ArgumentException("Argument t is not an Exception type");
            }

            //actual execution
            try
            {
                action();
            }
            catch (Exception e)
            {
                if (t.IsAssignableFrom(e.GetType()))
                {
                    HandleResult(AssertResult.Pass, "Assert.Throws", "[PASS] Expected an Exception of type {0}, and got {0}", t);
                }
                else
                {
                    HandleResult(AssertResult.Fail, "Assert.Throws", "[FAIL] Expected an Exception of type {0}, but got {1}", t, e.GetType().ToString());
                }
                return;
            }
            HandleResult(AssertResult.Fail, "Assert.Throws", "[FAIL] Expected an Exception of type {0} but no Exception occurred", t);
        }

        /// <summary>
        /// Asserts that an action throws a certain exception
        /// </summary>
        public static void Throws<T>(Action action) where T : Exception
        {
            Throws<T>(action, (e) => { });
        }

        /// <summary>
        /// Asserts that an action throws a certain exception.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="continuation">The action to perform upon any exception being thrown.</param>
        public static void Throws<T>(Action action, Action<T> continuation) where T : Exception
        {
            Type t = typeof(T);
            try
            {
                action();
                HandleResult(AssertResult.Fail, "Assert.Throws", "[FAIL] Expected an Exception of type {0} but no Exception occurred", t);
            }
            catch (T e)
            {
                HandleResult(AssertResult.Pass, "Assert.Throws", "[PASS] Expected an Exception of type {0}, and got {0}", t);
                continuation(e as T);
            }
            catch (Exception e)
            {
                HandleResult(AssertResult.Fail, "Assert.Throws", "[FAIL] Expected an Exception of type {0}, but got {1}", t, e);
                continuation(e as T);
            }
        }

        #endregion

    } // class

} // namespace
