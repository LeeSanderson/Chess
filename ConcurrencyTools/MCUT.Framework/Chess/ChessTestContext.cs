using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.UnitTesting.Chess
{
    /// <summary>The context of a single Chess unit test.</summary>
    public class ChessTestContext : TestContext
    {

        public ChessTestContext(string name, string expResultKey, string expChessResultKey, MChessOptions options)
            : base(name, expResultKey)
        {
            ExpectedChessResultKey = expChessResultKey;
            Options = options;
        }

        /// <summary>
        /// When set, indicates the key for the explicit result to expect. This is mainly used when a test may have
        /// different results based on the context.
        /// Some test types don't use this feature, in which case this property will always return null.
        /// </summary>
        public string ExpectedChessResultKey { get; private set; }

        /// <summary>Gets the script to execute before the testing system executes mchess.</summary>
        public string PreRunScript { get; set; }

        public MChessOptions Options { get; private set; }

    }
}
