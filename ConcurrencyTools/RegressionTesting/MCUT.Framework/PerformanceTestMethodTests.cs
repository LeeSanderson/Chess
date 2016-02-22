using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Drawing;


namespace Microsoft.Concurrency.TestTools.UnitTesting.RegressionTests
{
    /// <summary>
    /// Tests the functionality of the <see cref="PerformanceTestMethodAttribute"/> attribute.
    /// </summary>
    public class PerformanceTestMethodTests
    {

        private TaskMeter prologue = new TaskMeter("prologue",  TaskMeter.Color.Yellow);
        private TaskMeter parloop = new TaskMeter("parloop", TaskMeter.Color.Orange);
        private TaskMeter partask = new TaskMeter("partask", TaskMeter.Color.Red);
        private TaskMeter epilogue = new TaskMeter("epilogue", TaskMeter.Color.Purple);

        [PerformanceTestMethod]
        public void EmptyTest()
        {
        }

        int metercount = 0;
        private TaskMeter[] meters = new TaskMeter[10];

        [PerformanceTestMethod(WarmupRepetitions = 10, Repetitions = 10)]
        public void MeterCreation()
        {
            if (!TaskMeter.MeasurementIsUnderway())
            {
                // create taskmeters during warmup!
                meters[metercount % 10] = new TaskMeter("meter number " + metercount);
                metercount++;
            }
            else
            {
                for (int j = 0; j < 10; j++)
                    meters[j].Measure(() => { for (int i = 1; i < 1000; i++) ; });
            }
        }

        [PerformanceTestMethod(WarmupRepetitions = 10, Repetitions = 5)]
        public void ParallelSleepTest()
        {
            prologue.Start();
            System.Threading.Thread.Sleep(10);
            prologue.End();

            parloop.Start();
            System.Threading.Tasks.Parallel.For(0, 50, i => 
                {
                    partask.Start();
                    System.Threading.Thread.Sleep(5);
                    partask.End();
                });
            parloop.End();

            epilogue.Start();
            System.Threading.Thread.Sleep(10);
            epilogue.End();
        }
         

    }
}
