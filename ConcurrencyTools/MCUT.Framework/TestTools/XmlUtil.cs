using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.Concurrency.TestTools
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static class XmlUtil
    {

        public static XAttribute CreateAttributeIfNotNull(XName name, string value)
        {
            return value == null ? null : new XAttribute(name, value);
        }

        public static XAttribute CreateAttributeIfNotNull<T>(XName name, T? value)
            where T : struct
        {
            return value.HasValue ? new XAttribute(name, value) : null;
        }

        public static XElement CreateElementIfNotNull(XName name, string value)
        {
            return value == null ? null : new XElement(name, value);
        }

        public static bool ParseXmlBool(XAttribute x)
        {
            return x != null && ParseXmlBool(x.Value);
        }

        public static bool ParseXmlBool(XElement x)
        {
            return x != null && ParseXmlBool(x.Value);
        }

        public static bool ParseXmlBool(string v)
        {
            bool b;
            if (Boolean.TryParse(v, out b))
                return b;
            return v == "1";
        }

        public static T? ParseXmlEnum<T>(this XElement x)
            where T : struct
        {
            return x == null ? default(T?) : ParseXmlEnum<T>(x.Value);
        }

        public static T? ParseXmlEnum<T>(this XAttribute x)
            where T : struct
        {
            return x == null ? default(T?) : ParseXmlEnum<T>(x.Value);
        }

        public static T ParseXmlEnum<T>(string stringValue)
            where T : struct
        {
            Type enumType = typeof(T);
            return (T)Enum.Parse(enumType, stringValue, true);
        }

        /// <summary>
        /// Returns a filtered collection of the child elements of this element or document,
        /// in document order. Only elements that match one of the specified System.Xml.Linq.XNames
        /// are included in the collection.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="names">The names for which to match.</param>
        /// <returns></returns>
        public static IEnumerable<XElement> Elements(this XElement source, params XName[] names)
        {
            if (source == null)
                return Enumerable.Empty<XElement>();
            if (names == null || names.Length == 0)
                throw new ArgumentException("No names supplied.");
            if (names.Length == 1)
                return source.Elements(names[0]);
            else
                return source.Elements()
                    .Where(x => names.Contains(x.Name));
        }

    }
}
