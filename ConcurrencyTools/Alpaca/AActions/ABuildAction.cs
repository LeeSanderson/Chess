/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.UnitTesting;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.TestTools.Alpaca.AActions
{
    /// <summary>
    /// Action that runs all tests contained within the current entity (including self).
    /// </summary>
    internal class ABuildAction : AAction
    {

        internal ABuildAction(string text)
            : base(text)
        {
        }

        public bool Rebuild { get; set; }
        public bool RunAfterBuild { get; set; }

        protected override void OnBindToContext()
        {
            base.OnBindToContext();

            if (!Applicable)
                return;

            if (!(Target is TestGroupingEntity || Target is TestEntity))
            {
                Applicable = false;
                return;
            }

            // See if this entity or one of it's descendants define a build
            Applicable &= FindAncestorBuilds(Target).Any()
                || FindDescendantBuilds(Target).Any()
                ;

            if (Applicable && Rebuild)
                Applicable &= FindAncestorBuilds(Target).Any(b => b.SupportsRebuild)
                    || FindDescendantBuilds(Target).Any(b => b.SupportsRebuild)
                    ;

            if (RunAfterBuild)  // Run & Build:
            {
                // To run, there needs to be at least one test specified
                Applicable &= Target.IsTestOrHasTests();
            }
        }

        /// <summary>Finds the BuildEntity entity that applies to this entity.</summary>
        private static IEnumerable<IBuildableEntity> FindDescendantBuilds(EntityBase entity)
        {
            return entity.Descendants<IBuildableEntity>().Where(b => b.IsBuildable);
        }

        /// <summary>Finds the BuildEntity entity that applies to this entity.</summary>
        private static IEnumerable<IBuildableEntity> FindAncestorBuilds(EntityBase entity)
        {
            return from ancestor in entity.AncestorsAndSelf()
                   where ancestor is IBuildableEntity || ancestor is IDefinesBuild
                   let b = (ancestor as IBuildableEntity) ?? ((IDefinesBuild)ancestor).BuildEntity
                   where b != null && b.IsBuildable
                   select b;
        }

        public override void Go()
        {
            var cmd = new BuildAndRunCommand(Target.DataElement, !RunAfterBuild, true) { Rebuild = Rebuild };
            Context.Model.controller.AddNewCommand(cmd);
        }

    }
}
