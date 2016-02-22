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

  static Thread t = new Thread(RunStatic);

  // validate input 
  public static bool Startup(string[] s) {
    return true;
  }

  public static bool Run() {
    t.Start();
    // put parent thread code here
    t.Join();
    // check consistency of state
    return true;
  }

  static void RunStatic() {
    // do nothing for now
  }

  public static void Shutdown() {
    
  } 
}

