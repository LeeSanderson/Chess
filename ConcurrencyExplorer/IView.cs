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
    internal interface IView
    {
        void StartView();
        void CloseView();
    }

    internal delegate void Thunk();
}
