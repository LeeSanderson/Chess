using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Scenario;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Drawing;

namespace Microsoft.Concurrency.TestTools.TaskoMeter
{
    /// <summary>
    /// Utility class for starting and stopping metering along with showing an interactive
    /// UI.
    /// </summary>
    public static class Metering
    {

        /// <summary>
        /// Spans a thread that runs a UI for running and viewing the performance statistics
        /// for some action.
        /// </summary>
        public static Thread MeasureInteractively(Action a, string title)
        {
            return MeasureInteractively(a, 1, null, 0, title);
        }

        /// <summary>
        /// Spans a thread that runs a UI for running and viewing the performance statistics
        /// for some action.
        /// </summary>
        public static Thread MeasureInteractively(Action a, int warmupruns, string title)
        {
            return MeasureInteractively(a, 1, a, warmupruns, title);
        }

        /// <summary>
        /// Spans a thread that runs a UI for running and viewing the performance statistics
        /// for some action.
        /// </summary>
        public static Thread MeasureInteractively(Action a, int reps, Action warmup, int warmupreps, string title)
        {
            var t = new System.Threading.Thread(() => {
                Stats stats = new Stats(title, a, reps, warmup, warmupreps);
                System.Windows.Forms.Application.Run(stats);
            });
            t.Start();
            return t;
        }

        /// <summary>Starts a metering session.</summary>
        public static void Start()
        {
            Start("");
        }

        /// <summary>Starts a metering session.</summary>
        public static void Start(string title)
        {
            Metering.title = title;
            StartWarmup();
            EndWarmup();
            StartMetering();
        }

        /// <summary>Ends the metering session.</summary>
        public static void End()
        {
            if (TaskMeter._StopWatch == null)
                return;
            if (end > 0)
                throw new Exception("Taskometer: can not end measure twice");
            StopMetering();
            ManualResetEvent mre = new ManualResetEvent(false);
            new System.Threading.Thread(() => {
                Stats stats = new Stats(title, null, 0, null, 0);
                mre.Set();
                System.Windows.Forms.Application.Run(stats);

            }).Start();
            mre.WaitOne();
            foreach (TaskMeter m in TaskMeter._Tasks)
                m.ClearData();
            TaskMeter._StopWatch = null;
            end = 0;
        }

        /// <summary>
        /// Itentifies that a task meter should measure speadup compared to a test meter.
        /// </summary>
        public static void MeasureSpeedup(TaskMeter before, TaskMeter after)
        {
            after.ReferenceMeter = before;
        }

        private static TaskMeter total = new TaskMeter("TOTAL", TaskMeter.Color.LightBlue);
        private static Scenario.Scenario scenario = new Scenario.Scenario();
        private static int count = 0;

        internal static void StartWarmup()
        {
            scenario.Begin(count, "Taskometer Warmup Begin");
        }
        internal static void EndWarmup()
        {
            scenario.End(count, "Taskometer Warmup End");
            scenario.Reset();
        }


        internal static void StartMetering()
        {
            if (TaskMeter._StopWatch != null)
                throw new Exception("Taskometer: can not start measure twice");
            TaskMeter._StopWatch = new System.Diagnostics.Stopwatch();
            TaskMeter._StopWatch.Start();
            scenario.Begin(count, "Taskometer Measurement Begin");
            total.Start();
        }
        internal static void ContinueMetering()
        {
            total.End();
            scenario.End(count, "Taskometer Measurement End");
            scenario.Begin(count, "Taskometer Measurement Begin");
            total.Start();
        }
        internal static void StopMetering()
        {
            total.End();
            scenario.End(count, "Taskometer Measurement End");
            TaskMeter._StopWatch.Stop();
            end = TaskMeter._StopWatch.ElapsedTicks;
            scale = ((double)TaskMeter._StopWatch.ElapsedMilliseconds) / end;
            TaskMeter._StopWatch = null;
            scenario.Reset();
            count++;
        }

        private static string title;
        internal static long end = 0;
        internal static double scale = 1;

    }
}
