/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "Win32Base.h"
#include "SyncManager.h"
#include "Semaphore.h"
#include "ChessWrapperSentry.h"

class Win32SyncVarManager;
class Win32WrapperManager;

class WIN32CHESS_API Win32SyncManager : public SyncManager{
public:
	Win32SyncManager();
	~Win32SyncManager();

	virtual void Init(Task initTask);
	virtual void Reset();
	virtual void ShutDown();
	virtual void SetInitState();

	//virtual void ForkHandshake(Task tid);

	virtual void TaskEnd(Task curr);

	//virtual void ScheduleFirstTask(Task tid);

	virtual void ScheduleTask(Task tid, bool atTermination);

//	virtual bool WaitForAllTasks(int timeout);

	//virtual void AllTasksDone();

	virtual void EpsilonSleep();

	virtual void EnterChess() {
		ChessWrapperSentry::EnterChessImpl();
	}

	virtual void LeaveChess() {
		ChessWrapperSentry::LeaveChessImpl();
	}

	//static bool WrappersDisabled(){
	//	return !initializedFlag || Chess::InChess();
	//}

	//virtual std::ostream& Out() {
	//	return std::cout;
	//}

	//virtual std::istream& In() {
	//	return std::cin;
	//}

	//virtual std::ostream& Err()  {
	//	return std::cerr;
	//}

	// Win32 Specific Functions
	void ThreadBegin(Semaphore selfSemaphore);
	void ThreadEnd();

	void RegisterThreadSemaphore(Task child, Semaphore sem, BOOL isJoinable = FALSE);

	bool RegisterTestModule(HMODULE hModule);

	void JoinableThreadEnd();

	void TimerThreadEnd(HANDLE timerIntermediateEvent, 
									  HANDLE timerCompletionEvent, 
									  HANDLE timerQueueIntermediateEvent, 
									  HANDLE timerQueueCompletionEvent);

	void AddChildHandle(Task child, HANDLE hChildThread);
	void DuplicateHandle(HANDLE orig, HANDLE copy);

	void AssociateNamedHandle(LPCSTR lpName, HANDLE handle);
	void AssociateNamedHandle(LPCWSTR lpName, HANDLE handle);


	SyncVar GetNewSyncVar();
	Task GetTid(HANDLE h);
	SyncVar GetSyncVarFromHandle(HANDLE h);
	SyncVar GetSyncVarFromAddress(void* addr);


	virtual bool RenameSymmetricTasks();

	virtual void Exit(int code);
	virtual void DebugBreak();

	virtual bool IsDebuggerPresent();

	virtual bool QueuePeriodicTimer(int timerPeriodInMilliseconds, void (*timerFn)());

	virtual bool GetCurrentStackTrace(int n, int m, char* procedure[], char* filename[], int lineno[]);
	virtual bool GetCurrentStackTrace(int start, int num, __int64 pcs[]);
	virtual bool GetStackTraceSymbols(__int64 pc, int n, char* procedure, char* filename, int* lineno);

	virtual __int64 GetCurrentTickCount();
	virtual int ConvertTickCountToMs(__int64 tickCount);

private:
	// Task Management State
	HANDLE TerminatingThread;
	HANDLE TimerIntermediateEvent;
	HANDLE TimerCompletionEvent;
	HANDLE TimerQueueIntermediateEvent;
	HANDLE TimerQueueCompletionEvent;

	Win32SyncVarManager* syncVarIdManager;
	Win32WrapperManager* wrapperManager;

	void OnWakeup();

	static bool initializedFlag;
	
	Task runningTid;
	Task initTask;

	HANDLE timerHandle;

	void AddJoinableThread(Task t);
	void RemoveJoinableThread(Task t);
	bool IsJoinableThread(Task t);
	void* joinableThreads;
	
	DWORD tlsCurrentTaskId;
};

WIN32CHESS_API Win32SyncManager* GetWin32SyncManager();
