using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Execution.AppTasks
{
    public delegate void AppTaskEventHandler(AppTask task, EventArgs e);
    public delegate void AppTaskEventHandler<T>(AppTask task, T e) where T : EventArgs;
}
