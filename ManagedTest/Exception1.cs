/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

namespace NamespaceException1 {
public class ChessTest
{
    public static void Main(string[] args)
    {
    }

    public static bool Startup(string[] args)
    {
        return true;
    }
    
    public static bool Run()
    {
        throw new Exception();
        return true;
    }
    
    public static bool Shutdown()
    {
        return true;
    }
}
}