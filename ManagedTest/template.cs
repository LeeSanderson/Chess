/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

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
    Thread t = new Thread(delegate() { 
      // put child thread code here
    });
    t.Start();
    // put parent thread code here
    t.Join();
    // check consistency of state
    return true;
  }

  public static void Shutdown() {
    
  } 
}

