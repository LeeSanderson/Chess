/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test42
{
    public class ChessTest
    {
        public static bool Run()
        {
            TimerExample.Main();
            return true;
        }
    }

    public class TimerExample
    {
        public static void Main()
        {
            AutoResetEvent autoEvent = new AutoResetEvent(false);
            StatusChecker statusChecker = new StatusChecker(10);

            // Create the delegate that invokes methods for the timer.
            TimerCallback timerDelegate =
                new TimerCallback(statusChecker.CheckStatus);

            // Create a timer that signals the delegate to invoke 
            // CheckStatus after one second, and every 1/4 second 
            // thereafter.
            // Console.WriteLine("{0} Creating timer.\n", 
            //    DateTime.Now.ToString("h:mm:ss.fff"));
            Timer stateTimer =
                    new Timer(timerDelegate, autoEvent, Timeout.Infinite, Timeout.Infinite);

            stateTimer.Change(20, 20);
            // When autoEvent signals, change the period to every 
            // 1/2 second.
            autoEvent.WaitOne();
            stateTimer.Change(0, 500);
            // Console.WriteLine("\nChanging period.\n");

            // When autoEvent signals the second time, dispose of 
            // the timer.
            autoEvent.WaitOne();
            stateTimer.Dispose();
            //Console.WriteLine("\nDestroying timer.");
        }
    }

    class StatusChecker
    {
        int invokeCount, maxCount;

        public StatusChecker(int count)
        {
            invokeCount = 0;
            maxCount = count;
        }

        // This method is called by the timer delegate.
        public void CheckStatus(Object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            ++invokeCount;
            //Console.WriteLine("{0} Checking status {1,2}.", 
            //    DateTime.Now.ToString("h:mm:ss.fff"), 
            //    (++invokeCount).ToString());

            if (invokeCount == maxCount)
            {
                // Reset the counter and signal Main.
                invokeCount = 0;
                autoEvent.Set();
            }
        }
    }
}