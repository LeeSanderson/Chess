using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.Execution;
using System.IO;

namespace Microsoft.Concurrency.TestTools.UnitTesting.MCutConsole
{
    class SessionEntity : ISessionEntity
    {

        private const string TempFolderName = "session.tmp";
        private const string ObservationFilesFolderName = "obs";
        private const string TaskFoldersFolderName = "tasks";

        private int _nextTaskID = 1;
        private int _nextEntityID = 1;

        internal SessionEntity(string sessionFilePath)
        {
            FolderPath = Path.GetDirectoryName(sessionFilePath);
            BaseTasksFolderRelativePath = Path.Combine(TempFolderName, TaskFoldersFolderName);
            BaseObservationsFolderRelativePath = Path.Combine(TempFolderName, ObservationFilesFolderName);
        }

        public string FolderPath { get; private set; }
        public string BaseTasksFolderRelativePath { get; private set; }
        public string BaseTasksFolderPath { get { return Path.Combine(FolderPath, BaseTasksFolderRelativePath); } }
        public string BaseObservationsFolderRelativePath { get; private set; }
        public string BaseObservationsFolderPath { get { return Path.Combine(FolderPath, BaseObservationsFolderRelativePath); } }

        public int GetNextTaskID()
        {
            return _nextTaskID++;
        }

        public int GetNextEntityID()
        {
            return _nextEntityID++;
        }

        public void RegisterTestAssembly(TestAssemblyEntity testAssembly)
        {
            // Make sure an entity id is set
            int entityID = testAssembly.EntityID;
            if (entityID == 0)
            {
                entityID = GetNextEntityID();
                testAssembly.EntityID = entityID;
            }

            // For assemblies that generate observation files, create the temp folder
            if (testAssembly.IsTestOrHasTestsOfType<ObservationGeneratorEntity>())
            {
                // Make sure the observations folder is specified
                string obsFolderPath = testAssembly.ObservationFilesFolderPath;
                if (String.IsNullOrEmpty(obsFolderPath))
                {
                    obsFolderPath = Path.Combine(BaseObservationsFolderPath, "testassy_" + entityID);
                    testAssembly.ObservationFilesFolderPath = obsFolderPath;
                }
            }
        }

    }
}
