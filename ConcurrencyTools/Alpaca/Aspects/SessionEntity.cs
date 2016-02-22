using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using Microsoft.Concurrency.TestTools;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using Microsoft.Concurrency.TestTools.Execution.AppTasks;

namespace Microsoft.Concurrency.TestTools.Alpaca.Aspects
{
    /// <summary>Represents the user's current session.</summary>
    class SessionEntity : EntityBase, ISessionEntity
    {

        public const int MaxAssyNameLength = 20;
        private const string TempFolderName = "session.tmp";
        private const string ObservationFilesFolderName = "obs";
        private const string TaskFoldersFolderName = "tasks";

        SessionRuntimeState _state;

        public SessionEntity(XElement el)
            : base(el)
        {
        }

        public override string DisplayName { get { return "Session"; } }

        public SessionRuntimeState RuntimeState
        {
            get
            {
                if (_state == null)
                {
                    XElement xstate = DataElement.Element(XSessionNames.SessionState);
                    if (xstate == null)
                    {
                        xstate = new XElement(XSessionNames.SessionState);
                        DataElement.AddFirst(xstate);
                    }

                    _state = Model.EntityBuilder.EnsureEntityBound<SessionRuntimeState>(xstate);
                }

                return _state;
            }
        }

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            // NOTE: We purposely don't include the SessionState element
            return DataElement.Elements(
                XNames.Testlist
                , XNames.TestProject
                , XNames.TestAssembly
                );
        }

        public override IEnumerable<TaskRunEntity> DescendantRuns()
        {
            // We don't contain runs our self and we don't know the exact type of the xrun
            // so we ignore the DescendantXRuns and just go thru the child entities.
            return GetChildEntities()
                .SelectMany(childEntity => childEntity.DescendantRunsAndSelf());
        }

        internal void InitializeSessionEntity(string sessionFilePath)
        {
            FolderPath = Path.GetDirectoryName(sessionFilePath);
            BaseTasksFolderRelativePath = Path.Combine(TempFolderName, TaskFoldersFolderName);
            BaseObservationsFolderRelativePath = Path.Combine(TempFolderName, ObservationFilesFolderName);
        }

        protected override void OnChildrenLoaded()
        {
            base.OnChildrenLoaded();

            // Find the highest entity id
            var existingIDs = from x in DataElement.Descendants()
                              let xid = x.Attribute(XSessionNames.AEntityID)
                              where xid != null
                              select (int)xid;
            if (existingIDs.Any())
                RuntimeState.InitializeNextEntityID(existingIDs.Max());

            // Find the highest task id
            if (DescendantRuns().Any())
                RuntimeState.InitializeNextTaskID(DescendantRuns().Max(r => r.TaskID));

            // Initialize the Run objects with the RunEntities
            foreach (var run in DescendantRuns())
            {
                if (run.Task == null)
                {
                    // Re-create the task from the run entity
                    run.AssociateTaskFromRun(run);
                }
            }
        }

        /// <summary>Add the test list to the session.</summary>
        internal void AddTestContainer(string containerFilePath)
        {
            // Create an include element for this container
            var xinclude = IncludeEntity.CreateXInclude(containerFilePath);
            this.AddEntity<IncludeEntity>(xinclude);
        }

        #region ISessionEntity Members

        /// <summary>Gets the full path to the session's working folder (NOT to the temp folder).</summary>
        /// <remarks>This is a runtime property. i.e. not saved to xml.</remarks>
        public string FolderPath { get; private set; }
        public string BaseTasksFolderRelativePath { get; private set; }
        public string BaseTasksFolderPath { get { return Path.Combine(FolderPath, BaseTasksFolderRelativePath); } }
        public string BaseObservationsFolderRelativePath { get; private set; }
        public string BaseObservationsFolderPath { get { return Path.Combine(FolderPath, BaseObservationsFolderRelativePath); } }

        /// <summary>
        /// Registers a test assembly within this session.
        /// </summary>
        /// <param name="testAssembly"></param>
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
            if (testAssembly.Descendants<ObservationGeneratorEntity>().Any())
            {
                // Make sure the observations folder is specified
                string obsFolderPath = testAssembly.ObservationFilesFolderPath;
                if (String.IsNullOrEmpty(obsFolderPath))
                {
                    obsFolderPath = Path.Combine(BaseObservationsFolderRelativePath, "testassy_" + entityID);
                    testAssembly.ObservationFilesFolderPath = obsFolderPath;
                }
            }
        }

        public int GetNextTaskID()
        {
            return RuntimeState.NextTaskID++;
        }

        public int GetNextEntityID()
        {
            return RuntimeState.NextEntityID++;
        }

        #endregion

    }

    class SessionRuntimeState : EntityBase
    {

        public const int DefaultMaxConcurrentTasks = 4;

        public static readonly XName XName_ANextTaskID = "nextTaskID";
        public static readonly XName XName_ANextEntityID = "nextEntityID";
        public static readonly XName XName_AConfirmAutoRefresh = "confirmAutoRefresh";
        public static readonly XName XName_AMaxConcurrentTasks = "maxConcurrentTasks";
        public static readonly XName XName_AEnableRegressionTestingMode = "enableRegressionTestingMode";
        public static readonly XName XName_AUseGoldenObservationFiles = "useGoldenObservationFiles";

        public SessionRuntimeState(XElement el)
            : base(el)
        {
        }

        protected override IEnumerable<XElement> GetChildEntityElements() { yield break; }

        public override bool RaisesEntityChangedEvents { get { return false; } }

        /// <summary>
        /// Indicates whether when detecting a source file has changed if the UI
        /// should confirm with the user whether to refresh it.
        /// </summary>
        public bool ConfirmAutoRefresh
        {
            get { return (bool?)DataElement.Attribute(XName_AConfirmAutoRefresh) ?? true; }
            set { DataElement.SetAttributeValue(XName_AConfirmAutoRefresh, value); }
        }

        public bool EnableRegressionTestingMode
        {
            get { return (bool?)DataElement.Attribute(XName_AEnableRegressionTestingMode) ?? false; }
            set { DataElement.SetAttributeValue(XName_AEnableRegressionTestingMode, value); }
        }

        /// <summary>
        /// Indicates whether to use the Golden Observation Files folders if an assembly defines one
        /// instead of the session's temp folder.
        /// </summary>
        public bool UseGoldenObservationFiles
        {
            get { return (bool?)DataElement.Attribute(XName_AUseGoldenObservationFiles) ?? false; }
            set { DataElement.SetAttributeValue(XName_AUseGoldenObservationFiles, value); }
        }

        /// <summary>
        /// Gets the latest backend that was selected.
        /// Returns an empty string if not set.
        /// </summary>
        public int MaxConcurrentTasks
        {
            get { return (int?)DataElement.Attribute(XName_AMaxConcurrentTasks) ?? DefaultMaxConcurrentTasks; }
            set { DataElement.SetAttributeValue(XName_AMaxConcurrentTasks, value); }
        }

        public int NextTaskID
        {
            get { return (int?)DataElement.Attribute(XName_ANextTaskID) ?? 1; }
            set { DataElement.SetAttributeValue(XName_ANextTaskID, value); }
        }

        public int NextEntityID
        {
            get { return (int?)DataElement.Attribute(XName_ANextEntityID) ?? 1; }
            set { DataElement.SetAttributeValue(XName_ANextEntityID, value); }
        }

        public void ResetTaskCounter()
        {
            DataElement.SetAttributeValue(XName_ANextTaskID, null);
            //DataElement.SetAttributeValue(XName_ANextEntityID, null);
        }

        internal void InitializeNextTaskID(int highestTaskID)
        {
            int nextID = NextTaskID;
            if (nextID <= highestTaskID)
                NextTaskID = highestTaskID + 1;
        }

        internal void InitializeNextEntityID(int highestEntityID)
        {
            int nextID = NextEntityID;
            if (nextID <= highestEntityID)
                NextEntityID = highestEntityID + 1;
        }

    }
}
