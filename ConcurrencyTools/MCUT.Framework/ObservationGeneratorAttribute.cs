using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using System.Text.RegularExpressions;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ObservationGeneratorAttribute : TestTypeAttributeBase
    {

        /// <summary>
        /// 
        /// </summary>
        public const string FileIdentifierRegexPattern = "[_a-zA-Z][a-zA-Z0-9_]*";

        internal static void ValidateObservationFileIdentifier(string fileIdentifier)
        {
            if (!Regex.IsMatch(FileIdentifierRegexPattern, fileIdentifier))
                throw new ArgumentException("Invalid file identifier. Must match the pattern: "
                    + FileIdentifierRegexPattern);
        }

        public ObservationGeneratorAttribute()
        {
            Granularity = ObservationGranularity.Serial;
        }

        /// <summary>
        /// The default is Serial.
        /// </summary>
        public ObservationGranularity Granularity { get; set; }

        private string _fileIdentifier;
        /// <summary>
        /// The custom file identifier used to compute the final filename for the generated observation file.
        /// Care should be taken to make sure that each custom FileIdentifier is unique within
        /// each assembly. It is recommended to keep this set to null so the system can auto generate
        /// unique file identifiers.
        /// </summary>
        public string FileIdentifier
        {
            get { return _fileIdentifier; }
            set
            {
                // Verify the value is valid
                if (value != null)
                {
                    if (String.IsNullOrWhiteSpace(value))
                        value = null;
                    else
                        ValidateObservationFileIdentifier(value);
                }

                _fileIdentifier = value;
            }
        }


        protected override XElement CreateTestTypeXml(MethodInfo testMethod)
        {
            return new XElement(XNames.ObservationGenerator
                , new XAttribute("Granularity", Granularity)
                , FileIdentifier == null ? null : new XAttribute("FileIdentifier", FileIdentifier)
                );
        }

    }
}