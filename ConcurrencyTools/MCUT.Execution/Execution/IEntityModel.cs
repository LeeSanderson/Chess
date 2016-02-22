using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Represents the base contract that a model managing entities should implement.
    /// </summary>
    public interface IEntityModel
    {

        EntityBuilderBase EntityBuilder { get; }

        ISessionEntity Session { get; }

    }
}
