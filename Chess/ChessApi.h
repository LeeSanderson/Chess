/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#ifndef SINGULARITY

#ifdef CHESS_IMPORT
#define CHESS_API __declspec(dllimport)
#else
#define CHESS_API __declspec(dllexport)
#endif

#else // SINGULARITY
#define CHESS_API
#endif // SINGULARITY


#ifdef __cplusplus
extern "C" {
#endif

// Make a nondeterministic choice among n choices
//   return value ret satisfies: 0 <= ret < n
CHESS_API int __stdcall ChessChoose(int n);

// Disable and enable preemptions in a thread
CHESS_API void __stdcall ChessPreemptionDisable();
CHESS_API void __stdcall ChessPreemptionEnable();
// Prioritize and unprioritize preemptions in a thread
CHESS_API void __stdcall ChessPrioritizePreemptions();
CHESS_API void __stdcall ChessUnprioritizePreemptions();

// Observations
CHESS_API void __stdcall ChessObserveOperationCall(void *object, const char *methodname);
CHESS_API void __stdcall ChessObserveOperationReturn();
CHESS_API void __stdcall ChessObserveIntValue(const char *label, long long value);
CHESS_API void __stdcall ChessObservePointerValue(const char *label, void *value);
CHESS_API void __stdcall ChessObserveStringValue(const char *label, const char *value);

// Do not use this API, use ChessSchedulePoint instead
CHESS_API void __stdcall ChessSyncVarAccess(int varAddress);

// Insert a schedule point
CHESS_API void __stdcall ChessSchedulePoint();

CHESS_API void ChessDataVarAccess(void* address, int size, bool isWrite, int pcId);
CHESS_API void ChessInterleavingDataVarAccess(void* address, int size, bool isWrite, int pcId);



// exit codes - Should match with Concurrency.UnitTestingFramework\ChessExitCode.cs
// Be sure to update Chess.cpp\GetChessExitCodeString() too.
#define CHESS_EXIT_TEST_FAILURE -1
#define CHESS_EXIT_DEADLOCK -2
#define CHESS_EXIT_LIVELOCK -3
#define CHESS_EXIT_TIMEOUT -4
#define CHESS_EXIT_NONDET_ERROR -5
#define CHESS_EXIT_INVALID_TEST -6
#define CHESS_EXIT_RACE -7
#define CHESS_EXIT_INCOMPLETE_INTERLEAVING_COVERAGE -8
#define CHESS_EXIT_INVALID_OBSERVATION -9
#define CHESS_EXIT_ATOMICITY_VIOLATION -10
#define CHESS_EXIT_INTERNAL_ERROR -100

// backward compat defines
#define CHESS_EXIT_ERROR CHESS_EXIT_INTERNAL_ERROR
#define CHESS_INVALID_TEST CHESS_EXIT_INVALID_TEST

CHESS_API const char* GetChessExitCodeString(int exitCode);

// OnErrorCallback
//  Called right before CHESS exits, with the exit code 
//
//  A return value of true means that the handler has handled the error, and cleaned
//  up the program state in a sensible fashion. CHESS will prune the current execution
//  and start exploring the next execution. 
typedef bool (__stdcall *CHESS_ON_ERROR_CALLBACK)(int exitCode, char* details);

// Queues an OnErrorCallback. Will return the previous one (or NULL)
CHESS_API CHESS_ON_ERROR_CALLBACK ChessQueueOnErrorCallback(CHESS_ON_ERROR_CALLBACK newCallback);

// returns length of the schedule string.
// If the return value >= buflen, then you should call GetChessSchedule again a buffer whose length > return value
CHESS_API int GetChessSchedule(char* buf, int buflen);

// returns true if the function was successful
CHESS_API bool SetChessSchedule(const char* buf, int buflen);

#ifdef __cplusplus
}
#endif
