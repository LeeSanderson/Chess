using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.Alpaca.AActions
{
    abstract class AActionModifier : AAction
    {

        protected AActionModifier(string text)
            : base(text)
        {
        }

        public override void Go()
        {
            var p = Parent;
            while (p != null && p is AAction)
            {
                if (p.AcceptsModifier(this))
                {
                    p.ApplyModifier(this);
                    break;
                }
                p = p.Parent;
            }
            // Note: It's possible that a modifier can't modify any of it's parents, in which case the modifier is ignored.

            // Now execute the parent, since a modifier doesn't actually execute anything.
            Parent.Go();
        }

    }

    class AMChessOptionsModifier : AActionModifier
    {

        internal AMChessOptionsModifier(string text, MChessOptions options = null, IEnumerable<AAction> children = null)
            : base(text)
        {
            MChessOptions = options;
            if (children != null)
                Children.AddRange(children);
        }

        public MChessOptions MChessOptions;

    }

}
