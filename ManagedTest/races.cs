/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test49
{
    public class ChessTest
    {
        public static void Main(string[] s)
        {
            if (Test49.ChessTest.Startup(s))
            {
                bool ret = Test49.ChessTest.Run();
                Console.WriteLine(ret);
            }
        }

        public static bool Startup(string[] args)
        {
            try
            {
                testmethod = typeof(TestRace).GetMethod(args[0], new Type[0]);
                return (testmethod != null);
            }
            catch
            {
                return false;
            }
        }

        private static System.Reflection.MethodInfo testmethod;

        public static bool Run()
        {
            TestRace tr = new TestRace();
            try
            {
                testmethod.Invoke(tr, new object[0]);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class TestRace
    {
        private int x = 0;
        private int y = 0;
        private int z = 0;

        private volatile int vx = 0;
        private volatile int vy = 0;
        private volatile int vz = 0;

        private void nonvolread(ref int target, ref int source)
        {
            target = source;
        }
        private void nonvolwrite(ref int target, int val)
        {
            target = val;
        }

        private void Run(ThreadStart a, ThreadStart b)
        {
            var child = new Thread(b);
            child.Start();
            a();
            child.Join();
        }

        public void RaceTest1()  // expect to detect each of 3 races once (only one sched explored)
        {
            Run(() => {  x = 1; }, () => { x = 1; } );  // RACE dwrite, dwrite
            Run(() => {  x = 1; }, () => { y = x; } );  // RACE dwrite, dread
            Run(() => {  y = x; }, () => { x = 1; } );  // RACE dread, dwrite
            Run(() => {  x = y; }, () => { z = y; } );  // NORACE dread, dread
        }

        public void RaceTest2()  // expect to detect each of 3 races once (only one sched explored)
        {
            Run(() => { nonvolwrite(ref vx, 1); }, () => { vx = 1; });  // RACE dwrite, vwrite
            Run(() => { nonvolwrite(ref vx, 1); }, () => { vy = vx; } );  // RACE dwrite, vread
            Run(() => { nonvolread(ref y, ref vx); }, () => { vx = 1; } );  // RACE dread, vwrite
            Run(() => { nonvolread(ref x, ref vy); }, () => { vz = y; } );  // NORACE dread, vread
        }

        public void RaceTest3a()  // expect to detect the race twice
        {
            Run(() => { vx = 1; }, () => { nonvolwrite(ref vx, 1); });  // RACE vwrite, dwrite
        }
        public void RaceTest3b()  // expect to detect the race twice
        {
            Run(() => { vz = 1; }, () => { nonvolread(ref y, ref vz); });  // RACE vwrite, dread
        }
        public void RaceTest3c()  // expect to detect the race twice
        {
            Run(() => { vy = vx; }, () => { nonvolwrite(ref vx, 1); });  // RACE vread, dwrite
        }
        public void RaceTest3d()  // expect to detect no race
        {
            Run(() => { vx = vy; }, () => { nonvolread(ref z, ref vy); });  // NORACE vread, dread
        }

        public void RaceTest4() // not a race... correct use of volatile flag
        {
            Run(() =>
            {
                vx = 1;
                y = 1;
            }, () =>
            {
                if (vx == 1)
                    y = y + 1;  
            });
        }
    }
}
