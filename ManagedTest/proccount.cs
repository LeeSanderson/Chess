/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
namespace ProcessorCount
{
    public class ChessTest
    {
	public static void Main() {
           Run();
        }
        public static bool Run() {
           Console.WriteLine("Environment.ProcessorCount = {0}", Environment.ProcessorCount); 
           return true;
        }

    }

}
