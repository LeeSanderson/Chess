/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "Win32Base.h"
#include "ChessAssert.h"
#include "Chess.h"
#include "Win32Wrappers.h"
#include "Win32WrapperAPI.h"
#include "Win32SyncManager.h"
#include "IChessWrapper.h"

#define IGNORE_UNKNOWN_QUEUES

class TimerQueueInfo;

class TimerInfo {
	BOOLEAN deleted;
	BOOLEAN pending;
	BOOLEAN done;
	HANDLE CompletionEvent;
	SyncVar syncVar;

	TimerInfo(){
		deleted = FALSE;
		pending = FALSE;
		done = FALSE;
		CompletionEvent = NULL;
		syncVar = SyncVar(0);
	}
	friend class TimerQueueInfo;
public:
	SyncVar GetSyncVar(){
		assert(syncVar != 0);
		return syncVar;
	}

	BOOLEAN IsDeleted() const{
		return deleted;
	}
	BOOLEAN IsPending() const{
		return pending;
	}
	BOOLEAN IsDone() const{
		return done;
	}

private:
	void Start(){
		pending = TRUE;
	}

	void Delete(HANDLE hEvent){
		assert(!deleted);
		deleted = TRUE;
		if(hEvent != NULL && hEvent != INVALID_HANDLE_VALUE){
			CompletionEvent = hEvent;
		}
	}

	void Finish(){
		assert(pending);
		pending = FALSE;
		if(deleted && CompletionEvent != NULL){
			__wrapper_SetEvent(CompletionEvent);
			CompletionEvent = NULL;
		}
	}
	void Done(){
		done = TRUE;
	}
};

class TimerQueueInfo {
	BOOLEAN deleted; 
	int numPendingTimers;
	int numNotDoneTimers;
	HANDLE CompletionEvent;
	SyncVar syncVar;
	HANDLE hTimerQueue;

	TimerQueueInfo(HANDLE handle){
		deleted = FALSE;
		numPendingTimers = 0;
		numNotDoneTimers = 0;
		CompletionEvent = NULL;
		syncVar = SyncVar(0);
		hTimerQueue = handle;
	}

	//maps TimerQueueHandle -> TimerQueueInfo
	static stdext::hash_map< HANDLE, TimerQueueInfo* > TimerQueues;

	//maps (TimerQueueHandle x TimerQueueTimerHandle) -> TimerInfo
	static stdext::hash_map< HANDLE, stdext::hash_map<HANDLE, TimerInfo*> > Timers;
public:
	static void CreateTimerQueueInfo(HANDLE handle){
		if (TimerQueues.find(handle) != TimerQueues.end())
		{
			delete (TimerQueues[handle]);
			stdext::hash_map<HANDLE, TimerInfo *> info = Timers[handle];
			stdext::hash_map<HANDLE, TimerInfo *>::iterator iter;
			for (iter = info.begin(); iter != info.end(); iter++)
			{
				delete iter->second;
			}
		}
		TimerQueues[handle] = new TimerQueueInfo(handle);
	}

	static TimerQueueInfo* GetTimerQueueInfo(HANDLE handle){
		if(handle == NULL)
			CreateTimerQueueInfo(NULL);
#ifdef IGNORE_UNKNOWN_QUEUES
		if(TimerQueues.find(handle) == TimerQueues.end())
			return NULL;
#endif
		assert(TimerQueues.find(handle) != TimerQueues.end());
		return TimerQueues[handle];
	}

	static void Reset(){
		stdext::hash_map<HANDLE, TimerQueueInfo* >::iterator iter;
		for (iter = TimerQueues.begin(); iter != TimerQueues.end(); iter++)
		{
			delete iter->second;
		}
		TimerQueues.clear();
		stdext::hash_map< HANDLE, stdext::hash_map<HANDLE, TimerInfo*> >::iterator iter1;
		for (iter1 = Timers.begin(); iter1 != Timers.end(); iter1++)
		{
			stdext::hash_map<HANDLE, TimerInfo *> info = iter1->second;
			stdext::hash_map<HANDLE, TimerInfo *>::iterator iter2;
			for (iter2 = info.begin(); iter2 != info.end(); iter2++)
			{
				delete iter2->second;
			}
		}
		Timers.clear();
//		CreateTimerQueueInfo(NULL);
	}
	
	SyncVar GetSyncVar(){
		if(syncVar == 0){
			syncVar = GetWin32SyncManager()->GetNewSyncVar();
		}
		return syncVar;
	}

	void GetAggregateSyncVar(TimerInfo* timerInfo, std::vector<SyncVar>& v){
		//SyncVar vars[2];
		//vars[0] = this->GetSyncVar();
		//vars[1] = timerInfo->GetSyncVar();
		//return SyncVar(vars, 2);
		v.push_back(this->GetSyncVar());
		v.push_back(timerInfo->GetSyncVar());
	}

	void GetAggregateSyncVar(std::vector<SyncVar>& v) {
		stdext::hash_map<HANDLE, TimerInfo *> info = Timers[this->hTimerQueue];
		//SyncVar *vars = new SyncVar[info.size()+1];
		//int numSyncVars = 0;
		//vars[numSyncVars++] = this->GetSyncVar();
		v.push_back(this->GetSyncVar());
		stdext::hash_map<HANDLE, TimerInfo *>::iterator iter;
		for (iter = info.begin(); iter != info.end(); iter++)
		{
			if(!(iter->second->IsDeleted())){
//				vars[numSyncVars++] = iter->second->GetSyncVar();
				v.push_back(iter->second->GetSyncVar());
			}
		}
		//SyncVar ret = SyncVar(vars, numSyncVars);
		//delete[] vars;
		//return ret;
	}

	static TimerInfo* CreateTimerInfo() {
		return new TimerInfo();
	}
	static void DeleteTimerInfo(TimerInfo* timerInfo) {
		delete timerInfo;
	}

	void RegisterTimerHandle(HANDLE hTimer, TimerInfo* timerInfo, Task timerTask){
		if (Timers.find(hTimerQueue) != Timers.end() &&
			Timers[hTimerQueue].find(hTimer) != Timers[hTimerQueue].end())
		{
			delete Timers[hTimerQueue][hTimer];
		}
		Timers[hTimerQueue][hTimer] = timerInfo;
		timerInfo->syncVar = timerTask;

		// a new timer is starting
		numNotDoneTimers++;
	}
	
	TimerInfo* GetTimerInfoFromHandle(HANDLE hTimer){
		assert(Timers.find(hTimerQueue) != Timers.end());
		assert(Timers[hTimerQueue].find(hTimer) != Timers[hTimerQueue].end());
		return Timers[hTimerQueue][hTimer];
	}

	BOOLEAN IsDeleted() const{
		return deleted;
	}

	BOOLEAN IsPending() const{
		return numPendingTimers != 0;
	}

	BOOLEAN IsDone() const{
		return numNotDoneTimers == 0;
	}

	void StartTimer(TimerInfo *timerInfo){
		timerInfo->Start();
		assert(timerInfo->IsPending());
		this->numPendingTimers++;
	}

	void FinishTimer(TimerInfo *timerInfo){
		assert(timerInfo->IsPending());
		timerInfo->Finish();
		assert(!timerInfo->IsPending());
		this->numPendingTimers--;
		if (this->numPendingTimers == 0 && this->deleted && this->CompletionEvent != NULL){
			__wrapper_SetEvent(this->CompletionEvent);
			this->CompletionEvent = NULL;
		}

	}

	void DoneTimer(TimerInfo *timerInfo){
		timerInfo->Done();
		assert(timerInfo->IsDone());
		this->numNotDoneTimers--;
	}

	static void DeleteTimer(TimerInfo* timerInfo, HANDLE hEvent) {
		timerInfo->Delete(hEvent);
	}

	void Delete(HANDLE hEvent){
		assert(!deleted);
		deleted = TRUE;
		stdext::hash_map< HANDLE, stdext::hash_map<HANDLE, TimerInfo*> >::iterator iter1;
		for (iter1 = Timers.begin(); iter1 != Timers.end(); iter1++)
		{
			stdext::hash_map<HANDLE, TimerInfo *> info = iter1->second;
			stdext::hash_map<HANDLE, TimerInfo *>::iterator iter2;
			for (iter2 = info.begin(); iter2 != info.end(); iter2++)
			{
				TimerInfo* timerInfo = iter2->second;
				if (!timerInfo->IsDeleted())
					DeleteTimer(timerInfo, NULL);
			}
		}
		if(hEvent != NULL && hEvent != INVALID_HANDLE_VALUE)
			this->CompletionEvent = hEvent;				
	}

};

stdext::hash_map< HANDLE, TimerQueueInfo* > TimerQueueInfo::TimerQueues;
stdext::hash_map< HANDLE, stdext::hash_map<HANDLE, TimerInfo*> > TimerQueueInfo::Timers;


void RWSOReset();

void Win32TimersReset(){
	TimerQueueInfo::Reset();
	RWSOReset();
}

struct TimerRoutineArg {
	WAITORTIMERCALLBACK Function; 
	PVOID lpParameter;
	DWORD DueTime;
	DWORD Period;
	TimerInfo *timerInfo;
	TimerQueueInfo *timerQueueInfo;
	Semaphore selfSemaphore;
	Semaphore parentSemaphore;
};

VOID CALLBACK TimerCreateWrapper(PVOID arg, BOOLEAN TimerOrWaitFired) {
	struct TimerRoutineArg* timerArgs = (struct TimerRoutineArg*)arg;
	WAITORTIMERCALLBACK Function = timerArgs->Function;
	PVOID lpParameter = timerArgs->lpParameter;
	DWORD DueTime = timerArgs->DueTime;
	DWORD Period = timerArgs->Period;
	TimerInfo *timerInfo = timerArgs->timerInfo;
	TimerQueueInfo *timerQueueInfo = timerArgs->timerQueueInfo;
	Semaphore sem = timerArgs->selfSemaphore;
	Semaphore parentSem = timerArgs->parentSemaphore;
	free(timerArgs);
	
	parentSem.Up(); //parent handshake
	GetWin32SyncManager()->ThreadBegin(sem);

	while(true){
		// Accesses the state of the timerInfo
		Chess::SyncVarAccess(timerInfo->GetSyncVar(), SVOP::RWVAR_READWRITE);	
		if (timerInfo->IsDeleted()){
			Chess::CommitSyncVarAccess();
			break;
		}
		timerQueueInfo->StartTimer(timerInfo);
		Chess::CommitSyncVarAccess();

		Chess::LeaveChess();
		(*Function)(lpParameter, TimerOrWaitFired);
		Chess::EnterChess();

		// Accesses the deleted bit of the timerQ and the state of the timerInfo
		std::vector<SyncVar> varvec;
		timerQueueInfo->GetAggregateSyncVar(timerInfo, varvec);
		SyncVar* timerQueueAgg = new SyncVar[varvec.size()];
		for(size_t i=0; i<varvec.size(); i++) timerQueueAgg[i] = varvec[i];
		
		Chess::AggregateSyncVarAccess(timerQueueAgg, varvec.size(), SVOP::RWVAR_READWRITE);
		Chess::CommitSyncVarAccess();
		delete[] timerQueueAgg;
		timerQueueInfo->FinishTimer(timerInfo);

		if(Period == 0){
			break;
		}
				
		Chess::TaskYield();
	}

	timerQueueInfo->DoneTimer(timerInfo);
	Chess::TaskEnd();
}

HANDLE WINAPI __wrapper_CreateTimerQueue(void)
{
	HANDLE retVal = CreateTimerQueue();
	ChessErrorSentry sentry;
	if (retVal != NULL)
	{
		TimerQueueInfo::CreateTimerQueueInfo(retVal);
	}
	return retVal;
}

BOOL WINAPI __wrapper_CreateTimerQueueTimer( 
	PHANDLE phNewTimer, 
	HANDLE TimerQueue, 
	WAITORTIMERCALLBACK Callback, 
	PVOID Parameter, 
	DWORD DueTime, 
	DWORD Period, 
	ULONG Flags )
{
	TimerQueueInfo* timerQueueInfo = TimerQueueInfo::GetTimerQueueInfo(TimerQueue);
#ifdef IGNORE_UNKNOWN_QUEUES
	if(timerQueueInfo == NULL){
		// Unknown calls should not arise here.. if they do, handle them properly
		Chess::AbnormalExit(-1, "Invalid TimerQueue Handle in CreateTimerQueueTimer");
		return false;
	}
#endif

	Chess::SyncVarAccess(timerQueueInfo->GetSyncVar(), SVOP::RWVAR_READWRITE);
	Chess::CommitSyncVarAccess();

	assert(!timerQueueInfo->IsDeleted());

	struct TimerRoutineArg* timerArgs = (struct TimerRoutineArg*) malloc(sizeof(struct TimerRoutineArg));
	if (!timerArgs) {
		Chess::AbnormalExit(-1, "out of memory");
        return false; // should never reach this
	}
	timerArgs->Function = Callback;
	timerArgs->lpParameter = Parameter;
	timerArgs->DueTime = DueTime;
	timerArgs->Period = Period;

	TimerInfo* timerInfo = timerQueueInfo->CreateTimerInfo();

	timerArgs->timerQueueInfo = timerQueueInfo;
	timerArgs->timerInfo = timerInfo;

	Semaphore childSem;
	childSem.Init();

	timerArgs->selfSemaphore = childSem;
	
	Semaphore handshake;
	handshake.Init();

	timerArgs->parentSemaphore = handshake;

	BOOL retVal = CreateTimerQueueTimer(phNewTimer, TimerQueue, TimerCreateWrapper, timerArgs, 0, 0, Flags );
	DWORD errorCode = GetLastError();
	if (retVal) 
	{
		Task child;
		Chess::TaskFork(child);
		handshake.Down();
		handshake.Clear(); 

		GetWin32SyncManager()->RegisterThreadSemaphore(child, childSem);
		timerQueueInfo->RegisterTimerHandle(*phNewTimer, timerInfo, child);

		Chess::ResumeTask(child);

	}
	else
	{
		handshake.Clear(); 
		childSem.Clear();
		timerQueueInfo->DeleteTimerInfo(timerInfo);
		free(timerArgs);
	}
	SetLastError(errorCode);
	return retVal;
}

BOOL WINAPI __wrapper_DeleteTimerQueueTimer(
	HANDLE queue,
	HANDLE timer,
	HANDLE CompletionEvent
	)
{
	TimerQueueInfo* timerQueueInfo = TimerQueueInfo::GetTimerQueueInfo(queue);
#ifdef IGNORE_UNKNOWN_QUEUES
	if(timerQueueInfo == NULL) {
		return DeleteTimerQueueTimer(queue, timer, CompletionEvent);
	}
#endif

	TimerInfo* timerInfo = timerQueueInfo->GetTimerInfoFromHandle(timer);
	assert(!timerInfo->IsDeleted());

	if(CompletionEvent == INVALID_HANDLE_VALUE){
		// Block till timer is not pending
		while(true){
			Chess::SyncVarAccess(timerInfo->GetSyncVar(), SVOP::RWVAR_READWRITE);
			if(!timerInfo->IsPending()){
				timerQueueInfo->DeleteTimer(timerInfo, CompletionEvent);
				Chess::CommitSyncVarAccess();
				break;
			}
			Chess::LocalBacktrack();
		}

		// Block till timer thread is done
		while(true){
			Chess::SyncVarAccess(timerInfo->GetSyncVar(), SVOP::WAIT_ANY);
			if(timerInfo->IsDone()){
				Chess::CommitSyncVarAccess();
				break;
			}
			Chess::LocalBacktrack();
		}
		//This should not block infinitely
		return DeleteTimerQueueTimer(queue, timer, INVALID_HANDLE_VALUE);
	}

	BOOL retVal = DeleteTimerQueueTimer(queue, timer, NULL);
	DWORD errorCode = GetLastError(); 
	if(!retVal && errorCode != ERROR_IO_PENDING)
		return retVal;

	timerQueueInfo->DeleteTimer(timerInfo, CompletionEvent);
	retVal = !timerInfo->IsPending();
	if(retVal){
		// Block till timer thread is done
		while(true){
			Chess::SyncVarAccess(timerInfo->GetSyncVar(), SVOP::WAIT_ANY);
			if(timerInfo->IsDone()){
				Chess::CommitSyncVarAccess();
				break;
			}
			Chess::LocalBacktrack();
		}
	}
	assert(retVal || errorCode == ERROR_IO_PENDING);
	SetLastError(errorCode);
	return retVal;
}	


BOOL WINAPI __wrapper_DeleteTimerQueueEx(
	HANDLE queue,
	HANDLE CompletionEvent
	)
{
	TimerQueueInfo* timerQueueInfo = TimerQueueInfo::GetTimerQueueInfo(queue);
#ifdef IGNORE_UNKNOWN_QUEUES
	if(timerQueueInfo == NULL) {
		return DeleteTimerQueueEx(queue, CompletionEvent);
	}
#endif
	assert(!timerQueueInfo->IsDeleted());

	if(CompletionEvent == INVALID_HANDLE_VALUE){\
		std::vector<SyncVar> varvec;
		timerQueueInfo->GetAggregateSyncVar(varvec);
		SyncVar* timerQueueAgg = new SyncVar[varvec.size()];
		for(size_t i=0; i<varvec.size(); i++) timerQueueAgg[i] = varvec[i];

		while(true){
			Chess::AggregateSyncVarAccess(timerQueueAgg, varvec.size(), SVOP::RWVAR_READWRITE);
			if(!timerQueueInfo->IsPending()){
				timerQueueInfo->Delete(CompletionEvent);
				Chess::CommitSyncVarAccess();
				break;
			}
			Chess::LocalBacktrack();
		}

		while(true){
			Chess::AggregateSyncVarAccess(timerQueueAgg, varvec.size(), SVOP::WAIT_ALL);
			if(timerQueueInfo->IsDone()){
				Chess::CommitSyncVarAccess();
				break;
			}
			Chess::LocalBacktrack();
		}
		delete[] timerQueueAgg;
		// should not block infinitely
		return DeleteTimerQueueEx(queue, INVALID_HANDLE_VALUE);
	}
	
	//SyncVar timerQueueAgg = timerQueueInfo->GetAggregateSyncVar();
	
	BOOL retVal = DeleteTimerQueueEx(queue, NULL);
	DWORD errorCode = GetLastError(); 
	if(!retVal && errorCode != ERROR_IO_PENDING)
		return retVal;
	
	timerQueueInfo->Delete(CompletionEvent);
	retVal = !timerQueueInfo->IsPending();
	assert(retVal || errorCode == ERROR_IO_PENDING);
	
	if(retVal){
		std::vector<SyncVar> varvec;
		timerQueueInfo->GetAggregateSyncVar(varvec);
		SyncVar* timerQueueAgg = new SyncVar[varvec.size()];
		for(size_t i=0; i<varvec.size(); i++) timerQueueAgg[i] = varvec[i];
		while(true){
			Chess::AggregateSyncVarAccess(timerQueueAgg, varvec.size(), SVOP::WAIT_ALL);
			if(timerQueueInfo->IsDone()){
				Chess::CommitSyncVarAccess();
				break;
			}
			Chess::LocalBacktrack();
		}
		delete[] timerQueueAgg;
	}

	SetLastError(errorCode);
	return retVal;
}

BOOL WINAPI __wrapper_DeleteTimerQueue(
									   HANDLE queue
									   )
{
	return __wrapper_DeleteTimerQueueEx(queue, NULL);
}

BOOL WINAPI __wrapper_ChangeTimerQueueTimer(
  HANDLE TimerQueue,
  HANDLE Timer,
  ULONG DueTime,
  ULONG Period
)
{
	fprintf(stderr, "ChangeTimerQueueTimer Not Implemented in CHESS (yet)\n");
	return ChangeTimerQueueTimer(TimerQueue, Timer, DueTime, Period);
}

HANDLE WINAPI __wrapper_CreateWaitableTimer(
  LPSECURITY_ATTRIBUTES lpTimerAttributes,
  BOOL bManualReset,
  LPCTSTR lpTimerName
)
{
	fprintf(stderr, "CreateWaitableTimer Not Implemented in CHESS (yet)\n");
#pragma warning( push )  
#pragma warning( disable: 25068)
	return CreateWaitableTimer(lpTimerAttributes, bManualReset, lpTimerName);
#pragma warning( pop )  
}


HANDLE WINAPI __wrapper_CreateWaitableTimerEx(
  LPSECURITY_ATTRIBUTES lpTimerAttributes,
  LPCTSTR lpTimerName,
  DWORD dwFlags,
  DWORD dwDesiredAccess
)
{
	fprintf(stderr, "CreateWaitableTimerEx Not Implemented in CHESS (yet)\n");
	//return CreateWaitableTimerEx(lpTimerAttributes, lpTimerName, dwFlags, dwDesiredAccess);
	return NULL;
}


BOOL WINAPI __wrapper_SetWaitableTimer(
  HANDLE hTimer,
  const LARGE_INTEGER* pDueTime,
  LONG lPeriod,
  PTIMERAPCROUTINE pfnCompletionRoutine,
  LPVOID lpArgToCompletionRoutine,
  BOOL fResume
)
{
	fprintf(stderr, "SetWaitableTimer Not Implemented in CHESS (yet)\n");
	return TRUE; 
		//SetWaitableTimer(hTimer, pDueTime, lPeriod, pfnCompletionRoutine, lpArgToCompletionRoutine, fResume);
}


BOOL WINAPI __wrapper_CancelWaitableTimer(
  HANDLE hTimer
)
{
	fprintf(stderr, "CancelWaitableTimer Not Implemented in CHESS (yet)\n");
	return CancelWaitableTimer(hTimer);
}

//logically RegisterWaitForSingleObject is similar to a timer
stdext::hash_map<HANDLE, bool> RWSODoneFlag;

void RWSOReset(){
	RWSODoneFlag.clear();
}

struct RWSOArgs{
	HANDLE hObject;
	WAITORTIMERCALLBACK Callback;
	PVOID Context;
	ULONG dwMilliseconds;
	ULONG dwFlags;
	HANDLE handle;
	Semaphore selfSemaphore;
};

VOID CALLBACK RWSOWrapper(PVOID lpParameter, BOOLEAN timer){
	RWSOArgs* args = (RWSOArgs*)lpParameter;

	HANDLE hObject = args->hObject;
	WAITORTIMERCALLBACK Callback = args->Callback;
	PVOID Context = args->Context;
	ULONG dwMilliseconds = args->dwMilliseconds;
	ULONG dwFlags = args->dwFlags;
	HANDLE handle = args->handle;
	Semaphore sem = args->selfSemaphore;
	delete args;

	GetWin32SyncManager()->ThreadBegin(sem);

	SyncVar vars[2];
	vars[0] = GetWin32SyncManager()->GetSyncVarFromHandle(handle);
	vars[1] = GetWin32SyncManager()->GetSyncVarFromHandle(hObject);
	//SyncVar agg(vars, 2);

	while(true){
		bool cbcalled = false;
		Chess::AggregateSyncVarAccess(vars, 2, SVOP::WAIT_ANY);
		DWORD ret = WaitForSingleObject(hObject, 0);
		switch(ret){
			case WAIT_OBJECT_0 :
				{
					Chess::CommitSyncVarAccess();
					Chess::LeaveChess();
					(*Callback)(Context, FALSE);
					cbcalled = true;
					Chess::EnterChess();
					break;
				}
			case WAIT_TIMEOUT :
				{
					if(RWSODoneFlag[handle]){
						Chess::CommitSyncVarAccess();
						goto done;
					}
					if(dwMilliseconds != INFINITE)
					{
						// we are treating finite blocks as blocks with zero timeout
						Chess::CommitSyncVarAccess();
						Chess::TaskYield();
						Chess::LeaveChess();
						(*Callback)(Context, TRUE);
						cbcalled = true;
						Chess::EnterChess();
						break;
					}
					Chess::LocalBacktrack();
					break;
				}
			case WAIT_ABANDONED :
				{
					Chess::CommitSyncVarAccess();
					fprintf(stderr, "WAIT_ABANDONED case not implemented in CHESS"); //really dont know what we should do here
					break;
				}
			default :
				{
					assert(false);
					Chess::CommitSyncVarAccess();
					goto done;
				}
		}
		if(cbcalled){
			if(dwFlags & WT_EXECUTEONLYONCE){
				goto done;
			}
		}
	}

done:
	Chess::TaskEnd();
}

BOOL
WINAPI
__wrapper_RegisterWaitForSingleObject(
    __deref_out PHANDLE phNewWaitObject,
    __in        HANDLE hObject,
    __in        WAITORTIMERCALLBACK Callback,
    __in_opt    PVOID Context,
    __in        ULONG dwMilliseconds,
    __in        ULONG dwFlags
    )
{
	if(!ChessWrapperSentry::Wrap("RegisterWaitForSingleObject")){
		return RegisterWaitForSingleObject(phNewWaitObject, hObject, Callback, Context, dwMilliseconds, dwFlags);
	}
	ChessWrapperSentry sentry;
	//return true;

	RWSOArgs* args = new RWSOArgs();
	args->hObject = hObject;
	args->Callback = Callback;
	args->Context = Context;
	args->dwMilliseconds = dwMilliseconds;
	args->dwFlags = dwFlags;

	Semaphore childSem;
	childSem.Init();
	args->selfSemaphore = childSem;

	HANDLE hSem = CreateSemaphoreW(NULL, 0, 1, NULL);
	if (!hSem) {
		Chess::AbnormalExit(-1, "could not create semaphore");
		return 0; // should never reach this
	}
	BOOL ret = RegisterWaitForSingleObject(phNewWaitObject, hSem, RWSOWrapper, args, INFINITE, WT_EXECUTEONLYONCE | WT_EXECUTELONGFUNCTION);
	ChessErrorSentry errorSentry;
	if(!ret)
		return ret;

	args->handle = *phNewWaitObject;
	RWSODoneFlag[args->handle] = false;

	Task child;
	Chess::TaskFork(child);
	GetWin32SyncManager()->RegisterThreadSemaphore(child, childSem);
	Chess::ResumeTask(child);
	
	LONG prev;
	ReleaseSemaphore(hSem, 1, &prev);


	return ret;
}

BOOL WINAPI __wrapper_UnregisterWait(
  HANDLE WaitHandle
)
{
	fprintf(stderr, "UnregisterWait not implemented in CHESS (yet)\n");
	return UnregisterWait(WaitHandle);
	//if(!ChessWrapperSentry::Wrap("UnregisterWait")){
	//	return UnregisterWait(WaitHandle);
	//}
	//ChessWrapperSentry sentry;

	//if(RWSODoneFlag.find(WaitHandle) == RWSODoneFlag.end()){
	//	return UnregisterWait(WaitHandle);
	//}

	//Chess::SyncVarAccess(GetWin32SyncManager()->GetSyncVarFromHandle(WaitHandle), SVOP::RWVAR_READWRITE);
	//RWSODoneFlag[WaitHandle] = true;
	

}
