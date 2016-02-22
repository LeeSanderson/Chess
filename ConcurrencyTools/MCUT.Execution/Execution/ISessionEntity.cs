using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Represents the base contract that a testing session should implement.
    /// </summary>
    public interface ISessionEntity
    {

        /// <summary>Gets the full folder path for the current session.</summary>
        string FolderPath { get; }

        /// <summary>Gets the path (relative to the FolderPath) to where all task folders should be created.</summary>
        string BaseTasksFolderRelativePath { get; }

        /// <summary>Gets the full path to where all task folders should be created.</summary>
        string BaseTasksFolderPath { get; }

        /// <summary>Gets the path (relative to the FolderPath) to where all temp observation folders should be created.</summary>
        string BaseObservationsFolderRelativePath { get; }

        /// <summary>Gets the full path to where all temp observation folders should be created.</summary>
        string BaseObservationsFolderPath { get; }

        /// <summary>Gets the next task id available in the session.</summary>
        int GetNextTaskID();

        /// <summary>
        /// Registers a test assembly with the session and assigns it any session-time
        /// state that's needed to run tests.
        /// </summary>
        void RegisterTestAssembly(TestAssemblyEntity testAssembly);

    }
}
