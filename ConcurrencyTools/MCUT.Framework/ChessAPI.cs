
﻿using System;

// all calls from testcases into MChess reference this dll and class
// but actual calls get wrapped by ER, so all methods here are no-ops
// (except choose which uses random)

namespace Baschi
{
    public class Whatever
    {
        // new API methods, good for convenient unit testing
        public static void RunWithoutPreemptions(Action a)
        {
            try
            {

                lock (new Object()) { }

                Microsoft.Concurrency.TestTools.UnitTesting.Chess.ChessAPI.TraceEvent("Hi THERE");
                Microsoft.Concurrency.TestTools.UnitTesting.Chess.ChessAPI.PreemptionDisable();
                a();
            }
            finally
            {
                Microsoft.Concurrency.TestTools.UnitTesting.Chess.ChessAPI.PreemptionEnable();
            }
        }
    }
}

namespace Microsoft.Concurrency.TestTools.UnitTesting.Chess
{
    public static class ChessAPI
    {
        private static Random random = new Random();

        /// <summary>Performs a misc task using a Random number generator instance.</summary>
        /// <remarks>This method is provided mainly for regression testing.</remarks>
        /// <param name="numChoices">arbitrary number. This becomes the exclusive upper bound on the number returned.</param>
        /// <returns></returns>
        public static int Choose(int numChoices) { return random.Next(numChoices); }

        public static void TraceEvent(string info) {}
        public static void PreemptionDisable() {}
        public static void PreemptionEnable() {}
        public static void PrioritizePreemptions() {}
        public static void UnprioritizePreemptions() {}
        public static void ObserveOperationCall(object obj, string name) {}
        public static void ObserveOperationCall(string name) {}
        public static void ObserveOperationReturn() {}
        public static void ObserveCallback(object obj, string name) {}
        public static void ObserveCallback(string name) {}
        public static void ObserveCallbackReturn() {}
        public static void ObserveInteger(string label, int val) {}
        public static void ObserveInteger(string label, uint val) {}
        public static void ObserveInteger(string label, long val) {}
        public static void ObserveInteger(string label, ulong val) {}
        public static void ObserveBoolean(string label, bool val) {}
        public static void ObserveString(string label, string val) {}
        public static void ObserveObjectIdentity(string label, object obj) {}
        public static bool IsBreakingDeadlock() { return false;  }

        // new API methods, good for convenient unit testing
        public static void RunWithoutPreemptions(Action a)
        {
            try
            {
                TraceEvent("Hi THERE");
                PreemptionDisable();
                a();
            }
            finally
            {
                PreemptionEnable();
            }
        }
    }
}

