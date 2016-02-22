/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    /// <summary>
    /// Summary description for badthread
    /// </summary>
    [TestClass]
    public class badthread
    {
        [TestMethod]
        [HostType("Chess")]
        // test is inconclusive, but we don't have a good error message about it.
        [TestProperty("ChessExpectedResult","invalidtest")]
        public void ThreadNotEnded()
        {
            // nobody does a set, so child thread never ends
            var eventHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            Thread child = new Thread(() => { eventHandle.WaitOne(); });
            child.Start();
            Thread.Sleep(1);
            // oops, we don't join
        }

        [TestMethod]
        [HostType("Chess")]
        // test is inconclusive, but we don't have a good error message about it.
        [TestProperty("ChessExpectedResult", "invalidtest")]
        public void ThreadNotEnded2()
        {
            Thread child = new Thread(() => { });
            // child isn't even started, but CHESS counts it as running!
        }

        static int count = 0;

        [TestMethod]
        [HostType("Chess")]
        // test is inconclusive, but we don't have a good error message about it.
        [TestProperty("ChessExpectedResult", "incompleteinterleavingcoverage")]
        public void DoesntResetState()
        {
            Thread child = new Thread(() => { Thread.Sleep(1); });
            child.Start();
            if (count % 2 == 0)
                Thread.Sleep(1);
            count++;
            child.Join();
        }
    }
}
