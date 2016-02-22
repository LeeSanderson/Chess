using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Microsoft.Concurrency.TestTools.UnitTesting.RegressionTests
{
    /// <summary>
    /// This test is a test for the CodePlex Issue Item # 8077.
    /// It shows a data race bug found that's inside a constructor,
    /// but there shouldn't be any way the data should be made available
    /// until the ctor has finished.??? Unless something else is going on.
    /// </summary>
    public class Issue8077_CtorDataRaceTests
    {

        public class MyData
        {
            private string data;

            // Here, this constructor only sets its internal field.
            public MyData()
            {
                data = "value";
            }

            public void Cleanup()
            {
                data = null;
            }

        }

        /// <summary>
        /// I don't think there should be any way possible for this data race to happen.
        /// </summary>
        [DataRaceTestMethod]
        public void AddAndRemove()
        {
            ConcurrentDictionary<string, MyData> ccd = new ConcurrentDictionary<string, MyData>();

            Parallel.Invoke(
                // This task does an add to the concurrent dict.
                // The parameter passed in a new object created inline.
                // I thought that all constructor logic is guaranteed to be complete
                // when the constructor returns and is data race free if the ctor
                // doesn't make any unsafe method calls.
            () => {
                // Data race happens inside the MyData ctor:
                ccd.TryAdd("k1", new MyData());
            },
            () => {
                MyData data;
                // If we are able to actually remove it, then the 'data'
                // reference should be to the completely constructed object created
                // in the prev delegate.
                if (ccd.TryRemove("k1", out data))
                {
                    // Data race happens in this call:
                    data.Cleanup();
                }
            });
        }

        // This has the same problem but clarifies that the ctor has been fully executed
        // before adding to the queue.
        [DataRaceTestMethod]
        public void AddAndRemove_alternative1()
        {
            ConcurrentDictionary<string, MyData> ccd = new ConcurrentDictionary<string, MyData>();

            Parallel.Invoke(
                // This task does an add to the concurrent dict.
                // The parameter passed in a new object created inline.
                // I thought that all constructor logic is guaranteed to be complete
                // when the constructor returns and is data race free if the ctor
                // doesn't make any unsafe method calls.
            () => {
                // Data race happens inside the MyData ctor:
                MyData data = new MyData();
                ccd.TryAdd("k1", data);
            },
            () => {
                MyData data;
                // If we are able to actually remove it, then the 'data'
                // reference should be to the completely constructed object created
                // in the prev delegate.
                if (ccd.TryRemove("k1", out data))
                {
                    // Data race happens in this call:
                    data.Cleanup();
                }
            });
        }

    }
}
