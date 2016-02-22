/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test6
{
    public class ChessTest
    {

        // the regular test entry point
        public static void Main(string[] s)
        {
            if (Startup(s))
            {
                bool r = Run();
                Console.WriteLine(r);
                Shutdown();
            }
        }

        // validate input 
        public static bool Startup(string[] s)
        {
            return true;
        }

        public static bool Run()
        {
            Account a = new Account();
            Thread t = new Thread(delegate(object o)
            {
                Account b = (Account)o;
                b.withdraw(10);
            });
            t.Start(a);
            // put parent thread code here
            a.deposit(10);
            t.Join();
            return (a.read() == 10);
        }

        public static bool Shutdown()
        {
            return true;
        }
    }


    public class Account
    {

        int balance;

        public Account()
        {
            balance = 10;
        }

        public void withdraw(int n)
        {
            int r = read();
            lock (this)
            {
                balance = r - n;
            }
        }

        public int read()
        {
            int r;
            lock (this)
            {
                r = balance;
            }
            return r;
        }

        public void deposit(int n)
        {
            lock (this)
            {
                balance = balance + n;
            }
        }
    }
}