using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.Execution.Chess;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Represents a unit test that runs via the MChessTestRunner in mcut but
    /// just uses the settings in MChess that enable data race detection while
    /// exploring all schedules.
    /// </summary>
    [AutoRegisterEntity]
    public class ObservationTestEntity : MChessBasedTestEntity
    {

        public static readonly XName EntityXName = XNames.ObservationTest;

        #region Constructors

        public ObservationTestEntity(XElement el)
            : base(el)
        {
        }

        #endregion

        #region Properties

        public override string TestTypeDisplayName { get { return "Observation Test"; } }

        public ObservationTestCheckingMode CheckingMode { get { return DataElement.Attribute("CheckingMode").ParseXmlEnum<ObservationTestCheckingMode>().Value; } }

        public string GeneratorFullName { get { return (string)DataElement.Attribute("GeneratorFullName"); } }
        public string FileIdentity { get { return (string)DataElement.Attribute("FileIdentifier"); } }

        public override bool CanRun
        {
            // Only allow us to run the test if we have some observation files created in the temp folder
            get { return base.CanRun && DoObservationFilesExist(); }
        }

        #endregion

        /// <summary>Gets the generator for this test.</summary>
        public ObservationGeneratorEntity GetGenerator()
        {
            if (String.IsNullOrEmpty(GeneratorFullName))
                return null;

            // Find the generator entity
            var generatorMethod = OwningTestMethod.OwningClass.OwningAssembly.FindTestMethod(GeneratorFullName);
            if (generatorMethod == null)
                throw new Exception(String.Format("{0} {1}: Could not find the generator method '{2}' in the same test assembly.", TestTypeDisplayName, OwningTestMethod.MethodName, GeneratorFullName));

            // Find the generator entity
            return generatorMethod.EntityOfType<ObservationGeneratorEntity>();
        }

        /// <summary>
        /// Gets the runtime file identity to use for this test.
        /// </summary>
        /// <returns></returns>
        public string ResolveFileIdentity()
        {
            if (!String.IsNullOrEmpty(FileIdentity))
                return FileIdentity;

            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(GeneratorFullName), "The test list xml should always specify the generator or file identity.");
            var generator = GetGenerator();
            if (generator == null)
                throw new Exception(String.Format("{0} {1}: The method '{2}' is not an Observation Generator.", TestTypeDisplayName, OwningTestMethod.MethodName, GeneratorFullName));

            // Use the generator's identity
            return generator.FileIdentity;
        }

        public override void SetBaseMChessOptionsForTestExecution(AppTasks.RunMChessBasedTestTask runTestTask, MChessOptions opts)
        {
            base.SetBaseMChessOptionsForTestExecution(runTestTask, opts);

            opts.CheckObservations = true;
            opts.ObservationMode = CheckingMode.ToChessObservationMode();

            string fileIdentity = ResolveFileIdentity();
            string contextName = runTestTask.TestContext.Name;
            string obsFileName = ObservationGeneratorEntity.ComposeObservationFilename(OwningTestMethod, fileIdentity, contextName, runTestTask.TestArgs);
            opts.ObservationFile = obsFileName;
        }

        /// <summary>
        /// Gets a value indicating whether any observation files exist for this test by searching in the
        /// test assembly's observation files folder. It does not distinguish whether an observation file
        /// exists for a specific test case.
        /// </summary>
        /// <returns></returns>
        public bool DoObservationFilesExist()
        {
            string fileIdentity = ResolveFileIdentity();
            string assyFldrPath = OwningTestMethod.OwningClass.OwningAssembly.ObservationFilesFullFolderPath;
            var assyObsFldr = new DirectoryInfo(assyFldrPath);
            return assyObsFldr.Exists
                && assyObsFldr.EnumerateFileSystemInfos(fileIdentity + "*" + ObservationGeneratorEntity.ObservationFileExtension)
                    .Any(fsi => {
                        string filename = Path.GetFileNameWithoutExtension(fsi.Name);
                        return filename == fileIdentity
                            || filename.StartsWith(fileIdentity + ObservationGeneratorEntity.ObservationFilenamePartSeparator);
                    });
        }

    }
}
