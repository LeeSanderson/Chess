using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Execution
{
    /// <summary>
    /// A placeholder entity that allows space for some action.
    /// </summary>
    [AutoRegisterEntity]
    public class PlaceholderEntity : EntityBase
    {
        public static readonly XName EntityXName = XNames.Placeholder;

        public static PlaceholderEntity CreatePlaceholder(IEntityModel model, string message)
        {
            return (PlaceholderEntity)model.EntityBuilder.CreateEntityAndBindToElement(
                new XElement(XNames.Placeholder, message)
                );
        }

        public PlaceholderEntity(XElement el)
            : base(el)
        {
        }

        public string Message
        {
            get { return DataElement.Value; }
            set { DataElement.SetValue(value); }
        }

        public override string DisplayName { get { return Message; } }

        protected override IEnumerable<XElement> GetChildEntityElements()
        {
            yield break;
        }


    }
}
