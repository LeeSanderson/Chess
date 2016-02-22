/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Concurrency.TestTools.Alpaca.Aspects;
using Microsoft.Concurrency.TestTools.Execution;

namespace Microsoft.Concurrency.TestTools.Alpaca
{
    public delegate void ModelEventHandler();
    public delegate void ModelEventHandler<T>(T t);
    public delegate void ModelEventHandler<T, S>(T t, S s);
    public delegate void ModelEventHandler<T, S, R>(T t, S s, R r);

    /// <summary>The handler for when something happens to an entity in the model.</summary>
    delegate void ModelEntityEventHandler<TArgs>(EntityBase entity, TArgs e) where TArgs : ModelEntityEventArgs;

    /// <summary>The handler for when something happens to a specific entity type in the model.</summary>
    delegate void ModelEntityEventHandler<TEntity, TArgs>(TEntity model, TArgs e)
        where TEntity : EntityBase
        where TArgs : ModelEntityEventArgs;

}
