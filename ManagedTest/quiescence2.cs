/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Quiescence2Namespace {

    public class ChessTest {
        public static bool Run()
        {
            CalculateTest.Main();
            return true;
        }
    }

    public class CalculateTest
    {
        public static void Main()
        {
            Calculate calc = new Calculate();
            double res1 = calc.Result(234);
        }
    }

    class Calculate
    {
        double baseNumber, firstTerm, secondTerm, thirdTerm;
        ManualResetEvent manualEvent;
	int count;

        // Generate random numbers to simulate the actual calculations.
        Random randomGenerator;

        public Calculate()
        {
            manualEvent = new ManualResetEvent(false);
	    count = 3;
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

	    int temp;	
	    Monitor.Enter(this);
	    count--;
	    temp = count;
	    Monitor.Exit(this);
	    if (temp == 0) {
	       manualEvent.Reset();
	    }
        }

        void CalculateSecondTerm(object stateInfo)
        {
            double preCalc = randomGenerator.NextDouble();
            manualEvent.WaitOne();
            secondTerm = preCalc * baseNumber *
                randomGenerator.NextDouble();

	    int temp;	
	    Monitor.Enter(this);
	    count--;
	    temp = count;
	    Monitor.Exit(this);
	    if (temp == 0) {
	       manualEvent.Reset();
	    }
        }

        void CalculateThirdTerm(object stateInfo)
        {
            double preCalc = randomGenerator.NextDouble();
            manualEvent.WaitOne();
            thirdTerm = preCalc * baseNumber *
                randomGenerator.NextDouble();

	    int temp;	
	    Monitor.Enter(this);
	    count--;
	    temp = count;
	    Monitor.Exit(this);
	    if (temp == 0) {
	       manualEvent.Reset();
	    }
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

            return firstTerm + secondTerm + thirdTerm;
        }
    }
}