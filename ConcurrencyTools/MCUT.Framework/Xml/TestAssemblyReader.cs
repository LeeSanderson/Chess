using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Concurrency.TestTools;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.UnitTesting.Xml
{
    /// <summary>
    /// Reader that reads the Concurrent Unit Test information from an assembly.
    /// </summary>
    public class TestAssemblyReader
    {

        /// <summary>
        /// Creates a valid XElement instance that can be used as a placeholder to load
        /// an assembly that may or may not actually exist yet. (i.e. may have not been built)
        /// </summary>
        /// <param name="assyLocation"></param>
        /// <returns></returns>
        public static XElement CreateXTestAssemblyPlaceholder(string assyLocation)
        {
            return new XElement(XNames.TestAssembly
                , new XAttribute(XNames.ALocation, assyLocation)
                //, new XAttribute(XNames.ADetectChanges, true.ToXmlBool())
                );
        }

        private string _assyLocation;
        private Assembly _testAssembly;

        /// <summary>
        /// Creates a new instance for reading the assembly at the specified location.
        /// </summary>
        /// <param name="assemblyLocation">The path to the assembly to read. The assembly doesn't need to exist.</param>
        public TestAssemblyReader(string assemblyLocation)
        {
            _assyLocation = Path.GetFullPath(assemblyLocation);
        }

        /// <summary>
        /// Creates a new instance that reads the specified assembly.
        /// </summary>
        /// <param name="testAssembly">The assembly already loaded in memory.</param>
        public TestAssemblyReader(Assembly testAssembly)
        {
            _testAssembly = testAssembly;
            _assyLocation = Path.GetFullPath(_testAssembly.Location);
        }

        /// <summary>
        /// Reads the assembly specified in the constructor.
        /// If the file doesn't exist, an empty test assembly element is returned.
        /// </summary>
        /// <returns></returns>
        public XElement Read()
        {
            // Initialize the assembly node
            XElement xtestAssy = CreateXTestAssemblyPlaceholder(_assyLocation);

            // The assembly may not actually exist yet because it hasn't been built
            // so only load the assembly if it exists
            if (File.Exists(_assyLocation))
            {
                try
                {
                    if (_testAssembly == null)
                        _testAssembly = Assembly.LoadFrom(_assyLocation);

                    // Add assembly-level attribute representations
                    var assyAttrs = _testAssembly.GetCustomAttributes(true);


                    // Get the explicit and implicit InstrumentAssemblyAttributes
                    var instrumentAssyAttrs = assyAttrs.OfType<ChessInstrumentAssemblyAttribute>();
                    var assysToExclude = instrumentAssyAttrs
                        .Where(attr => attr.Exclude)
                        .Select(attr => attr.Assembly);

                    // Generate the implicit InstrumentAssemblyAttributes
                    var implicitInstrumentAssyAttrs = GetReferencedAssembliesToInstrument(_testAssembly)
                        .Where(refAssyName => !assysToExclude.Contains(refAssyName))
                        .Select(refAssyName => new ChessInstrumentAssemblyAttribute(refAssyName));
                    instrumentAssyAttrs = implicitInstrumentAssyAttrs.Union(instrumentAssyAttrs);

                    xtestAssy.Add(
                        // TODO: Make this a generic feature so other types of tests properties will be recognized
                        instrumentAssyAttrs
                        .Select(attr => attr.ToTestlistXml())
                        );

                    // Add the rest of the assembly-level setting attributes
                    xtestAssy.Add(
                        assyAttrs.OfType<ChessDefaultPreemptabilityAttribute>()
                        .Select(attr => attr.ToTestlistXml())
                        );
                    xtestAssy.Add(
                        assyAttrs.OfType<ChessTogglePreemptabilityAttribute>()
                        .Select(attr => attr.ToTestlistXml())
                        );


                    // Add test classes
                    xtestAssy.Add(
                        from t in _testAssembly.GetTypes()
                        where !t.IsAbstract
                        // I'm deciding to let this error propagate up to the user as an error
                        // rather than the user not knowing why it's not showing up.
                        //where t.IsVisible
                        //let ctor = t.GetConstructor(Type.EmptyTypes)
                        //where ctor != null && ctor.IsPublic
                        let attributes = t.GetCustomAttributes(false)
                        where !attributes.OfType<IgnoreAttribute>().Any()
                        let xtestClass = ReadTestClass(t)
                        where xtestClass.Descendants(XNames.TestMethod).Any() // Where the class had some test methods defined
                        orderby t.FullName
                        select xtestClass
                        );
                }
                catch (Exception ex)
                {
                    xtestAssy.Add(XConcurrencyNames.CreateXError(ex));
                }
            }

            return xtestAssy;
        }

        public static IEnumerable<string> GetReferencedAssembliesToInstrument(Assembly srcAssy)
        {
            System.Diagnostics.Debug.WriteLine(srcAssy.Location, "srcAssy.Location");
            string srcAssyFolderPath = Path.GetDirectoryName(srcAssy.Location);
            DirectoryInfo srcAssyDir = new DirectoryInfo(srcAssyFolderPath);
            foreach (var refAssyName in srcAssy.GetReferencedAssemblies())
            {
                System.Diagnostics.Debug.WriteLine(refAssyName.Name, "refAssyName.CodeBase");
                System.Diagnostics.Debug.WriteLine(refAssyName.FullName, "refAssyName.CodeBase");
                System.Diagnostics.Debug.WriteLine(refAssyName.CodeBase, "refAssyName.CodeBase");

                // Only include assemblies that are located within the same folder as the source assembly
                if (srcAssyDir.EnumerateFiles(refAssyName.Name + ".*")
                    .Any(fi => fi.Extension.ToLower() == ".exe" || fi.Extension.ToLower() == ".dll"))
                {
                    // Exclude the MCUT.Framework assembly, just in case the user doesn't have it in their GAC
                    // and their VS project copies it into the output folder.
                    if (refAssyName.Name != Assembly.GetExecutingAssembly().GetName().Name)
                        yield return refAssyName.Name;
                }
            }
        }

        private XElement ReadTestClass(Type testType)
        {
            //
            XElement xtestClass = new XElement(XNames.TestClass
                , new XAttribute(XNames.AFullName, testType.FullName)
                );

            // Add class-level attribute representations
            var classAttrs = testType.GetCustomAttributes(true);
            xtestClass.Add(
                // TODO: Make this a generic feature so other types of tests properties will be recognized
                classAttrs.OfType<ChessInstrumentAssemblyAttribute>()
                .Select(attr => attr.ToTestlistXml())
                );
            xtestClass.Add(
                // TODO: Make this a generic feature so other types of tests properties will be recognized
                classAttrs.OfType<ChessDefaultPreemptabilityAttribute>()
                .Select(attr => attr.ToTestlistXml())
                );
            xtestClass.Add(
                // TODO: Make this a generic feature so other types of tests properties will be recognized
                classAttrs.OfType<ChessTogglePreemptabilityAttribute>()
                .Select(attr => attr.ToTestlistXml())
                );

            // Get class-level contexts
            var contexts = testType.GetCustomAttributes(true)
                .OfType<ITestContextsProviderAttribute>()
                .SelectMany(attr => attr.GetContexts(testType))
                .OrderBy(ctx => ctx.Name);
            if (contexts.GroupBy(ctx => ctx.Name).Any(g => g.Count() > 1))
                throw new Exception(String.Format("The class {0} has contexts with duplicate names.", testType.FullName));
            // TODO: Verify a valid format for context names. Just use the same identifier pattern as Observation File Identities
            xtestClass.Add(contexts.Select(ctx => TestContextToXElement(ctx)));

            // Add test methods
            xtestClass.Add(
                from m in testType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                let attributes = m.GetCustomAttributes(true)
                where attributes.OfType<ITestTypeAttribute>().Any() && !attributes.OfType<IgnoreAttribute>().Any()
                select ReadTestMethod(m)
                );

            return xtestClass;
        }

        private XElement ReadTestMethod(MethodInfo testMethod)
        {
            XElement xtestMethod = new XElement(XNames.TestMethod
                , new XAttribute(XNames.AName, testMethod.Name)
                );

            // Element: parameters
            var testParams = testMethod.GetParameters();
            int parameterCount = testParams.Length;
            if (parameterCount != 0)
            {
                xtestMethod.Add(new XElement(XNames.Parameters,
                    from p in testParams
                    select new XElement(XNames.Param
                        , new XAttribute(XNames.AName, p.Name)
                        , new XAttribute(XNames.AType, p.ParameterType.FullName)
                        )
                    ));
            }

            var methodAttrs = testMethod.GetCustomAttributes(true);

            // Elements: testArgs
            // We only allow them for methods that actually take parameters
            if (parameterCount != 0)
            {
                xtestMethod.Add(
                    from argsProviderAttr in methodAttrs.OfType<ITestMethodArgsProvider>()
                    from args in argsProviderAttr.GetArgs(testMethod)
                    where args.Length == parameterCount  // Only take valid args sets
                    select new XElement(XNames.TestArgs,
                        args.Select(arg => new XElement(XNames.Arg, arg.ToString()))
                        )
                    );
            }

            // Element: expectedTestResults
            var expResultAttrs = methodAttrs
                .OfType<ExpectedResultAttribute>()
                .OrderBy(attr => attr.Key);
            if (expResultAttrs.GroupBy(ctx => ctx.Key).Any(g => g.Count() > 1))
                throw new Exception(String.Format("The method {0} on class {1} has {2} attributes with duplicate keys.", testMethod.Name, testMethod.ReflectedType.FullName, typeof(ExpectedResultAttribute).Name));
            foreach (var expResultAttr in expResultAttrs)
            {
                xtestMethod.Add(new XElement(XNames.ExpectedTestResult
                    , XmlUtil.CreateAttributeIfNotNull(XNames.AKey, expResultAttr.Key)
                    , new XAttribute(XNames.AResultType, expResultAttr.Result)
                    , XmlUtil.CreateElementIfNotNull(XTestResultNames.ResultMessage, expResultAttr.Message)
                    ));
            }

            // Element: expectedRegressionTestResult
            var expRegressionResultAttr = methodAttrs.OfType<RegressionTestExpectedResultAttribute>().SingleOrDefault();
            if (expRegressionResultAttr != null)
            {
                xtestMethod.Add(new XElement(XNames.ExpectedRegressionTestResult
                    , new XAttribute(XNames.AResultType, String.Join(" ", expRegressionResultAttr.ExpectedResultTypes))
                    ));
            }

            // Element: instrumentAssembly
            // Included at this level to make them easily available to any test type that uses chess
            // Elements: instrumentAssembly
            xtestMethod.Add(
                methodAttrs.OfType<ChessInstrumentAssemblyAttribute>()
                .Select(attr => attr.ToTestlistXml())
                );
            xtestMethod.Add(
                methodAttrs.OfType<ChessDefaultPreemptabilityAttribute>()
                .Select(attr => attr.ToTestlistXml())
                );
            xtestMethod.Add(
                methodAttrs.OfType<ChessTogglePreemptabilityAttribute>()
                .Select(attr => attr.ToTestlistXml())
                );


            // Test Types elements
            xtestMethod.Add(
                methodAttrs
                .OfType<ITestTypeAttribute>()
                .Select(testTypeAttr => testTypeAttr.GetTestTypeXml(testMethod))
                .OrderBy(x => x.Name.LocalName) // To keep things consistent since attributes may be in any order
                );

            return xtestMethod;
        }

        private XElement TestContextToXElement(ITestContext ctx)
        {
            // Parse chess contexts using the custom context element
            var chessCtx = ctx as ChessTestContext;
            if (chessCtx != null)
                return TestContextToXElement(chessCtx);

            //var testCtx = ctx as TestContext;
            //if (testCtx != null)
            //    return TestContextToXElement(testCtx);

            // Ignore context types that aren't currently supported
            return null;
        }

        internal static XElement TestContextToXElement(ChessTestContext chessCtx)
        {
            XElement xctx = new XElement(XNames.ChessContext
                , XmlUtil.CreateAttributeIfNotNull(XNames.AName, chessCtx.Name)
                , XmlUtil.CreateAttributeIfNotNull(XNames.AExpectedResultKey, chessCtx.ExpectedResultKey)
                , XmlUtil.CreateAttributeIfNotNull(XNames.AExpectedChessResultKey, chessCtx.ExpectedChessResultKey)
                );

            if (!String.IsNullOrWhiteSpace(chessCtx.PreRunScript))
                xctx.Add(new XElement(XNames.MChessPreRunScript, chessCtx.PreRunScript));

            // Add mchess options
            MChessOptions opts = chessCtx.Options;
            XElement xopts = opts.ToXElement();
            if (xopts.HasAttributes)
            {
                // Add the serialized options, keeping the same attribute names
                foreach (var attr in xopts.Attributes())
                    xctx.Add(new XAttribute(attr));
            }

            if (xopts.HasElements)
            {
                // Add the serialized options, keeping the same element names and namespaces
                foreach (var el in xopts.Elements())
                    xctx.Add(new XElement(el));
            }

            return xctx;
        }

    }
}
