/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ConcurrencyExplorer
{
    internal interface IController
    {
        // control the controller
        void Start();
        void Join();
    }
}
