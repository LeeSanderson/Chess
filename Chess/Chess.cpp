/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

// Chess.cpp : Defines the entry point for the console application.

#if SINGULARITY

#include "hal.h"

#ifdef SINGULARITY_KERNEL
#include "halkd.h"
#endif

#endif

#include "ChessStl.h"
#include "ChessExecution.h"
#include "BestFirstExecution.h"
#include "Chess.h"
#include "SyncVar.h"
#include "ChessEvent.h"

#include "DBStrategy.h"
#include "DfsStrategy.h"
#include "CBStrategy.h"
#include "BestFirstStrategy.h"
#include "StatsMonitor.h"
#include "StateMonitor.h"
#include "SleepSetCBStrategy.h"
#include "ChessMonitorTest.h"
#include "CacheRaceMonitor.h"
#include "ChessLog.h"
#include "TracePrinter.h"
#include "ChessProfilerTimer.h"
#include "AggregateMonitor.h"
#include "SyncVarManager.h"
#include "ResultsPrinter.h"
#include "ObservationMonitor.h"
#include "NondeterminismHandler.h"
#include "PCTStrategy.h"
#include "PCTExecution.h"
#include "PCTwithVBExecution.h"
#include "VBPruning.h"
#include "DeRandomizedPCTExecution.h"
#include "DeRandomizedPCTStrategy.h"


#include <sstream>

#ifdef UNDER_CE
#include <winbase.h>
#endif

#if !SINGULARITY
#include "ExecPrinter.h"
#endif
#include "HBMonitor.h"

//#include "Win32SyncManager.h"

//#define USE_SEPARATE_CLIENT 1

#ifdef USE_SEPARATE_CLIENT
#include "pipes.h"

Pipe pipeCtrl;
#endif

#include "ChessImpl.h"

// kfuncs.h in CE defines DebugBreak() to be __debugBreak
#ifdef UNDER_CE
#undef DebugBreak
#endif

#ifdef UNDER_CE
#define ERROR_STREAM_NAME   "\\release\\ChessErr"
#define OUT_STREAM_NAME     "\\release\\ChessOut"
#endif

std::ostream* GetChessErrorStream()
{
#ifndef UNDER_CE
	return &(std::cerr);
#else
	static std::ofstream errorStream;
	static bool fStreamOpened = false;

	if ( !fStreamOpened )
	{
		errorStream.open(ERROR_STREAM_NAME,  std::ios_base::out);
		fStreamOpened = true;
	}
	return &errorStream;
#endif
}

std::ostream* GetChessOutputStream()
{
#ifndef UNDER_CE
	return &(std::cout);
#else
	static std::ofstream outStream;
	static bool fStreamOpened = false;

	if ( !fStreamOpened )
	{
		outStream.open(OUT_STREAM_NAME,  std::ios_base::out);
		fStreamOpened = true;
	}
	return &outStream;
#endif
}

void CloseErrorStream()
{
#ifdef UNDER_CE
	static_cast<std::ofstream*>(GetChessErrorStream())->close();
#endif
}

void CloseOutputStream()
{
#ifdef UNDER_CE
	static_cast<std::ofstream*>(GetChessOutputStream())->close();
#endif
}

//XXX: Split this page into Chess.cpp and ChessImpl.cpp

const ChessOptions ChessImpl::opts;
bool ChessImpl::optsInitialized = false;
ChessExecution* ChessImpl::currExecution;
VBPruning* ChessImpl::vbpruner;
SyncManager* ChessImpl::syncManager;
SyncVarManager* ChessImpl::syncVarManager;
ResultsPrinter *ChessImpl::resultsPrinter;
int ChessImpl::numThreads(0);
Task ChessImpl::running_tid(0);
bool ChessImpl::chess_is_attached = false;
bool ChessImpl::break_deadlock_mode = 0;
Task ChessImpl::deadlock_continuation(0);
bool ChessImpl::chess_shutdown_called = false;
IChessStrategy* ChessImpl::strategy;
StatsMonitor* ChessImpl::chessStats = 0;
StateMonitor* ChessImpl::stateMonitor = 0;
CacheRaceMonitor* ChessImpl::cacheRaceMonitor = 0;
EventCounter ChessImpl::eventCounter;
Task ChessImpl::initialTask;
int ChessImpl::numInitStateTasks;
BitVector ChessImpl::tasksAtInitialState;
BitVector ChessImpl::activeTasks;
ChessProgressTracker* ChessImpl::progressTracker = 0;
ObservationMonitor* ChessImpl::observationMonitor = 0;
//int chooser;
int ChessImpl::chess_exit_code = 0;
CHESS_ON_ERROR_CALLBACK ChessImpl::onErrorCallback = 0;
AggregateMonitor* ChessImpl::aggregateMonitor = 0;
bool ChessImpl::doing_context_switch = false;
bool ChessImpl::doing_preemption = false;
bool ChessImpl::firstTest = true;
NondeterminismHandler* ChessImpl::nondetHandler=0;


void ChessImpl::Reset()
{
	CHESS_LOG(EXECUTION, "Reset");

	numThreads = numInitStateTasks+1; 

	syncManager->Reset();
	syncVarManager->Reset();
	currExecution->Reset();

	running_tid = initialTask;
	CHESS_LOG(SCHEDULE, "Scheduling ", running_tid);

	eventCounter.reset();
}

void ChessImpl::Init(SyncManager* sm)
{
	CHESS_LOG(EXECUTION, "Init");

	firstTest = true;
	chess_exit_code = 0;

	strategy = 0;
	chessStats = 0;
	activeTasks.clear();
	tasksAtInitialState.clear();
	
	syncVarManager = new SyncVarManager();

	numInitStateTasks = 1;
	numThreads = 1; // tids start from 1
	int n = 1;
	initialTask = AddNewTask();
	activeTasks.insert(initialTask);
	syncManager  = sm;
	syncManager->Init(initialTask);

	Task t[1];
	t[0] = initialTask;
	// Create a BestFirstExecution instead, if needed (BFS)

	if(opts.var_bound > 0 && opts.delay_bound >= 0) {
		ChessImpl::vbpruner = new VBPruning(opts.var_bound);
	} else {
		ChessImpl::vbpruner = 0;
	}

	if (opts.best_first) {
		currExecution = new BestFirstExecution(t, 1);
	} else if(opts.PCT && opts.var_bound == -1) {
		currExecution = new PCTExecution(t,1,opts.bug_depth, opts.pct_seed);
	} else if(opts.DeRandomizedPCT) {
		currExecution = new DeRandomizedPCTExecution(t,1);
	} else if(opts.PCT && opts.var_bound > 0) {
		currExecution = new PCTwithVBExecution(t,1,opts.bug_depth,opts.var_bound, opts.pct_seed);
	} else {
		currExecution = new ChessExecution(t, 1);
	}


	aggregateMonitor = new AggregateMonitor();
	StatsMonitor* stm = new StatsMonitor();
	chessStats = stm;
	nondetHandler = new NondeterminismHandler();
	bool is_repro = (opts.load_schedule && opts.max_executions == 1);

	if(is_repro){
		nondetHandler->SetNumAttemptsAtTargetExecution(5);
	}

	if(opts.load_schedule){
		SetSchedule(opts.load_schedule_file);
	}
	else if(opts.recover_schedule){
		SetSchedule(opts.recover_schedule_file);
		bool ret = currExecution->Recover();
		if(!ret){
			// restart;
			*GetChessOutputStream() << "Recovery done : No more schedules to explore, exiting..." << std::endl;
			syncManager->Exit(0);
		}
	}

	resultsPrinter = new ResultsPrinter(&opts, stm);

	progressTracker = new ChessProgressTracker();
	RegisterMonitor(progressTracker);
	if(opts.max_exec_time){
		if(! syncManager->QueuePeriodicTimer(opts.max_exec_time * 1000, &ChessImpl::ProgressTrackerFn)){
			ReportWarning("Cannot Queue Max Execution Timeout, CHESS will no longer detect long running executions", "", false);
		}
	}
	else{
		ReportWarning("CHESS is running with max_exec_time == 0, CHESS will no longer detect long running executions", "", false);
	}

	if(opts.use_exec_printer){
#if !SINGULARITY
		RegisterMonitor(new ExecPrinter(&opts));
#endif
	}
	RegisterMonitor(chessStats, true);
	if(opts.show_hbexecs) RegisterMonitor(new HBMonitor(chessStats), true);
#ifndef NDEBUG
	// Commenting this out as the Monitor interface needs to change when nondeterminism is on
	// It currently assert fails at many places
	//RegisterMonitor(new ChessMonitorTest());
#endif
	eventCounter.clear();

	if (opts.sober)
		RegisterMonitor(cacheRaceMonitor = new CacheRaceMonitor(opts.load_schedule, opts.load_schedule || opts.logging, opts.sober_dataracesonly, opts.sober_targetrace));

	if ((opts.enumerate_observations != NULL && opts.enumerate_observations[0] != '\0') || 
		(opts.check_observations != NULL && opts.check_observations[0] != '\0'))
	{
		observationMonitor = new ObservationMonitor(&opts, cacheRaceMonitor);
		RegisterMonitor(observationMonitor);
	}

	if (opts.trace || opts.gui || opts.logging)
		RegisterMonitor(new TracePrinter(&opts));

	// do this one last!!!
	RegisterMonitor(resultsPrinter);

#ifdef USE_SEPARATE_CLIENT
	if(opts.use_remote_test_driver){
		BOOL ctrlSucc;
		LPTSTR pipeNameCtrl = TEXT("\\\\.\\pipe\\UMDFCheckerControlPipe");

		ctrlSucc = pipeCtrl.myCreateNamedPipe(pipeNameCtrl, PIPE_ACCESS_DUPLEX);
		assert(ctrlSucc);

		*GetChessErrorStream() << "Waiting for connection from the TestApplication" << std::endl;
		fflush(stderr);

		if(! pipeCtrl.myConnectNamedPipe())
			assert(false);	
	}
#endif

	chess_is_attached=true;

	Reset();
	LeaveChess();
}

void ChessImpl::ProgressTrackerFn(){
	if (!chess_is_attached) return; 
	if(progressTracker && !GetSyncManager()->IsDebuggerPresent())
		progressTracker->OnTimeout();
}

void ChessImpl::OnFirstTest(){
	numInitStateTasks = numThreads-1;
	currExecution->SetInitStack();
	syncManager->SetInitState();
	syncVarManager->SetInitState();
	eventCounter.setInitState();
	activeTasks.Copy(tasksAtInitialState);

	// Create a BestFirstStrategy instead, if needed.  It takes care of 
	// sleep sets/preemption bound itself (BFS)
	if (opts.best_first) {
		strategy = new BestFirstStrategy();
	} else if (opts.delay_bound >= 0) {
		strategy = new DBStrategy(opts.delay_bound);
	} else if (opts.DeRandomizedPCT) {
		strategy = new DeRandomizedPCTStrategy(opts.bug_depth);
	} else if (opts.PCT) {
		strategy = new PCTStrategy(opts.num_of_runs);
	} else {
		// Added this to get rid of the preemption bound in the unbounded case.  Really, I think
		// that neither the bound nor sleep sets should be implemented as a separate strategy, but
		// I didn't want to mess with the existing code more than was necessary (BFS)
		if(opts.do_sleep_sets){
			strategy = new SleepSetCBStrategy(opts.bounded ? opts.preemption_bound : 10000);
		} else {
			strategy = new CBStrategy(opts.bounded ? opts.preemption_bound : 10000);
		}
	}

	currExecution = strategy->InitialExecution();

	assert(running_tid == initialTask);
}

// Moved this method here from ChessExecution.cpp because it didn't really
// belong in ChessExecution - it kept just calling into ChessImpl, which in turn
// called back into currExecution, which could change while waiting for quiescence
// if non-determinism was detected by the best first search (BFS)
void ChessImpl::WaitForQuiescence(Task tid) {
	// should mimic a wrapper

	SyncVarAccess(SyncVarManager::QuiescenceVar, SVOP::RWVAR_READWRITE);

	currExecution->SetTaskWaitingForQuiescence(tid);
	CommitSyncVarAccess();
	
	while(true){

		if(!SyncVarAccess(SyncVarManager::QuiescenceVar, SVOP::QUIESCENT_WAIT)) return;
		if(! currExecution->ReachedQuiescence()){
			LocalBacktrack();
			continue;
		}
		currExecution->SetTaskWaitingForQuiescence(Task(0));
		CommitSyncVarAccess();
		break;
	}
}

bool ChessImpl::StartTest(){
	if(!chess_is_attached) return false;

	if(firstTest && opts.profile){
		ChessProfilerTimer::EnableProfiling();
	}
	EnterChess();

	if(firstTest){
		WaitForQuiescence(running_tid);

		SaveSchedule(opts.schedule_file);
		firstTest = false;

		OnFirstTest();

		// rename the tasks in a canonical order before the first test
		syncManager->RenameSymmetricTasks();
	
		aggregateMonitor->EnabledMonitors();
		aggregateMonitor->OnExecutionBegin(currExecution);
	}

#ifdef USE_SEPARATE_CLIENT
	if(opts.use_remote_test_driver){
		char message = 1;
		BOOL res = pipeCtrl.myWriteFile(&message, 1);
		assert(res);
	}
#endif

	currExecution->PulseQuiescence();
	LeaveChess();

	return true;
}

bool ChessImpl::EndTest(){
	if(!chess_is_attached) return false;

	EnterChess();

	WaitForQuiescence(running_tid);

	// compare current set of activeTasks with tasksAtInitialState
	if(!(activeTasks == tasksAtInitialState)){
		std::stringstream s;
		s << "RunTest is not idempotent with respect to active threads\n";
		s << " Threads at the start of RunTest(): {"; 
		assert(!tasksAtInitialState.IsEmpty() && !activeTasks.IsEmpty());
		Task first = tasksAtInitialState.FindIndexLargerThan(0);
		Task t = first;
		while(true){
			s << t << ", ";
			t = tasksAtInitialState.FindIndexLargerThan(t);
			if(t == first)
				break;
		}
		//copy(tasksAtInitialState.begin(), tasksAtInitialState.end(), std::ostream_iterator<Task>(s, ", "));
		s << "}\n";
		s << " Threads at the end   of RunTest(): {"; 
		first = activeTasks.FindIndexLargerThan(0);
		t = first;
		while(true){
			s << t << ", ";
			t = activeTasks.FindIndexLargerThan(t);
			if(t == first)
				break;
		}
		//copy(activeTasks.begin(), activeTasks.end(), std::ostream_iterator<Task>(s, ", "));
		s << "}\n";
 
		if(opts.die_on_nonidempotence){
			ChessAssertion(s.str().c_str(), CHESS_INVALID_TEST);
			return false;
		}
	}

	bool ret = ChessImpl::Backtrack();
	if(ret){
		//stdext::hash_map<Task, Task> empty;
		syncManager->RenameSymmetricTasks();

		LeaveChess();
		return ret;
	}

	//final test
	aggregateMonitor->OnShutdown(); // monitors are shutdown, but CHESS itself is not. This makes sure wrappers are enabled for TestShutDown
	aggregateMonitor->DisableMonitors();
	resultsPrinter->OnShutdown(0);

	currExecution->BacktrackToInitStack();
	//strategy->PruneExecution(currExecution->NumTransitions());

#ifdef USE_SEPARATE_CLIENT
	if(opts.use_remote_test_driver){
		char message = 0;
		BOOL res = pipeCtrl.myWriteFile(&message, 1);
		assert(res);
		CloseHandle(pipeCtrl.getHandle());
	}

#endif

	currExecution->PulseQuiescence();
	//PulseQuiescentTasks();
	//ChessImpl::TaskEnd();

	LeaveChess();
	return false;
}

bool ChessImpl::IsInitialized(){ 
	return chess_is_attached;
}

bool ChessImpl::PrunetheSchedule(ChessExecution* exec,IQueryEnabled *qEnabled)
{
	return vbpruner->ComputePremptionVars(exec,qEnabled);
}

SyncVar ChessImpl::GetNextSyncVar(){
	if(!chess_is_attached) return 0;
	return syncVarManager->GetNextSyncVar();
}

void ChessImpl::MergeSyncAndDataVar(SyncVar svar, void *dvar)
{
	if(!chess_is_attached) return;
	if (cacheRaceMonitor)
		return cacheRaceMonitor->MergeSyncAndDataVar(svar, dvar);
}

ChessExecution* ChessImpl::EmptyExecution(){
	if(!chess_is_attached) return NULL;
	currExecution->Reset();
	return currExecution;
}

// these two functions can be called before chess is attached and after chess is detached
void ChessImpl::SetOptions(ChessOptions& o){ 
	if(!optsInitialized){
		optsInitialized = true;
		(*((ChessOptions*)&opts)).SetOptionsFromFile("chess.options");
	}
	(*((ChessOptions*)&opts)).Update(o);
}
const ChessOptions& ChessImpl::GetOptions(){return opts;}

SyncManager* ChessImpl::GetSyncManager(){
		if(!chess_is_attached) return NULL;
		return syncManager;
	}

// Give DPOR access to cache race monitor so it can use the vector clocks (BFS)
CacheRaceMonitor* ChessImpl::GetRaceMonitor() {
	if (!chess_is_attached) return NULL;
	// initialize the cache race monitor if it has not already been created
	if (cacheRaceMonitor == NULL) {
		RegisterMonitor(cacheRaceMonitor = new CacheRaceMonitor(opts.load_schedule, opts.load_schedule || opts.logging, opts.sober_dataracesonly, opts.sober_targetrace));
	}
	return cacheRaceMonitor;
}

IChessStrategy* ChessImpl::GetStrategy(){
		if(!chess_is_attached) return NULL;
		return strategy;
	}

Task ChessImpl::GetCurrentTid(){
	if(!chess_is_attached) return 0;
	return running_tid;
}

Task ChessImpl::AddNewTask()
{
	Task tid(numThreads++);
	return tid;
}

CHESS_API int __stdcall ChessChoose(int numChoices){
	assert(numChoices > 0);
	Chess::EnterChess();
	int ret = Chess::Choose(numChoices);
	Chess::LeaveChess();
	return ret;
}

CHESS_API void __stdcall ChessPreemptionDisable(){

	Chess::EnterChess();
	Chess::PreemptionDisable();
	Chess::LeaveChess();
}

CHESS_API void __stdcall ChessPreemptionEnable(){
	Chess::EnterChess();
	Chess::PreemptionEnable();
	Chess::LeaveChess();
}

// Start marking the running thread's steps as important for the best first search (BFS)
CHESS_API void __stdcall ChessPrioritizePreemptions() {
	Chess::EnterChess();
	Chess::PrioritizePreemptions();
	Chess::LeaveChess();
}

// Stop marking the running thread's steps as important for the best first search (BFS)
CHESS_API void __stdcall ChessUnprioritizePreemptions() {
	Chess::EnterChess();
	Chess::UnprioritizePreemptions();
	Chess::LeaveChess();
}

CHESS_API void __stdcall ChessObserveOperationCall(void *object, const char* opname){
	Chess::EnterChess();
	Chess::ObserveOperationCall(object, opname);
	Chess::LeaveChess();
}
CHESS_API void __stdcall ChessObserveOperationReturn(){
	Chess::EnterChess();
	Chess::ObserveOperationReturn();
	Chess::LeaveChess();
}
CHESS_API void __stdcall ChessObserveIntValue(const char *label, long long value){
	Chess::EnterChess();
	Chess::ObserveIntValue(label, value);
	Chess::LeaveChess();
}
CHESS_API void __stdcall ChessObservePointerValue(const char *label, void *value){
	Chess::EnterChess();
	Chess::ObservePointerValue(label, value);
	Chess::LeaveChess();
}
CHESS_API void __stdcall ChessObserveStringValue(const char *label, const char *value){
	Chess::EnterChess();
	Chess::ObserveStringValue(label, value);
	Chess::LeaveChess();
}

CHESS_API bool __stdcall IsBreakingDeadlock(){
	Chess::EnterChess();
	bool retval = ChessImpl::IsBreakingDeadlock();
	Chess::LeaveChess();
	return retval;
}


CHESS_API const char* GetChessExitCodeString(int exitCode){
	switch(exitCode){
		case 0 : return "CHESS_EXIT_NO_ERROR";
		case CHESS_EXIT_TEST_FAILURE: return "CHESS_EXIT_TEST_FAILURE";
		case CHESS_EXIT_DEADLOCK: return "CHESS_EXIT_DEADLOCK";
		case CHESS_EXIT_LIVELOCK: return "CHESS_EXIT_LIVELOCK";
		case CHESS_EXIT_TIMEOUT: return "CHESS_EXIT_TIMEOUT";
		case CHESS_EXIT_NONDET_ERROR: return "CHESS_EXIT_NONDET_ERROR";
		case CHESS_EXIT_INVALID_TEST: return "CHESS_EXIT_INVALID_TEST";
		case CHESS_EXIT_RACE: return "CHESS_EXIT_RACE";
		case CHESS_EXIT_INCOMPLETE_INTERLEAVING_COVERAGE: return "CHESS_EXIT_INCOMPLETE_INTERLEAVING_COVERAGE";
		case CHESS_EXIT_INTERNAL_ERROR: return "CHESS_EXIT_TEST_INTERNAL_ERROR";
		default: return "CHESS_INVALID_ERROR_CODE";
	}
}

int ChessImpl::Choose(int numChoices){
	if(!chess_is_attached) return 0;

	EventId id(eventCounter.getNext(running_tid, true));
	SyncVarOp op = SVOP::CHOICE;
	CHESS_LOG(EXECUTION, "Choose [", currExecution->NumTransitions(), "]", running_tid, SVOP::ToString(op));
	aggregateMonitor->OnSchedulePoint(IChessMonitor::CHOOSE, id, SyncVarManager::NullSyncVar, op, currExecution->NumTransitions());
	SetEventAttribute(id, STATUS, "c");
	// call CommitSyncVarAccess on the execution to give it a chance to update its state (BFS)
	int result = currExecution->Choose(running_tid, numChoices);
	currExecution->CommitSyncVarAccess(id.nr);
	return result;
}

void ChessImpl::PreemptionDisable(){
	if(!chess_is_attached) return;
	EventId id(TraceEvent(std::string("PreemptionDisable")));
	currExecution->PreemptionDisable(running_tid);
}

void ChessImpl::PreemptionEnable(){
	if(!chess_is_attached) return;
	EventId id(TraceEvent(std::string("PreemptionEnable")));
	currExecution->PreemptionEnable(running_tid);
}

// Start marking the running thread's steps as important for the best first search (BFS)
void ChessImpl::PrioritizePreemptions() {
	if(!chess_is_attached) return;
	if (opts.best_first) {
		EventId id(TraceEvent(std::string("PrioritizePreemptions")));
		static_cast<BestFirstExecution*>(currExecution)->PrioritizePreemptions(id.tid);
	}
}

// Stop marking the running thread's steps as important for the best first search (BFS)
void ChessImpl::UnprioritizePreemptions() {
	if(!chess_is_attached) return;
	if (opts.best_first) {
		EventId id(TraceEvent(std::string("UnprioritizePreemptions")));
		static_cast<BestFirstExecution*>(currExecution)->UnprioritizePreemptions(id.tid);
	}
}

void ChessImpl::ObserveOperationCall(void *object, const char *opname){
	if(!chess_is_attached) return;

	if (observationMonitor)
	{
		// insert manual schedule point (to make sure preemptions can happen before each operation) 
		SyncVarAccess(SyncVarManager::OperationVar, SVOP::RWVAR_READWRITE);
		CommitSyncVarAccess();
		if (!break_deadlock_mode)
			observationMonitor->CheckBlock(currExecution->GetQueryEnabled(), currExecution->NumTransitions());
		observationMonitor->Call(running_tid, object, opname, false);
	}
}
void ChessImpl::ObserveOperationReturn(){
	if(!chess_is_attached) return;
	if (observationMonitor)
	{
		observationMonitor->Return(running_tid, false);
	}
}
void ChessImpl::ObserveCallback(void *object, const char *opname){
	if(!chess_is_attached) return;
	if (observationMonitor)
	{
		observationMonitor->Call(running_tid, object, opname, true);
	}
}
void ChessImpl::ObserveCallbackReturn(){
	if(!chess_is_attached) return;
	if (observationMonitor)
	{
		// insert manual schedule point (to make sure preemptions can happen before each operation) 
		SyncVarAccess(SyncVarManager::OperationVar, SVOP::RWVAR_READWRITE);
		CommitSyncVarAccess();
		if (!break_deadlock_mode)
			observationMonitor->CheckBlock(currExecution->GetQueryEnabled(), currExecution->NumTransitions());
		observationMonitor->Return(running_tid, true);
	}
}
void ChessImpl::ObserveIntValue(const char *label, long long value){
	if(!chess_is_attached) return;
	if (observationMonitor)
	{
		observationMonitor->IntValue(label, value);
	}
}
void ChessImpl::ObservePointerValue(const char *label, void *value){
	if(!chess_is_attached) return;
	if (observationMonitor)
	{
		observationMonitor->PointerValue(label, value);
	}
}
void ChessImpl::ObserveStringValue(const char *label, const char *value){
	if(!chess_is_attached) return;
	if (observationMonitor)
	{
		observationMonitor->StringValue(label, value);
	}
}


CHESS_API void __stdcall ChessSyncVarAccess(int varAddress){
	ChessSchedulePoint();
}

CHESS_API void __stdcall ChessSchedulePoint(){
	Chess::EnterChess();
	Chess::SyncVarAccess(SyncVarManager::AnonymousVar, SVOP::MANUAL_SCHEDULE);
	Chess::CommitSyncVarAccess();
	Chess::LeaveChess();
}

CHESS_API void ChessDataVarAccess(void* address, int size, bool isWrite, int pcId){
	Chess::EnterChess();
	Chess::DataVarAccess(address, size, isWrite, pcId);
	Chess::LeaveChess();
}

CHESS_API void ChessInterleavingDataVarAccess(void* address, int size, bool isWrite, int pcId){
	ChessSchedulePoint();
	ChessDataVarAccess(address, size, isWrite, pcId);
}

ChessProfilerTimer InChessTimer("InChess");
bool ChessImpl::EnterChess(){
	InChessTimer.Start();
	if(!chess_is_attached && !break_deadlock_mode) return false;
	if(progressTracker) {
		progressTracker->Disable();
	}
	syncManager->EnterChess();
	return true;
}

void ChessImpl::LeaveChess(){
	InChessTimer.Stop();
	if(!chess_is_attached && !break_deadlock_mode) return;
	if(progressTracker) {
		progressTracker->Enable();
	}
	syncManager->LeaveChess();
}


//bool Chess::GetOrSetThreadId(Task running){
//	//DWORD tid = ::GetCurrentThreadId();
//	//if(taskThreadIds.find(running) == taskThreadIds.end()){
//	//	taskThreadIds[running] = tid;
//	//	return true;
//	//}
//	//else{
//	//	if(tid != taskThreadIds[running]){
//	//		fprintf(stderr, "Potential nondeterminism Task %d executed by both %d and %d\n", 
//	//			running, tid, taskThreadIds[running]);
//	//		return false;
//	//	}
//	//	return true;
//	//}
//}
bool ChessImpl::AggregateSyncVarAccess(const SyncVar vars[], int n, SyncVarOp op){
	if(!chess_is_attached) return false;
	if(n == 1){
		return SyncVarAccess(vars[0], op);
	}
	return SyncVarAccess(syncVarManager->GetAggregateSyncVar(vars, n), op);
}

bool ChessImpl::SyncVarAccess(SyncVar var, SyncVarOp op)
{
	if(!chess_is_attached){
		return false;
	}
	ChessProfilerSentry profSentry("ChessImpl::SyncVarAccess");

	EventId id(eventCounter.peekNext(running_tid));

	CHESS_LOG(EXECUTION, "SVAccess [", currExecution->NumTransitions(), "]", running_tid, var, SVOP::ToString(op));

	aggregateMonitor->OnSchedulePoint(IChessMonitor::SVACCESS, id, var, op, currExecution->NumTransitions());

	// This loop is here as threads need multiple attempts to succeed a SyncVarAccess
	while(true){
		if(!chess_is_attached) return false;
	
		// Threads after wakeup will eventually call SyncVarAccess
		// We are using this opportunity to break right after a context switch or a preemption
		if(doing_context_switch){
			if(opts.break_after_context_switch){
				syncManager->DebugBreak();
			}
			else if(opts.break_after_preemptions && doing_preemption){
				syncManager->DebugBreak();
			}
		}
		doing_context_switch = doing_preemption = false;

		int ret = currExecution->SyncVarAccess(running_tid, var, op);

		if(ret == ChessExecution::SUCCESS){
			if(opts.max_stack_size && currExecution->NumTransitions() > (size_t)opts.max_stack_size){
				OnMaxStackSize(opts.max_stack_size);
				return false;
			}
			return true;
		}
		
		if(ret == ChessExecution::REQUIRE_NONDETERMINISM_PROCESSING){
			currExecution = nondetHandler->AccessNondeterminism(currExecution, running_tid, var, op);

			// retry sync var access
			continue;
		}

		assert(ret == ChessExecution::REQUIRE_CONTEXT_SWITCH);

		Task curr = running_tid;
		Task next;

		// The NextTaskToSchedule method may detect non-determinism in the best-first search so
		// we have to deal with it (BFS)
		int result = currExecution->NextTaskToSchedule(next);
		if(result == ChessExecution::FAILURE){

			if ((opts.enumerate_observations != NULL && opts.enumerate_observations[0] != '\0'))
			{ // in observation mode, we break free of deadlocks by throwing a deadlock exception in all blocked threads
			//	syncManager->ThrowDeadlockExceptions();
				// update enabled info: all threads are now mobile again
			}
			ChessImpl::OnDeadlock(false);
			return false;
		} else if (result == ChessExecution::REQUIRE_NONDETERMINISM_PROCESSING) {
			currExecution = nondetHandler->EnabledNondeterminism(currExecution, running_tid);

			// retry sync var access
			continue;
		}
		assert (result == ChessExecution::SUCCESS);

		doing_context_switch = (running_tid != next);
		doing_preemption = (running_tid != next &&
			currExecution->GetQueryEnabled()->IsEnabledAtStep(currExecution->NumTransitions(), running_tid));

		if(opts.break_on_context_switch){
			if(doing_context_switch){
				syncManager->DebugBreak();
			}
		}
		else if(opts.break_on_preemptions){
			if(doing_preemption){
				syncManager->DebugBreak();
			}
		}

		if (doing_preemption)
		{
			SetEventAttribute(id, STATUS, "p"); // set to preempted status
		}

		if(running_tid != next){
			CHESS_LOG(SCHEDULE, "Scheduling ", next);
			running_tid = next;

			profSentry.Stop();
			syncManager->ScheduleTask(next, false);
			profSentry.Start();
		}
		if(!chess_is_attached) return false;

	}
	//should never come here
	assert(false);
	return false;
}

bool ChessImpl::LocalBacktrack(){
	if(!chess_is_attached){
		if(syncManager){
			syncManager->Sleep(1);
		}
		return false;
	}

	CHESS_LOG(EXECUTION, "LocalBacktrack", running_tid);
	SetNextEventAttribute(STATUS, "b");

	bool ret = currExecution->LocalBacktrack();
	while(!ret){
		currExecution = nondetHandler->EnabledNondeterminism(currExecution, running_tid);
		ret = currExecution->LocalBacktrack();
	}

	// The NextTaskToSchedule method may detect non-determinism in the best-first search so
	// we have to deal with it (BFS)
	Task next;
	int result = currExecution->NextTaskToSchedule(next);
	if(result == ChessExecution::FAILURE){
		ChessImpl::OnDeadlock(false);
		return false;
	}
	while (result == ChessExecution::REQUIRE_NONDETERMINISM_PROCESSING) {
		currExecution = nondetHandler->EnabledNondeterminism(currExecution, running_tid);
		result = currExecution->NextTaskToSchedule(next);
		if (result == ChessExecution::FAILURE) {
			ChessImpl::OnDeadlock(true);
			return false;
		}
	}

	if(opts.break_on_context_switch){
		if(running_tid != next){
			syncManager->DebugBreak();
		}
	}
	CHESS_LOG(SCHEDULE, "Scheduling ", next);
	running_tid = next;
	syncManager->ScheduleTask(next, false);
	if (break_deadlock_mode)
		return false;
	return true;
}


bool ChessImpl::TaskFork(Task& child)
{
	if(!chess_is_attached) return false;

	Task curr_tid = running_tid;
	child = AddNewTask();
	activeTasks.insert(child);
	currExecution->AddNewTask(child);
	SyncVarAccess(child, SVOP::TASK_FORK);
	CommitSyncVarAccess();
	return true;
}

bool ChessImpl::ResumeTask(Task tid){
	if(!chess_is_attached) return false;
	if(opts.break_on_task_resume){
		syncManager->DebugBreak();
	}
	if(ChessImpl::SyncVarAccess(tid, SVOP::TASK_RESUME)){
		ChessImpl::CommitSyncVarAccess();
		return true;
	}
	return false;
}

bool ChessImpl::SuspendTask(Task tid){
	if(!chess_is_attached) return false;
	if(ChessImpl::SyncVarAccess(tid, SVOP::TASK_SUSPEND)){
		ChessImpl::CommitSyncVarAccess();
		return true;
	}
	return false;
}

bool ChessImpl::TaskYield(){
	if(!chess_is_attached) return false;

	if(ChessImpl::SyncVarAccess(running_tid, SVOP::TASK_YIELD)){
		ChessImpl::CommitSyncVarAccess();
		return true;
	}
	return false;
}

bool ChessImpl::TaskBegin(){
	if(!chess_is_attached) return false;
	if(ChessImpl::SyncVarAccess(running_tid, SVOP::TASK_BEGIN)){
		ChessImpl::CommitSyncVarAccess();
		return true;
	}
	return false;
}

bool ChessImpl::TaskEnd()
{
	if(!chess_is_attached) return false;
	if(ChessImpl::SyncVarAccess(running_tid, SVOP::TASK_END)){
		activeTasks.erase(running_tid);
		syncManager->TaskEnd(running_tid);
		ChessImpl::CommitSyncVarAccess();

		// The NextTaskToSchedule method may detect non-determinism in the best-first search so
		// we have to deal with it (BFS)
		Task next;
		int result = currExecution->NextTaskToSchedule(next);
		if(result == ChessExecution::FAILURE){
			ChessImpl::OnDeadlock(true);
			return false;
		}
		while (result == ChessExecution::REQUIRE_NONDETERMINISM_PROCESSING) {
			currExecution = nondetHandler->EnabledNondeterminism(currExecution, running_tid);
			
			result = currExecution->NextTaskToSchedule(next);
			if (result == ChessExecution::FAILURE) {
				ChessImpl::OnDeadlock(true);
				return false;
			}
		}
		if(opts.break_on_context_switch){
			if(running_tid != next){
				syncManager->DebugBreak();
			}
		}
		CHESS_LOG(SCHEDULE, "Scheduling ", next);
		running_tid = next;
		syncManager->ScheduleTask(next, true);
		return true;
	}
	return false;
}

bool ChessImpl::Backtrack()
{
	ChessProfilerSentry sentry("ChessImpl::Backtrack");

	if(!chess_is_attached){
		return false;
	}

	// mark redundant executions (BFS)
	const int numHbExecs = chessStats->GetNumHBExecutions();
	aggregateMonitor->OnExecutionEnd(currExecution);
	if (GetOptions().show_hbexecs && chessStats->GetNumHBExecutions() == numHbExecs) {
		SetNextEventAttribute(STATUS, "r");
	}

	ChessExecution* next = 0;

	// call the nondeterminism handler to recover from any nondeterminism
	NondeterminismHandler::RecoveryStatus status;
	nondetHandler->RecoverNondeterminismOnBacktrack(currExecution, status);
	
	if(status.action == NondeterminismHandler::RecoveryStatus::NO_RECOVERY
		|| status.action == NondeterminismHandler::RecoveryStatus::BACKTRACK_FROM_EXECUTION)
	{
		// call into the strategy to find the next execution
		ChessExecution* e = currExecution;
		size_t depthBound = 0;

		if(status.action == NondeterminismHandler::RecoveryStatus::BACKTRACK_FROM_EXECUTION){
			e = status.execution;
			depthBound = status.backtrackStep;
			// the best-first strategy needs to do some cleanup when we detect non-determinism
			// and give up on executing the original target execution (BFS)
			if (opts.best_first) {
				static_cast<BestFirstStrategy*>(strategy)->ReplaceExecution(currExecution, status.execution);
			}
		}

		if(opts.depth_bound && (size_t)opts.depth_bound < depthBound){
			depthBound = (size_t)opts.depth_bound;
		}

		strategy->CompletedExecution(e);
		next = strategy->NextExecution(e, e->GetQueryEnabled(), depthBound);
	}
	else{
		assert(status.action == NondeterminismHandler::RecoveryStatus::EXECUTE_EXECUTION);
		// or else we are not handling a case of status.action
		next = status.execution;
		// Best first search needs to be re-initialized so its stack can be cleaned up, etc. (BFS)
		if (opts.best_first) {
			static_cast<BestFirstExecution*>(next)->Reinitialize();
		}
	}

	if(next == 0){
		Reset();
		//Shutdown(0);
		return false;
	}

	currExecution = next;

	SaveSchedule(opts.schedule_file);
	//SetSchedule(opts.schedule_file);

	// Make this call out before the Reset()
	// This way, the execution's topIndex is not set to 0
	aggregateMonitor->OnExecutionBegin(next);	

	Reset();
	return true;
}

bool ChessImpl::CommitSyncVarAccess(){
	if(!chess_is_attached) return false;

	assert(currExecution->NumTransitions() > 0);
	size_t sid = currExecution->NumTransitions()-1;
	ChessTransition trans = currExecution->Transition(sid);
	EventId id(eventCounter.getNext(trans.tid, true));
	// call the execution's CommitSyncVarAccess until it succeeds - failure implies non-determinism,
	// which the best-first search may only detect at commit time.
	while (!currExecution->CommitSyncVarAccess(id.nr)) {
		// nondeterminism
		currExecution = nondetHandler->EnabledNondeterminism(currExecution, running_tid);
	}

	if(!syncVarManager->IsAggregate(trans.var)){
		aggregateMonitor->OnSyncVarAccess(id, trans.tid, trans.var, trans.op, sid);	
	}
	else{
		const SyncVar* varvec = syncVarManager->GetAggregateVector(trans.var);
		int n = syncVarManager->GetAggregateVectorSize(trans.var);
		aggregateMonitor->OnAggregateSyncVarAccess(id, trans.tid, varvec, n, trans.op, sid);
	}

	SetEventAttribute(id, STATUS, "c");
	return true;
}

void ChessImpl::MarkTimeout()
{
	if(!chess_is_attached) return;
	// The current transition is timing out
	// This should be called before commit has been called
	currExecution->MarkTimeout();
}

EventId ChessImpl::DataVarAccess(void* p, int size, bool isWrite, size_t pcId){
	if(!chess_is_attached) return EventId(0,0);

	EventId id(eventCounter.getNext(running_tid, false));
	aggregateMonitor->OnDataVarAccess(id, p, size, isWrite, pcId);
	SetEventAttribute(id, STATUS, "c");
	return id;
}

EventId ChessImpl::TraceEvent(const std::string &info){
	if(!chess_is_attached) return EventId(0,0);

	EventId id(eventCounter.getNext(running_tid, false));
	aggregateMonitor->OnTraceEvent(id, info);
	SetEventAttribute(id, STATUS, "c");
	return id;
}


void ChessImpl::SetEventAttribute(EventId id, EventAttribute attr, const char *value) {
	if(!chess_is_attached) return;
	aggregateMonitor->OnEventAttributeUpdate(id, attr, value);
}

void ChessImpl::SetEventAttribute(EventId id, EventAttribute attr, size_t value) {
	if(!chess_is_attached) return;
	static char buf[16];
	sprintf_s(buf, 16, "%d", value);
	aggregateMonitor->OnEventAttributeUpdate(id, attr, buf);
}

void ChessImpl::SetNextEventAttribute(EventAttribute attr, const char *value) {
	if(!chess_is_attached) return;
	aggregateMonitor->OnEventAttributeUpdate(eventCounter.peekNext(running_tid), attr, value);	
}


void ChessImpl::OnDeadlock(bool atTermination) {
	if(!chess_is_attached) return;

	if(opts.break_on_deadlock)
		syncManager->DebugBreak();

	if (opts.tolerate_deadlock)
	{
		BreakDeadlock();
	}
	else
	{
		if(strategy) strategy->CompletedExecution(currExecution);
		aggregateMonitor->OnExecutionEnd(currExecution);	

		ChessAssertion("Deadlock", CHESS_EXIT_DEADLOCK);
	}
}

void ChessImpl::EnterQuiescence(){
	if(!chess_is_attached) return;
	currExecution->EnterQuiescence();

	//if(!opts.quiescence){
	//	return;
	//}
	////CommitTransition();
	//// block for quiescence
	//if(currExecution->SyncVarAccess(running_tid, -2, SVOP::QUIESCENT_WAIT)){
	//	LocalBacktrack();
	//}
	//else{
	//	//replay mode
	//	Task next;
	//	if(!currExecution->NextTaskToSchedule(next)){
	//		assert(!"running_tid not enabled in SyncVarAcess");
	//	}
	//	running_tid = next;
	//	syncManager->ScheduleTask(next, false);
	//	bool ret = currExecution->SyncVarAccess(running_tid, -2, SVOP::QUIESCENT_WAIT);
	//	assert(ret);
	//}
}

// This function can be called before chess is attached and after chess is detached
void ChessImpl::RegisterMonitor(IChessMonitor* mon, bool atHead){
	aggregateMonitor->RegisterMonitor(mon, atHead);
}

GetStateCallback ChessImpl::RegisterGetStateCallback(GetStateCallback cb){
	// this function can be called before chess is attached and after chess is detached
	if(!stateMonitor){
		stateMonitor = new StateMonitor();
		RegisterMonitor(stateMonitor);
	}
	return stateMonitor->RegisterGetStateCallback(cb);
}


IChessStats* ChessImpl::GetStats()
{
	return chessStats;
}

void ChessImpl::ReportStatus(){
}

void ChessImpl::AbnormalExit(int exitCode, const char* reason){
	if(!chess_is_attached) return;

	std::string s = "Program exited abnormally : ";
	s += reason;
	ChessAssertion(s.c_str(), exitCode);
}

bool ChessImpl::OnMaxExecTimeout(){
	if(!chess_is_attached)
		return true; // no timeouts when you are shutting down

	if(progressTracker) progressTracker->Cancel();

	if(opts.break_on_timeout){
		syncManager->DebugBreak();
	}
	std::stringstream s;
	s << "Execution Timeout - A single run took more than " << opts.max_exec_time << " seconds \n";
	s << " This is a strong indication of a livelock\n";
	s << " In some cases, this indicates a use of synchronization that CHESS is unaware of\n";
	// TODO: this message is specific to Checker.cpp
	s << " You can also try increasing the exec timeout (-maxexectime:<time>)" << std::endl;
	ChessAssertion(s.str().c_str(), CHESS_EXIT_TIMEOUT);
	return false;
}

void ChessImpl::CaptureResult(char category, const char *description, const char *xmlargs, bool includeschedule, const ErrorInfo* errorInfo) {
	resultsPrinter->CaptureResult(category, std::string(description), std::string(xmlargs), includeschedule, errorInfo);
}

void ChessImpl::ChessAssertion(const char* error, int exitcode){
	// callback
	if(onErrorCallback){
		if(onErrorCallback(exitcode, (char*)error)){
			// now we need to recover from the error
			// Assume the test finished at this point
			// To implement this, we need a clean detach
			// XXX: Implement later
		}
	}

	if(progressTracker) progressTracker->Cancel();

	ReportError(error, "", NULL);

	chess_exit_code = exitcode;
	Shutdown(exitcode);
	CloseErrorStream();
	CloseOutputStream();
	syncManager->Exit(exitcode);
}

void ChessImpl::ReportWarning(const char* error, const char* action, bool withrepro)
{
	resultsPrinter->AddWarning(error, action, withrepro);
	*GetChessErrorStream() << "WARNING: ";
	*GetChessErrorStream() << error << std::endl;
}

void ChessImpl::ReportError(const char* error, const char* action, const ErrorInfo* errorInfo)
{
	resultsPrinter->AddError(error, action, errorInfo);

	*GetChessErrorStream()<< "\n";
	*GetChessErrorStream()<< "***************** CHESS assertion ***********************\n";
	*GetChessErrorStream()<< error  << std::endl;

	SaveSchedule(std::string(opts.output_prefix).append("errorsched").c_str());
	if(opts.print_sched_on_error){
		*GetChessErrorStream()<< " Error Trace (saved on errorsched)\n";
		*GetChessErrorStream()<< *currExecution << std::endl;
	}

	if (opts.break_on_assert) {
		syncManager->DebugBreak();
	}
}

void ChessImpl::ReportFinalStatistics(int exitCode)
{
	resultsPrinter->ReportFinalStatistics(exitCode);
}

void ChessImpl::CloseResults()
{
	resultsPrinter->CloseResults();
}

bool ChessImpl::OnMaxStackSize(size_t size){
	if (!chess_is_attached) return false; 
	std::stringstream s;
	s << "Length of the execution exceeded " << size << ", a strong indication of a livelock " << std::endl;
	ChessAssertion(s.str().c_str(),CHESS_EXIT_LIVELOCK);
	return false;
}

//bool first = true;
void ChessImpl::SaveSchedule(const char* filename){
	static std::ofstream fout;
	while(true) {
		fout.open(filename, std::ios_base::binary | std::ios_base::out | std::ios_base::trunc);
		currExecution->Serialize(fout);
		fout.close();
		if (fout.fail()) {
#ifndef UNDER_CE
			(void)std::remove(filename);
#else
			WCHAR wszBuf[MAX_PATH];
			wszBuf[0] = L'\0';
			::MultiByteToWideChar(CP_ACP, 0, filename, -1, wszBuf, sizeof(wszBuf)/sizeof(wszBuf[0]));
			(void)::DeleteFile(wszBuf);
#endif			
			fout.clear();
		} else {
			break;
		}
	}
}

void ChessImpl::SetSchedule(const char* filename){
	std::ifstream f(filename, std::ios_base::in | std::ios_base::binary);
	currExecution->Deserialize(f);
}

int ChessImpl::GetChessSchedule(__in_ecount(buflen) char* buf, int buflen){
	std::ifstream f(opts.schedule_file, std::ios_base::in | std::ios_base::binary);

	int i = 0;
	while(!f.eof()) {
		char ch = f.get();
		if (i<buflen)
			buf[i] = ch;
		i++;
	}
	f.close();
	return i;
}

bool ChessImpl::SetChessSchedule(const char* buf, int buflen){
	std::ofstream f(opts.schedule_file, std::ios_base::out | std::ios_base::binary);
	for(int i=0; i<buflen; i++){
		f.put(buf[i]);
	}
	f.close();
	return true;
}

void ChessImpl::Shutdown(int exitcode){
	if (chess_shutdown_called)
		return;
	chess_shutdown_called = true;

	aggregateMonitor->OnShutdown();

	ChessProfilerTimer::PrintProfileTimers();

	resultsPrinter->OnShutdown(exitcode);
	
	//Detach();
}

void ChessImpl::Detach(){
	if (!chess_is_attached) return; 
	chess_is_attached = false;
	// schedule every active task
	// Setting the 'atTermination' flag tells the syncManager not to block
	// the current thread - it is a nice, hacky use of this flag
	assert(!activeTasks.IsEmpty());
	Task firsttask = activeTasks.FindIndexLargerThan(0);
	Task t = firsttask;
	while(true){
		if(t != running_tid)
			syncManager->ScheduleTask(t, true);
		t = activeTasks.FindIndexLargerThan(t);
		if(t == firsttask)
			break;
	}
	//std::set<Task>::iterator i;
	//for(i = activeTasks.begin(); i!= activeTasks.end(); i++){
	//	if(*i != running_tid){
	//		syncManager->ScheduleTask(*i, true);
	//	}
	//}
	//return to the current running thread
}

void ChessImpl::BreakDeadlock()
{
	assert(!activeTasks.IsEmpty());
	assert(!break_deadlock_mode);
	assert(chess_is_attached);

	break_deadlock_mode = 1;
	deadlock_continuation = 0;
	chess_is_attached = false;
}

bool ChessImpl::IsBreakingDeadlock()
{
	return break_deadlock_mode != 0;
}

void ChessImpl::WakeNextDeadlockedThread(bool iscontinuation, bool isDeadlockedThread)
{
	assert(!chess_is_attached);
	assert(break_deadlock_mode);
	Task this_tid = running_tid;

	if (isDeadlockedThread && observationMonitor)
		observationMonitor->Deadlock(running_tid);

	if (! iscontinuation)
	{
		activeTasks.erase(running_tid);
		syncManager->TaskEnd(running_tid);
	}
	else
	{
		assert(deadlock_continuation == 0);
		deadlock_continuation = running_tid;
	}

	// release threads in numerical order, but continuation thread is last
	Task t = activeTasks.FindIndexLargerThan(0);
	if (t == deadlock_continuation)
			t = activeTasks.FindIndexLargerThan(t);

	// schedule thread
	if (t != running_tid)
	{
		CHESS_LOG(SCHEDULE, "Scheduling (for BreakDeadlock) ", t);
		running_tid = t;
		syncManager->ScheduleTask(t, !iscontinuation);
	}

	// reattach
	if (iscontinuation)
	{
		assert(this_tid == deadlock_continuation);
		break_deadlock_mode = 0;
		chess_is_attached = true;
		EventId(eventCounter.getNext(t, false)); // finish up blocked event
		TraceEvent("BROKE DEADLOCK");
	}

	return;		
}

int ChessImpl::GetExitCode(){
	return chess_exit_code;
}

int ChessImpl::SetExitCode(int exitCode){
	int ret = chess_exit_code;
	chess_exit_code = exitCode;
	return ret;
}

CHESS_ON_ERROR_CALLBACK ChessImpl::QueueOnErrorCallback(CHESS_ON_ERROR_CALLBACK newCallback){
	CHESS_ON_ERROR_CALLBACK old = onErrorCallback;
	onErrorCallback = newCallback;
	return old;
}

void ChessImpl::Done(bool enter){
	if(!chess_is_attached) return;

	if (enter)
		EnterChess();

	Shutdown(0); // shutdown is already called on backtrack returns null

	delete currExecution;
	currExecution = NULL;

	//syncManager = NULL;

	delete strategy;
	strategy = NULL;

	delete aggregateMonitor;

	//chessStats = NULL;
}


void Chess::Init(SyncManager* sm){ 
	ChessImpl::Init(sm); 
}

bool Chess::IsInitialized(){ 
	return ChessImpl::IsInitialized();
}

bool Chess::StartTest(){
	return ChessImpl::StartTest();
}

bool Chess::EndTest(){ 
	return ChessImpl::EndTest();
}

void Chess::Done(bool enter){ 
	ChessImpl::Done(enter);
}

void Chess::AbnormalExit(int exitCode, const char* reason){
	ChessImpl::AbnormalExit(exitCode, reason);
}

void Chess::WakeNextDeadlockedThread(bool isContinuation, bool isDeadlockedThread) {
	ChessImpl::WakeNextDeadlockedThread(isContinuation, isDeadlockedThread);
}

SyncVar Chess::GetNextSyncVar(){
	return ChessImpl::GetNextSyncVar();
}

int Chess::Choose(int numChoices){
	return ChessImpl::Choose(numChoices);
}

void Chess::PreemptionDisable(){
	ChessImpl::PreemptionDisable();
}

void Chess::PreemptionEnable(){
	ChessImpl::PreemptionEnable();
}

// Start marking the running thread's steps as important for the best first search (BFS)
void Chess::PrioritizePreemptions() {
	ChessImpl::PrioritizePreemptions();
}

// Stop marking the running thread's steps as important for the best first search (BFS)	
void Chess::UnprioritizePreemptions() {
	ChessImpl::UnprioritizePreemptions();
}

void Chess::MergeSyncAndDataVar(SyncVar svar, void *dvar){
	return ChessImpl::MergeSyncAndDataVar(svar, dvar);
}

void Chess::ObserveOperationCall(void *object, const char *opname)
{
	ChessImpl::ObserveOperationCall(object, opname);
}
void Chess::ObserveOperationReturn()
{
	ChessImpl::ObserveOperationReturn();
}
void Chess::ObserveCallback(void *object, const char *opname)
{
	ChessImpl::ObserveCallback(object, opname);
}
void Chess::ObserveCallbackReturn()
{
	ChessImpl::ObserveCallbackReturn();
}
void Chess::ObserveIntValue(const char *label, long long value)
{
	ChessImpl::ObserveIntValue(label, value);
}
void Chess::ObservePointerValue(const char *label, void *value)
{
	ChessImpl::ObservePointerValue(label, value);
}
void Chess::ObserveStringValue(const char *label, const char *value)
{
	ChessImpl::ObserveStringValue(label, value);
}
bool Chess::IsBreakingDeadlock()
{
	return ChessImpl::IsBreakingDeadlock();
}

bool Chess::TaskFork(Task& childTid){ 
	return ChessImpl::TaskFork(childTid);
}

bool Chess::TaskEnd(){
	return ChessImpl::TaskEnd();
}

bool Chess::TaskBegin(){
	return ChessImpl::TaskBegin();
}

bool Chess::SyncVarAccess(SyncVar var, SyncVarOp op){
	return ChessImpl::SyncVarAccess(var, op);
}
	
bool Chess::AggregateSyncVarAccess(const SyncVar vars[], int n, SyncVarOp op){
	return ChessImpl::AggregateSyncVarAccess(vars, n, op);
}

bool Chess::CommitSyncVarAccess(){
	return ChessImpl::CommitSyncVarAccess();
}

void Chess::MarkTimeout(){
	ChessImpl::MarkTimeout();
}

bool Chess::ResumeTask(Task tid){
	return ChessImpl::ResumeTask(tid);
}

bool Chess::SuspendTask(Task tid){
	return ChessImpl::SuspendTask(tid);
}

bool Chess::LocalBacktrack(){
	return ChessImpl::LocalBacktrack();
}

bool Chess::EnterChess(){
	return ChessImpl::EnterChess();
}

void Chess::LeaveChess(){
	return ChessImpl::LeaveChess();
}

bool Chess::TaskYield(){
	return ChessImpl::TaskYield();
}

void Chess::SetOptions(ChessOptions& o){
	ChessImpl::SetOptions(o);
}

const ChessOptions& Chess::GetOptions(){
	return ChessImpl::GetOptions();
}

SyncManager* Chess::GetSyncManager(){
	return ChessImpl::GetSyncManager();
}

Task Chess::GetCurrentTid(){
	return ChessImpl::GetCurrentTid();	
}
	
void Chess::RegisterMonitor(IChessMonitor* mon){
	ChessImpl::RegisterMonitor(mon);	
}

GetStateCallback Chess::RegisterGetStateCallback(GetStateCallback cb){
	return ChessImpl::RegisterGetStateCallback(cb);	
}

EventId Chess::DataVarAccess(void* p, int size, bool isWrite, int pcId){
	return ChessImpl::DataVarAccess(p, size, isWrite, pcId);
}

EventId Chess::TraceEvent(const std::string &info) {
	return ChessImpl::TraceEvent(info);
}

void Chess::SetEventAttribute(EventId id, EventAttribute attr, const char *value) {
	ChessImpl::SetEventAttribute(id, attr, value);
}

void Chess::SetNextEventAttribute(EventAttribute attr, const char *value) {
	ChessImpl::SetNextEventAttribute(attr, value);
}

int Chess::GetExitCode(){
	return ChessImpl::GetExitCode();
}

void Chess::ReportError(const char* description, const char *action, const ErrorInfo* errorInfo)
{
	ChessImpl::ReportError(description, action, errorInfo);
}

void Chess::ReportWarning(const char* description, const char *action, bool withschedule)
{
	ChessImpl::ReportWarning(description, action, withschedule);
}

void Chess::CaptureResult(char category, const char *description, const char *reproargs, bool includeschedule, const ErrorInfo* errorInfo) 
{
	ChessImpl::CaptureResult(category, description, reproargs, includeschedule, errorInfo);
}

void Chess::ReportFinalStatistics(int exitCode)
{
	ChessImpl::ReportFinalStatistics(exitCode);
}

void Chess::CloseResults()
{
	ChessImpl::CloseResults();
}

IChessStats* Chess::GetStats()
{
	return ChessImpl::GetStats();
}

CHESS_ON_ERROR_CALLBACK Chess::QueueOnErrorCallback(CHESS_ON_ERROR_CALLBACK newCallback){
	return ChessImpl::QueueOnErrorCallback(newCallback);
}


CHESS_API void __stdcall ChessInit(ChessOptions& opts, SyncManager* sm)
{
	Chess::SetOptions(opts);
	Chess::Init(sm);
}

CHESS_API void __stdcall ChessStartTest(){
	Chess::StartTest();

}

CHESS_API bool __stdcall ChessEndTest(){
	return Chess::EndTest();
}

CHESS_API void __stdcall ChessDone(){
	Chess::Done();
}

CHESS_API CHESS_ON_ERROR_CALLBACK ChessQueueOnErrorCallback(CHESS_ON_ERROR_CALLBACK newCallback){
	return Chess::QueueOnErrorCallback(newCallback);
}


// TODO: This api should be removed
CHESS_API void __stdcall ChessEnterQuiescence(){
	return ChessImpl::EnterQuiescence();
}

CHESS_API void __stdcall ChessRegisterMonitor(IChessMonitor* mon){
	Chess::RegisterMonitor(mon);
}

CHESS_API GetStateCallback __stdcall ChessRegisterGetStateCallback(GetStateCallback cb){
	return Chess::RegisterGetStateCallback(cb);
}

int Chess::GetChessSchedule(__in_ecount(buflen) char* buf, int buflen){
	return ChessImpl::GetChessSchedule(buf, buflen);
}

bool Chess::SetChessSchedule(const char* buf, int buflen){
	return ChessImpl::SetChessSchedule(buf, buflen);
}

CHESS_API int GetChessSchedule(char* buf, int buflen){
	return ChessImpl::GetChessSchedule(buf, buflen);
}

CHESS_API bool SetChessSchedule(const char* buf, int buflen){
	return ChessImpl::SetChessSchedule(buf, buflen);
}

