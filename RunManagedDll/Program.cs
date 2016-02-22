/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace RunDLL
{
    public class ChessTest
    {
        public static bool Startup(string[] a)
        {
            return Program.ParseArgs(a);
        }

        public static bool Run()
        {
            return Program.RunMethod();
        }

    }
    
    internal class Program
    {
        static string assemblyname, fulltypename, methodname;
        static Assembly assembly;
        static Type type;
        static ConstructorInfo ctor;
        static MethodInfo method;

        public static bool ParseArgs(string[] args) 
        {
            if (args.Length == 3)
            {
                // /a=assembly.dll /t=fulltypename /m=methodname
                assemblyname = args[0];
                fulltypename = args[1];
                methodname = args[2];
                // load the assembly
                try {
                    assembly = Assembly.LoadFile(assemblyname);
                    type = assembly.GetType(fulltypename);
                    ctor = type.GetConstructor(Type.EmptyTypes);
                    method = type.GetMethod(methodname, BindingFlags.Public | BindingFlags.Instance);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
                return true;
            }
            else
            {
                Console.WriteLine("usage: rundll.exe <assembly> <fulltypename> <methodname>");
                return false;
            }
        }

        public static bool RunMethod()
        {
            try
            {
                object value = ctor.Invoke(null);
                method.Invoke(value, null);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public static void Main(string[] args)
        {
        }
    }
}
