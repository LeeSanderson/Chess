using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.Concurrency.TestTools.Alpaca.AActions
{
    class AActionMenuItem : DropDownMenuItem
    {
        internal AActionMenuItem(AAction action)
        {
            if (action == null) throw new ArgumentNullException("action");

            this.action = action;
            this.Text = action.Text;
            this.Enabled = action.Enabled;
        }

        AAction action;

        public override bool IsActionable { get { return action.Actionable; } }

        protected override void OnDoAction()
        {
            action.Go();
        }
    }

}
