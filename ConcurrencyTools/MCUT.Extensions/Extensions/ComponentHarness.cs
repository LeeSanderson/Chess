using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.Extensions
{
    /// <summary>
    /// Helper class for constructing test harnesses for concurrent objects.
    /// </summary>
    /// <typeparam name="T">The type of the object being tested.</typeparam>
    public class ComponentHarness<T>
    {
        /// <summary>
        /// Create a harness object.
        /// </summary>
        public ComponentHarness()
        {
        }

        /// <summary>
        /// Register a way to construct an object under test.
        /// </summary>
        /// <param name="name">The name used to refer to this constructor.</param>
        /// <param name="constructor">The delegate returning the constructed object.</param>
        public void DefineConstructor(string name, Func<T> constructor)
        {
            if (constructors.ContainsKey(name))
                throw new ArgumentException("Constructor \"" + name + "\" already defined.");
            constructors.Add(name, constructor);
        }

        private Dictionary<string, Func<T>> constructors = new Dictionary<string,Func<T>>();
        
        /// <summary>
        /// Register an operation to perform on the object under test.
        /// </summary>
        /// <param name="name">The name used to refer to this operation.</param>
        /// <param name="operation">The delegate performing the observed operation
        /// (must include explicit observation calls for observed arguments and return values).</param>
        public void DefineOperation(string name, Action<T> operation)
        {
            if (operations.ContainsKey(name))
                throw new ArgumentException("Operation \"" + name + "\" already defined.");
            operations.Add(name,operation);
        }

        private Dictionary<string, Action<T>> operations = new Dictionary<string,Action<T>>();

        /// <summary>
        /// Run a specified concurrent test.
        /// </summary>
        /// <param name="constructor">The constructor to use.</param>
        /// <param name="matrix">The operation sequences to use for each thread.</param>
        public void RunTest(string constructor, string[][] matrix)
        {
            // look up constructor
            Func<T> c = null;
            if (!constructors.TryGetValue(constructor, out c))
                throw new ArgumentException("No constructor \"" + constructor + "\" defined.");
 
            // look up ops
            Action<T>[][] ops = new Action<T>[matrix.Length][];
            for (int t = 0; t < matrix.Length; t++)
            {
                ops[t] = new Action<T>[matrix[t].Length];
                for (int o = 0; o < matrix[t].Length; o++)
                {
                     if (!operations.TryGetValue(matrix[t][o], out ops[t][o]))
                        throw new ArgumentException("No operation \"" + matrix[t][o] + "\" defined.");
                }
            }

            // run the test.
            ForkJoin(c(), matrix, ops);
        }


        private static void ForkJoin(T objundertest, string[][] matrix, Action<T>[][] ops)
        {
            Thread[] threads = new Thread[ops.Length];

            // Create all threads
            for (int t = 0; t < threads.Length; t++)
            {
                int tt = t;
                threads[t] = new Thread(() =>
                    {
                        for (int o = 0; o < ops[tt].Length; o++)
                        {
                            try
                            {
                                ChessAPI.ObserveOperationCall(matrix[tt][o]);
                                ops[tt][o](objundertest);
                            }
                            catch(Exception e)
                            {
                                if (ChessAPI.IsBreakingDeadlock())
                                    break;

                                ChessAPI.ObserveString("exception", e.Message);
                                ChessAPI.ObserveOperationReturn();
                            }
                        }
                    });
            }

            // start all threads
            foreach(var t in threads)
               t.Start();

            // join all threads
            foreach (var t in threads)
                t.Join();
        }

        /// <summary>
        /// Run a randomly selected concurrent test.
        /// </summary>
        /// <param name="seed">The random seed to use.</param>
        /// <param name="threads">The number of threads to use.</param>
        /// <param name="ops">The number of operations to use per thread.</param>
        public void RunRandomTest(int seed, int threads, int ops)
        {
            Random r = new Random(seed);

            string c = constructors.Keys.ElementAt(r.Next(constructors.Count));
 
            string[][] matrix = new string[threads][];

            for (int t = 0; t < threads; t++)
            {
                matrix[t] = new string[ops];
                for (int o = 0; o < ops; o++)
                {
                    matrix[t][o] = operations.Keys.ElementAt(r.Next(operations.Count));
                }
            }

            RunTest(c, matrix);
        }

    }
}
