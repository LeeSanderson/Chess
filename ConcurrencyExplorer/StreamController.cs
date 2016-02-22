/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace Microsoft.ConcurrencyExplorer
{
    internal class StreamController : IController
    {
        internal StreamController(TextReader stream, IModel model)
        {
            input = stream;
            this.model = model;
            controllerthread = new Thread(new ThreadStart(parse));
            controllerthread.Name = "streamcontroller";
        }

        private IModel model;
        private Thread controllerthread;

        public void Start()
        {
            controllerthread.Start();
        }
 
        public void Join()
        {
            controllerthread.Join();
        }

        private TextReader input;
        private int cur;

        private void parse()
        {
            cur = input.Read();
            while (cur != -1)
            {
                if (cur == new_exec_mark)
                {
                    lock (model)
                    {
                        model.StartNewExecution();
                    }
                    cur = input.Read(); // skip mark
                }
                else if (cur == 10 || cur == 13)
                {
                    cur = input.Read(); // skip newline/linefeed characters
                }
                else if (cur >= zero && cur <= nine)
                {
                    int thread = parse_unsigned();
                    cur = input.Read(); // skip blank
                    int nr = parse_unsigned();
                    cur = input.Read(); // skip blank
                    int attr = parse_unsigned();
                    cur = input.Read(); // skip blank
                    int strlen2 = parse_unsigned();
                    cur = input.Read(); // skip blank
                    String val = parse_string(strlen2);
                    lock (model)
                    {
                        model.ProcessTuple(thread, nr, attr, val);
                    }
                }
                else
                    parse_error("unexpected character: " + cur + " '" + Convert.ToChar(cur) + "'");

            }
            lock (model)
            {
                model.SetComplete();
            }
        }

        private int parse_unsigned()
        {
            int x = 0;
            if (cur < zero || cur > nine)
                parse_error("expect unsigned string");
            do
            {
                x = 10 * x + (cur - zero);
                cur = input.Read();
            } while (cur >= zero && cur <= nine);
            return x;
        }

        private String parse_string(int len)
        {
            StringBuilder sb = new StringBuilder();
            while (len-- > 0)
            {
                sb.Append(Convert.ToChar(cur));
                cur = input.Read();
                if (cur == -1)
                    parse_error("unexpected end of file");
            }
            return sb.ToString();
        }

        private int zero = Convert.ToInt32('0');
        private int nine = Convert.ToInt32('9');
        private int new_exec_mark = Convert.ToInt32('#');

        private static void parse_error(String message)
        {
            System.Console.Error.WriteLine(message);
            Environment.Exit(-1);
        }

    }
}
