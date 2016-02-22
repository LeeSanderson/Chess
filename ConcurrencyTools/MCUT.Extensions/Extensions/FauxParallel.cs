using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.Concurrency.TestTools.Extensions
{
    /// <summary>
    /// An .net 3.5 valid implementation of the 4.0's Parallel class.
    /// </summary>
    /// <remarks>
    /// This is provided as a temporary replacement until the Chess wrappers
    /// for the TPL is finished.
    /// </remarks>
    [Obsolete("Use the .net v4.0 TPL's implementation of Parallel.xxx because Chess now has wrappers for it.")]
    public static class FauxParallel
    {

        private class ThreadInfo
        {
            public readonly Thread Thread;
            public volatile Exception Error;

            public ThreadInfo(Thread t)
            {
                Thread = t;
            }

        }

        private static void ForkJoin(ThreadInfo[] threads)
        {
            // Start and wait for all threads
            foreach (var ti in threads)
                ti.Thread.Start(ti);
            foreach (var ti in threads)
                ti.Thread.Join();

            // Now see if any threw an exception
            var errors = from ti in threads
                         where ti.Error != null
                         select ti.Error;
            if (errors.Any())
                throw new AggregateException(errors);
        }

        /// <summary>
        /// Executes each of the provided actions, possibly in parallel.
        /// </summary>
        /// <param name="actions">the action(s) to execute.</param>
        /// <exception cref="AggregateException">The exception that is thrown when any action in the actions array throws an exception.</exception>
        public static void Invoke(params Action[] actions)
        {
            // Lets just schedule one thread per action
            var threads = (from a in actions
                           select new ThreadInfo(new Thread((object o) => {
                               ThreadInfo info = (ThreadInfo)o;
                               try
                               {
                                   a.Invoke();
                               }
                               catch (Exception ex)
                               {
                                   info.Error = ex;
                               }
                           }))
                          ).ToArray();

            ForkJoin(threads);
        }

        /// <summary>
        /// Executes a for loop in which iterations may run in parallel.
        /// </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        public static void For(int fromInclusive, int toExclusive, Action<int> body)
        {
            // Lets just schedule one thread per action
            Invoke(Enumerable.Range(fromInclusive, toExclusive - fromInclusive)
                .Select(i => new Action(() => body(i)))
                .ToArray()
                );
        }

        /// <summary>
        /// Executes a for each operation on an System.Collections.IEnumerable{TSource}
        /// in which iterations may run in parallel.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the source.</typeparam>
        /// <param name="source">An enumerable data source.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        public static void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            Invoke(source
                .Select(src => new Action(() => body(src)))
                .ToArray()
                );
        }

    }
}
