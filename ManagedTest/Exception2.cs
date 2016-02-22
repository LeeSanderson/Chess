/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace NamespaceException2 {
public class ChessTest
{
    public static void Main(string[] args)
    {
    }

    public static bool Startup(string[] args)
    {
        return true;
    }
    
    public static void Begin()
    {
        throw new Exception();
    }

    public static bool Run()
    {
        Thread t = new Thread(Begin);
        t.Start();
        t.Join();
        return true;
    }

    public static bool Shutdown()
    {
        return true;
    }
}
}