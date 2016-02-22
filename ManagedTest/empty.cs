/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Threading;

public class ChessTest {

    static void Main() {
        Console.WriteLine("Use MChess man!");
        Run();
    }

    public static bool Run() {
        try {
            var t2 = new Thread(() => {
            });
            t2.Start();

            t2.Join();

            return true;

        } catch (Exception e) {
            Console.WriteLine(e);
            return false;
        }
    }

    public static bool Shutdown() {
        Console.WriteLine("Shutdown");
        return true;
    }

} // class