/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Threading;
using Bank;

namespace TestBank
{
    // define this class for use with mchess or ChessBoard
    public class ChessTest
    {
        static public bool Run()
        {
            new TestBank().WithdrawAndDepositConcurrently();
            return true;
        }
    }
}