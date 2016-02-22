/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RefinementExamples
{
    partial class Tests
    {
        private class Flag
        {
            public volatile bool value;
        }

        public void o1()
        {
            DoParallel d = new DoParallel();
            IStack<Flag> stack = new ObjStackWrapper<Flag>(new LockfreeStack<Flag>());

            d.Add("t1", () => { stack.Push(new Flag()); stack.Push(new Flag()); });
            d.Add("t2", () => { Flag elt; stack.Pop(out elt); stack.Pop(out elt); });

            d.Execute();
        }


    }
}
