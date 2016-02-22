/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;
using System.Threading.Tasks;

public class ChessTest {

  static void DoWork() { Console.WriteLine("here"); }

  public static bool Run() { 
    var t2 = Task.Factory.StartNew(() => DoWork());
    t2.Wait(); 
    return true;
  }

  public static void Main() {
	Run();
  }
}
