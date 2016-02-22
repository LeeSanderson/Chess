using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using ExampleTestLibrary;

namespace Microsoft.Concurrency.TestTools.UnitTesting.RegressionTests
{
    public class AutomaticInstrumentingOfReferencedAssembliesTest
    {

        public ChessTestContext ChessTestContext { get; set; }

        [ChessTestMethod]
        public void ClassOnly()
        {
            // Need to have some code in this project that actually uses the assembly or else it won't get added to the assembly's GetReferencedAssemblies.
            ExampleUtil.GetYear();

            Assert.IsNotNull(ChessTestContext);
            Assert.IsNotNull(ChessTestContext.Options.IncludedAssemblies, "ChessTestContext.Options.IncludedAssemblies");

            string msg = " is an assembly reference, but shouldn't be automatically instrumented because it's in the GAC.";
            Assert.IsFalse(ChessTestContext.Options.IncludedAssemblies.Contains("Microsoft.Concurrency.UnitTestingFramework"), "Microsoft.Concurrency.UnitTestingFramework" + msg);
            Assert.IsFalse(ChessTestContext.Options.IncludedAssemblies.Contains("System"), "System" + msg);
            Assert.IsFalse(ChessTestContext.Options.IncludedAssemblies.Contains("System.Core"), "System.Core" + msg);
            Assert.IsFalse(ChessTestContext.Options.IncludedAssemblies.Contains("System.Data"), "System.Data" + msg);
            Assert.IsFalse(ChessTestContext.Options.IncludedAssemblies.Contains("System.Data.DataSetExtensions"), "System.Data.DataSetExtensions" + msg);
            Assert.IsFalse(ChessTestContext.Options.IncludedAssemblies.Contains("System.Drawing"), "System.Drawing" + msg);
            Assert.IsFalse(ChessTestContext.Options.IncludedAssemblies.Contains("System.Xml"), "System.Xml" + msg);
            Assert.IsFalse(ChessTestContext.Options.IncludedAssemblies.Contains("System.Xml.Linq"), "System.Xml.Linq" + msg);
        }

    }
}
