/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// THIS LINE (+ a reference to Microsoft.ManagedChessAPI.dll) is needed to access the chess api
using Microsoft.ManagedChessAPI;

namespace RefinementExamples
{
    partial class Tests
    {
        public void i0()
        {
            DoParallel d = new DoParallel();
            IStack<int> stack = new IntStackWrapper(new LockfreeStack<int>());

            d.Add("t1", () => { stack.Push(1); });
            d.Add("t2", () => { int elt; stack.Pop(out elt); });

            d.Execute();
        }
        public void i1()
        {
            DoParallel d = new DoParallel();
            IStack<int> stack = new IntStackWrapper(new LockfreeStack<int>());

            d.Add("t1", () => { stack.Push(1);});
            d.Add("t2", () => { int elt; stack.Pop(out elt); stack.Pop(out elt);});

            d.Execute();
        }
        public void i2()
        {
            DoParallel d = new DoParallel();
            IStack<int> stack = new IntStackWrapper(new LockfreeStack<int>());

            d.Add("t1", () => { stack.Push(1); stack.Push(2); });
            d.Add("t2", () => { int elt; stack.Pop(out elt); stack.Pop(out elt);});

            d.Execute();
        }
        public void i3()
        {
            DoParallel d = new DoParallel();
            IStack<int> stack = new IntStackWrapper(new LockfreeStack<int>());

            d.Add("t1", () => { stack.Push(1); });
            d.Add("t2", () => { stack.Push(2); });
            d.Add("t3", () => { int elt; stack.Pop(out elt); });
            d.Add("t4", () => { int elt; stack.Pop(out elt); });

            d.Execute();
        }
        public void i4()
        {
            DoParallel d = new DoParallel();
            IStack<int> stack1 = new IntStackWrapper(new LockfreeStack<int>());
            IStack<int> stack2 = new IntStackWrapper(new LockfreeStack<int>());

            d.Add("t1", () => { stack1.Push(1); stack2.Push(2); });
            d.Add("t2", () => { int elt; stack1.Pop(out elt); stack2.Pop(out elt); });

            d.Execute();
        }
        public void i5()
        {
            DoParallel d = new DoParallel();
            IStack<int> stack1 = new IntStackWrapper(new LockfreeStack<int>());
            IStack<int> stack2 = new IntStackWrapper(new LockfreeStack<int>());

            d.Add("t1", () => { stack1.Push(1); int elt; stack2.Pop(out elt); });
            d.Add("t2", () => { stack2.Push(2); int elt; stack1.Pop(out elt);  });

            d.Execute();
        }
        public void i6()
        {
            DoParallel d = new DoParallel();
            IStack<int> stack = new IntStackWrapper(new LockfreeStack<int>());

            d.Add("t1", () => { stack.Push(1); });
            d.Add("t2", () => { stack.Push(2); });
            d.Add("t3", () => { int x = Chess.Choose(2) + 1; stack.Contains(x); });

            d.Execute();
        }
       public void i7()
        {
            DoParallel d = new DoParallel();
            IStack<int> stack = new IntStackWrapper(new LockfreeStack<int>());

            d.Add("t1", () => { stack.Push(1); });
            d.Add("t2", () => { stack.Push(2); });
            d.Add("t3", () => { int elt; stack.Pop(out elt); });
            d.Add("t4", () => { int x = Chess.Choose(2) + 1; stack.Contains(x); });

            d.Execute();
        }

   
    }
}
