/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "ChessApi.h"
#include "SyncVar.h"
#include "ChessEvent.h"
#include "SyncManager.h"
#include "ChessOptions.h"
#include "IChessMonitor.h"
#include "IChessStrategy.h"
#include "IChessStats.h"
#include "EventAttribute.h"

typedef int (* GetStateCallback) (char* buf, int len);

extern "C" {
	CHESS_API void __stdcall ChessInit(ChessOptions& opts, SyncManager* sm);
	CHESS_API void __stdcall ChessStartTest();
	CHESS_API bool __stdcall ChessEndTest();
	CHESS_API void __stdcall ChessDone();

	CHESS_API void __stdcall ChessRegisterMonitor(IChessMonitor* mon);

	CHESS_API void __stdcall ChessRegisterStrategy(IChessStrategy* strategy);

	CHESS_API void __stdcall ChessEnterQuiescence();

	CHESS_API GetStateCallback __stdcall ChessRegisterGetStateCallback(GetStateCallback cb);
}

class CHESS_API Chess{
public:	
	static void Init(SyncManager* sm);
	static bool IsInitialized();

	static void SetOptions(ChessOptions& o);
	static const ChessOptions& GetOptions();


	// Controlling a Test
	static bool StartTest();
	static bool EndTest();
	static void Done(bool enter=true);
	static void AbnormalExit(int exitCode, const char* reason);
	

	static bool EnterChess();
	static void LeaveChess();

	// SyncVar related functions
	// These have to be called when the thread is "in CHESS", 
	//  i.e. the thread has called EnterChess() before
	// These functions fail when CHESS has been detached

	static bool SyncVarAccess(SyncVar var, SyncVarOp op);
	static bool AggregateSyncVarAccess(const SyncVar vars[], int n, SyncVarOp op);
	static bool CommitSyncVarAccess(); // commit previous sync var access
	
	// Task related SyncVarAccesses
	// Everything except TaskFork and TaskEnd are macros that call
	// SyncVarAccess() with the right arguments
	static bool TaskFork(Task& childTid); // Creates a suspended thread that can later be resumed
	static bool TaskBegin();
	static bool TaskEnd();
	static bool ResumeTask(Task tid);
	static bool SuspendTask(Task tid);
	static bool TaskYield();

	// Abort a previous SyncVarAccess
	static bool LocalBacktrack();
	
	// End SyncVar related functions

	static int Choose(int numChoices);

	static void PreemptionDisable();
	static void PreemptionEnable();

    // observation sets
	static void ObserveOperationCall(void *object, const char *opname);
	static void ObserveOperationReturn();
	static void ObserveIntValue(const char *label, long long value);
	static void ObservePointerValue(const char *label, void *value);
	static void ObserveStringValue(const char *label, const char *value);


	static SyncManager* GetSyncManager();

	//returns false if CHESS has not exitted
	static int GetExitCode();

	static CHESS_ON_ERROR_CALLBACK QueueOnErrorCallback(CHESS_ON_ERROR_CALLBACK newCallback);

	static Task GetCurrentTid();
	
	static void RegisterMonitor(IChessMonitor* mon);
	
	static void RegisterStrategy(IChessStrategy* strategy);

	static GetStateCallback RegisterGetStateCallback(GetStateCallback cb);

	static EventId DataVarAccess(void* p, int size, bool isWrite, int pcId);
	static EventId TraceEvent(const std::string &info);

    static void MergeSyncAndDataVar(SyncVar svar, void* dvar);

	// capture results
	static void ReportError(const char *description, const char *action);
	static void ReportWarning(const char *description, const char *action, bool includeschedule);
    static void CaptureResult(char category, const char *description, const char *actions, bool includeschedule);

	// event attributes
	static void SetEventAttribute(EventId id, EventAttribute attr, const char *value);
	static void SetNextEventAttribute(EventAttribute attr, const char *value);

	static SyncVar GetNextSyncVar();

	static int GetChessSchedule(__in_ecount(buflen) char* buf, int buflen);

	static bool SetChessSchedule(const char* buf, int buflen);

	static IChessStats* GetStats();
};
