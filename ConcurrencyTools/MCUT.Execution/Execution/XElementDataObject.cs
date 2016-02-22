using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// Represents the base class for a data object based on an <see cref="XElement"/>.
    /// </summary>
    public abstract class XElementDataObject
    {

        protected XElementDataObject(XElement xel)
        {
            if (xel == null)
                throw new ArgumentNullException();

            DataElement = xel;
        }

        public XElement DataElement { get; private set; }

    }
}
