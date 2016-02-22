using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static class CollectionAssert
    {

        /// <summary>
        /// Asserts that two collections are equal.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="composeElementAssertFailureMessage">When specified, will allow for custom formatting of a message based on the element that failed.</param>
        public static void AreEqual(ICollection expected, ICollection actual, Func<int, object, object, string> composeElementAssertFailureMessage = null)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.IsNotNull(actual);

            int cnt = expected.Count;
            Assert.AreEqual(expected.Count, actual.Count, "actual.Count");

            int i = 0;
            var actualEnumerator = actual.GetEnumerator();
            actualEnumerator.MoveNext();
            foreach (var expItem in expected)
            {
                var actualItem = actualEnumerator.Current;

                AreElementsEqual(i, expItem, actualItem, composeElementAssertFailureMessage);

                i++;
                actualEnumerator.MoveNext();
            }
        }

        static string FormatMsg_AreElementsEqual_Failed(string message, object expected, object actual)
        {
            if (actual != null && expected != null && !actual.GetType().Equals(expected.GetType()))
            {
                // The value types are different
                return string.Format("[FAIL] Expected:[{1} ({2})], Actual:[{3} ({4})]. {0}",
                    message, expected, expected.GetType().FullName, actual, actual.GetType().FullName);
            }
            else
            {
                String actualObjectString = (actual == null) ? "(null)" : actual.ToString();
                String expectedObjectString = (expected == null) ? "(null)" : expected.ToString();

                return string.Format("[FAIL] Expected:[{1}], Actual:[{2}]. {0}", message, expectedObjectString, actualObjectString);
            }
        }

        private static void AreElementsEqual(int idx, object expected, object actual, Func<int, object, object, string> composeElementAssertFailureMessage)
        {
            if (!Object.Equals(expected, actual))   // The same logic from Assert.AreEqual
            {
                // Format the message
                string msg;
                if (composeElementAssertFailureMessage == null)
                    msg = String.Format("elementIdx={0}", idx);
                else
                    msg = composeElementAssertFailureMessage(idx, expected, actual);

                // Lets do it again so we get consistent message formatting
                msg = FormatMsg_AreElementsEqual_Failed(msg ?? string.Empty, expected, actual);
                throw new AssertFailedException(msg);
            }
        }

    }
}
