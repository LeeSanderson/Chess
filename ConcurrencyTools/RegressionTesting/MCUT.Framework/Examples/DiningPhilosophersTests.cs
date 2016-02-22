using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.UnitTesting.RegressionTests.Examples
{
    /// <summary>
    /// This is taken from the PPCP course homework 2 from Ganesh (Fall 2010).
    /// </summary>
    /// <remarks>
    /// Circle and identify any correctness issues.
    /// Correct any correctness issues that you previously found.
    /// </remarks>
    public class DiningPhilosophersTests
    {

        public class DiningPhilosophers
        {
            const int NUM_PHILOSOPHERS = 5;
            const int NUM_MEALS = 1;

            object[] locks = new object[NUM_PHILOSOPHERS];

            void RunPhilosopher(object id)
            {
                int i = (int)id;
                Random rand = new Random();
                int meals = NUM_MEALS;
                while (meals-- > 0)
                {
                    if (rand.Next(0, 1) == 0)
                    {//left fork then right
                        lock (locks[i])
                        {
                            lock (locks[(i + 1) % NUM_PHILOSOPHERS])
                            {
                                Console.WriteLine("Philosopher {0} ate", i);
                            }
                        }
                    }
                    else
                    {//right fork then left
                        lock (locks[(i + 1) % NUM_PHILOSOPHERS])
                        {
                            lock (locks[i])
                            {
                                Console.WriteLine("Philosopher {0} ate", i);
                            }
                        }
                    }
                }
            }

            [ScheduleTestMethod]
            [RegressionTestExpectedResult(TestResultType.Deadlock)]
            public void Test()
            {
                System.Threading.Thread[] t = new System.Threading.Thread[NUM_PHILOSOPHERS];
                for (int l = 0; l < NUM_PHILOSOPHERS; ++l) locks[l] = new object();
                for (int p = 0; p < NUM_PHILOSOPHERS; ++p)
                {
                    t[p] = new System.Threading.Thread(RunPhilosopher);
                    t[p].Start(p);
                }
                for (int p = 0; p < NUM_PHILOSOPHERS; ++p)
                {
                    t[p].Join();
                }
            }
        }
    }
}
