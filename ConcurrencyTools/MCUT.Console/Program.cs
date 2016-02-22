using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution;
using System.Text.RegularExpressions;

namespace Microsoft.Concurrency.TestTools.UnitTesting.MCutConsole
{
    class Program
    {

        // Flags and options
        private static bool showLogo = true;
        private static string resultFilePath = "testResult.xml";

        #region Main() and cmd-line handling

        enum ProgramCommand
        {
            NotSpecified,
            ShowUsage,
            GetTestListFromAssembly,
            RunAllTests,
            RunTestCase,
        }

        static void PrintUsage(TextWriter outStream)
        {
            outStream.WriteLine(@"Usage: {0} <command> [inputs and options]
Commands:
help | /?
    Displays usage.

getTestListFromAssembly <testAssembly>
    Writes the Concurrent Unit Testing information contained in an assembly to
    standard out.

runAllTests [<testAssembly> | <testList>] [/maxConcurrentTests <num>]
            [/classNameFilter <regexPattern>]
            [/testMethodNameFilter <regexPattern>]
            [/testTypeNameFilter <regexPattern>]
    Runs all tests in the specified container file.
    
runTestCase [<descriptionFile>] [/resultFile <filePath>]
    Runs a single test given the specified test description file. If the
    descriptionFile isn't specified, than the standard input stream is assumed
    to contain the description xml.

Inputs and Options:
<testAssembly>      The path to the test assembly to read. This would be either
                    a dll or exe.
<testList>          The path to the xml testlist file to use.
<descriptionFile>   Path to the xml file that describes the test case.
/maxConcurrentTests Specifies the maximum number of tests that can be executing
                    concurrently.

The following options use a regex pattern to filter what tests to run:
/classNameFilter    Filters on the full class name.
/testMethodNameFilter
                    Filters on the test method name.
/testTypeNameFilter Filters on the name of the test type.
"
                , Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                );
        }

        static int Main(string[] args)
        {
            ProgramCommand cmd;
            string errMsg = null;
            string inputFilePath = null;
            Regex classNameFilter = null;
            Regex testMethodNameFilter = null;
            Regex testTypeNameFilter = null;
            int maxConcurrentTests = 1;

            if (args.Length == 0)
            {
                cmd = ProgramCommand.NotSpecified;
                errMsg = "No command specified.";
            }
            else
            {
                string commandTxt = args[0];
                int argIdx = 1; // 0 = command

                if (String.Equals(commandTxt, "help", StringComparison.OrdinalIgnoreCase)
                    || String.Equals(commandTxt, "/?", StringComparison.OrdinalIgnoreCase))
                {
                    cmd = ProgramCommand.ShowUsage;
                    argIdx = args.Length;// Ignore any other args
                }
                else if (String.Equals(commandTxt, ProgramCommand.GetTestListFromAssembly.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    cmd = ProgramCommand.GetTestListFromAssembly;
                    if (argIdx >= args.Length)
                        errMsg = "<testAssembly> not specified.";

                    showLogo = false;
                    inputFilePath = args[argIdx++];
                }
                else if (String.Equals(commandTxt, ProgramCommand.RunAllTests.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    cmd = ProgramCommand.RunAllTests;
                    if (argIdx >= args.Length)
                        errMsg = "no tests container specified.";

                    inputFilePath = args[argIdx++];

                    // Now read any options
                    while (argIdx < args.Length && errMsg == null)
                    {
                        if (args[argIdx].StartsWith("/"))
                        {
                            string optionName = args[argIdx++].Substring(1);
                            switch (optionName)
                            {
                                case "classNameFilter":
                                    classNameFilter = ParseCommandlineRegexPattern(optionName, args[argIdx++], ref errMsg);
                                    break;
                                case "testMethodNameFilter":
                                    testMethodNameFilter = ParseCommandlineRegexPattern(optionName, args[argIdx++], ref errMsg);
                                    break;
                                case "testTypeNameFilter":
                                    testTypeNameFilter = ParseCommandlineRegexPattern(optionName, args[argIdx++], ref errMsg);
                                    break;
                                case "maxConcurrentTests":
                                    maxConcurrentTests = int.Parse(args[argIdx++]);
                                    break;

                                default: errMsg = "Unrecognized option: " + optionName; break;
                            }
                        }
                        else
                            errMsg = "Invalid option: " + args[argIdx];
                    }
                }
                else if (String.Equals(commandTxt, ProgramCommand.RunTestCase.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    cmd = ProgramCommand.RunTestCase;
                    // The input file is optional
                    if (argIdx < args.Length && !args[argIdx].StartsWith("/"))
                        inputFilePath = args[argIdx++];

                    // Now read any options
                    while (argIdx < args.Length && errMsg == null)
                    {
                        if (args[argIdx].StartsWith("/"))
                        {
                            string optionName = args[argIdx++].Substring(1);
                            switch (optionName)
                            {
                                case "resultFile": resultFilePath = args[argIdx++]; break;

                                default: errMsg = "Unrecognized option: " + optionName; break;
                            }
                        }
                        else
                            errMsg = "Invalid option: " + args[argIdx];
                    }
                }
                else
                {
                    cmd = ProgramCommand.NotSpecified;
                    errMsg = "Invalid usage.";
                }

                if (cmd != ProgramCommand.NotSpecified && argIdx < args.Length && errMsg != null)
                {
                    errMsg = "Invalid usage. Unrecognized options.";
                }
            }

            // Handle incorrect usages first
            if (errMsg != null)
            {
                Console.Error.WriteLine(errMsg);
                PrintUsage(Console.Out);
                return -1;
            }
            System.Diagnostics.Debug.Assert(cmd != ProgramCommand.NotSpecified, "If NotSpecified, then an errMsg should have also been set.");

            if (showLogo)
                WriteLogo();

            switch (cmd)
            {
                case ProgramCommand.NotSpecified:
                    Console.Error.WriteLine("No command specified.");
                    PrintUsage(Console.Out);
                    return -1;

                case ProgramCommand.ShowUsage:
                    PrintUsage(Console.Out);
                    return 0;

                case ProgramCommand.GetTestListFromAssembly:
                    return WriteTestAssemblyXml(inputFilePath);

                case ProgramCommand.RunAllTests:
                    return RunAllTests(inputFilePath, maxConcurrentTests, classNameFilter, testMethodNameFilter, testTypeNameFilter);

                case ProgramCommand.RunTestCase:
                    return RunTestCase(inputFilePath);

                default:
                    throw new NotImplementedException("ProgramCommand not implemented: " + cmd.ToString());
            }

        }

        private static Regex ParseCommandlineRegexPattern(string optionName, string pattern, ref string errMsg)
        {
            try
            {
                return new Regex(pattern, RegexOptions.ECMAScript);
            }
            catch
            {
                errMsg = String.Format("  {0}: The regular expression pattern is invalid.", optionName);
                return null;
            }
        }

        #endregion

        private static void WriteLogo()
        {
            Console.WriteLine("Concurrency Unit Testing (MCUT) Console.");
            Console.WriteLine("Copyright (C) Microsoft Corporation, 2010.");
        }

        private static int WriteTestAssemblyXml(string assemblyFilePath)
        {
            XElement xtestAssy = null;
            try
            {
                var assyReader = new TestAssemblyReader(assemblyFilePath);
                xtestAssy = assyReader.Read();
            }
            catch (Exception ex)
            {
                xtestAssy = XNames.CreateXError(ex);
            }

            using (XmlWriter w = XmlWriter.Create(Console.Out))
            {
                if (xtestAssy != null)
                    xtestAssy.WriteTo(w);
            }

            return 0;
        }

        private static int RunAllTests(string testContainerFilePath, int maxConcurrentTests, Regex classNameFilter, Regex testMethodNameFilter, Regex testTypeNameFilter)
        {
            try
            {
                Model.Instance.InitializeSession(Path.Combine(Environment.CurrentDirectory, "session.xml"));
                var engine = new RunTestsEngine(testContainerFilePath) {
                    ResultsFilePath = "allResults.xml",
                    MaxConcurrentTests = maxConcurrentTests,
                    UseGoldenObservationFiles = true,   // For now, lets default to using golden files if available
                    ClassNameFilter = classNameFilter,
                    TestMethodNameFilter = testMethodNameFilter,
                    TestTypeNameFilter = testTypeNameFilter,
                };
                engine.RunContainer();

                int exitCode = engine.TotalTestCasesCount == 0 ? -1 : engine.FailedCount; // Note, we ignore inconclusive failures wrt exit code
                return exitCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Error running all tests: " + ex.ToString());
                Console.Error.WriteLine("Error running all tests: " + ex.Message);
                Console.WriteLine("Finished.");
                return -1;
            }
        }

        private static int RunTestCase(string testCaseFilePath)
        {
            TestResultEntity result = RunTestCaseGetResult(testCaseFilePath);

            // Print result to std err no matter what
            Console.Error.WriteLine();
            Console.Error.WriteLine("MCUT Test Case Result: {0}", result.ResultType);
            Console.Error.WriteLine(result.Message);

            // Write the result to the output file
            try
            {
                using (XmlWriter xwriter = XmlWriter.Create(resultFilePath, new XmlWriterSettings() { Indent = true }))
                {
                    result.DataElement.WriteTo(xwriter);
                }

                return result.ExitCode;
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine("Error writing to result xml file: " + ex.ToString());
                return (int)TestResultType.Error;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error with test result xml: " + ex.ToString());
                return (int)TestResultType.Error;
            }
        }

        private static TestResultEntity RunTestCaseGetResult(string testCaseFilePath)
        {
            XElement xtestResults;

            var engine = new TestCaseRunnerEngine();
            if (!engine.LoadTestCase(testCaseFilePath))
            {
                // Report these errors as is since we can't load the test case
                xtestResults = TestResultUtil.CreateErrorXTestResult(engine.Error, engine.ErrorEx);
                return Model.Instance.EntityBuilder.EnsureEntityBound<TestResultEntity>(xtestResults);
            }

            xtestResults = engine.RunTestCase();
            if (xtestResults == null)
                xtestResults = TestResultUtil.CreateErrorXTestResult("RunTestCaseGetResult returned a null xml element.");

            TestResultEntity testResult = Model.Instance.EntityBuilder.EnsureEntityBound<TestResultEntity>(xtestResults);

            testResult = engine.PreProcessResults(testResult);
            testResult = engine.ProcessExpectedResultAssertions(testResult);

            // Lastly, do the assertions for regression tests if the TestCase defines it.
            testResult = engine.ProcessRegressionTestAsserts(testResult);

            return testResult;
        }

    }
}
