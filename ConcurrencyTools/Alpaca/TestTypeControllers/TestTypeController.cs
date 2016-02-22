using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.Execution.Chess;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Alpaca.AActions;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    abstract class TestTypeController
    {

        #region Static Members

        static Dictionary<Type, TestTypeController> _controllersByTestType;

        internal static TestTypeController GetController(TestEntity test)
        {
            if (test == null) return null;

            if (_controllersByTestType == null)
            {
                _controllersByTestType = (from t in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                                          where !t.IsAbstract
                                          where typeof(TestTypeController).IsAssignableFrom(t)
                                          let controller = (TestTypeController)t.GetConstructor(Type.EmptyTypes).Invoke(null)
                                          select controller)
                                          .ToDictionary(c => c.TestEntityType);
            }

            Type testType = test.GetType();
            return _controllersByTestType[testType];
        }

        #endregion

        public readonly Type TestEntityType;

        protected TestTypeController(Type testEntityType)
        {
            TestEntityType = testEntityType;
        }

        internal abstract IEnumerable<AAction> CreateTestActions(AActionContext context);

    }
}
