/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bank
{
    public class Account
    {
        private int balance;

        public Account(int amount)
        {
            balance = amount;
        }

        public void Withdraw(int amount)
        {
            int temp = Read();
            // oops, temp could become stale if we are
            // preempted here
            lock (this)
            {
                balance = temp - amount;
            }
        }

        public int Read()
        {
            int temp;
            lock (this)
            {
                temp = balance;
            }
            return temp;
        }

        public void Deposit(int amount)
        {
            lock (this)
            {
                balance = balance + amount;
            }
        }
    }
}
