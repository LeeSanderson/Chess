/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// Stephen Toub
// stoub@microsoft.com

using System;
using System.Threading;
using System.Collections;

namespace NetMatters
{
	public class TestHarness
	{
		public static void Main()
		{
			using (ThreadPoolWait tpw = new ThreadPoolWait())
			{
				// Queue up a bunch of items and wait for them all
				for(int i=0; i<2; i++)
				{
					tpw.QueueUserWorkItem(new WaitCallback(DoSomething), i);
				}
				while(!tpw.WaitOne(50, false)); // can call WaitOne with a timeout
				Console.WriteLine("done");

				// Queue up a bunch of items and wait for them all
				for(int i=0; i<2; i++)
				{
					tpw.QueueUserWorkItem(new WaitCallback(DoSomething), i);
				}
				tpw.WaitOne(); // wait indefinitely
				Console.WriteLine("done");
			}
		}

        public static void MainFixed()
        {
            using (ThreadPoolWaitFixed tpw = new ThreadPoolWaitFixed())
            {
                // Queue up a bunch of items and wait for them all
                for (int i = 0; i < 2; i++)
                {
                    tpw.QueueUserWorkItem(new WaitCallback(DoSomething), i);
                }
                while (!tpw.WaitOne(50, false)) ; // can call WaitOne with a timeout
                Console.WriteLine("done");

                // Queue up a bunch of items and wait for them all
                for (int i = 0; i < 2; i++)
                {
                    tpw.QueueUserWorkItem(new WaitCallback(DoSomething), i);
                }
                tpw.WaitOne(); // wait indefinitely
                Console.WriteLine("done");
            }
        }
		public static void DoSomething(object state)
		{
			for(int i=0; i<2; i++);
			Console.WriteLine(state.ToString());
		}
	}
}