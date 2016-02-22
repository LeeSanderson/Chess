/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"
#include "ChessStl.h"

#include "Chess.h"
#include "ChessProgressTracker.h"
#include "ChessTransition.h"
#include "BitVector.h"
#include "ChessExecution.h"
#include "EventCounter.h"
#include "ErrorInfo.h"

class IChessStrategy;
class StatsMonitor;
class StateMonitor;
class ChessExecution;
class AggregateMonitor;
class SyncVarManager;
class ResultsPrinter;
class ObservationMonitor;
class CacheRaceMonitor;
class NondeterminismHandler;
class VBPruning;
//class StackTrace;

class ChessImpl{
private:
	static const ChessOptions opts;
	static bool optsInitialized;

	static SyncManager* syncManager;
	static SyncVarManager* syncVarManager;
	static IChessStrategy* strategy;
	static ChessExecution* currExecution;

	static Task initialTask;
	static int numInitStateTasks;
	static void ReportStatus();

	static bool chess_is_attached;
	static bool break_deadlock_mode;
	static Task deadlock_continuation;
	static bool chess_shutdown_called;

	// Thread ID management
	static int numThreads;
	static Task running_tid;

	static Task AddNewTask();
	static bool firstTest;

	static void Reset();
	static void Shutdown(int exitcode);
	static void OnDeadlock(bool atTermination);
	static void OnFirstTest();

	static AggregateMonitor* aggregateMonitor;
	static StatsMonitor* chessStats;

	static EventCounter eventCounter;


	static StateMonitor* stateMonitor;

	static NondeterminismHandler* nondetHandler;

	static BitVector tasksAtInitialState;
//	static std::set<Task> activeTasks;
	static BitVector activeTasks;

	static int chess_exit_code;
	static CHESS_ON_ERROR_CALLBACK onErrorCallback;

	static bool doing_preemption;
	static bool doing_context_switch;

public: // makes life easier... can access these easily from any monitor
    static ResultsPrinter *resultsPrinter;
	static CacheRaceMonitor* cacheRaceMonitor;
	static VBPruning* vbpruner;

public:
	static bool IsInitialized();

	static void Init(SyncManager* sm);

	static bool PrunetheSchedule(ChessExecution* exec,IQueryEnabled *qEnabled);
	static bool EnterChess();
	static void LeaveChess();

	static bool StartTest();
	static bool EndTest();
	// moved this method here rather than within ChessExecution (Katie)
	static void WaitForQuiescence(Task tid);
	static void Done(bool enter);
	static void AbnormalExit(int exitCode, const char* reason);
	static void WakeNextDeadlockedThread(bool isContinuation, bool isDeadlockedThread);
    static bool IsBreakingDeadlock();

	static void ChessAssertion(const char* error, int exitCode);
	

	// result reporting
	static void ReportError(const char* error, const char* action, const ErrorInfo* errorInfo);
	static void ReportWarning(const char* error, const char* action, bool withrepro);
    static void CaptureResult(char category, const char *description, const char *xmlargs, bool includeschedule, const ErrorInfo* errorInfo);
	static void ReportFinalStatistics(int exitCode);
	static void CloseResults();

	static ChessExecution* EmptyExecution();
	static ChessExecution* CurrExecution(){
		return currExecution;
	}

	static bool Backtrack();

	static int Choose(int numChoices);

	static void PreemptionDisable();
	static void PreemptionEnable();

	static void MarkTimeout(); // called right before committing a sync var access that timed out

	// Start marking steps interesting for best first search (Katie)
	static void PrioritizePreemptions();
	// Stop marking steps interesting for best first search (Katie)
	static void UnprioritizePreemptions();

    // observation sets
	static void ObserveOperationCall(void *object, const char *opname);
	static void ObserveOperationReturn();
	static void ObserveCallback(void *object, const char *opname);
	static void ObserveCallbackReturn();
	static void ObserveIntValue(const char *label, long long value);
	static void ObservePointerValue(const char *label, void *value);
	static void ObserveStringValue(const char *label, const char *value);

	// Creates a suspended thread that can later be resumed
    static bool TaskFork(Task& childTid);
	static bool TaskBegin();
	static bool TaskEnd();

	static bool SyncVarAccess(SyncVar var, SyncVarOp op);
	static bool AggregateSyncVarAccess(const SyncVar vars[], int n, SyncVarOp op);
	static bool CommitSyncVarAccess(); // commit previous sync var access

	static bool ResumeTask(Task tid);
	static bool SuspendTask(Task tid);

	static bool LocalBacktrack();

	static bool TaskYield();

	static void EnterQuiescence();

	static bool InTestStartup(){return firstTest;}

	static void SaveSchedule(const char* filename);
	static void SetSchedule(const char* filename);

	static void SetOptions(ChessOptions& o);
	static const ChessOptions& GetOptions();

	static SyncManager* GetSyncManager();

	static SyncVarManager* GetSyncVarManager(){
		if (!chess_is_attached) return NULL;
		return syncVarManager;
	}

	// Give DPOR access to cache race monitor so it can use the vector clocks (Katie)
	static CacheRaceMonitor* GetRaceMonitor();

	static IChessStrategy* GetStrategy();

	static IChessStats* GetStats();

	static EventCounter* GetEventCounter() {return &eventCounter;}

	static ResultsPrinter* GetResultsPrinter() {return resultsPrinter;}

	// DPOR needs to know how many threads exist in the system (Katie)
	static size_t NumThreads() { return numThreads; }

	static Task GetCurrentTid();
	
	static void RegisterMonitor(IChessMonitor* mon, bool atHead=false);

	static GetStateCallback RegisterGetStateCallback(GetStateCallback cb);

	static void Detach();
	static void BreakDeadlock();

	static int GetExitCode();
	static int SetExitCode(int code);

	static SyncVar GetNextSyncVar();

    static void MergeSyncAndDataVar(SyncVar svar, void* dvar);

	static CHESS_ON_ERROR_CALLBACK QueueOnErrorCallback(CHESS_ON_ERROR_CALLBACK newCallback);

	static int GetChessSchedule(__in_ecount(buflen) char* buf, int buflen);

	static bool SetChessSchedule(const char* buf, int buflen);



	//Exceptional events
	static bool OnMaxExecTimeout();
	//static void RecoverFromNondeterminism();
	//static void OnNondeterminismDetection(int kind, const ChessTransition& trans, const ChessTransition& exp);
	//static void ReportNondeterministicExecution(char* err, int kind, const ChessTransition& trans, const ChessTransition& exp);
	static bool OnMaxStackSize(size_t size);

	//static void CommitTransition(ChessTransition& trans);
	static EventId DataVarAccess(void* p, int size, bool isWrite, size_t pcId);
	static EventId TraceEvent(const std::string &info);

	// event attributes
	static void SetEventAttribute(EventId id, EventAttribute attr, const char *value);
	static void SetEventAttribute(EventId id, EventAttribute attr, size_t value);
	static void SetNextEventAttribute(EventAttribute attr, const char *value);

	static void ChessImpl::ProgressTrackerFn();
	static ChessProgressTracker* progressTracker;

    // observations
    static ObservationMonitor *observationMonitor;

	// vestigial ops

	enum {WRITE_WRITE, READ_WRITE, WRITE_READ};
	static void OnRace(void* loc, Task owner, int inst, Task performer, int inst2, int op){}

};
