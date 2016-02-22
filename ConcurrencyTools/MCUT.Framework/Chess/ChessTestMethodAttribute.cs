using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.UnitTesting.Chess
{
    /// <summary>
    /// Marks a method as a chess test method and creates a single test context.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ChessTestMethodAttribute : TestTypeAttributeBase, ITestContextsProviderAttribute
    {

        public ChessTestMethodAttribute()
        {
        }

        #region ITestContextsProvider Implementation

        IEnumerable<ITestContext> ITestContextsProviderAttribute.GetContexts(MethodInfo testMethod)
        {
            var contextAttributes = (ChessTestContextAttribute[])testMethod.GetCustomAttributes(typeof(ChessTestContextAttribute), true);

            // Create the initial options as provided by this attribute
            MChessOptions baseOptions = new MChessOptions() {
                //IncludedAssemblies = ChessInstrumentAssemblyAttribute.GetAssembliesToInstrument(testMethod)
            };

            foreach (var ctxAttr in contextAttributes)
            {
                // Copy the settings from the attribute over to the options
                MChessOptions opts = baseOptions.Clone();
                opts.MergeWith(ctxAttr);

                yield return new ChessTestContext(ctxAttr.Name
                    , ctxAttr.ExpectedResultKey
                    , ctxAttr.ExpectedChessResultKey
                    , opts
                    ) {
                        PreRunScript = ctxAttr.PreRunScript
                    };
            }
        }

        IEnumerable<ITestContext> ITestContextsProviderAttribute.GetContexts(Type testClass)
        {
            throw new InvalidOperationException("This method doesn't apply to classes.");
        }

        #endregion


        protected override XElement CreateTestTypeXml(MethodInfo testMethod)
        {
            var methodAttrs = testMethod.GetCustomAttributes(true);

            XElement xtest = new XElement(XNames.ChessUnitTest);

            // Elements: instrumentAssembly
            // Not included here, already included at the testMethod level.

            // Elements: chessContext
            var contexts = methodAttrs
                .OfType<ITestContextsProviderAttribute>()
                .SelectMany(attr => attr.GetContexts(testMethod))
                .OfType<ChessTestContext>()
                .OrderBy(ctx => ctx.Name);
            if (contexts.GroupBy(ctx => ctx.Name).Any(g => g.Count() > 1))
                throw new Exception(String.Format("The method {0} on class {1} has contexts with duplicate names.", testMethod.Name, testMethod.ReflectedType.FullName));
            xtest.Add(
                contexts.Select(TestAssemblyReader.TestContextToXElement)
                );

            // Elements: expectedChessResult
            xtest.Add(
                from attr in methodAttrs.OfType<ExpectedChessResultAttribute>()
                select new XElement(XNames.ExpectedChessResult
                    , XmlUtil.CreateAttributeIfNotNull(XNames.AKey, attr.Key)
                    , attr._exitCode.HasValue ? new XAttribute("ExitCode", attr.ExitCode) : null
                    , attr.SchedulesRan > 0 ? new XAttribute("SchedulesRan", attr.SchedulesRan) : null
                    , attr.LastThreadCount > 0 ? new XAttribute("LastThreadCount", attr.LastThreadCount) : null
                    , attr.LastExecSteps > 0 ? new XAttribute("LastExecSteps", attr.LastExecSteps) : null
                    , attr.LastHBExecSteps > 0 ? new XAttribute("LastHBExecSteps", attr.LastHBExecSteps) : null
                    ));

            return xtest;
        }

    }
}