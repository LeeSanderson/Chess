using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.UnitTesting.RegressionTests
{
    /// <summary>
    /// Tests the functionality of Chess test methods.
    /// </summary>
    [ChessInstrumentAssembly("System.Core")]
    [ChessInstrumentAssembly("System.Xml")]
    public class InstrumentChessAssemblyTests
    {

        public ChessTestContext ChessTestContext { get; set; }

        [ChessTestMethod]
        public void ClassOnly()
        {
            Assert.IsNotNull(ChessTestContext);
            Assert.IsNotNull(ChessTestContext.Options.IncludedAssemblies, "ChessTestContext.Options.IncludedAssemblies");

            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Core"), "System.Core");
            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Xml"), "System.Xml");
        }

        [ChessTestMethod]
        [ChessInstrumentAssembly("System.Xml", Exclude = true)]
        public void ClassExcluded()
        {
            Assert.IsNotNull(ChessTestContext);
            Assert.IsNotNull(ChessTestContext.Options.IncludedAssemblies, "ChessTestContext.Options.IncludedAssemblies");

            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Core"), "System.Core");
            Assert.IsFalse(ChessTestContext.Options.IncludedAssemblies.Contains("System.Xml"), "System.Xml");
        }

        [ChessTestMethod]
        [ChessInstrumentAssembly("System.Drawing")]
        public void ClassPlusMethod()
        {
            Assert.IsNotNull(ChessTestContext);
            Assert.IsNotNull(ChessTestContext.Options.IncludedAssemblies, "ChessTestContext.Options.IncludedAssemblies");

            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Core"), "System.Core");
            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Xml"), "System.Xml");
            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Drawing"), "System.Drawing");
        }

        [ChessTestMethod]
        [ChessInstrumentAssembly("System.Drawing")]
        [ChessInstrumentAssembly("System.Data", Exclude = true)]
        public void ClassPlusMethod_WithUselessExclude()
        {
            Assert.IsNotNull(ChessTestContext);
            Assert.IsNotNull(ChessTestContext.Options.IncludedAssemblies, "ChessTestContext.Options.IncludedAssemblies");

            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Core"), "System.Core");
            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Xml"), "System.Xml");
            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Drawing"), "System.Drawing");
            Assert.IsFalse(ChessTestContext.Options.IncludedAssemblies.Contains("System.Data"), "System.Data");
        }

        [ScheduleTestMethod]
        public void ScheduleTest_AssertAssembliesInstrumented_ClassOnly()
        {
            Assert.IsNotNull(ChessTestContext);
            Assert.IsNotNull(ChessTestContext.Options.IncludedAssemblies, "ChessTestContext.Options.IncludedAssemblies");

            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Core"), "Class-level attribute not applied: System.Core");
            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Xml"), "Class-level attribute not applied: System.Xml");
        }

        [ScheduleTestMethod]
        [ChessInstrumentAssembly("System.Drawing")]
        public void ScheduleTest_AssertAssembliesInstrumented_ClassPlusMethod()
        {
            Assert.IsNotNull(ChessTestContext);
            Assert.IsNotNull(ChessTestContext.Options.IncludedAssemblies, "ChessTestContext.Options.IncludedAssemblies");

            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Core"), "Class-level attribute not applied: System.Core");
            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Xml"), "Class-level attribute not applied: System.Xml");
            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Drawing"), "Method-level attribute not applied: System.Drawing");
        }

        [DataRaceTestMethod]
        public void DataRaceTest_AssertAssembliesInstrumented_ClassOnly()
        {
            Assert.IsNotNull(ChessTestContext);
            Assert.IsNotNull(ChessTestContext.Options.IncludedAssemblies, "ChessTestContext.Options.IncludedAssemblies");

            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Core"), "Class-level attribute not applied: System.Core");
            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Xml"), "Class-level attribute not applied: System.Xml");
        }

        [DataRaceTestMethod]
        [ChessInstrumentAssembly("System.Drawing")]
        public void DataRaceTest_AssertAssembliesInstrumented_ClassPlusMethod()
        {
            Assert.IsNotNull(ChessTestContext);
            Assert.IsNotNull(ChessTestContext.Options.IncludedAssemblies, "ChessTestContext.Options.IncludedAssemblies");

            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Core"), "Class-level attribute not applied: System.Core");
            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Xml"), "Class-level attribute not applied: System.Xml");
            Assert.IsTrue(ChessTestContext.Options.IncludedAssemblies.Contains("System.Drawing"), "Method-level attribute not applied: System.Drawing");
        }

    }
}
