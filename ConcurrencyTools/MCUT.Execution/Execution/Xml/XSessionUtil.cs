using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using System.Reflection;
using System.IO;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.Execution.Xml
{
    public static class XSessionUtil
    {

        private static object _schemasSync = new object();

        private static XmlSchema _sessionSchema;

        /// <summary>Gets the schema for session xml files.</summary>
        /// <returns></returns>
        public static XmlSchema GetSessionSchema()
        {
            if (_sessionSchema == null)
            {
                lock (_schemasSync)
                {
                    if (_sessionSchema == null)
                    {
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        using (Stream stream = assembly.GetManifestResourceStream(typeof(XSessionUtil), "XSession.xsd"))
                        {
                            _sessionSchema = XmlSchema.Read(stream, null);
                        }
                    }
                }
            }

            return _sessionSchema;
        }

        /// <summary>
        /// Validates a session xml document.
        /// </summary>
        /// <param name="xdoc"></param>
        public static void ValidateSessionXml(XDocument xdoc, ValidationEventHandler validationHandler = null)
        {
            // First, make sure it's a testcase
            if (xdoc == null)
                throw new ArgumentNullException("xdoc");
            if (xdoc.Root == null)
                throw new ArgumentException("Xml document doesn't contain a root element.", "xdoc");
            if (xdoc.Root.Name != XSessionNames.Session)
                throw new ArgumentException(String.Format("The root element should be \"{0}\" but is \"{1}\".", XSessionNames.Session, xdoc.Root.Name));

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(UnitTestingSchemaUtil.GetConcurrencySchema());
            schemaSet.Add(UnitTestingSchemaUtil.GetTestListSchema());
            schemaSet.Add(UnitTestingSchemaUtil.GetChessSchema());
            schemaSet.Add(XSessionUtil.GetSessionSchema());
            schemaSet.Compile();

            xdoc.Validate(schemaSet, validationHandler);
        }

    }
}
