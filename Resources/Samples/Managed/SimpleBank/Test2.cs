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
    public class Test2
    {
        public static bool Run()
        {
            Account account = new Account(10);
            int bal = 0;

            // set up 3 threads to withdraw/deposit/getbalance concurrently
            Thread t = new Thread(() => account.Withdraw(5));
            Thread s = new Thread(() => account.Deposit(5));
            Thread r = new Thread(() => bal = account.GetBalance());

            t.Start();
            s.Start();
            r.Start();
            t.Join();
            s.Join();
            r.Join();

            return (account.GetBalance() == 10
                              && (bal == 5 || bal == 10 || bal == 15));
        }
    }
}
