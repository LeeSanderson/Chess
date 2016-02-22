using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution.Chess;
using System;
using System.Xml;
using Microsoft.Concurrency.TestTools.Execution.Xml;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Represents a run of an mcut test case.
    /// </summary>
    [AutoRegisterEntity]
    public class MCutTestCaseRunEntity : TestRunEntity
    {

        public static readonly XName EntityXName = XSessionNames.MCutTestRun;

        #region Constructors

        public MCutTestCaseRunEntity(XElement el)
            : base(el)
        {
        }

        #endregion

        public override string DisplayName { get { return String.Format("Run {0}: {1}", OwningTest.TestTypeDisplayName, TestCase); } }

        new public MCutTestEntity OwningTest { get { return (MCutTestEntity)base.OwningTest; } }

        /// <summary>Returns the <see cref="TestCaseEntity"/> associated with this run.</summary>
        public TestCaseEntity TestCase { get { return this.EntityOfType<TestCaseEntity>(); } }
        public MCutTestRunType RunType { get { return DataElement.Attribute(XSessionNames.AMCutTestRunType).ParseXmlEnum<MCutTestRunType>().Value; } }

        new public AppTasks.RunMCutTestCaseAppTask Task
        {
            get { return (AppTasks.RunMCutTestCaseAppTask)base.Task; }
            set { base.Task = value; }
        }

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            return DataElement.Elements(
                XTestCaseNames.TestCase
                , XTestResultNames.TestResult
                );
        }

        protected override IEnumerable<XElement> DescendantXRuns()
        {
            return DataElement.Elements(XTestResultNames.TestResult)
                .Descendants(XSessionNames.MCutTestRun);
        }

        public override string GetInvocationDetails()
        {
            StringBuilder sb = new StringBuilder();

            var startScriptPath = Path.Combine(Task.TaskFolderPath, Task.StartScriptFilename);
            sb.AppendLine(" " + Task.StartScriptFilename);
            sb.AppendLine(new string('*', Task.StartScriptFilename.Length + 2));
            if (File.Exists(startScriptPath))
                sb.Append(File.ReadAllText(startScriptPath));
            sb.AppendLine();

            // If the test uses mchess, then lets show its startup script too.
            if (OwningTest is ITestUsesMChess)
            {
                var mchessStartScriptPath = Path.Combine(Task.TaskFolderPath, AppTasks.ExecuteMChessTask.StartMChessScriptFilename);
                sb.AppendLine(" " + AppTasks.ExecuteMChessTask.StartMChessScriptFilename);
                sb.AppendLine(new string('*', AppTasks.ExecuteMChessTask.StartMChessScriptFilename.Length + 2));
                if (File.Exists(mchessStartScriptPath))
                    sb.Append(File.ReadAllText(mchessStartScriptPath));
                sb.AppendLine();
            }

            //
            sb.AppendLine("Test Case Xml");
            sb.AppendLine("*************");
            sb.AppendLine(TestCase.DataElement.ToString());
            sb.AppendLine();

            // And finally add the run's xml
            sb.AppendLine(" Run Xml");
            sb.AppendLine("*********");
            sb.AppendLine(CopyDataElement(DataElement, true, XTestCaseNames.TestCase, XTestResultNames.TestResult).ToString());

            return sb.ToString();
        }

    }
}
