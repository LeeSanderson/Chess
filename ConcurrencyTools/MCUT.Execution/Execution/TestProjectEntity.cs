using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using System.Text.RegularExpressions;
using Microsoft.Concurrency.TestTools.Execution.MSBuild;

namespace Microsoft.Concurrency.TestTools.Execution
{
    [AutoRegisterEntity]
    public class TestProjectEntity : TestGroupingEntity, IBuildableEntity, IMSBuildSettings
    {
        public static readonly XName EntityXName = XNames.TestProject;

        private MSBuildProject _msbuildProj;

        public TestProjectEntity(XElement el)
            : base(el)
        {
            var xmsbProj = DataElement.Element(XSessionMSBuildNames.Project);
            if (xmsbProj != null)
            {
                _msbuildProj = new MSBuildProject(xmsbProj);

                // Load the current configuration
                UpdateProjectConfiguration();
            }

            // WARNING: Can't do this here because we may trigger the xml tree changed event on itself.
            //// Make sure the loadTestAssembly container element is there
            //if (DataElement.Element(XSessionNames.LoadedTestAssembly) == null)
            //{
            //    if (xmsbProj == null)
            //        DataElement.Add(new XElement(XSessionNames.LoadedTestAssembly));
            //    else
            //        xmsbProj.AddBeforeSelf(new XElement(XSessionNames.LoadedTestAssembly));
            //}

            //LoadProjectFile();
        }

        #region Properties

        public string SourceFilePath { get { return DataElement.Attribute(XNames.ALocation).Value; } }

        /// <summary>Indicates whether the project file has been loaded.</summary>
        public bool IsProjectFileLoaded { get { return _msbuildProj != null; } }

        /// <summary>Indicates whether there was an error loading the project file.</summary>
        public bool HasProjectLoadError { get { return DataElement.Elements(XConcurrencyNames.Error).Any(); } }

        /// <summary>Gets the error that occurred while trying to load the project file.</summary>
        public ErrorEntity ProjectLoadError { get { return this.EntityOfType<ErrorEntity>(); } }

        public override string DisplayName
        {
            get { return IsProjectFileLoaded ? _msbuildProj.AssemblyName : Path.GetFileNameWithoutExtension(SourceFilePath); }
        }

        /// <summary>
        /// The name of the configuration to use. If null, then the default configuration is used.
        /// </summary>
        public string Configuration
        {
            get { return (string)DataElement.Attribute(XNames.AConfiguration); }
            set
            {
                if (!IsProjectFileLoaded)
                    throw new InvalidOperationException("Cannot set the configuration unless the project file has been loaded.");

                if (value != Configuration)
                {
                    if (value != null && !_msbuildProj.Configurations.Any(cfg => cfg.Configuration == Configuration))
                        throw new ArgumentException(String.Format("{0}: The configuration '{1}' wasn't found.", Path.GetFileName(SourceFilePath), Configuration));

                    DataElement.SetAttributeValue(XNames.AConfiguration, value);
                    OnConfigurationChanged();
                }
            }
        }

        /// <summary>The current project configuration being used.</summary>
        public MSBuildProjectConfiguration ProjectConfiguration { get; private set; }

        /// <summary>
        /// Gets the relative folder path (relative to the SourceFilePath) to the golden
        /// observation files folder for this test assembly.
        /// </summary>
        public string GoldenObservationFilesFolderPath { get { return (string)DataElement.Element(XNames.GoldenObservationFilesFolderPath); } }

        /// <summary>Gets the full path to the observation files for this assembly.</summary>
        public string GoldenObservationFilesFullFolderPath
        {
            get
            {
                string goldenFldrPath = GoldenObservationFilesFolderPath;
                if (String.IsNullOrEmpty(goldenFldrPath))
                    return null;
                return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(SourceFilePath), goldenFldrPath));
            }
        }

        public TestAssemblyEntity TestAssembly { get { return this.EntityOfType<TestAssemblyEntity>(); } }

        #endregion

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            // Start with the build runs
            IEnumerable<XElement> children;
            if (_msbuildProj == null)
                children = Enumerable.Empty<XElement>();
            else
                children = _msbuildProj.DataElement.Elements(XSessionNames.BuildRun);

            return children
                .Concat(DataElement
                .Elements(
                    XNames.TestAssembly
                    , XNames.Include    // For loading the test assembly once we know what it's full location is from the project file
                    , XNames.Placeholder
                    , XConcurrencyNames.Error   // Failure to load the project file
                ));
        }

        public override IEnumerable<TaskRunEntity> DescendantRuns()
        {
            // We don't contain runs our self and we don't know the exact type of the xrun
            // so we ignore the DescendantXRuns and just go thru the child entities.
            return GetChildEntities()
                .SelectMany(childEntity => childEntity.DescendantRunsAndSelf())
                ;
        }

        public override string GetInvocationDetails()
        {
            return CopyDataElement(DataElement, true, XNames.TestAssembly)
                .ToString();
        }

        public bool TryLoadProjectFile()
        {
            // Remove our previous error
            // It's not in the try-catch because any error is due to other code, not due to a loading failure.
            if (ProjectLoadError != null)
                ProjectLoadError.DataElement.Remove();

            XElement xmsbProj = null;
            XElement xerror = null;
            try
            {
                xmsbProj = MSBuildProject.ParseProjectFileToXElement(SourceFilePath);
                if (xmsbProj == null)
                    xerror = XNames.CreateXError("Project file could not be found.");

                // Replace existing element or add it
                // Note: The element isn't an entity
                if (_msbuildProj != null)
                {
                    // We replace so the order of the child elements are preserved (i.e. if a TestAssembly el also exists)
                    Debug.Assert(_msbuildProj.DataElement.Parent == this.DataElement);

                    // move the previous runs to the new project element
                    var prevRuns = _msbuildProj.DataElement.Elements(XSessionNames.BuildRun).ToArray();
                    foreach (var prevRun in prevRuns)
                    {
                        prevRun.Remove();
                        xmsbProj.Add(prevRun);
                    }

                    //
                    _msbuildProj.DataElement.ReplaceWith(xmsbProj);
                }
                else
                    DataElement.Add(xmsbProj);

                // create the new one
                _msbuildProj = new MSBuildProject(xmsbProj);
            }
            catch (Exception ex)
            {
                xerror = XNames.CreateXError("An error occurred trying to parse the project file.", ex);
            }

            if (xerror != null)
            {
                // Clear out the existing extra elements if they exist
                var elementsToRemove = DataElement.Elements(XSessionMSBuildNames.Project, XNames.TestAssembly, XNames.Include);
                foreach (var xel in elementsToRemove)
                    xel.Remove();

                // Add the error and then eject
                this.AddEntity<ErrorEntity>(xerror);
                return false;
            }

            // Make sure we use the correct configuration name
            if (Configuration != null && !_msbuildProj.Configurations.Any(cfg => cfg.Configuration == Configuration))
                Configuration = null; // Reset to use the default configuration
            else  // Call the changed event manually so we get a new ProjectConfiguration obj that's part of the new MSBuildProject instance
                OnConfigurationChanged();

            return true;
        }

        private void UpdateProjectConfiguration()
        {
            // If the Configuration property is set to null, than we use the default
            string configuration = this.Configuration ?? _msbuildProj.DefaultConfiguration;
            string cfgKey = MSBuildProjectConfiguration.CreateConfigurationKey(configuration, _msbuildProj.DefaultPlatform);

            ProjectConfiguration = _msbuildProj.Configurations.Single(cfg => cfg.Key == cfgKey);
            Debug.Assert(ProjectConfiguration != null);
        }

        private void OnConfigurationChanged()
        {
            UpdateProjectConfiguration();

            // Now, determine whether the TestAssembly entity is still valid.
            string testAssyPath = Path.GetFullPath(Path.Combine(_msbuildProj.ProjectFolderPath, ProjectConfiguration.OutputFolderPath, _msbuildProj.OutputFilename));
            bool addIncludeAssy = true;
            var existingAssy = this.TestAssembly;
            if (existingAssy != null)
            {
                // If the assembly path has changed then we'll need to import the new one
                if (existingAssy.SourceFilePath.Equals(testAssyPath, StringComparison.OrdinalIgnoreCase))
                    addIncludeAssy = false;
                else
                    existingAssy.DataElement.Remove();
            }

            // If an include entity already exists, only re-create it if needed.
            var assyInclude = this.EntityOfType<IncludeEntity>();
            if (assyInclude != null)
            {
                if (assyInclude.TestContainerLocation.Equals(testAssyPath, StringComparison.OrdinalIgnoreCase))
                    addIncludeAssy = false;
                else
                    assyInclude.DataElement.Remove();
            }

            if (addIncludeAssy)
            {
                // Create a new testAssembly stub
                var xinclude = IncludeEntity.CreateXInclude(testAssyPath);
                this.AddEntity<IncludeEntity>(xinclude);
            }
        }


        #region IBuildableEntity Members

        bool IBuildableEntity.SupportsRebuild { get { return true; } }
        bool IBuildableEntity.IsBuildable { get { return IsProjectFileLoaded; } }

        public AppTasks.BuildAppTaskBase CreateBuildTask()
        {
            return new AppTasks.MSBuildAppTask() {
                MSBuildSettings = this,
                XParent = _msbuildProj.DataElement
            };
        }

        #endregion

        #region IMSBuildSettings Members

        string IMSBuildSettings.ProjectFullPath { get { return _msbuildProj.ProjectFilePath; } }

        string IMSBuildSettings.Configuration { get { return ProjectConfiguration.Configuration; } }

        string IMSBuildSettings.Platform { get { return ProjectConfiguration.Platform; } }

        string IMSBuildSettings.OtherProperties { get { return null; } }

        #endregion
    }
}
