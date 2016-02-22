using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    /// <summary>Represents a command that uses an action delegate rather than a custom command class.</summary>
    internal class CustomCommand : Command
    {

        private Action<Model> _action;

        public CustomCommand(string name, bool interactive, Action<Model> action)
            : base(interactive)
        {
            Name = name;
            _action = action;
        }

        public string Name { get; private set; }

        protected override bool PerformExecute(Model model)
        {
            try
            {
                _action(model);
            }
            catch (Exception ex)
            {
                SetError(String.Format("{0}:\n{1}", Name, ex.Message));
            }

            return true;
        }

    }
}
