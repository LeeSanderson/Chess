/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bank;

namespace TestBank
{
    [TestClass]
    public class TestBank
    {
        [TestMethod]
        public void WithdrawAndDepositConcurrently()
        {
            var account = new Account(10);
            var child = new Thread(
               o => { (o as Account).Withdraw(2); }
               );
            child.Start(account);
            account.Deposit(1);
            child.Join();

            Assert.AreEqual<int>(9, account.Read());
        }
    }
}