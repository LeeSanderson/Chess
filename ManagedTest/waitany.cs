/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.IO;
using System.Threading;

namespace Test47
{
    public class ChessTest
    {
        public static bool Run()
        {
            Main();
            return true;
        }

        static void Main()
        {
            Search search = new Search();
            search.FindFile("foo");
        }
    }


    class Search
    {
        static string[] diskLetters = { "C", "D", "E" };
        static string[] files = {
        "C:foo", "D:bar", "C:baz", 
        "E:bar", "E:foo", "D:foo",
    };

        // Maintain state information to pass to FindCallback.
        class State
        {
            public AutoResetEvent autoEvent;
            public string fileName;

            public State(AutoResetEvent autoEvent, string fileName)
            {
                this.autoEvent = autoEvent;
                this.fileName = fileName;
            }
        }

        AutoResetEvent[] autoEvents;

        public Search()
        {
            autoEvents = new AutoResetEvent[diskLetters.Length];
            for (int i = 0; i < diskLetters.Length; i++)
            {
                autoEvents[i] = new AutoResetEvent(false);
            }
        }

        // Search for fileName in the root directory of all disks.
        public void FindFile(string fileName)
        {
            for (int i = 0; i < diskLetters.Length; i++)
            {
                Console.WriteLine("Searching for {0} on {1}.",
                    fileName, diskLetters[i]);
                ThreadPool.QueueUserWorkItem(
                    new WaitCallback(FindCallback),
                    new State(autoEvents[i], diskLetters[i] + ":" + fileName));
            }

            // Wait for the first instance of the file to be found.
            int index = WaitHandle.WaitAny(
                autoEvents, new TimeSpan(0, 0, 3), false);
            if (index == WaitHandle.WaitTimeout)
            {
                Console.WriteLine("\n{0} not found.", fileName);
            }
            else
            {
                Console.WriteLine("\n{0} found on {1}.", fileName,
                    diskLetters[index]);
            }
        }

        // Search for stateInfo.fileName.
        void FindCallback(object state)
        {
            State stateInfo = (State)state;

            foreach (string s in files)
            {
                if (s.Equals(stateInfo.fileName))
                {
                    stateInfo.autoEvent.Set();
                    break;
                }
            }
        }
    }
}