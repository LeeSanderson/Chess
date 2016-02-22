using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Drawing;
using Microsoft.Concurrency.TestTools.Execution;

namespace Microsoft.Concurrency.TestTools.Alpaca.Views
{
    /// <summary>
    /// The base class for all tree nodes.
    /// </summary>
    class XTreeNode : TreeNode
    {

        public XTreeNode(EntityBase dataEntity)
        {
            DataEntity = dataEntity;

            // Bind the element to this node using the Xannotation
            DataEntity.DataElement.AddAnnotation(this);
        }

        /// <summary>
        /// Creates the an entity and a new XTreeNode instance bound to the new entity.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="el"></param>
        public XTreeNode(Model model, XElement el)
            : this(model.EntityBuilder.CreateEntityAndBindToElement(el))
        {
        }

        public EntityBase DataEntity { get; private set; }

        /// <summary>Gets the element bound to this tree node.</summary>
        public XElement xelement { get { return DataEntity.DataElement; } }

    }
}
