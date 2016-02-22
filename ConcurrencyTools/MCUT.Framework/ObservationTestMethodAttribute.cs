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
    public class ObservationTestMethodAttribute : TestTypeAttributeBase
    {

        public ObservationTestMethodAttribute()
        {
            CheckingMode = ObservationTestCheckingMode.Linearizability;
        }

        /// <summary>
        /// The default is Linearizability.
        /// </summary>
        public ObservationTestCheckingMode CheckingMode { get; set; }

        /// <summary>
        /// The name of the method with an <see cref="ObservationGeneratorAttribute"/> attribute
        /// applied to it.
        /// You can specify the name of a method within the same test class, or include the full name
        /// of the method within the test assembly: "namespace.className.methodName".
        /// If left blank, then the FileIdentifier is used or it assumes the generator is the same
        /// test method (the <see cref="ObservationGeneratorAttribute"/> attribute still needs to be specified).
        /// </summary>
        public string GeneratorName { get; set; }

        private string _fileIdentifier;
        /// <summary>
        /// The custom file identifier used to compute the filename of the observation file to use
        /// as input into this test.
        /// It's preferred to set the ObservationGenerator property over this one.
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
                        ObservationGeneratorAttribute.ValidateObservationFileIdentifier(value);
                }

                _fileIdentifier = value;
            }
        }

        protected override XElement CreateTestTypeXml(MethodInfo testMethod)
        {
            XAttribute obsFileAttr = null;
            if (FileIdentifier != null)
                obsFileAttr = new XAttribute("FileIdentifier", FileIdentifier);
            else
            {
                // Then use a generator method name
                string generatorName = GeneratorName;
                if (String.IsNullOrEmpty(generatorName))
                {
                    // Assume we're our own generator
                    if (!Attribute.IsDefined(testMethod, typeof(ObservationGeneratorAttribute)))
                        throw new InvalidUnitTestConfigurationException(String.Format("No observation file or generator specified for test method {0}.{1}. Are you missing the ObservationGeneratorAttribute declaration?", testMethod.ReflectedType.Name, testMethod.Name));

                    generatorName = testMethod.Name;
                }

                // Detect when the user has specified the name of a method in the current test class
                // and append the test class's full name.
                if (!generatorName.Contains('.'))
                    generatorName = testMethod.ReflectedType.FullName + "." + generatorName;

                obsFileAttr = new XAttribute("GeneratorFullName", generatorName);
            }

            System.Diagnostics.Debug.Assert(obsFileAttr != null, "We must have an attribute that identifies where we can get the observation file from.");
            return new XElement(XNames.ObservationTest
                , new XAttribute("CheckingMode", CheckingMode)
                , obsFileAttr
                );
        }

    }
}