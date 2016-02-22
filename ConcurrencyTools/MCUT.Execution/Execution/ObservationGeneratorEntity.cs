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
using System.Diagnostics;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Represents a test that generates a Chess observation file.
    /// </summary>
    [AutoRegisterEntity]
    public class ObservationGeneratorEntity : MChessBasedTestEntity
    {

        public static readonly XName EntityXName = XNames.ObservationGenerator;

        #region Static Members

        /// <summary>
        /// Composes the filename for an observation file for the test
        /// </summary>
        /// <param name="testMethod">The test method that's running either as a generator or an observation test.</param>
        /// <param name="fileIdentity">The base fileIdentity of the observation file.</param>
        /// <param name="contextName"></param>
        /// <param name="args">The arguments being passed into the test method.</param>
        /// <returns></returns>
        public static string ComposeObservationFilename(TestMethodEntity testMethod, string fileIdentity, string contextName, TestArgs args)
        {
            string argsNamePart = null;
            if (args != null)
            {
                argsNamePart = UnitTestsUtil.GetTestArgsDisplayText(testMethod, args, false);
                // For now, lets just keep the text the same, just escape any special chars
                var invalidChars = System.IO.Path.GetInvalidFileNameChars()
                    .Concat(new[] { '.', '$', '#' })
                    .Where(c => argsNamePart.IndexOf(c) != -1)
                    .ToArray();
                foreach (var c in invalidChars)
                    argsNamePart = argsNamePart.Replace(c, '_');
            }

            string filename;

            // If there's no context name or args, then just return the fileIdentity w/o annotating it
            if (String.IsNullOrEmpty(contextName) && String.IsNullOrEmpty(argsNamePart))
            {
                filename = fileIdentity;
            }
            else
            {
                // Annotate the file identity with the context name and arguments
                if (String.IsNullOrEmpty(argsNamePart))
                    filename = String.Concat(fileIdentity, ObservationFilenamePartSeparator, contextName);
                else
                    filename = String.Concat(fileIdentity, ObservationFilenamePartSeparator, contextName, ObservationFilenamePartSeparator, args);
            }

            // And finally, append the extension
            return filename + ObservationFileExtension;
        }

        #endregion

        public const string ObservationFileExtension = ".obs";
        public const char ObservationFilenamePartSeparator = '$';

        #region Constructors

        public ObservationGeneratorEntity(XElement el)
            : base(el)
        {
        }

        #endregion

        #region Properties

        public override string TestTypeDisplayName { get { return "Observation Generator"; } }

        public ObservationGranularity Granularity { get { return DataElement.Attribute("Granularity").ParseXmlEnum<ObservationGranularity>().Value; } }

        public string FileIdentity { get { return (string)DataElement.Attribute("FileIdentifier") ?? GetDefaultObservationFileIdentity(); } }

        #endregion

        public override IEnumerable<TestEntity> TestsAndSelf()
        {
            // Even though our subclass (TestEntity) declares us as a test, we aren't a true test
            yield break;
        }

        private string GetDefaultObservationFileIdentity()
        {
            // Use the full name of the test method
            return UnitTestsUtil.GetTestMethodFullName(OwningTestMethod);
        }

        public override void SetBaseMChessOptionsForTestExecution(AppTasks.RunMChessBasedTestTask runTestTask, MChessOptions opts)
        {
            base.SetBaseMChessOptionsForTestExecution(runTestTask, opts);

            opts.EnumerateObservations = true;
            opts.ObservationMode = Granularity.ToChessObservationMode();

            string contextName = runTestTask.TestContext.Name;
            string obsFileName = ComposeObservationFilename(OwningTestMethod, FileIdentity, contextName, runTestTask.TestArgs);
            opts.ObservationFile = obsFileName;
        }

        public bool DoObservationFilesExist()
        {
            return GetGeneratedObservationFiles().Any();
        }

        public IEnumerable<string> GetGeneratedObservationFiles()
        {
            string fileIdentity = FileIdentity; // We just use the FileIdentity property since it handles when it should be the default identity
            string assyFldrPath = OwningTestMethod.OwningClass.OwningAssembly.ObservationFilesFullFolderPath;
            var assyObsFldr = new DirectoryInfo(assyFldrPath);
            if (!assyObsFldr.Exists)
                return Enumerable.Empty<string>();

            return assyObsFldr.EnumerateFileSystemInfos(fileIdentity + "*" + ObservationGeneratorEntity.ObservationFileExtension)
                .Where(fsi => {
                    string filename = Path.GetFileNameWithoutExtension(fsi.Name);
                    return filename == fileIdentity
                        || filename.StartsWith(fileIdentity + ObservationGeneratorEntity.ObservationFilenamePartSeparator);
                })
                .Select(fsi => fsi.FullName);
        }

    }
}
