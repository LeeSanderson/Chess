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
using System.Windows.Forms;
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using Microsoft.Concurrency.TestTools.Execution;
using System.IO;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    internal class DeleteObservationFilesCommand : Command
    {
        private EntityBase _sourceEntity;

        /// <summary>
        /// </summary>
        /// <param name="entities">Set to null to indicate to remove all entities from the model's session.</param>
        /// <param name="runsonly"></param>
        /// <param name="interactive"></param>
        /// <param name="resetcounter"></param>
        internal DeleteObservationFilesCommand(EntityBase entity)
            : base(true)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            _sourceEntity = entity;
        }

        protected override bool PerformExecute(Model model)
        {
            var assys = _sourceEntity.DescendantsAndSelf<TestAssemblyEntity>()
                .Where(assy => assy.Tests()
                    .OfType<ObservationGeneratorEntity>()
                    .Any()
                //.Any(g => g.DoObservationFilesExist())
                    );
            foreach (var assy in assys)
            {
                // Delete the assembly's observation files folder
                string assyFldrPath = assy.ObservationFilesFullFolderPath;
                if (assyFldrPath != null && Directory.Exists(assyFldrPath))
                {
                    // First, try just deleting the folder
                    try
                    {
                        Directory.Delete(assyFldrPath, true);
                    }
                    catch (IOException)
                    {
                        // Second, try just deleting all the files in the directory
                        foreach (var obsFilePath in Directory.EnumerateFiles(assyFldrPath))
                            File.Delete(obsFilePath);
                    }
                }
            }

            // Delete any existing observation generator's files
            var obsGenerators = _sourceEntity.DescendantsAndSelf<ObservationGeneratorEntity>();
            foreach (var obsGen in obsGenerators)
            {
                foreach (var obsFilePath in obsGen.GetGeneratedObservationFiles())
                    File.Delete(obsFilePath);
            }

            return Successful(); // if we were not successful we will try again
        }

    }
}
