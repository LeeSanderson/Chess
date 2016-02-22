/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test7
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
                b.withdraw(5);
            });
            Thread u = new Thread(delegate(object o)
            {
                Account b = (Account)o;
                b.withdraw(5);
            });
            t.Start(a);
            u.Start(a);
            // put parent thread code here
            a.deposit(10);
            t.Join();
            u.Join();
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
            lock (this)
            {
                balance = balance - n;
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