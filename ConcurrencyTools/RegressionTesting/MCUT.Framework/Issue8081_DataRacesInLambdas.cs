using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Concurrency.TestTools.UnitTesting.RegressionTests
{
    /// <summary>
    /// This test shows data races in LINQ queries.
    /// Not sure if these are real data races or not.
    /// </summary>
    public class Issue8081_DataRacesInLambdas
    {

        [DataRaceTestMethod]
        public void StringJoin_ForeachLoopCausesNoDataRace()
        {
            var parameters = new[] { new object(), "test", 5 };
            string result1 = null;
            string result2 = null;

            Parallel.Invoke(
            () => {
                result1 = ParametersToTypeListUsingForeachLoop(parameters);
            },

            () => {
                result2 = ParametersToTypeListUsingForeachLoop(parameters);
            }
            );

            Assert.AreEqual("Object, String, Int32", result1);
            Assert.AreEqual("Object, String, Int32", result2);
        }

        private static string ParametersToTypeListUsingForeachLoop(object[] parameters)
        {
            List<string> result = new List<string>();
            foreach (var p in parameters)
            {
                result.Add(p == null ? null : p.GetType().Name);
            }
            return string.Join(", ", result);
        }

        /// <remarks>
        /// Not sure why this is a data race.
        /// </remarks>
        [DataRaceTestMethod]
        public void StringJoin_LambdaCausesDataRace()
        {
            var parameters = new[] { new object(), "test", 5 };
            string result1 = null;
            string result2 = null;

            Parallel.Invoke(
            () => {
                result1 = ParametersToTypeListUsingLambda(parameters);
            },

            () => {
                result2 = ParametersToTypeListUsingLambda(parameters);
            }
            );

            Assert.AreEqual("Object, String, Int32", result1);
            Assert.AreEqual("Object, String, Int32", result2);
        }

        private static string ParametersToTypeListUsingLambda(object[] parameters)
        {
            return string.Join(", ", parameters.Select(p => p == null ? null : p.GetType().Name).ToArray());
        }



        [DataRaceTestMethod]
        public void IEnumerable_ForeachLoopCausesNoDataRace()
        {
            var input = new List<string>() { "1", "2", "3" };
            object result1 = null;
            object result2 = null;

            Parallel.Invoke(
            () => {
                result1 = GetItemsUsingForeach(input);
            },

            () => {
                result2 = GetItemsUsingForeach(input);
            }
            );

            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
        }

        private static IEnumerable<string> GetItemsUsingForeach(IEnumerable<string> input)
        {
            foreach (var item in input)
            {
                if (item != null)
                {
                    yield return item;
                }
            }
        }

        /// <remarks>
        /// Not sure why this is a data race.
        /// </remarks>
        [DataRaceTestMethod]
        public void IEnumerable_LambdaCausesDataRace()
        {
            var input = new List<string>() { "1", "2", "3" };
            object result1 = null;
            object result2 = null;

            Parallel.Invoke(
            () => {
                result1 = GetItemsUsingLambda(input);
            },

            () => {
                result2 = GetItemsUsingLambda(input);
            }
            );

            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
        }

        private static IEnumerable<string> GetItemsUsingLambda(IEnumerable<string> input)
        {
            return input.Where(item => item != null);
        }

    }
}
