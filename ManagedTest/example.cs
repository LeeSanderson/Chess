/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test15 {
    public class State {
        public State(int n) { N = n; }
        public int signal = 0;
        public int changes = 0;
        public int N;
    }

    public class ChessTest
    {
        static int N;
        public static void Main(string[] s)
        {
            if (Startup(s))
            {
                Run();
            }
        }

        public static bool Startup(string[] s)
        {
            if (s.Length == 1)
            {
                if (Int32.TryParse(s[0], out N))
                {
                    return true;
                }
            }
            N = 2;
            return true;
        }

        public static bool Run()
        {
            State s = new State(N);
            Thread t = new Thread(WriteY);
            t.Start(s);
            for (int i = 0; i < s.N; i++)
            {
                lock (s)
                {
                    if (s.signal == 1)
                        s.changes++;
                    s.signal = 0;
                };
            }
            t.Join();
            return (s.changes <= 6);
        }

        public static void WriteY(object o)
        {
            State s = (State)o;
            for (int j = 0; j < s.N; j++)
            {
                lock (s)
                {
                    if (s.signal == 0)
                        s.changes++;
                    s.signal = 1;
                };
            }
        }
    }
}