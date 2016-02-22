using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    public interface IRunTestOptions
    {
        //void VisitTask<T>(T runTestTask) where T : AppTask, IRunTestTask;
        void VisitTask(RunMCutTestCaseAppTask runTestTask);
        void VisitTask(RunMChessTestTask runTestTask);
        void VisitTask(RunMChessBasedTestTask runTestTask);
    }
}
