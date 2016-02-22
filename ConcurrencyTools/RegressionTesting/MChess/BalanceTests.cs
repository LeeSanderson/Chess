using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    public class BalanceTests : MChessRegressionTestBase
    {
        // ORIG: balance.cs
        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 3, LastThreadCount = 2, LastExecSteps = 15, LastHBExecSteps = 2)]
        [ExpectedChessResult("csb2", ChessExitCode.UnitTestAssertFailure)]
        [ExpectedChessResult("csb3", ChessExitCode.UnitTestAssertFailure)]
        public void BalanceTest()
        {
            Account1 a = new Account1();

            Thread t = new Thread(delegate(object o) {
                Account1 b = (Account1)o;
                b.withdraw(10);
            });
            t.Start(a);
            // put parent thread code here
            a.deposit(10);
            t.Join();

            int finalBalance = a.read();
            Assert.AreEqual(Account1.InitialBalance, finalBalance);
        }

        // ORIG: balance2.cs
        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.Success, SchedulesRan = 19, LastThreadCount = 3, LastExecSteps = 20, LastHBExecSteps = 6)]
        [ExpectedChessResult("csb2", ChessExitCode.Success, SchedulesRan = 33, LastThreadCount = 3, LastExecSteps = 20, LastHBExecSteps = 6)]
        [ExpectedChessResult("csb3", ChessExitCode.Success, SchedulesRan = 41, LastThreadCount = 3, LastExecSteps = 20, LastHBExecSteps = 6)]
        public void BalanceTest2()
        {
            Account2 a = new Account2();

            Thread t = new Thread(delegate(object o) {
                Account2 b = (Account2)o;
                b.withdraw(5);
            });
            Thread u = new Thread(delegate(object o) {
                Account2 b = (Account2)o;
                b.withdraw(5);
            });
            t.Start(a);
            u.Start(a);
            // put parent thread code here
            a.deposit(10);
            t.Join();
            u.Join();

            int finalBalance = a.read();
            Assert.AreEqual(Account2.InitialBalance, finalBalance);
        }

        public class Account1
        {
            public const int InitialBalance = 10;

            int balance;

            public Account1()
            {
                balance = InitialBalance;
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

        public class Account2
        {
            public const int InitialBalance = 10;

            int balance;

            public Account2()
            {
                balance = InitialBalance;
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
}
