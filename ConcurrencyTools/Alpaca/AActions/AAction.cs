using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.Execution;

namespace Microsoft.Concurrency.TestTools.Alpaca.AActions
{
    abstract class AAction
    {

        protected AAction(string text)
        {
            Text = text;
            Children = new List<AAction>();

            Actionable = true;
            Applicable = true;
            Enabled = true;
        }

        public string Text { get; set; }

        private List<AAction> _children;
        public List<AAction> Children
        {
            get { return _children; }
            set { _children = value ?? new List<AAction>(); }
        }

        /// <summary>
        /// The children to add if after binding to a context this action
        /// is still Applicable.
        /// </summary>
        public IEnumerable<AAction> ChildrenIfApplicable { get; set; }

        /// <summary>
        /// Note: This is set during BindToContext, not in the ctor.
        /// </summary>
        public AAction Parent { get; private set; }

        public AActionContext Context { get; private set; }
        public EntityBase Target { get { return Context.Target; } }

        public bool Actionable { get; set; }
        public bool Applicable { get; set; }
        public bool Enabled { get; set; }

        /// <summary>
        /// The action that is done during binding to a context, after OnBindToContext
        /// and only if this action is Actionable.
        /// </summary>
        public System.Action BoundToContext { get; set; }

        internal void BindToContext(AActionContext context)
        {
            Context = context;

            OnBindToContext();
            if (Applicable && BoundToContext != null)
                BoundToContext();

            if (Applicable && ChildrenIfApplicable != null)
                Children.AddRange(ChildrenIfApplicable);

            // Then bind children
            foreach (var child in Children)
            {
                child.Parent = this;
                child.BindToContext(Context);
            }
        }

        protected virtual void OnBindToContext()
        {
        }

        public virtual bool AcceptsModifier(AActionModifier modifier) { return false; }

        public virtual void ApplyModifier(AActionModifier modifier)
        {
        }

        public abstract void Go();

    }
}
