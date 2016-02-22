/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System.Configuration;
using System;

namespace TestAppConfigFile
{
    public class ChessTest
    {
        public static void Main()
        {
            Console.WriteLine(Run());
        }
        // note: requires presence of config.exe.config with contents:
        //
        //<?xml version="1.0" encoding="utf-8" ?>
        //<configuration>
        //  <appSettings>
        //    <add key="foo" value="bar"/>
        //  </appSettings>
        //</configuration>

        public static bool Run()
        {
            return("bar" == ConfigurationSettings.AppSettings["foo"]);
        }
    }
}