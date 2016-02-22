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
    public class ChessTest
    {
        static public void Main(string[] args)
        {
            Console.WriteLine(Run());
        }

        static public bool Run()
        {
            var account = new Account(10);
            var child = new Thread(
               o => { (o as Account).Withdraw(2); }
               );
            child.Start(account);
            account.Deposit(1);
            child.Join();

            return (account.Read()==9);
        }
    }
}