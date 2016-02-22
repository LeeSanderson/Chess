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
    // helper class for constructing concurrent unit tests
    //
    class IntStackWrapper : IStack<int>
    {
        public IntStackWrapper(IStack<int> impl)
        {
            this.impl = impl;
        }
        private IStack<int> impl;

        public void Push(int elt)
        {
            Chess.ObserveOperationCall(impl, "Push");
            Chess.ObserveInteger("elt", elt);
            impl.Push(elt);
            Chess.ObserveOperationReturn();
        }

        public bool Pop(out int elt)
        {
            Chess.ObserveOperationCall(impl, "Pop");
            bool retval = impl.Pop(out elt);
            Chess.ObserveBoolean("retval", retval);
            Chess.ObserveInteger("elt", elt);
            Chess.ObserveOperationReturn();
            return retval;
        }

        public void Clear()
        {
            Chess.ObserveOperationCall(impl, "Clear");
            impl.Clear();
            Chess.ObserveOperationReturn();
        }

        public bool Contains(int elt)
        {
            Chess.ObserveOperationCall(impl, "Contains");
            Chess.ObserveInteger("elt", elt);
            bool retval = impl.Contains(elt);
            Chess.ObserveBoolean("retval", retval);
            Chess.ObserveOperationReturn();
            return retval;
        }

    }

   class ObjStackWrapper<T> : IStack<T>
    {
        public ObjStackWrapper(IStack<T> impl)
        {
            this.impl = impl;
        }
        private IStack<T> impl;

        public void Push(T elt)
        {
            Chess.ObserveOperationCall(impl, "Push");
            Chess.ObserveObjectIdentity("elt", elt);
            impl.Push(elt);
            Chess.ObserveOperationReturn();
        }

        public bool Pop(out T elt)
        {
            Chess.ObserveOperationCall(impl, "Pop");
            bool retval = impl.Pop(out elt);
            Chess.ObserveBoolean("retval", retval);
            Chess.ObserveObjectIdentity("elt", elt);
            Chess.ObserveOperationReturn();
            return retval;
        }

        public void Clear()
        {
            Chess.ObserveOperationCall(impl, "Clear");
            impl.Clear();
            Chess.ObserveOperationReturn();
        }

        public bool Contains(T elt)
        {
            Chess.ObserveOperationCall(impl, "Contains");
            Chess.ObserveObjectIdentity("elt", elt);
            bool retval = impl.Contains(elt);
            Chess.ObserveBoolean("retval", retval);
            Chess.ObserveOperationReturn();
            return retval;
        }

    }
}
