using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;



namespace Microsoft.Concurrency.TestTools.UnitTesting.RegressionTests
{
    /// <summary>
    /// Tests the functionality of the <see cref="ConflictSerializabilityTestMethodAttribute"/> attribute.
    /// </summary>
    public class ConflictSerializabilityTestMethodTests
    {

        [ConflictSerializabilityTestMethod]
        public void NoopTest()
        {
        }

        volatile int a;
        volatile int b;

        [ConflictSerializabilityTestMethod]
        public void Test0()
        {
            a = 0;
            b = 0;

            System.Threading.Tasks.Parallel.Invoke(
                () =>
                {
                    ChessAPI.ObserveOperationCall("first");
                    a = 1;
                    a = 1;
                    b = a;
                    ChessAPI.ObserveOperationReturn();
                },
                () =>
                {
                    ChessAPI.ObserveOperationCall("second");
                    b = 1;
                    ChessAPI.ObserveOperationReturn();
                }
                );
        }

        [ConflictSerializabilityTestMethod]
        [ExpectedResult(TestResultType.Error)]   // TODO make special error type for these?
        public void Test1()
        {

            System.Threading.Tasks.Parallel.Invoke(
                () =>
                {
                    ChessAPI.ObserveOperationCall("first");
                    a = 1;
                    a = 1;
                    b = a;
                    ChessAPI.ObserveOperationReturn();
                },
                () =>
                {
                    ChessAPI.ObserveOperationCall("second");
                    b = 1;
                    b = 1;
                    ChessAPI.ObserveOperationReturn();
                }
                );
        }

    }

}
