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
    public class Account
    {
        int balance;

        public Account(int initial_balance)
        {
            this.balance = initial_balance;
        }

        public void Deposit(int amount)
        {
            this.balance += amount;
        }

        public void Withdraw(int amount)
        {
            this.balance -= amount;
        }

        public int GetBalance()
        {
            return balance;
        }
        
    }
}
