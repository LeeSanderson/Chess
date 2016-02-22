using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Alpaca.AActions
{
    class SeparatorFauxAction : AAction
    {

        public SeparatorFauxAction()
            : base("-")
        {
            Actionable = false;
        }

        public override void Go()
        {
            throw new InvalidOperationException();
        }

    }
}
