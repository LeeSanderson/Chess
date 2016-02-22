using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Execution
{
    public abstract class TestGroupingEntity : EntityBase
    {

        public TestGroupingEntity(XElement el)
            : base(el)
        {
        }

        /// <summary>Returns all the descendent tests.</summary>
        /// <returns></returns>
        public override IEnumerable<TestEntity> Tests()
        {
            return GetChildEntities()
                .SelectMany(child => child.TestsAndSelf());
        }

    }
}
