using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using System.IO;
using System.Diagnostics;
using Microsoft.Concurrency.TestTools.Execution.Chess;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    public class RunUnitTestTask : RunMCutTestCaseAppTask
    {

        public RunUnitTestTask(UnitTestEntity test)
            : base(test)
        {
        }

        public RunUnitTestTask(MCutTestCaseRunEntity run) : base(run) { }

        protected override string TestTypeName { get { return TestTypeNames.UnitTest; } }

        new public UnitTestEntity Test { get { return (UnitTestEntity)base.Test; } }

        protected override XElement CreateXTestCase()
        {
            XElement xtestCase = base.CreateXTestCase();

            // Nothing to add

            return xtestCase;
        }

        protected override void AcceptRunOptions(IRunTestOptions runOptions)
        {
            runOptions.VisitTask(this);
        }

    }
}
