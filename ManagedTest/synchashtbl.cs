/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;
using System.Collections;

public class ChessTest
{
    static public void Main()
    {
        Console.WriteLine(Run());
    }

    static public bool Run()
    {
        Hashtable original = new Hashtable();
        Hashtable synchronized = Hashtable.Synchronized(original);
        if (!synchronized.IsSynchronized)
        {
            return false;
        }

        Thread thread = new Thread(() => synchronized.Add(true, true));

        lock (synchronized.SyncRoot)
        {
            thread.Start();
            thread.Join();   // should deadlock because child thread should block indefinitely on Add
        }
        return true;
    }
}
