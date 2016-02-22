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
    public class TestMethodParameter
    {

        public TestMethodParameter(XElement xparam)
        {
            System.Diagnostics.Debug.Assert(xparam.Name == XNames.Param, "UnitTestPArameter instantiated with incorrect element.");

            Name = xparam.Attribute(XNames.AName).Value;
            TypeName = xparam.Attribute(XNames.AType).Value;
        }

        public readonly string Name;
        public readonly string TypeName;

        public string TypeDisplayName
        {
            get
            {
                string name = TypeName;

                // Remove any assembly info
                int idx = TypeName.IndexOf(',');
                if (idx != -1)
                    name = name.Substring(0, idx);

                // Remove any namespace
                // NOTE: This may make sub-classes ambiguous
                idx = name.LastIndexOf('.');
                if (idx != -1)
                    name = name.Substring(idx + 1);

                return name;
            }
        }
    }
}
