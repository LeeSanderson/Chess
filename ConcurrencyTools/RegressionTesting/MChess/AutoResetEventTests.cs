using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    public class AutoResetEventTests : MChessRegressionTestBase
    {

        // autoresetevent.cs
        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 657, LastThreadCount = 5, LastExecSteps = 27, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 1125, LastThreadCount = 5, LastExecSteps = 27, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 1415, LastThreadCount = 5, LastExecSteps = 27, LastHBExecSteps = 1)]
        public void AutoResetEvent()
        {
            Calculate calc = new Calculate();
            double res1 = calc.Result(234);
        }

        // autoresetevent1.cs
        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.ChessDeadlock, SchedulesRan = 1, LastThreadCount = 2, LastExecSteps = 7, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb2", ChessExitCode.ChessDeadlock, SchedulesRan = 1, LastThreadCount = 2, LastExecSteps = 7, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb3", ChessExitCode.ChessDeadlock, SchedulesRan = 1, LastThreadCount = 2, LastExecSteps = 7, LastHBExecSteps = 1)]
        public void AutoResetEvent1()
        {
            AutoResetEvent e = new AutoResetEvent(false);
            Thread t = new Thread(delegate(object o) {
                AutoResetEvent e2 = (AutoResetEvent)o;
                // put child thread code here
                e2.Set();
                Console.WriteLine("child thread");
                e2.Set();
            });
            t.Start(e);
            // put parent thread code here
            e.WaitOne();
            Console.WriteLine("parent thread");
            e.WaitOne();
            // check consistency of state
        }

        // autoresetevent2.cs
        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 2, LastThreadCount = 2, LastExecSteps = 11, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 2, LastThreadCount = 2, LastExecSteps = 11, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 2, LastThreadCount = 2, LastExecSteps = 11, LastHBExecSteps = 1)]
        public void AutoResetEvent2()
        {
            AutoResetEvent e1 = new AutoResetEvent(false);
            AutoResetEvent e2 = new AutoResetEvent(false);
            Thread t = new Thread(delegate() {
                // put child thread code here
                e1.WaitOne();
                // Console.WriteLine("child thread");
                e2.Set();
            });
            t.Start();
            // put parent thread code here
            e1.Set();
            // Console.WriteLine("parent thread");
            e2.WaitOne();
            t.Join();
            // check consistency of state
        }

        class Calculate
        {
            double baseNumber, firstTerm, secondTerm, thirdTerm;
            AutoResetEvent[] autoEvents;
            ManualResetEvent manualEvent;

            // Generate random numbers to simulate the actual calculations.
            Random randomGenerator;

            public Calculate()
            {
                autoEvents = new AutoResetEvent[]
            {
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false)
            };

                manualEvent = new ManualResetEvent(false);
            }

            void CalculateBase(object stateInfo)
            {
                baseNumber = randomGenerator.NextDouble();

                // Signal that baseNumber is ready.
                manualEvent.Set();
            }

            // The following CalculateX methods all perform the same
            // series of steps as commented in CalculateFirstTerm.

            void CalculateFirstTerm(object stateInfo)
            {
                // Perform a precalculation.
                double preCalc = randomGenerator.NextDouble();

                // Wait for baseNumber to be calculated.
                manualEvent.WaitOne();

                // Calculate the first term from preCalc and baseNumber.
                firstTerm = preCalc * baseNumber *
                    randomGenerator.NextDouble();

                // Signal that the calculation is finished.
                autoEvents[0].Set();
            }

            void CalculateSecondTerm(object stateInfo)
            {
                double preCalc = randomGenerator.NextDouble();
                manualEvent.WaitOne();
                secondTerm = preCalc * baseNumber *
                    randomGenerator.NextDouble();
                autoEvents[1].Set();
            }

            void CalculateThirdTerm(object stateInfo)
            {
                double preCalc = randomGenerator.NextDouble();
                manualEvent.WaitOne();
                thirdTerm = preCalc * baseNumber *
                    randomGenerator.NextDouble();
                autoEvents[2].Set();
            }

            public double Result(int seed)
            {
                randomGenerator = new Random(seed);

                // Simultaneously calculate the terms.
                ThreadPool.QueueUserWorkItem(
                    new WaitCallback(CalculateBase));
                ThreadPool.QueueUserWorkItem(
                    new WaitCallback(CalculateFirstTerm));
                ThreadPool.QueueUserWorkItem(
                    new WaitCallback(CalculateSecondTerm));
                ThreadPool.QueueUserWorkItem(
                    new WaitCallback(CalculateThirdTerm));

                // Wait for all of the terms to be calculated.
                WaitHandle.WaitAll(autoEvents);

                // Reset the wait handle for the next calculation.
                manualEvent.Reset();

                return firstTerm + secondTerm + thirdTerm;
            }
        }
    }
}
