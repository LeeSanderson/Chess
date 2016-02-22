using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.UnitTesting.RegressionTests
{
    /// <summary>
    /// Tests the functionality of the <see cref="ObservationTestMethodAttribute"/> attribute.
    /// </summary>
    public class ObservationTestMethodTests
    {

        [ObservationGenerator]
        [ObservationTestMethod]
        public void NoopTest_WithSelfAsGenerator()
        {
        }

        [ObservationGenerator]
        public void NoopTest_WithDifferentGeneratorMethod_Generator()
        {
        }

        [ObservationTestMethod(GeneratorName = "NoopTest_WithDifferentGeneratorMethod_Generator")]
        public void NoopTest_WithDifferentGeneratorMethod()
        {
        }

        volatile int a;
        volatile int b;

        [ObservationGenerator]
        [ObservationTestMethod]
        public void SmallPassingSample1()
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
                             ChessAPI.ObserveInteger("val", 2);
                             ChessAPI.ObserveOperationReturn();
                         },
                         () =>
                         {
                             ChessAPI.ObserveOperationCall("second");
                             b = 1;
                             ChessAPI.ObserveString("ret", "hurra");
                             b = 1;
                             ChessAPI.ObserveOperationReturn();
                         }
                         );
        }

        [ObservationGenerator]
        [ObservationTestMethod]
        public void SmallFailingSample1()
        {
            a = 0;
            b = 0;
            System.Threading.Tasks.Parallel.Invoke(
                         () =>
                         {
                             ChessAPI.ObserveOperationCall("first");
                             a = 1;
                             ChessAPI.ObserveInteger("val", a);
                             ChessAPI.ObserveOperationReturn();
                         },
                         () =>
                         {
                             ChessAPI.ObserveOperationCall("second");
                             a = 2;
                             ChessAPI.ObserveOperationReturn();
                         }
                         );
        }


        // a little mocked-up concurrent object
        public class Pair
        {
            public volatile int a;
            public volatile int b;

            public int Read_a() { return a; }
            public int Read_both() { return a + b; }
            public void Write_a(int val) { a = val; }
            public void Write_b(int val) { b = val; }
            public void Write_both(int val_a, int val_b) { a = val_a; b = val_b; }
        }

        // harness for testing "Pair" objects
        private ComponentHarness<Pair> CreateHarness()
        {
            var harness = new ComponentHarness<Pair>();

            harness.DefineConstructor("default", () =>
            {
                return new Pair();
            });
            harness.DefineOperation("read a", (Pair p) =>
            {
                int val = p.Read_a();
                ChessAPI.ObserveInteger("val", val); 
            });
            harness.DefineOperation("read both", (Pair p) =>
            {
                int val = p.Read_both();
                ChessAPI.ObserveInteger("val", val); 
            });
            for (int i = 0; i < 2; i++)
            {
                int val = i * 10;
                harness.DefineOperation("set a=" + val, (Pair p) =>
                {
                    p.Write_a(val);
                });
                harness.DefineOperation("set b=" + val, (Pair p) =>
                {
                    p.Write_b(val);
                });
            }
            harness.DefineOperation("set a=b=10", (Pair p) =>
            {
                p.Write_both(10, 10);
            });

            return harness;
        }


        [ObservationGenerator]
        [ObservationTestMethod]
        public void Sample2()
        {
            Pair p = new Pair();
            Thread t1 = new Thread(() =>
                {
                    ChessAPI.ObserveOperationCall("set a=0");
                    p.Write_a(0);
                    ChessAPI.ObserveOperationReturn();
                    ChessAPI.ObserveOperationCall("read both");
                    ChessAPI.ObserveInteger("val", p.Read_both());
                    ChessAPI.ObserveOperationReturn();
                    ChessAPI.ObserveOperationCall("read a");
                    ChessAPI.ObserveInteger("val", p.Read_a());
                    ChessAPI.ObserveOperationReturn();
                });
            Thread t2 = new Thread(() =>
            {
                ChessAPI.ObserveOperationCall("set b=10");
                p.Write_b(10);
                ChessAPI.ObserveOperationReturn();
                ChessAPI.ObserveOperationCall("read both");
                ChessAPI.ObserveInteger("val", p.Read_both());
                ChessAPI.ObserveOperationReturn();
            });
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();
        }

        [ObservationGenerator]
        [ObservationTestMethod]
        public void Sample2_with_harness()
        {
            var harness = CreateHarness();

            harness.RunTest(
               "default",
               new string[][] {
                   new string[] {  "set a=0", "read both", "read a"    },
                   new string[] {  "set b=10","read both"    }
               }
            );
        }


        [ObservationGenerator]
        [ObservationTestMethod]
        public void FailingSample2_with_harness()
        {
            var harness = CreateHarness();

            harness.RunTest(
               "default",
               new string[][] {
                   new string[] {  "set a=0", "read both", "read a"    },
                   new string[] {  "read both", "set a=b=10"    }
               }
            );
        }


        [ObservationGenerator]
        [ObservationTestMethod]
        public void FailingSample3_with_random_harness()
        {
            var harness = CreateHarness();

            harness.RunRandomTest(0, 3, 3);
        }

    }

}
