/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SimpleBank
{
    public class Test1
    {
        public static bool Run()
        {
            Account account = new Account(10);

            // set up two threads to withdraw/deposit concurrently
            Thread t = new Thread(() => account.Withdraw(5));
            Thread s = new Thread(() => account.Deposit(5));

            t.Start();
            s.Start();
            t.Join();
            s.Join();

            return (account.GetBalance() == 10);
        }
    }
}
