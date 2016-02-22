/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace Test4 {
public class ChessTest {

  // the regular test entry point
  public static void Main(string [] s) { 
     if (Startup(s)) {
       Run();
     }
  }

  // validate input 
  public static bool Startup(string[] s) {
    return true;
  }

  public static bool Run() {
    AutoResetEvent e = new AutoResetEvent(false);
    Thread t = new Thread(delegate(object o) { 
      AutoResetEvent e2 = (AutoResetEvent) o;
      // put child thread code here
      e2.Set();
      Console.WriteLine("child thread");
      e2.Set();
    });
    t.Start(e);
    // put parent thread code here
    e.WaitOne();
    Console.WriteLine("parent thread");
    e.WaitOne();
    // check consistency of state
    return true;
  }
}

}