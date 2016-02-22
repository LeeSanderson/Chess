using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Reflection;
using System.IO;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.UnitTesting.Xml
{
    /// <summary>
    /// </summary>
    public static class UnitTestingSchemaUtil
    {

        private static object _schemasSync = new object();
        private static XmlSchema _concurrencySchema;
        private static XmlSchema _testListSchema;
        private static XmlSchema _chessSchema;

        /// <summary>Gets the schema for xml test list files.</summary>
        /// <returns></returns>
        public static XmlSchema GetTestListSchema()
        {
            if (_testListSchema == null)
            {
                lock (_schemasSync)
                {
                    if (_testListSchema == null)
                    {
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        using (Stream stream = assembly.GetManifestResourceStream(typeof(XNames), "TestListSchema.xsd"))
                            _testListSchema = XmlSchema.Read(stream, null);
                    }
                }
            }

            return _testListSchema;
        }

        /// <summary>Gets the schema for chess results.xml files.</summary>
        /// <returns></returns>
        public static XmlSchema GetChessSchema()
        {
            if (_chessSchema == null)
            {
                lock (_schemasSync)
                {
                    if (_chessSchema == null)
                    {
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        using (Stream stream = assembly.GetManifestResourceStream(typeof(XChessNames), "ChessSchema.xsd"))
                            _chessSchema = XmlSchema.Read(stream, null);
                    }
                }
            }

            return _chessSchema;
        }

        /// <summary>Gets the schema for common concurrency xml types.</summary>
        /// <returns></returns>
        public static XmlSchema GetConcurrencySchema()
        {
            if (_concurrencySchema == null)
            {
                lock (_schemasSync)
                {
                    if (_concurrencySchema == null)
                    {
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        using (Stream stream = assembly.GetManifestResourceStream(typeof(XConcurrencyNames), "ConcurrencySchema.xsd"))
                            _concurrencySchema = XmlSchema.Read(stream, null);
                    }
                }
            }

            return _concurrencySchema;
        }

        /// <summary>
        /// Validates a test list or test assembly xml document.
        /// </summary>
        /// <param name="xdoc"></param>
        public static void ValidateTestListXml(XDocument xdoc)
        {
            // First, make sure it's a testcase
            if (xdoc == null)
                throw new ArgumentNullException("xdoc");
            if (xdoc.Root == null)
                throw new ArgumentException("Xml document doesn't contain a root element.", "xdoc");
            if (xdoc.Root.Name != XNames.Testlist
                && xdoc.Root.Name != XNames.TestProject
                && xdoc.Root.Name != XNames.TestAssembly
                )
                throw new ArgumentException(String.Format("The root element \"{0}\" should be either \"{1}\", \"{2}\" or \"{3}\".", xdoc.Root.Name, XNames.Testlist, XNames.TestProject, XNames.TestAssembly));

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(UnitTestingSchemaUtil.GetConcurrencySchema());
            schemaSet.Add(UnitTestingSchemaUtil.GetTestListSchema());
            schemaSet.Add(UnitTestingSchemaUtil.GetChessSchema());
            schemaSet.Compile();

            xdoc.Validate(schemaSet, null);
        }

        /// <summary>
        /// Validates a test case xml document.
        /// These documents may be passed into mcut when using the 'runTestCase' command.
        /// </summary>
        /// <param name="xdoc"></param>
        public static void ValidateTestCaseXml(XDocument xdoc)
        {
            // First, make sure it's a testcase
            if (xdoc == null)
                throw new ArgumentNullException("xdoc");
            if (xdoc.Root == null)
                throw new ArgumentException("Xml document doesn't contain a root element.", "xdoc");
            if (xdoc.Root.Name != XTestCaseNames.TestCase)
                throw new ArgumentException(String.Format("The root element should be \"{0}\" but is \"{1}\".", XTestCaseNames.TestCase, xdoc.Root.Name));

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(UnitTestingSchemaUtil.GetConcurrencySchema());
            schemaSet.Add(UnitTestingSchemaUtil.GetTestListSchema());
            schemaSet.Add(UnitTestingSchemaUtil.GetChessSchema());
            schemaSet.Compile();

            xdoc.Validate(schemaSet, null);
        }

        /// <summary>
        /// Validates a test result xml document.
        /// </summary>
        /// <param name="xdoc"></param>
        public static void ValidateTestResultXml(XDocument xdoc)
        {
            // First, make sure it's a testcase
            if (xdoc == null)
                throw new ArgumentNullException("xdoc");
            if (xdoc.Root == null)
                throw new ArgumentException("Xml document doesn't contain a root element.", "xdoc");
            if (xdoc.Root.Name != XTestResultNames.TestResult)
                throw new ArgumentException(String.Format("The root element should be \"{0}\" but is \"{1}\".", XTestResultNames.TestResult, xdoc.Root.Name));

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(UnitTestingSchemaUtil.GetConcurrencySchema());
            schemaSet.Add(UnitTestingSchemaUtil.GetTestListSchema());
            schemaSet.Add(UnitTestingSchemaUtil.GetChessSchema());
            schemaSet.Compile();

            xdoc.Validate(schemaSet, null);
        }

    }
}
