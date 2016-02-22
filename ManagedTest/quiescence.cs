/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace QuiescenceNamespace {

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

        // Generate random numbers to simulate the actual calculations.
        Random randomGenerator;

        public Calculate()
        {
        }

        void CalculateBase(object stateInfo)
        {
            baseNumber = randomGenerator.NextDouble();
        }

        // The following CalculateX methods all perform the same
        // series of steps as commented in CalculateFirstTerm.

        void CalculateFirstTerm(object stateInfo)
        {
            // Perform a precalculation.
            double preCalc = randomGenerator.NextDouble();

            // Calculate the first term from preCalc and baseNumber.
            firstTerm = preCalc * baseNumber *
                randomGenerator.NextDouble();
        }

        void CalculateSecondTerm(object stateInfo)
        {
            double preCalc = randomGenerator.NextDouble();
            secondTerm = preCalc * baseNumber *
                randomGenerator.NextDouble();
        }

        void CalculateThirdTerm(object stateInfo)
        {
            double preCalc = randomGenerator.NextDouble();
            thirdTerm = preCalc * baseNumber *
                randomGenerator.NextDouble();
        }

        public double Result(int seed)
        {
            randomGenerator = new Random(seed);

	    CalculateBase(null);
            // Simultaneously calculate the terms.
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