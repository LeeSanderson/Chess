using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.Execution
{
    public class TestArgs
    {

        public TestArgs(XElement xargs)
        {
            System.Diagnostics.Debug.Assert(xargs.Name == XNames.TestArgs);

            DataElement = xargs;
            Name = (string)xargs.Attribute(XNames.AName);
            Values = xargs.Elements(XNames.Arg).SelectXValues().ToArray();
            Debug.Assert(Values.Length != 0, "The TestListSchema should ensure that if a test args element exists, then there is at least one arg.");
        }

        public string Name { get; private set; }
        /// <summary>The array of values. This property is never null and will always have at least 1 element.</summary>
        public string[] Values { get; private set; }
        public XElement DataElement { get; private set; }

    }
}
