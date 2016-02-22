using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.UnitTesting.RegressionTests
{
    /// <summary>
    /// Tests the functionality of the <see cref="DeterminismTestMethodAttribute"/> attribute.
    /// </summary>
    public class DeterminismTestMethodTests
    {

        [DeterminismTestMethod]
        public void NoopTest()
        {
        }

    }
}
