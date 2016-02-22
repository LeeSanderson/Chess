/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;
using System.Threading.Tasks;

public class ChessTest {

private static volatile int v;

  public static bool Run() { 
    v = 0;
    Parallel.For(0, 2, i => Console.WriteLine("i " + i + " v=" + (v++))); 
    return true;
  }

  public static void Main() {
	Run();
  }
}
