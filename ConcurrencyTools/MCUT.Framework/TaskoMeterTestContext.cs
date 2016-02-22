using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>The context of a TaskoMeter test.</summary>
    public class TaskoMeterTestContext : TestContext
    {

        public static TaskoMeterTestContext GetDefaultInstance()
        {
            return new TaskoMeterTestContext("");
        }

        public TaskoMeterTestContext(string name)
            :base(name, null)
        {
            // Set the defaults
            WarmupRepetitions = 0;
            Repetitions = 1;
        }

        public int WarmupRepetitions { get; set; }

        public int Repetitions { get; set; }

    }
}
