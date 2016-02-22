using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.UnitTesting;
using System.Threading;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;

namespace Microsoft.Concurrency.MChess.RegressionTests
{
    public class Deadlock : MChessRegressionTestBase
    {
        static Object lock1;
	    static Object lock2;
        static int x;

        public void init()
        {
            lock1 = new Object();
            lock2 = new Object();
            x = 0;
        }

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.ChessDeadlock, SchedulesRan = 4, LastThreadCount = 2, LastExecSteps = 15, LastHBExecSteps = 2)]
        [ExpectedChessResult("csb2", ChessExitCode.ChessDeadlock, SchedulesRan = 6, LastThreadCount = 2, LastExecSteps = 15, LastHBExecSteps = 2)]
        [ExpectedChessResult("csb3", ChessExitCode.ChessDeadlock, SchedulesRan = 6, LastThreadCount = 2, LastExecSteps = 15, LastHBExecSteps = 2)]
        public void Test()
        {
            init();
            Thread t = new Thread(Child);
            t.Start();
	        Parent();
            t.Join();
        }

	    void Parent()
        {
            lock (lock1)
            {
                lock (lock2)
                {
                    x++;
                }
            }
        }

        void Child()
        {
            lock (lock2)
            {
                lock (lock1)
                {
                    x++;
                }
            }
        }

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.ChessLivelock, SchedulesRan = 1, LastThreadCount = 3, LastExecSteps = 20, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb2", ChessExitCode.ChessLivelock, SchedulesRan = 1, LastThreadCount = 3, LastExecSteps = 20, LastHBExecSteps = 1)]
        [ExpectedChessResult("csb3", ChessExitCode.ChessLivelock, SchedulesRan = 1, LastThreadCount = 3, LastExecSteps = 20, LastHBExecSteps = 1)]
        public void Test2()
        {
            init();
            Thread t = new Thread(Child);
            Thread u = new Thread(Child2);
            t.Start();
            u.Start();
            Parent();
            t.Join();
            u.Join();
        }

        void Child2()
        {
            while (x != 2)
            {
                Thread.Sleep(2);
                ;
            }
        }

        [ChessTestMethod]
        [ExpectedChessResult("csb1", ChessExitCode.ChessLivelock, SchedulesRan = 3, LastThreadCount = 2, LastExecSteps = 23, LastHBExecSteps = 2)]
        [ExpectedChessResult("csb2", ChessExitCode.ChessLivelock, SchedulesRan = 11, LastThreadCount = 2, LastExecSteps = 23, LastHBExecSteps = 3)]
        [ExpectedChessResult("csb3", ChessExitCode.ChessLivelock, SchedulesRan = 11, LastThreadCount = 2, LastExecSteps = 23, LastHBExecSteps = 3)]
        public void Test3()
        {
            init();
            Thread t = new Thread(Child3);
            t.Start();
            Parent3();
            t.Join();
        }


        void Parent3()
        {
            bool done = false;
            while (!done)
            {
                lock (lock1)
                {
                    if (Monitor.TryEnter(lock2))
                    {
                        x++;
                        done = true;
                        Monitor.Exit(lock2);
                    }
                    if (!done)
                    {
                        Thread.Sleep(1);
                    }
                }
            }
        }

        void Child3()
        {
            bool done = false;
            while (!done)
            {
                lock (lock2)
                {
                    if (Monitor.TryEnter(lock1))
                    {
                        x++;
                        done = true;
                        Monitor.Exit(lock1);
                    }
                    if (!done)
                    {
                        Thread.Sleep(1);
                    }
                }
            }
        }
    }
}