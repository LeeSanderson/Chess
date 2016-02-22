using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    class ModelEventArgs : EventArgs
    {

        public ModelEventArgs()
        {
        }

    }

    class ModelEntityEventArgs : ModelEventArgs
    {

        public ModelEntityEventArgs()
        {
        }

    }

    /// <summary>The type of change made to an entity.</summary>
    public enum EntityChange
    {
        Add, Remove, Modified
    }

    class EntityChangeEventArgs : ModelEntityEventArgs
    {

        public EntityChangeEventArgs(EntityChange change, XElement xchanged, XObjectChange xchange)
        {
            EntityChange = change;
            XChangedElement = xchanged;
            XChange = xchange;
        }

        public EntityChange EntityChange { get; private set; }

        /// <summary>The actual element that was changed that caused this event.</summary>
        public XElement XChangedElement { get; private set; }
        /// <summary>The change that was made to the element that caused this event.</summary>
        public XObjectChange XChange { get; private set; }
    }

}
