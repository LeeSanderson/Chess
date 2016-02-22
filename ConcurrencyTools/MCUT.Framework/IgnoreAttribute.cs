using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Concurrency.TestTools.UnitTesting
{
    /// <summary>Marks a test class or method to be ignored.</summary>
    /// <remarks>This is a good way to keep code to compile but not actually be recognized or run by the CUT Framework.</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class IgnoreAttribute : Attribute
    {

        public IgnoreAttribute()
        {
        }

    }
}
