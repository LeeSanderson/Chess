using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.IO;
using System.Diagnostics;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    /// <summary>
    /// This test class makes sure that we've covered all available overloads of a particular group of 
    /// methods.
    /// </summary>
    public class WrapperOverloadCoverageTests
    {

        #region Core test functions

        private void VerifyAllOverloadsCovered(Type origType, Type wrapperType, BindingFlags bindingFlags, string methodName)
        {
            Console.WriteLine("Validating wrappers exist for all overloads of the method:");
            Console.WriteLine("   {0}.{1}", origType, methodName);
            Console.WriteLine();

            var overloads = from m in origType.GetMethods(bindingFlags)
                            where m.Name == methodName
                            select m
                            ;

            // Use a list because we're gonna remove each wrapper as we find it
            List<MethodInfo> wrapperOverloads = (from m in wrapperType.GetMethods(bindingFlags)
                                                 where m.Name == methodName
                                                 select m)
                                                 .ToList();

            Console.WriteLine("All wrapper overloads:");
            foreach (var m in wrapperOverloads)
                Console.WriteLine("   {0}", m);
            Console.WriteLine();

            List<MethodInfo> methodsWithWrappers = new List<MethodInfo>();
            List<MethodInfo> methodsWithoutWrappers = new List<MethodInfo>();
            foreach (var m in overloads)
            {
                //Console.WriteLine(m);

                // Lets first filter by those that are or are not generic methods
                var typeParams = m.GetGenericArguments();
                var mParams = m.GetParameters();
                var possibleWrappers = from wm in wrapperOverloads
                                       let wtparams = wm.GetGenericArguments()
                                       let wmParams = wm.GetParameters()
                                       where wm.IsGenericMethod == m.IsGenericMethod
                                           // Make sure the number of generic type params are the same
                                       && wtparams.Length == typeParams.Length
                                       && wmParams.Length == mParams.Length
                                       && AreTypesEquivalent(wm.ReturnType, m.ReturnType)
                                       select new { Method = wm, TypeParameters = wtparams, Parameters = wmParams };

                //Console.WriteLine("   Initial possible matches:");
                //foreach (var w in possibleWrappers)
                //    Console.WriteLine("     {0}", w.Method);
                //Console.WriteLine();

                // Filter the parameters
                possibleWrappers = possibleWrappers
                    .Where(w => {
                        //Console.WriteLine("   Compare to Wrapper: {0}", w.Method);
                        return Enumerable.Zip(mParams, w.Parameters, (mp, wmp) => new { mp, wmp })
                            .All(pair => {
                                bool areEquivalent = AreParametersEquivalent(pair.mp, pair.wmp);
                                //Console.WriteLine("   {0} ?= {1} : {2}", pair.mp, pair.wmp, areEquivalent);
                                return areEquivalent;
                            });
                    });

                var matchingWrappers = possibleWrappers.ToList();

                //Console.WriteLine("   Final matches:");
                //foreach (var w in matchingWrappers)
                //    Console.WriteLine("     {0}", w.Method);
                //Console.WriteLine();

                if (matchingWrappers.Count == 1)
                    methodsWithWrappers.Add(m);
                else
                {
                    methodsWithoutWrappers.Add(m);
                    if (matchingWrappers.Count != 0)
                    {
                        Console.WriteLine(m);
                        Console.WriteLine("   Multiple Matches Found:");
                        foreach (var w in matchingWrappers)
                            Console.WriteLine("     {0}", w.Method);
                        Console.WriteLine();
                    }
                }
            }

            Console.WriteLine("Overloads with wrappers:");
            foreach (var m in methodsWithWrappers)
                Console.WriteLine("     {0}", m);
            Console.WriteLine();
            Console.WriteLine("Overloads with NO wrappers:");
            foreach (var m in methodsWithoutWrappers)
                Console.WriteLine("     {0}", m);
            Console.WriteLine();

            if (methodsWithoutWrappers.Count != 0)
                Assert.Inconclusive("Chess wrappers missing for overloads.");
        }

        private static bool AreParametersEquivalent(ParameterInfo pSrc, ParameterInfo pOther)
        {
            return pSrc.IsIn == pOther.IsIn
                && pSrc.IsOut == pOther.IsOut
                && pSrc.IsRetval == pOther.IsRetval
                && AreTypesEquivalent(pSrc.ParameterType, pOther.ParameterType);
        }

        private static bool AreTypesEquivalent(Type t1, Type t2)
        {
            if (t1.IsGenericType && t2.IsGenericType)
            {
                // First, see if the main type is the same
                if (t1.GetGenericTypeDefinition() != t2.GetGenericTypeDefinition())
                    return false;

                // Need to make sure the type params specified are equivalent
                var genericArgs1 = t1.GetGenericArguments();
                var genericArgs2 = t2.GetGenericArguments();
                return Enumerable.Zip(genericArgs1, genericArgs2, (tp1, tp2) => new { tp1, tp2 })
                    // Recursively verify any generic types
                    .All(pair => AreTypesEquivalent(pair.tp1, pair.tp2));
            }
            else if (t1.IsGenericParameter && t2.IsGenericParameter)
            {
                return t1.GenericParameterPosition == t2.GenericParameterPosition;
            }
            else
                return t1 == t2;
        }

        #endregion

        #region Parallel.For,ForEach,Invoke

        private void VerifyOverloadsCovered_Parallel(string methodName)
        {
            Type wrapperType = typeof(__Substitutions.System.Threading.Tasks.Parallel);
            Assert.IsNotNull(wrapperType);

            Type origType = typeof(global::System.Threading.Tasks.Parallel);
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;

            VerifyAllOverloadsCovered(origType, wrapperType, bindingFlags, methodName);
        }

        [UnitTestMethod]
        public void Parallel_For()
        {
            VerifyOverloadsCovered_Parallel("For");
        }

        [UnitTestMethod]
        public void Parallel_ForEach()
        {
            VerifyOverloadsCovered_Parallel("ForEach");
        }

        [UnitTestMethod]
        public void Parallel_Invoke()
        {
            VerifyOverloadsCovered_Parallel("Invoke");
        }

        #endregion

    }
}
