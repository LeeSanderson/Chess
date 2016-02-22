/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.Diagnostics;
using Microsoft.ManagedChess;
using Microsoft.ManagedChess.EREngine;

using OMini = global::MiniMPI;

namespace __Substitutions.MiniMPI
{
    static public class MiniMPIProgram
    {
        public static void Execute(int processCount, Action<OMini.IMiniMPIStringAPI> processWork)
        {
            //Console.WriteLine("MiniMPIProgram.Execute {0}", processCount);
            // disable preemptions in each worker
            Action<OMini.IMiniMPIStringAPI> processWorkWrap =
                (x) => {
                    using(new WrapperSentry()){
                        MChessChess.PreemptionDisable();
                    }
                    processWork(x); 
                    using(new WrapperSentry()){
                        MChessChess.PreemptionEnable(); 
                    }
                };
            // disable preemptions in main
            using (new WrapperSentry())
            {
                MChessChess.PreemptionDisable();
            }
            OMini.MiniMPIProgram.Execute(processCount, processWorkWrap);
            using (new WrapperSentry())
            {
                MChessChess.PreemptionEnable();
            }
        }
    }
}
