using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>
    /// Marks a method as a performance method.
    /// By default, running of this test will cause performance metrics to be run on it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PerformanceTestMethodAttribute : TestTypeAttributeBase
    {

        /// <summary>Marks a method as a performance test method.</summary>
        public PerformanceTestMethodAttribute() { }

        private int? _warmupRepetitions;
        /// <summary>
        /// Specifies the number of warmup iterations should be executed before timings are started.
        /// The default is 0, indicating no warmup iterations.
        /// </summary>
        public int WarmupRepetitions
        {
            get { return _warmupRepetitions.Value; }
            set { _warmupRepetitions = value; }
        }

        private int? _repetitions;
        /// <summary>
        /// Specifies the number of times to execute a test method while being timed.
        /// The default is 1.
        /// </summary>
        public int Repetitions
        {
            get { return _repetitions.Value; }
            set { _repetitions = value; }
        }

        protected override XElement CreateTestTypeXml(MethodInfo testMethod)
        {
            return new XElement(XNames.PerformanceTest
                , _warmupRepetitions.HasValue ? new XAttribute("WarmupRepetitions", WarmupRepetitions) : null
                , _repetitions.HasValue ? new XAttribute("Repetitions", Repetitions) : null
                );
        }

    }
}