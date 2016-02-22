// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
using System;
using System.Collections.Generic;
using System.Text;
using Original = global::System.Threading.Tasks;
using Microsoft.ExtendedReflection.Monitoring;
using System.Runtime.InteropServices;
using System.Security;
using ClrThread = System.Int32;
using ChessTask = System.Int32;
using System.Threading;
using MChess;
using System.Diagnostics;
using Microsoft.ManagedChess.EREngine;
using System.Threading.Tasks;

namespace __Substitutions.System.Threading.Tasks
{
    [DebuggerNonUserCode]
    public static class TaskFactory
    {
        // continue when any
        /*
        public static Original::Task ContinueWhenAll<TAntecedentResult>(
            Original::TaskFactory self,Original::Task<TAntecedentResult>[] tasks, Action<Original::Task<TAntecedentResult>[]> continuationAction){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(
            Original::TaskFactory self,Original::Task<TAntecedentResult>[] tasks, Func<Original::Task<TAntecedentResult>[], TResult> continuationFunction){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task ContinueWhenAll(
            Original::TaskFactory self,Original::Task[] tasks, Action<Original::Task[]> continuationAction){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> ContinueWhenAll<TResult>(
            Original::TaskFactory self,Original::Task[] tasks, Func<Original::Task[], TResult> continuationFunction){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task ContinueWhenAll<TAntecedentResult>(
            Original::TaskFactory self,Original::Task<TAntecedentResult>[] tasks, Action<Original::Task<TAntecedentResult>[]> continuationAction, CancellationToken cancellationToken){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task ContinueWhenAll<TAntecedentResult>(
            Original::TaskFactory self,Original::Task<TAntecedentResult>[] tasks, Action<Original::Task<TAntecedentResult>[]> continuationAction, TaskContinuationOptions continuationOptions){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(
            Original::TaskFactory self,Original::Task<TAntecedentResult>[] tasks, Func<Original::Task<TAntecedentResult>[], TResult> continuationFunction, CancellationToken cancellationToken){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(
            Original::TaskFactory self,Original::Task<TAntecedentResult>[] tasks, Func<Original::Task<TAntecedentResult>[], TResult> continuationFunction, TaskContinuationOptions continuationOptions){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task ContinueWhenAll(
            Original::TaskFactory self,Original::Task[] tasks, Action<Original::Task[]> continuationAction, CancellationToken cancellationToken){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task ContinueWhenAll(
            Original::TaskFactory self,Original::Task[] tasks, Action<Original::Task[]> continuationAction, TaskContinuationOptions continuationOptions){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> ContinueWhenAll<TResult>(
            Original::TaskFactory self,Original::Task[] tasks, Func<Original::Task[], TResult> continuationFunction, CancellationToken cancellationToken){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> ContinueWhenAll<TResult>(
            Original::TaskFactory self,Original::Task[] tasks, Func<Original::Task[], TResult> continuationFunction, TaskContinuationOptions continuationOptions){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task ContinueWhenAll<TAntecedentResult>(
            Original::TaskFactory self,Original::Task<TAntecedentResult>[] tasks, Action<Original::Task<TAntecedentResult>[]> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(
            Original::TaskFactory self,Original::Task<TAntecedentResult>[] tasks, Func<Original::Task<TAntecedentResult>[], TResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task ContinueWhenAll(
            Original::TaskFactory self,Original::Task[] tasks, Action<Original::Task[]> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> ContinueWhenAll<TResult>(
            Original::TaskFactory self,Original::Task[] tasks, Func<Original::Task[], TResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler){ throw new NotImplementedException("TaskFactory"); }
        
        // continue when all
        public static Original::Task ContinueWhenAny<TAntecedentResult>(
            Original::TaskFactory self,Original::Task<TAntecedentResult>[] tasks, Action<Original::Task<TAntecedentResult>> continuationAction){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(
            Original::TaskFactory self,Original::Task<TAntecedentResult>[] tasks, Func<Original::Task<TAntecedentResult>, TResult> continuationFunction){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task ContinueWhenAny(
            Original::TaskFactory self,Original::Task[] tasks, Action<Original::Task> continuationAction){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> ContinueWhenAny<TResult>(
            Original::TaskFactory self,Original::Task[] tasks, Func<Original::Task, TResult> continuationFunction){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task ContinueWhenAny<TAntecedentResult>(
            Original::TaskFactory self,Original::Task<TAntecedentResult>[] tasks, Action<Original::Task<TAntecedentResult>> continuationAction, CancellationToken cancellationToken){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task ContinueWhenAny<TAntecedentResult>(
            Original::TaskFactory self,Original::Task<TAntecedentResult>[] tasks, Action<Original::Task<TAntecedentResult>> continuationAction, TaskContinuationOptions continuationOptions){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(
            Original::TaskFactory self,Original::Task<TAntecedentResult>[] tasks, Func<Original::Task<TAntecedentResult>, TResult> continuationFunction, CancellationToken cancellationToken){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(
            Original::TaskFactory self,Original::Task<TAntecedentResult>[] tasks, Func<Original::Task<TAntecedentResult>, TResult> continuationFunction, TaskContinuationOptions continuationOptions){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task ContinueWhenAny(
            Original::TaskFactory self,Original::Task[] tasks, Action<Original::Task> continuationAction, CancellationToken cancellationToken){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task ContinueWhenAny(
            Original::TaskFactory self,Original::Task[] tasks, Action<Original::Task> continuationAction, TaskContinuationOptions continuationOptions){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> ContinueWhenAny<TResult>(
            Original::TaskFactory self,Original::Task[] tasks, Func<Original::Task, TResult> continuationFunction, CancellationToken cancellationToken){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> ContinueWhenAny<TResult>(
            Original::TaskFactory self,Original::Task[] tasks, Func<Original::Task, TResult> continuationFunction, TaskContinuationOptions continuationOptions){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task ContinueWhenAny<TAntecedentResult>(
            Original::TaskFactory self,Original::Task<TAntecedentResult>[] tasks, Action<Original::Task<TAntecedentResult>> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(
            Original::TaskFactory self,Original::Task<TAntecedentResult>[] tasks, Func<Original::Task<TAntecedentResult>, TResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task ContinueWhenAny(
            Original::TaskFactory self,Original::Task[] tasks, Action<Original::Task> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> ContinueWhenAny<TResult>(
            Original::TaskFactory self,Original::Task[] tasks, Func<Original::Task, TResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler){ throw new NotImplementedException("TaskFactory"); }
        
        // from async
        public static Original::Task FromAsync(
            Original::TaskFactory self, IAsyncResult asyncResult, Action<IAsyncResult> endMethod){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> FromAsync<TResult>(
            Original::TaskFactory self, IAsyncResult asyncResult, Func<IAsyncResult, TResult> endMethod){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task FromAsync(
            Original::TaskFactory self, Func<AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, object state){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> FromAsync<TResult>(
            Original::TaskFactory self, Func<AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, object state){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task FromAsync(
            Original::TaskFactory self, IAsyncResult asyncResult, Action<IAsyncResult> endMethod, TaskCreationOptions creationOptions){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> FromAsync<TResult>(
            Original::TaskFactory self, IAsyncResult asyncResult, Func<IAsyncResult, TResult> endMethod, TaskCreationOptions creationOptions){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task FromAsync(
            Original::TaskFactory self, Func<AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, object state, TaskCreationOptions creationOptions){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> FromAsync<TResult>(
            Original::TaskFactory self, Func<AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, object state, TaskCreationOptions creationOptions){ throw new NotImplementedException("TaskFactory"); } 
        public static Original::Task FromAsync<TArg1>(
            Original::TaskFactory self, Func<TArg1, AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, TArg1 arg1, object state){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> FromAsync<TArg1, TResult>(
            Original::TaskFactory self, Func<TArg1, AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, TArg1 arg1, object state){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task FromAsync(
            Original::TaskFactory self, IAsyncResult asyncResult, Action<IAsyncResult> endMethod, TaskCreationOptions creationOptions, TaskScheduler scheduler){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> FromAsync<TResult>(
            Original::TaskFactory self, IAsyncResult asyncResult, Func<IAsyncResult, TResult> endMethod, TaskCreationOptions creationOptions, TaskScheduler scheduler){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task FromAsync<TArg1>(
            Original::TaskFactory self, Func<TArg1, AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, TArg1 arg1, object state, TaskCreationOptions creationOptions){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> FromAsync<TArg1, TResult>(
            Original::TaskFactory self, Func<TArg1, AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, TArg1 arg1, object state, TaskCreationOptions creationOptions){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task FromAsync<TArg1, TArg2>(
            Original::TaskFactory self, Func<TArg1, TArg2, AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, TArg1 arg1, TArg2 arg2, object state){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> FromAsync<TArg1, TArg2, TResult>(
            Original::TaskFactory self, Func<TArg1, TArg2, AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, TArg1 arg1, TArg2 arg2, object state){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task FromAsync<TArg1, TArg2>(
            Original::TaskFactory self, Func<TArg1, TArg2, AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, TArg1 arg1, TArg2 arg2, object state, TaskCreationOptions creationOptions) { throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> FromAsync<TArg1, TArg2, TResult>(
            Original::TaskFactory self, Func<TArg1, TArg2, AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, TArg1 arg1, TArg2 arg2, object state, TaskCreationOptions creationOptions) { throw new NotImplementedException("TaskFactory"); }
        public static Original::Task FromAsync<TArg1, TArg2, TArg3>(Original::TaskFactory self, Func<TArg1, TArg2, TArg3, AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, TArg1 arg1, TArg2 arg2, TArg3 arg3, object state) { throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> FromAsync<TArg1, TArg2, TArg3, TResult>(Original::TaskFactory self, Func<TArg1, TArg2, TArg3, AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, TArg1 arg1, TArg2 arg2, TArg3 arg3, object state){ throw new NotImplementedException("TaskFactory"); } 
        public static Original::Task FromAsync<TArg1, TArg2, TArg3>(Original::TaskFactory self, Func<TArg1, TArg2, TArg3, AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, TArg1 arg1, TArg2 arg2, TArg3 arg3, object state, TaskCreationOptions creationOptions){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> FromAsync<TArg1, TArg2, TArg3, TResult>(Original::TaskFactory self, Func<TArg1, TArg2, TArg3, AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, TArg1 arg1, TArg2 arg2, TArg3 arg3, object state, TaskCreationOptions creationOptions){ throw new NotImplementedException("TaskFactory"); }
        
        // start a new Original::Task
        public static Original::Task StartNew(Original::TaskFactory self, Action action){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> StartNew<TResult>(Original::TaskFactory self, Func<TResult> function){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task StartNew(Original::TaskFactory self, Action<object> action, object state){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task StartNew(Original::TaskFactory self, Action action, CancellationToken cancellationToken){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task StartNew(Original::TaskFactory self, Action action, TaskCreationOptions creationOptions){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> StartNew<TResult>(Original::TaskFactory self, Func<object, TResult> function, object state){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> StartNew<TResult>(Original::TaskFactory self, Func<TResult> function, CancellationToken cancellationToken){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> StartNew<TResult>(Original::TaskFactory self, Func<TResult> function, TaskCreationOptions creationOptions){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task StartNew(Original::TaskFactory self, Action<object> action, object state, CancellationToken cancellationToken){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task StartNew(Original::TaskFactory self, Action<object> action, object state, TaskCreationOptions creationOptions){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> StartNew<TResult>(Original::TaskFactory self, Func<object, TResult> function, object state, CancellationToken cancellationToken){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> StartNew<TResult>(Original::TaskFactory self, Func<object, TResult> function, object state, TaskCreationOptions creationOptions){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task StartNew(Original::TaskFactory self, Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> StartNew<TResult>(Original::TaskFactory self, Func<TResult> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task StartNew(Original::TaskFactory self, Action<object> action, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler){ throw new NotImplementedException("TaskFactory"); }
        public static Original::Task<TResult> StartNew<TResult>(Original::TaskFactory self, Func<object, TResult> function, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler){ throw new NotImplementedException("TaskFactory"); }
        */ 
    }
}