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
//using MChess;
//using Microsoft.ManagedChess;
using Microsoft.ManagedChess.EREngine;

using OMini = global::MiniMPI;

namespace __Substitutions.MiniMPI
{
    static public class MiniMPIProcessAPI
    {
        // TODO:
        // we need a wrapper for matching wildcard receive and calling Chess.Choose()
        // to resolve the nondeterministic receive.

        // TODO: we want some attribute on the MiniMPI Runtime to tell ER/CHESS
        // not to instrument it (rather than having to hack the tests)
        // INSTEAD: MiniMPI Runtime is now part of CHESS, so it should not be instrumented???
        // INSTEAD: check this... not sure

        internal static void MpiInit(OMini::MiniMPIProcessAPI self)
        {
            Console.WriteLine("MpiInit");
            self.MpiInit();
        }

        internal static void MpiFinalize(OMini::MiniMPIProcessAPI self)
        {
            Console.WriteLine("MpiFinalize");
            self.MpiFinalize();
        }

    }
}
