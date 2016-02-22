using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Execution
{
    public interface IBuildableEntity : IEntity
    {
        /// <summary>Indicates whether this entity can be built given its current state.</summary>
        bool IsBuildable { get; }
        bool SupportsRebuild { get; }
        AppTasks.BuildAppTaskBase CreateBuildTask();
    }
}
