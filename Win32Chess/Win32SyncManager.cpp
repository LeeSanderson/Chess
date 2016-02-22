/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "Win32Base.h"
#include "Win32SyncManager.h"
#include "Win32SyncVarManager.h"
#include "Win32Wrappers.h"
#include "ChessAssert.h"
#include "BitVector.h"
#include "Win32WrapperManager.h"

//#include "ChessExecution.h"
//#include "StackTraceMonitor.h"

bool Win32SyncManager::initializedFlag = false;

Win32SyncManager::Win32SyncManager(){
	syncVarIdManager = new Win32SyncVarManager();
	wrapperManager = new Win32WrapperManager();
	wrapperManager->LoadWrappers();
}

Win32SyncManager::~Win32SyncManager(){
	delete syncVarIdManager;
	delete wrapperManager;
}


#ifndef UNDER_CE
void ExitSignalHandler(int signal) {
	Chess::AbnormalExit(-1, "Test called abort");
}
#endif

void Win32SyncManager::Init(Task it){
	initializedFlag = true;
	syncVarIdManager->Init();

	initTask = it;
	timerHandle = NULL;
	
	tlsCurrentTaskId = TlsAlloc();

	Semaphore initSem;
	initSem.Init();
	syncVarIdManager->RegisterThreadSemaphore(initTask, initSem);
	TlsSetValue(tlsCurrentTaskId, (LPVOID)initTask);
	runningTid = initTask;
	//joinableThreads = new std::set<Task>();
	joinableThreads = new BitVector();

#ifndef UNDER_CE
	signal(SIGABRT, ExitSignalHandler);
#endif
	//_CrtSetReportHook(crtReportHook);
	if(!IsDebuggerPresent() && Chess::GetOptions().nopopups){
		_CrtSetReportMode( _CRT_ERROR, _CRTDBG_MODE_FILE );
		_CrtSetReportFile( _CRT_ERROR, _CRTDBG_FILE_STDERR );
		_CrtSetReportMode( _CRT_ASSERT, _CRTDBG_MODE_FILE );
		_CrtSetReportFile( _CRT_ASSERT, _CRTDBG_FILE_STDERR );
	}


	//ChessRegisterMonitor(new StackTraceMonitor());

	Reset();
}

//bool timerDisabled = true;
//int timerPauseCount = 0;

VOID CALLBACK Win32ChessWaitOrTimerCallback(PVOID lpParameter, BOOLEAN TimerOrWaitFired)
{
	//if(timerDisabled) return;
	//if(timerPauseCount && (--timerPauseCount) > 0) return;

	void (*f)() = (void (*)())lpParameter;
	f();
}


bool Win32SyncManager::QueuePeriodicTimer(int period, void (*timerFn)()){
#ifndef UNDER_CE
	if(timerHandle != NULL)
		return false; // can only have one timer right now

	BOOL ret = CreateTimerQueueTimer(&timerHandle, NULL, Win32ChessWaitOrTimerCallback, timerFn, period, period, WT_EXECUTELONGFUNCTION);
	if(!ret){
		return false;
	}
	//timerDisabled = false;
	//timerPauseCount = 0;
#endif
	return true;
}

void Win32SyncManager::Reset(){
	TerminatingThread = NULL;
	TimerIntermediateEvent = NULL;
	TimerCompletionEvent = NULL;
	TimerQueueIntermediateEvent = NULL;
	TimerQueueCompletionEvent = NULL;

//	SetEvent(timerArg->resetEvent);
	syncVarIdManager->Reset();
	wrapperManager->Reset();

	//WrappersReset();

	runningTid = initTask;
	// 'schedule' the initial task
	//Semaphore initSem;
	//initSem.Init();
	//syncVarIdManager->RegisterThreadSemaphore(initTask, initSem);
	//runningTid = initTask;

}

void Win32SyncManager::Exit(int code){
	//std::cout << "Exiting with " << code << std::endl;
	::exit(code);
}
void Win32SyncManager::DebugBreak(){
#ifndef UNDER_CE
	::DebugBreak();
#else
    // For CE, DebugBreak does nothing when the debugger is not attached
    // To prevent accidentally missing bugs, use RaiseException
    __try
    {
        OutputDebugString(L"Breaking into debugger");
        RaiseException(1, 0, 0, NULL);
    }
    __except(EXCEPTION_EXECUTE_HANDLER)
    {
    }
#endif
}

bool Win32SyncManager::IsDebuggerPresent(){
	return ::IsDebuggerPresent() != 0;
}


void Win32SyncManager::AddJoinableThread(Task t){
	//std::set<Task>* j = (std::set<Task>*)joinableThreads;
	BitVector* j = (BitVector*)joinableThreads;
	j->insert(t);
}
bool Win32SyncManager::IsJoinableThread(Task t){
//	std::set<Task>* j = (std::set<Task>*)joinableThreads;
//	return j->find(t) != j->end();
	const BitVector* j = (BitVector*)joinableThreads;
	return j->Contains(t);
}
void Win32SyncManager::RemoveJoinableThread(Task t){
//	std::set<Task>* j = (std::set<Task>*)joinableThreads;
	BitVector* j = (BitVector*)joinableThreads;
	j->erase(t);
}


void Win32SyncManager::ScheduleTask(Task next, bool atTermination){
	Task currTid = (Task)TlsGetValue(tlsCurrentTaskId);
	if(!atTermination)
		assert(runningTid == currTid);
	if(runningTid == next){
		if(atTermination){
			assert("Schedule running_tid at termination");
		}
		return;
	}

	Semaphore* currSem = 0;
	Semaphore* nextSem = syncVarIdManager->GetTaskSemaphore(next);
	//if(atTermination){
	//	assert(currSem->IsNull());
	//}
	if(!atTermination){
		currSem = syncVarIdManager->GetTaskSemaphore(runningTid);
	}
	runningTid = next;
	nextSem->AtomicUpDown(currSem);
	if(!atTermination){
		OnWakeup();
	}

	//nextSem->Up();

	//if(!atTermination){
	//	currSem->Down();	
	//	OnWakeup();
	//}
}

//bool Win32SyncManager::WaitForAllTasks(int timeout){
//	DWORD ret;
////	ret = WaitForMultipleObjects(numInitTasks, initHandles, TRUE, timeout);
//	ret = WaitForSingleObject(tasksDoneEvent, timeout);
//	if(ret == WAIT_TIMEOUT){
//		return false;
//	}
//	assert(WAIT_OBJECT_0 == ret);
////	ret = WaitForMultipleObjects(numInitTasks, initHandles, TRUE, INFINITE);
//	return true;
//}

//void Win32SyncManager::AllTasksDone(){
//	BOOL ret = SetEvent(tasksDoneEvent);
//	assert(ret);
//}

void Win32SyncManager::ShutDown(){
//	SetEvent(timerArg->doneEvent);
	initializedFlag = false;
//	free(initHandles);
}


//void Win32SyncManager::ForkHandshake(Task tid){
//	Semaphore* mysem = syncVarIdManager->ProtectedGetTaskSemaphore(tid);	
//	mysem->Down();
//}

void Win32SyncManager::EpsilonSleep(){
	::Sleep(1);
}

void Win32WrappersOnAlertableState();

void Win32SyncManager::ThreadBegin(Semaphore sem){
	sem.Down();
	OnWakeup();
	TlsSetValue(tlsCurrentTaskId, (LPVOID)runningTid);
	Chess::TaskBegin();
#ifndef UNDER_CE
	//A thread starts in an alertable state (i.e. Executes APCs that are queued before starting)
	WrappersOnAlertableState(runningTid);
#endif
}

void Win32AsyncProcThreadEnd(Task tid);

void Win32SyncManager::ThreadEnd(){
	Chess::TaskEnd();
	TlsSetValue(tlsCurrentTaskId, (LPVOID)0);
}

void Win32SyncManager::TaskEnd(Task curr){
	Task tid = Chess::GetCurrentTid();
	Task currTid = (Task)TlsGetValue(tlsCurrentTaskId);
	assert(currTid == tid);

	wrapperManager->TaskEnd(tid);
	//Win32AsyncProcThreadEnd(tid);

	GetWin32SyncManager()->JoinableThreadEnd();
	assert(runningTid == curr);
	syncVarIdManager->TaskEnd(curr);
}

extern stdext::hash_map<Task, Task> GetSymmetricTasks();
bool Win32SyncManager::RenameSymmetricTasks(){
	return true;
//	return syncVarIdManager->RenameSymmetricTasks(GetSymmetricTasks());
}


void Win32SyncManager::TimerThreadEnd(HANDLE timerIntermediateEvent, 
									  HANDLE timerCompletionEvent, 
									  HANDLE timerQueueIntermediateEvent, 
									  HANDLE timerQueueCompletionEvent)
{
	TimerIntermediateEvent = timerIntermediateEvent;
	TimerCompletionEvent = timerCompletionEvent;
	TimerQueueIntermediateEvent = timerQueueIntermediateEvent;
	TimerQueueCompletionEvent = timerQueueCompletionEvent;
}

void Win32SyncManager::JoinableThreadEnd(){
	if(!IsJoinableThread(runningTid))
		return;
	RemoveJoinableThread(runningTid);

	// make a copy of the current thread handle and store it in TerminatingThread
	::DuplicateHandle(GetCurrentProcess(), 
		GetCurrentThread(),
		GetCurrentProcess(),
		&TerminatingThread,
		0,
		FALSE,
		DUPLICATE_SAME_ACCESS);

}


void Win32SyncManager::OnWakeup(){
	if (TerminatingThread != NULL)
	{
		WaitForSingleObject(TerminatingThread, INFINITE);
		CloseHandle(TerminatingThread);
		TerminatingThread = NULL;
	}
	if (TimerIntermediateEvent != NULL)
	{
		assert(TimerCompletionEvent != NULL);
		if(TimerCompletionEvent != NULL){
			WaitForSingleObject(TimerIntermediateEvent, INFINITE);
			CloseHandle(TimerIntermediateEvent);
			SetEvent(TimerCompletionEvent);
			TimerIntermediateEvent = NULL;
			TimerCompletionEvent = NULL;
		}
	}
	if (TimerQueueIntermediateEvent != NULL)
	{
		assert(TimerQueueCompletionEvent != NULL);
		if(TimerQueueCompletionEvent != NULL){
			WaitForSingleObject(TimerQueueIntermediateEvent, INFINITE);
			CloseHandle(TimerQueueIntermediateEvent);
			SetEvent(TimerQueueCompletionEvent);
			TimerQueueIntermediateEvent = NULL;
			TimerQueueCompletionEvent = NULL;
		}
	}  
}

void Win32SyncManager::RegisterThreadSemaphore(Task child, Semaphore sem, BOOL isJoinable){
	if(isJoinable)
		AddJoinableThread(child);

	syncVarIdManager->RegisterThreadSemaphore(child, sem);
}

void Win32SyncManager::AddChildHandle(Task child, HANDLE hChildThread){
	assert(hChildThread != NULL);
	syncVarIdManager->AddThreadHandleMapping(child, hChildThread);
}

void Win32SyncManager::DuplicateHandle(HANDLE orig, HANDLE copy){
	assert(orig != NULL);
	if(orig == GetCurrentThread()){
		orig = syncVarIdManager->GetCurrentHandle(runningTid);
		if(orig == NULL){
			// first HANDLE for this thread
			syncVarIdManager->AddThreadHandleMapping(runningTid, copy);
			return;
		}
	}
	syncVarIdManager->DuplicateHandle(orig, copy);
	// XXX: Should duplicate handle state on all other wrappers as well --- madan
}

void Win32SyncManager::AssociateNamedHandle(LPCSTR name, HANDLE handle){
	std::string ascii_name(name);
	std::wstring unicode_name(ascii_name.begin(), ascii_name.end());
	HANDLE origHandle = syncVarIdManager->AssociateNamedHandle(unicode_name, handle);
	if(origHandle != handle){
		// treate 'handle' as a duplicate
		syncVarIdManager->DuplicateHandle(origHandle, handle);
	}
}

void Win32SyncManager::AssociateNamedHandle(LPCWSTR name, HANDLE handle){
	std::wstring unicode_name(name);
	HANDLE origHandle = syncVarIdManager->AssociateNamedHandle(unicode_name, handle);
	if(origHandle != handle){
		// treate 'handle' as a duplicate
		syncVarIdManager->DuplicateHandle(origHandle, handle);
	}
}

SyncVar Win32SyncManager::GetNewSyncVar(){
	return syncVarIdManager->GetNewSyncVar();
}

Task Win32SyncManager::GetTid(HANDLE h){
	return syncVarIdManager->GetTid(h);
}

SyncVar Win32SyncManager::GetSyncVarFromHandle(HANDLE h){
	return syncVarIdManager->GetSyncVarFromHandle(h);
}

SyncVar Win32SyncManager::GetSyncVarFromAddress(void* addr){
	return syncVarIdManager->GetSyncVarFromAddress(addr);
}

void Win32SyncManager::SetInitState(){
	syncVarIdManager->SetInitState();
}

#ifndef UNDER_CE
#define STACK_TRACE_PER_TRANSITION
#include "StackWalkHelper.cpp"
#endif

bool Win32SyncManager::GetCurrentStackTrace(int start, int num, __int64 pcs[]){
#ifndef UNDER_CE
	StackWalkHelper::GetCallerPC(start, num, (DWORD64*)pcs);
#endif
	return true;
}

bool Win32SyncManager::GetStackTraceSymbols(__int64 pc, int m, char* procedure, char* filename, int* line){
#ifndef UNDER_CE
	std::string file;
	std::string fn;
	int lineNo;
	if(!StackWalkHelper::GetSymbolInfoForPC(pc, fn, file, lineNo)){
		return false;
	}
	strncpy_s(filename, m, file.c_str(), m);
	strncpy_s(procedure, m, fn.c_str(), m);
	*line = lineNo;
#else
    procedure[0] = '\0';
    filename[0] = '\0';
    *line = 0;
#endif
	return true;
}

bool Win32SyncManager::GetCurrentStackTrace(int n, int m, char* procedure[], char* filename[], int lineno[]){
#ifndef UNDER_CE
	DWORD64* pcs = new DWORD64[n];

	// first parameter is a guess on how many frames of CHESS are on top of stack - hacky
	// not taking a chance and so setting it to 1
	StackWalkHelper::GetCallerPC(1, n, pcs);
	bool offbyone = false;
	for(int i=0; i<n; i++){
		std::string fileName;
		std::string fn;
		int lineNo;
		if(!StackWalkHelper::GetSymbolInfoForPC(pcs[i], fn, fileName, lineNo)){
			lineno[i] = 0;
			filename[i][0] = 0;
			procedure[i][0] = 0;
			continue;
		}
		strncpy_s(filename[i], m, fileName.c_str(), m);
		strncpy_s(procedure[i], m, fn.c_str(), m);
		lineno[i] = lineNo;
		if(offbyone)
			lineno[i] --;
		if(fn.compare(0, 10, "__wrapper_") == 0 || fn.compare(0, 8, "__Chess_") == 0){
			// the next one should be one off;
			offbyone = true;
		}
		else{
			offbyone = false;
		}
	}
	delete[] pcs;
#else
    for ( int i = 0; i < n; ++i )
    {
        procedure[i][0] = '\0';
        filename[i][0] = '\0';
        lineno[i] = 0;
    }
#endif
	return true;
}



//
//#include "IATDetours.h"
//#include "IChessWrapper.h"

//IChessWrapper* GetWrappersMSVCRT();
//void GetFltTable(stdext::hash_map<void*, void*>& wrapperTable);
//DWORD GetFunctionAddressByName(HMODULE hModule, char* moduleName, char* moduleFn);
//void GetWrappersConsoleTable(stdext::hash_map<void*, void*>& wrapperTable);

bool Win32SyncManager::RegisterTestModule(HMODULE hModule){
	return wrapperManager->RegisterTestModule(hModule);
}

__int64 Win32SyncManager::GetCurrentTickCount(){
	LARGE_INTEGER cnt;
	QueryPerformanceCounter(&cnt);
	return cnt.QuadPart;
}

int Win32SyncManager::ConvertTickCountToMs(__int64 tickCount){
	LARGE_INTEGER ticksPerSec;
	QueryPerformanceFrequency(&ticksPerSec);
	return (int)(tickCount*1000/ticksPerSec.QuadPart);
}
