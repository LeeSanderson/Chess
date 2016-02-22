/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace LazyInit
{
    public class ChessTest
    {
        static public volatile int count;
       
        static public void Main(string[] args)
        {
           Run();
           Run();
           Run();
        }

        
        static public bool Run() 
        {

            // side thread: read count, ignore value
            ThreadPool.QueueUserWorkItem((x) => { int dummy = count; });

            // main thread: purposefully not idempotent
       
            if (count == 0)
            {
                Interlocked.Increment(ref count);
                return true;
            }
            else
            {
                count = count + 1;
                return false;
            }
            
        }
    }
}
