using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.Concurrency.TestTools
{
    public static class XmlExtensions
    {

        /// <summary>Selects the XElement.Values from the list of elements.</summary>
        public static IEnumerable<string> SelectXValues(this IEnumerable<XElement> elements)
        {
            return elements.Select(x => (string)x);
        }

        /// <summary>Converts a boolean value to a valid boolean in xml according to xml schemas.</summary>
        /// <returns>"true" or "false" accordingly.</returns>
        public static string ToXmlBool(this bool value)
        {   // To meet the schema rules, must be lowercase
            return value ? "true" : "false";
        }

    }
}
