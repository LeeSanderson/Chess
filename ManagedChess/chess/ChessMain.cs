/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.ExtendedReflection.Collections;
using Microsoft.ManagedChess.EREngine.AllCallbacks;

namespace Microsoft.ManagedChess.EREngine
{
    internal delegate bool RunTestMethod();

#if !DEBUG  // We only want this defined when we aren't actually debugging yourself
    [DebuggerNonUserCode]
#endif
    internal class ChessMain
    {
        private ClrSyncManager manager;
        private MChessOptions mco;

        private Assembly testAssembly;

        private bool runUnitTest;
        private string unitTestMethodName;
        private MethodInfo unitTestMethod;
        private ManagedTestCase unitTestCase;

        private string testclass;
        private MethodInfo startup;
        private MethodInfo run;
        private MethodInfo shutdown;
        private MethodInfo onErrorCallback;

        static private SafeDictionary<string, Assembly> assemblies = new SafeDictionary<string, Assembly>();
        // we need to proactively get all the assemblies, otherwise we run
        // up against a limitation in ExtendedReflection
        private void TryLoadReferencedAssemblies(Assembly[] inputAssemblies)
        {
            var ws = new SafeDictionary<string, Assembly>();
            //foreach (Assembly b in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    ws.Add(b.GetName().FullName, b);
            //}
            foreach (Assembly a in inputAssemblies)
            {
                if (a == null)
                    continue;
                // recursively load all the assemblies reachables from the root!
                if (!assemblies.ContainsKey(a.GetName().FullName) && !ws.ContainsKey(a.GetName().FullName))
                {
                    ws.Add(a.GetName().FullName, a);
                }
                while (ws.Count > 0)
                {
                    var en = ws.Keys.GetEnumerator();
                    en.MoveNext();
                    var a_name = en.Current;
                    var a_assembly = ws[a_name];
                    assemblies.Add(a_name, a_assembly);
                    ws.Remove(a_name);
                    foreach (AssemblyName name in a_assembly.GetReferencedAssemblies())
                    {
                        Assembly b;
                        ExtendedReflection.Utilities.ReflectionHelper.TryLoadAssembly(name.FullName, out b);
                        if (b != null)
                        {
                            //Console.WriteLine("Loaded {0}", name.FullName);
                            if (!assemblies.ContainsKey(b.GetName().FullName) && !ws.ContainsKey(b.GetName().FullName))
                            {
                                ws.Add(b.GetName().FullName, b);
                            }
                        }
                    }
                }
            }
        }

        private bool OnErrorCallback(int errorCode, IntPtr details)
        {
            object[] arg = new object[2];
            arg[0] = errorCode;
            arg[1] = Marshal.PtrToStringAnsi(details);
            return (bool)onErrorCallback.Invoke(null, arg);
        }

        private MChessChess.OnErrorCallbackDelegate OnErrorCallbackDel = null;

        private void get_entries(Type t)
        {
            MethodInfo mi = t.GetMethod("Startup");
            if (mi != null)
            {
                if (mi.IsStatic && mi.ReturnType == typeof(bool) &&
                     mi.GetParameters().Length == 1 &&
                     mi.GetParameters()[0].ParameterType == typeof(string[]))
                {
                    startup = mi;
                }
                else
                {
                    var msg = "Error: Startup function must be of form [public static bool " + testclass + ".Startup(string [])]";
                    ReportErrorAndExit(msg, ChessExitCode.ChessInvalidTest, false, null);
                }
            }

            mi = t.GetMethod("Run");
            if (mi != null)
            {
                if (mi.IsStatic && mi.ReturnType == typeof(bool) &&
                mi.GetParameters().Length == 0)
                {
                    run = mi;
                }
                else
                {
                    var msg = "Error: Run function must be of form [public static bool " + testclass + ".Run()].";
                    ReportErrorAndExit(msg, ChessExitCode.ChessInvalidTest, false, null);
                }
            }

            mi = t.GetMethod("Shutdown");
            if (mi != null)
            {
                if (mi.IsStatic && mi.ReturnType == typeof(bool) && mi.GetParameters().Length == 0)
                {
                    shutdown = mi;
                }
                else
                {
                    var msg = "Error: Shutdown function must be of form [public static bool " + testclass + ".Shutdown()].";
                    ReportErrorAndExit(msg, ChessExitCode.ChessInvalidTest, false, null);
                }
            }

            mi = t.GetMethod("ChessOnErrorCallback");
            if (mi != null)
            {
                if (mi.IsStatic && mi.ReturnType == typeof(bool) &&
                   mi.GetParameters().Length == 2 &&
                   mi.GetParameters()[0].ParameterType == typeof(int) &&
                   mi.GetParameters()[1].ParameterType == typeof(string)
                   )
                {
                    onErrorCallback = mi;
                    OnErrorCallbackDel = new MChessChess.OnErrorCallbackDelegate(OnErrorCallback);
                    MChessChess.QueueOnErrorCallback(OnErrorCallbackDel);
                }
                else
                {
                    var msg = "Error: ChessOnErrorCallback function must be of form [public static bool " + testclass + ".ChessOnErrorCallBack(int,string)].";
                    ReportErrorAndExit(msg, ChessExitCode.ChessInvalidTest, false, null);
                }
            }
        }

        private void get_entries(Assembly a)
        {
            startup = run = shutdown = onErrorCallback = null;

            // If running a unit test, we're using direct entry methods
            if (runUnitTest)
                return;
            Debug.Assert(!String.IsNullOrEmpty(testclass), "If not running a unit test, the testClass type should be specified.");

            // Not runing a unit test. Use the old ChessTest class
            if (testclass == "ChessTest")
            {
                // NOTE: This simulates the code previously here = the first class that matches the name
                Type ct = a.GetTypes().FirstOrDefault(t => t.Name.EndsWith("ChessTest"));
                if (ct == null)
                    ReportErrorAndExit("Error: could not find class ChessTest", ChessExitCode.ChessInvalidTest, false, null);
                else
                    get_entries(ct);
            }
            else
            {
                try
                {
                    Type t = a.GetType(testclass, true, true);
                    get_entries(t);
                }
                catch (TypeLoadException)
                {
                    ReportErrorAndExit("Error: could not find class " + testclass, ChessExitCode.ChessInvalidTest, false, null);
                }
            }
            if (run == null)
                ReportErrorAndExit("Error: no run method  [public static bool " + testclass + ".Run()] found.", ChessExitCode.ChessInvalidTest, false, null);
        }

        private void popups(MChessOptions m)
        {
            // no popups
            if (m.nopopups)
            {
                Trace.Listeners.Clear();
                Microsoft.ExtendedReflection.Utilities.Safe.Diagnostics.SafeDebug.SetCrashOnAssert("");
            }
        }

        private void DiagnoseInstrumentedUninstrumented()
        {
            if (MyEngine.EnvironmentVars.Diagnose)
            {
                var uninstrumentedTypes = ThreadExecutionMonitorDispatcher.GetUninstrumentedTypes();

                var sortUninstrumented = new SafeOrderedSet<string>(StringComparer.CurrentCulture);
                foreach (var t in uninstrumentedTypes)
                {
                    if (t.Value == false)
                        sortUninstrumented.Add(t.Key.FullName);
                }

                var sortInstrumented = new SafeOrderedSet<string>(StringComparer.CurrentCulture);
                foreach (var t in uninstrumentedTypes)
                {
                    if (t.Value == true)
                        sortInstrumented.Add(t.Key.FullName);
                }
                var diagnose = @"/diagnose output. Uninstrumented types:
";
                foreach (var s in sortUninstrumented)
                {
                    diagnose += s + "\n";
                }
                diagnose += "\n/diagnose output. Instrumented types:";
                foreach (var s in sortInstrumented)
                {
                    diagnose += s + "\n";
                }
                MChessChess.ReportWarning(diagnose, "", false);
            }
        }

        private void ReportErrorAndExit(string message, ChessExitCode code, bool genRepro, Exception ex)
        {
            ReportErrorAndExitRaw(message, code, genRepro, false, ex);
        }

        private void ReportErrorAndExitRaw(string message, ChessExitCode code, bool genRepro, bool fromExitCallback, Exception ex)
        {
            DiagnoseInstrumentedUninstrumented();

            MErrorInfo errorInfo = ex == null ? null : new MErrorInfo(ex);

            // It's only safe to write to the results printer if we aren't in the exit callback
            if (!fromExitCallback)
            {
                // TODO: action? This var will always have the value of String.Empty.
                var action = "";
                if (message != "")
                {
                    MChessChess.ReportError(message, action, errorInfo);
                }
                else if (code != 0)
                {
                    // we have an error code but no message.
                    var newMessage = new ChessResult(code).ToString();
                    MChessChess.ReportError(newMessage, action, errorInfo);
                }

                MChessChess.ReportFinalStatistics((int)code);
            }

            if (genRepro)
            {
                // must call after all ReportWarning/ReportError calls
                MChessChess.Done(!fromExitCallback);
            }
            else
            {
                // Note: The MChessChess.Done already implicitly calls CloseResults, so
                // we only need to explicitly call it otherwise.
                MChessChess.CloseResults();
            }

            // Invariant: this should be the only exit in the engine!!!
            Environment.Exit((int)code);
        }

        public ChessMain(MChessOptions m, Assembly a, string testClassName, string unitTestName)
        {
            this.mco = m;
            this.testAssembly = a;
            this.testclass = testClassName;
            this.unitTestMethodName = unitTestName;
            this.runUnitTest = !String.IsNullOrEmpty(unitTestMethodName);

            this.manager = new ClrSyncManager(m);
            MChessChess.Init(manager, mco);

            TryLoadReferencedAssemblies(new[] { testAssembly });
            get_entries(testAssembly);
            popups(m);

            this.manager.SetExitCallBack((c, ex) => {
                string msg = "";
                if (ex != null)
                {
                    msg = @"Child thread raised exception
" + ex.Message + @"
Stack trace:
" + ex.StackTrace + @"
";
                    ReportErrorAndExit(msg, c, true, ex);
                }
                ReportErrorAndExitRaw("", c, true, true, null);
            });

            try
            {
                do_startup();

                // print warning if races are disabled
                if (MChessChess.GetExitCode() == 0 && !(mco.preemptAccesses || mco.sober
                               || mco.maxExecutions == 1 || !String.IsNullOrEmpty(mco.enumerateObservations)))
                    MChessChess.ReportWarning("Race Detection Disabled. Races May Hide Bugs.", "", false);

                // RunTest loop
                bool moreToTest = true;
                while (moreToTest)
                {
                    if (!MChessChess.StartTest())
                        ReportErrorAndExit("Internal failure: CHESS.StartTest failed", ChessExitCode.ChessFailure, false, null);

                    if (MyEngine.EnvironmentVars.FlipPreemptSense)
                    {
                        MChessChess.PreemptionDisable();
                    }
                    do_run();
                    if (MyEngine.EnvironmentVars.FlipPreemptSense)
                    {
                        MChessChess.PreemptionEnable();
                    }

                    if (manager.BreakDeadlockMode)
                    {
                        MChessChess.EnterChess();
                        MChessChess.WakeNextDeadlockedThread(true, false);
                        // we are now done with the deadlock-breaking mode!
                        Debug.Assert(!MChessChess.IsBreakingDeadlock());
                        manager.BreakDeadlockMode = false;
                        MChessChess.LeaveChess();
                    }
                    moreToTest = MChessChess.EndTest();
                }

                do_shutdown();
            }
            catch (Exception ex)
            {
                string message = @"CHESS internal failure.
" + ex.Message + @"
" + ex.StackTrace + @"
";
                ReportErrorAndExit(message, ChessExitCode.ChessFailure, false, ex);
            }
            ReportErrorAndExit("", (ChessExitCode)MChessChess.GetExitCode(), false, null);
        }

        private ChessTestContext CreateTestContext()
        {
            var envVars = MyEngine.EnvironmentVars;

            var opts = new Microsoft.Concurrency.TestTools.UnitTesting.Chess.MChessOptions() {
                // There isn't any way to get these options just passed straight thru
                //EnableAtomicityChecking = envVars.
                //EnableRaceDetection = envVars.,
                PreemptAllAccesses = envVars.PreemptAccesses,

                MaxChessTime = envVars.MaxChessTime,
                MaxPreemptions = envVars.MaxPreemptions,
                MaxExecs = envVars.MaxExecs,
                MaxExecSteps = envVars.MaxExecSteps,
                MaxExecTime = envVars.MaxExecTime,
                ProcessorCount = envVars.ProcessorCount,

                IncludedAssemblies = envVars.IncludeAssemblies.ToArray(),
                FlipPreemptionSense = envVars.FlipPreemptSense,
                DontPreemptAssemblies = envVars.DontPreemptAssemblies.ToArray(),
                DontPreemptNamespaces = envVars.DontPreemptNamespaces.ToArray(),
                DontPreemptTypes = envVars.DontPreemptTypes.ToArray(),
                DontPreemptMethods = envVars.DontPreemptMethods.ToArray(),
            };
            return new ChessTestContext(null, null, null, opts);
        }

        private void do_startup()
        {
            // this includes the executable as s[0], which we don't want
            string[] args = Environment.GetCommandLineArgs();
            Debug.Assert(args.Length >= 2, "Unexpected number of args. args[0]=exe, args[1]=dll to test");
            string[] new_args = args.Skip(2).ToArray();

            if (runUnitTest)
            {
                try
                {
                    // Setup the unit test case
                    unitTestMethod = UnitTestsUtil.FindUnitTestMethodByName(testAssembly, unitTestMethodName, new_args.Length);
                    unitTestCase = UnitTestsUtil.CreateUnitTestCase(unitTestMethod, CreateTestContext(), new_args);
                }
                catch (Exception ex)
                {
                    ReportErrorAndExit(ex.Message, ChessExitCode.TestFailure, false, ex);
                }
            }
            else
            {
                // Use the ChessTest.Startup method
                try
                {
                    if (startup != null)
                    {
                        bool ret = (bool)startup.Invoke(null, new Object[] { new_args });
                        if (!ret)
                            ReportErrorAndExit(testclass + ".Startup returned false.", ChessExitCode.TestFailure, false, null);
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    manager.RunFinalizers();
                }
                catch (Exception ex)
                {
                    if (ex is TargetInvocationException)
                        ex = ((TargetInvocationException)ex).InnerException;

                    string message = testclass + ".Startup threw unexpected exception: " + ex.GetType();
                    ReportErrorAndExit(message, ChessExitCode.UnitTestException, false, ex);
                }
            }
        }

        private void do_run()
        {
            if (runUnitTest)
            {
                object testObject = null;
                try
                {
                    testObject = unitTestCase.CreateNewTestObject();
                }
                catch (Exception ex)
                {
                    if (ex is TargetInvocationException)
                        ex = ((TargetInvocationException)ex).InnerException;

                    ReportErrorAndExit("Unit test threw exception while creating test class.", ChessExitCode.TestFailure, true, ex);
                }

                // Exec the unit test
                MethodInfo unitTestMethod = unitTestCase.Method;
                object[] unitTestArgs = unitTestCase.Arguments;
                try
                {
                    unitTestMethod.Invoke(testObject, unitTestArgs);
                    if (unitTestCase.ExpectedExceptionType != null)
                    {
                        string errMsg = AssertMessagesUtil.FormatAssertionMessage_ExpectedExceptionNotThrown(unitTestCase);
                        ReportErrorAndExit(errMsg, ChessExitCode.UnitTestAssertFailure, true, null);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is TargetInvocationException)
                        ex = ((TargetInvocationException)ex).InnerException;
                    Type exType = ex.GetType();

                    // Detect expected exceptions and handle Assert exceptions
                    if (unitTestCase.ExpectedExceptionType == null || !unitTestCase.ExpectedExceptionType.IsAssignableFrom(exType))
                    {
                        // Handle special UnitTest exceptions
                        if (ex is ConcurrencyUnitTestException)
                        {
                            // Pre-defined exceptions can just pass up their messages
                            if (ex is AssertFailedException)
                                ReportErrorAndExit(ex.Message, ChessExitCode.UnitTestAssertFailure, true, ex);
                            else if (ex is AssertInconclusiveException)
                                ReportErrorAndExit(ex.Message, ChessExitCode.UnitTestAssertInconclusive, true, ex);

#if DEBUG
                            // If there's another ex type that we've defined in the framework but haven't handled
                            // by the prev conditions lets warn the developer.
                            Type cutExType = typeof(ConcurrencyUnitTestException);
                            if (exType != cutExType && exType.Assembly.FullName == cutExType.Assembly.FullName)
                                Trace.TraceWarning("Unhandled ConcurrencyUnitTestException derived type in the CUT assembly: " + exType.Name + ". Custom handling should be added here.");
#endif

                            // If not a built in exception, then use the regular handling below.
                        }

                        // If not an assert, then it's an unexpected exception
                        if (unitTestCase.ExpectedExceptionType == null)
                        {
                            string errMsg = AssertMessagesUtil.FormatAssertionMessage_UnExpectedExceptionThrown(unitTestCase, ex);
                            // Write the exception to the trace log so a debugger can see it.
                            Trace.TraceError(errMsg + Environment.NewLine + "Stack Trace: " + ex.StackTrace);
                            ReportErrorAndExit(errMsg, ChessExitCode.UnitTestException, true, ex);
                        }
                        else // Then the exception isn't what's expected
                        {
                            string errMsg = AssertMessagesUtil.FormatAssertionMessage_ExceptionOfWrongTypeThrown(unitTestCase, ex);
                            // Write the exception to the trace log so a debugger can see it.
                            Trace.TraceError(errMsg + Environment.NewLine + "Stack Trace: " + ex.StackTrace);
                            ReportErrorAndExit(errMsg, ChessExitCode.UnitTestAssertFailure, true, ex);
                        }
                    }
                    // else - The exception was expected, so the test succeeded, keep going
                }
            }
            else
            {
                // Old code for when using the ChessTest class
                try
                {
                    if (!(bool)run.Invoke(null, null))
                    {
                        if (!manager.BreakDeadlockMode)
                            ReportErrorAndExit(testclass + ".Run returned false.", ChessExitCode.TestFailure, true, null);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is TargetInvocationException)
                        ex = ((TargetInvocationException)ex).InnerException;

                    if (!manager.BreakDeadlockMode)
                    {
                        string message = testclass + ".Run raised exception";
                        ReportErrorAndExit(message, ChessExitCode.UnitTestException, true, ex);
                    }
                }
            }

            // Always clean up the memory
            GC.Collect();
            GC.WaitForPendingFinalizers();
            manager.RunFinalizers();
        }

        private void do_shutdown()
        {
            if (shutdown != null)
            {
                try
                {
                    if (!(bool)shutdown.Invoke(null, null))
                        ReportErrorAndExit(testclass + ".Shutdown returned false.", ChessExitCode.TestFailure, false, null);
                }
                catch (Exception ex)
                {
                    if (ex is TargetInvocationException)
                        ex = ((TargetInvocationException)ex).InnerException;

                    string message = testclass + ".Shutdown threw unexpected exception: " + ex.GetType();
                    ReportErrorAndExit(message, ChessExitCode.UnitTestException, false, ex);
                }
            }
        }

    }
}