/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Examples.AdvancedProgramming.AsynchronousOperations
{

public class ChessTest {

  // the regular test entry point
  public static void Main(string [] s) { 
       Run();
  }

  public static bool Run() {
     return AsyncMain.Main2();
  }

}

    public class AsyncDemo 
    {
        // The method to be executed asynchronously.
        public string TestMethod(int callDuration, out int threadId) 
        {
            Thread.Sleep(1);
            threadId = Thread.CurrentThread.ManagedThreadId;
            return String.Format("{0}", threadId);
        }
    }
    // The delegate must have the same signature as the method
    // it will call asynchronously.
    public delegate string AsyncMethodCaller(int callDuration, out int threadId);

    public class AsyncMain 
    {
        public static bool Main2() 
        {
            // The asynchronous method puts the thread id here.
            int threadId;

            // Create an instance of the test class.
            AsyncDemo ad = new AsyncDemo();

            // Create the delegate.
            AsyncMethodCaller caller = new AsyncMethodCaller(ad.TestMethod);

            // Initiate the asychronous call.
            IAsyncResult result = funnyhelper(caller,out threadId);

            Thread.Sleep(0);

            // Call EndInvoke to wait for the asynchronous call to complete,
            // and to retrieve the results.
            string returnValue = caller.EndInvoke(out threadId, result);

            bool flag = (threadId == Convert.ToInt32(returnValue));
            return flag;
        }

	public static IAsyncResult funnyhelper(AsyncMethodCaller caller, out int tid) {
             try {
               return caller.BeginInvoke(3000, out tid, null, null);
             } catch (Exception) {
               tid = 0;
               return null;
             }
        }
    }
}
