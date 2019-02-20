# CHESS #

CHESS is a open source tool created by **[Microsoft Research](http://research.microsoft.com/en-us/projects/chess/)** for systematic and disciplined testing of concurrent programs. 

CHESS repeatedly runs a concurrent test ensuring that every run takes a different interleaving. If an interleaving results in an error, CHESS can reproduce the interleaving for improved debugging. 

CHESS is available for both managed and native programs.

This repository is a copy of the [original CHESS Codeplex repository](https://chesstool.codeplex.com/) which seems to have been inactive since 2013. The [latest thread](https://chesstool.codeplex.com/discussions/450787) on the Codeplex seems to indicate that  Microsoft were intending to incorporate CHESS inside the next version of Visual Studio (although this does not seem to have happened):

> There was a rumour running around that it would become a part of VS 2012, but I'm not sure. Think it was decided to shift it to the next release 

## Status ##

The code here can be compiled using [Visual Studio 2015 Community edition](https://go.microsoft.com/fwlink/?LinkId=691978&clcid=0x409) or [Microsoft Build Tools 2015](https://www.microsoft.com/en-us/download/details.aspx?id=48159). 

Some minor modifications have been made to the [original CHESS Codeplex repository](https://chesstool.codeplex.com/) in order to get the code to compile. These change include:

1. w32chess projects have been removed from the solution (although they still exist in the repository). These projects build tools to run CHESS against native programs and have been removed because of problems getting them to compile using the Visual Studio 2015 C++ compiler.

2. The CopyBin project has been removed from the solution (although it still exist in the repository). This project contains some pre and post build scripts that register the newly compiled version of CHESS in the GAC and registry. These scripts cause the build to fail with UAC errors unless the build is run under an administrator account. The functionality of these scripts is being migrated to the NANT script.  


## Documentation ##

There is lots of documentation about CHESS on [Codeplex](https://chesstool.codeplex.com/documentation) and on the [CHESS Microsoft Research pages](http://research.microsoft.com/en-us/projects/chess/).

## Installation ##

CHESS can be easily installed in a project using [NuGet](https://www.nuget.org/packages/Chess/).

> Install-Package Chess

The NuGet package include both a .net 4.0 library for defining concurrency unit tests (Microsoft.Concurrency.UnitTestingFramework.dll) as well as tools for executing these tests (mcut.exe). 

Once the package is installed you need to run the regClrMonitor.bat batch file so that the CLR profiler that is used to run/monitor your code. This needs to be run from an administrative command prompt.

## Writing unit tests ##


```csharp
    [TestFixture]
    public class TestClass
    {
        /// <summary>
        /// Test we can read while updating.
        /// </summary>
        [Test]
        [DataRaceTestMethod]
        public void TestMethod()
        {
          // 
        }
    }
```
In the example above we decorate the class with a NUnit **[TestFixture]** attribute and the test method with a NUnit **[Test]** attribute so that we can run the test from a test runner (you could just as easily use any other unit testing framework e.g. [MS test](https://msdn.microsoft.com/en-us/library/ms243147.aspx) or [XUnit](https://xunit.github.io/)). 

Then we add a **[ScheduleTestMethod]** attribute or a **[DataRaceTestMethod]** attribute from the Microsoft.Concurrency.UnitTestingFramework namespace. 

1. The **[ScheduleTestMethod]** attribute marks a test method for concurrent testing. Chess will execute the test multiple time interleaving the different threads of the code in an attempt to find common concurrency bugs e.g. deadlocks.

2. The **[DataRaceTestMethod]** attribute marks also marks a test method for concurrent testing in the same way as the **[ScheduleTestMethod]** attribute but additional tests are performed to detect race conditions - when two concurrent threads access the same memory location and one of those accesses is a write.


## Running unit tests ##

To run tests use the '*mcut*' command e.g.

> mcut runAllTests [path to your concurrency unit test assembly]

*NOTE: You must run mcut from an administrative command prompt (failure to do this can result in a -667 error code)*

## Examples ##
See my [Concurrency](https://github.com/LeeSanderson/Concurrency) project for a full example including [NANT](http://nant.sourceforge.net/) script that builds the project and executes the concurrency tests. 
