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

Win32SyncManager* GetWin32SyncManager(){
	return (Win32SyncManager*) Chess::GetSyncManager();
}

//void Win32AsyncProcReset();	
//void Win32IOCompletionReset();
//void Win32TimersReset();
//void Win32CriticalSectionReset();
//void Win32SRWLockReset();
//void Win32ConditionVariableReset();
//void HeapStatusReset();

//void WrappersReset()
//{
//	Win32AsyncProcReset();
//	Win32IOCompletionReset();
//	Win32TimersReset();
//	Win32CriticalSectionReset();
//	Win32SRWLockReset();
//	Win32ConditionVariableReset();
//
//	HeapStatusReset();
//}

#ifndef UNDER_CE
void ExecuteAsyncProcedures(Task tid);

void WrappersOnAlertableState(Task tid){
	ExecuteAsyncProcedures(tid);
}
#endif


bool IsInfiniteTimeout(DWORD dwMilliseconds){
	return dwMilliseconds == INFINITE || 
		Chess::GetOptions().infinite_timeout_bound && dwMilliseconds >= (DWORD)Chess::GetOptions().infinite_timeout_bound;
}

DWORD WINAPI ThreadCreateWrapper(LPVOID arg) {
	DWORD retVal;
	struct ThreadRoutineArg* threadArgs = (struct ThreadRoutineArg*)arg;
	LPTHREAD_START_ROUTINE Function = threadArgs->Function;
	PVOID Context = threadArgs->Context;
	Semaphore sem = threadArgs->selfSemaphore;
	free(threadArgs);
	
	GetWin32SyncManager()->ThreadBegin(sem);

	Chess::LeaveChess();
	retVal = (*Function)(Context);
	Chess::EnterChess();

	GetWin32SyncManager()->ThreadEnd();

	return retVal;
}

VOID WINAPI __wrapper_ExitThread(DWORD dwExitCode) {

	Chess::EnterChess();
	GetWin32SyncManager()->ThreadEnd();

	ExitThread(dwExitCode);
}

VOID WINAPI __wrapper_FreeLibraryAndExitThread(
  HMODULE hModule,
  DWORD dwExitCode
  )
{
	if(!ChessWrapperSentry::Wrap("FreeLibraryAndExitThread")){
		FreeLibraryAndExitThread(hModule, dwExitCode);		
	}
	ChessWrapperSentry sentry;

	Chess::EnterChess();
	GetWin32SyncManager()->ThreadEnd();
	FreeLibraryAndExitThread(hModule, dwExitCode);
}


HANDLE WINAPI __wrapper_CreateThread(
									 LPSECURITY_ATTRIBUTES lpThreadAttributes,
									 SIZE_T dwStackSize,
									 LPTHREAD_START_ROUTINE lpStartAddress,
									 LPVOID lpParameter,
									 DWORD dwCreationFlags,
									 LPDWORD lpThreadId)
{
	HANDLE retVal;

	struct ThreadRoutineArg* threadArgs = (struct ThreadRoutineArg*) malloc(sizeof(struct ThreadRoutineArg));
	if (!threadArgs) {
		Chess::AbnormalExit(-1, "out of memory");
        return 0; // should never reach this
	}
	threadArgs->Function = lpStartAddress;
	threadArgs->Context = lpParameter;

	Semaphore childSem;
	childSem.Init();

	threadArgs->selfSemaphore = childSem;

	retVal = CreateThread(lpThreadAttributes, dwStackSize, ThreadCreateWrapper, threadArgs, dwCreationFlags, lpThreadId);
	ChessErrorSentry sentry;
	if (retVal != NULL)
	{
		Task child; 
		Chess::TaskFork(child);
		GetWin32SyncManager()->RegisterThreadSemaphore(child, childSem, TRUE);
		GetWin32SyncManager()->AddChildHandle(child, retVal);
		if ((dwCreationFlags & CREATE_SUSPENDED) == 0){
			Chess::ResumeTask(child);
		}
		else{
			// Chess::TaskFork() by default creates a suspended task
		}
	}
	else{
		threadArgs->selfSemaphore.Clear();
		free(threadArgs);
	}
	return retVal;
}

#ifndef UNDER_CE
BOOL WINAPI __wrapper_QueueUserWorkItem(
										LPTHREAD_START_ROUTINE Function,
										PVOID Context,
										ULONG Flags)
{
	BOOL retVal;

	struct ThreadRoutineArg* threadArgs = (struct ThreadRoutineArg*) malloc(sizeof(struct ThreadRoutineArg));
	if (!threadArgs) {
		Chess::AbnormalExit(-1, "out of memory");
        return false; // should never reach this
	}
	threadArgs->Function = Function;
	threadArgs->Context = Context;

	Semaphore childSem;
	childSem.Init();

	threadArgs->selfSemaphore = childSem;

	retVal = QueueUserWorkItem(ThreadCreateWrapper, threadArgs, Flags);
	ChessErrorSentry sentry;
	if (retVal){
		// success
		Task child;
		Chess::TaskFork(child);
		GetWin32SyncManager()->RegisterThreadSemaphore(child, childSem);
		Chess::ResumeTask(child);
	}
	else{
		threadArgs->selfSemaphore.Clear();
		free(threadArgs);
	}
	return retVal;
}
#endif

DWORD WINAPI __wrapper_SuspendThread(
									 HANDLE hThread) 
{
	DWORD ret = SuspendThread(hThread);
	ChessErrorSentry sentry;
	if(ret == 0){
		// hThread just got suspended
		Chess::SuspendTask(GetWin32SyncManager()->GetSyncVarFromHandle(hThread)/*.ToTask()*/);
	}
	return ret;
}

DWORD WINAPI __wrapper_ResumeThread(
									HANDLE hThread)
{
	DWORD retVal;

	retVal = ResumeThread(hThread);
	ChessErrorSentry sentry;
	if (retVal == 1){
		Chess::ResumeTask(GetWin32SyncManager()->GetSyncVarFromHandle(hThread)/*.ToTask()*/);
	}
	return retVal;
}

class CHESS_API StackWalkHelper;
//#include "StackWalkHelper.h"
BOOL WINAPI __wrapper_SetEvent(HANDLE hEvent)
{
	BOOL res;
	Chess::SyncVarAccess(GetWin32SyncManager()->GetSyncVarFromHandle(hEvent), SVOP::RWEVENT);
	res = SetEvent(hEvent);
	Chess::CommitSyncVarAccess();
	return res;
}

BOOL WINAPI __wrapper_ResetEvent( HANDLE hEvent )
{
	BOOL res;
	Chess::SyncVarAccess(GetWin32SyncManager()->GetSyncVarFromHandle(hEvent), SVOP::RWEVENT);
	res = ResetEvent(hEvent);
	Chess::CommitSyncVarAccess();
	return res;
}

HANDLE WINAPI __wrapper_CreateEventA(
  LPSECURITY_ATTRIBUTES lpEventAttributes,
  BOOL bManualReset,
  BOOL bInitialState,
  LPCSTR lpName
){
	  HANDLE ret = CreateEventA(lpEventAttributes, bManualReset, bInitialState, lpName);
	  if(ret != NULL && lpName != NULL){
		  // this HANDLE is possibly a duplicate of an existing event. So associate this handle with its name
		  GetWin32SyncManager()->AssociateNamedHandle(lpName, ret);
	  }
	  return ret;
}
HANDLE WINAPI __wrapper_CreateEventW(
  LPSECURITY_ATTRIBUTES lpEventAttributes,
  BOOL bManualReset,
  BOOL bInitialState,
  LPCWSTR lpName
){
	  HANDLE ret = CreateEventW(lpEventAttributes, bManualReset, bInitialState, lpName);
	  if(ret != NULL && lpName != NULL){
		  // this HANDLE is possibly a duplicate of an existing event. So associate this handle with its name
		  GetWin32SyncManager()->AssociateNamedHandle(lpName, ret);
	  }
	  return ret;
}

HANDLE WINAPI __wrapper_CreateMutexA(
  LPSECURITY_ATTRIBUTES lpEventAttributes,
  BOOL bInitialOwner,
  LPCSTR lpName
){
	  HANDLE ret = CreateMutexA(lpEventAttributes, bInitialOwner, lpName);
	  if(ret != NULL && lpName != NULL){
		  // this HANDLE is possibly a duplicate of an existing event. So associate this handle with its name
		  GetWin32SyncManager()->AssociateNamedHandle(lpName, ret);
	  }
	  return ret;
}

HANDLE WINAPI __wrapper_CreateMutexW(
  LPSECURITY_ATTRIBUTES lpEventAttributes,
  BOOL bInitialOwner,
  LPCWSTR lpName
){
	  HANDLE ret = CreateMutexW(lpEventAttributes, bInitialOwner, lpName);
	  if(ret != NULL && lpName != NULL){
		  // this HANDLE is possibly a duplicate of an existing event. So associate this handle with its name
		  GetWin32SyncManager()->AssociateNamedHandle(lpName, ret);
	  }
	  return ret;
}

HANDLE WINAPI __wrapper_OpenMutexA(
	DWORD dwDesiredAccess,
	BOOL bInheritHandle,
	LPCSTR lpName
){
	  HANDLE ret = OpenMutexA(dwDesiredAccess, bInheritHandle, lpName);
	  if(ret != NULL && lpName != NULL){
		  // this HANDLE is possibly a duplicate of an existing event. So associate this handle with its name
		  GetWin32SyncManager()->AssociateNamedHandle(lpName, ret);
	  }
	  return ret;
}

HANDLE WINAPI __wrapper_OpenMutexW(
	DWORD dwDesiredAccess,
	BOOL bInheritHandle,
	LPCWSTR lpName
){
	  HANDLE ret = OpenMutexW(dwDesiredAccess, bInheritHandle, lpName);
	  if(ret != NULL && lpName != NULL){
		  // this HANDLE is possibly a duplicate of an existing event. So associate this handle with its name
		  GetWin32SyncManager()->AssociateNamedHandle(lpName, ret);
	  }
	  return ret;
}

/* PulseEvent is deprecated and unreliable; abort on call */
BOOL WINAPI __wrapper_PulseEvent( HANDLE hEvent ){
#ifndef UNDER_CE
	assert(false);
	return FALSE;
#else
    BOOL res;
    Chess::SyncVarAccess(GetWin32SyncManager()->GetSyncVarFromHandle(hEvent), SVOP::RWEVENT);
    res = PulseEvent(hEvent);
    Chess::CommitSyncVarAccess();
    return res;
#endif
}

LONG WINAPI __wrapper_InterlockedIncrement(__inout LONG volatile *lpAddend)
{
	Chess::SyncVarAccess(GetWin32SyncManager()->GetSyncVarFromAddress((void *)lpAddend), SVOP::RWVAR_READWRITE);
	LONG ret = InterlockedIncrement(lpAddend);
	Chess::CommitSyncVarAccess();
	return ret;
}

LONG WINAPI __wrapper_InterlockedDecrement(__inout LONG volatile *lpAddend)
{
	Chess::SyncVarAccess(GetWin32SyncManager()->GetSyncVarFromAddress((void *)lpAddend), SVOP::RWVAR_READWRITE);
	LONG ret = InterlockedDecrement(lpAddend);
	Chess::CommitSyncVarAccess();
	return ret;
}

LONG WINAPI __wrapper_InterlockedExchange(__inout LONG volatile *Target, __in LONG Value)
{
	Chess::SyncVarAccess(GetWin32SyncManager()->GetSyncVarFromAddress((void *)Target), SVOP::RWVAR_READWRITE);
	LONG ret = InterlockedExchange(Target, Value);
	Chess::CommitSyncVarAccess();
	return ret;
}

LONG WINAPI __wrapper_InterlockedCompareExchange(
	__inout LONG volatile *Destination, 
	__in LONG Exchange, 
	__in LONG Comperand)
{
	Chess::SyncVarAccess(GetWin32SyncManager()->GetSyncVarFromAddress((void *)Destination), SVOP::RWVAR_READWRITE);
	LONG ret = InterlockedCompareExchange(Destination, Exchange, Comperand);
	Chess::CommitSyncVarAccess();
	return ret;
}

BOOL WINAPI __wrapper_ReleaseMutex( HANDLE hMutex ){

	BOOL res;
//	Chess::ThreadSchedule();
	Chess::SyncVarAccess(GetWin32SyncManager()->GetSyncVarFromHandle(hMutex), SVOP::RWVAR_READWRITE);
	res = ReleaseMutex(hMutex);
	ChessErrorSentry sentry;
	Chess::CommitSyncVarAccess();
	return res;
}

BOOL WINAPI __wrapper_ReleaseSemaphore( HANDLE hSemaphore, LONG lReleaseCount, LPLONG lpPreviousCount ){
	BOOL res;

//	Chess::ThreadSchedule();
	Chess::SyncVarAccess(GetWin32SyncManager()->GetSyncVarFromHandle(hSemaphore), SVOP::RWVAR_READWRITE);
	res = ReleaseSemaphore(hSemaphore, lReleaseCount, lpPreviousCount);
	ChessErrorSentry sentry;
	Chess::CommitSyncVarAccess();
	return res;
}

//void WINAPI __wrapper_Sleep(DWORD dwMilliseconds)
//{
//	__wrapper_SleepEx(dwMilliseconds, FALSE);
//}
//
//SyncVar AsyncQueueSyncVar(Task tid);
//
//DWORD WINAPI __wrapper_SleepEx( DWORD dwMilliseconds, BOOL bAlertable ){
//	assert(dwMilliseconds != INFINITE || bAlertable);
//	//Chess::ThreadSchedule();
//	SyncVar accessVar = bAlertable ? AsyncQueueSyncVar(Chess::GetCurrentTid()) : SyncVarUnknownId;
//	Chess::SyncVarAccess(accessVar, SVOP::RWVAR_READWRITE);
//
//	DWORD retVal;
//	while(true){
//		retVal = SleepEx(0, bAlertable);
//		if(retVal == 0 && dwMilliseconds == INFINITE){
//			Chess::LocalBacktrack();
//			continue;
//		}
//		// Either Async functions were called or we timedout
//		assert(retVal == 0 || retVal == WAIT_IO_COMPLETION);
//		if (retVal == 0)
//		{
//			Chess::TaskYield();
//		}
//		else{
//			//assert(retVal == WAIT_IO_COMPLETION);
//			WrappersOnAlertableState(Chess::GetCurrentTid());
//		}
//		return retVal;
//	}
//	assert(false);
//	return retVal;
//}

void WINAPI __wrapper_Sleep(DWORD dwMilliseconds)
{
	if(dwMilliseconds == INFINITE){
		fprintf(stderr, "Call to Infinite Sleep");
	}
	Chess::TaskYield();
}

SyncVar AsyncQueueSyncVar(Task tid);

#ifndef UNDER_CE
DWORD WINAPI __wrapper_SleepEx( DWORD dwMilliseconds, BOOL bAlertable ){
	if(!bAlertable){
		__wrapper_Sleep(dwMilliseconds);
		return 0;
	}

	assert(bAlertable);		
	//Chess::ThreadSchedule();
	SyncVar accessVar = AsyncQueueSyncVar(Chess::GetCurrentTid());

	DWORD retVal;
	while(true){
		Chess::SyncVarAccess(accessVar, SVOP::RWVAR_READWRITE);
		retVal = SleepEx(0, bAlertable);
		ChessErrorSentry sentry;
		if(retVal == 0 && IsInfiniteTimeout(dwMilliseconds)){
			Chess::LocalBacktrack();
			continue;
		}
		Chess::CommitSyncVarAccess();
		// Either Async functions were called or we timedout
		assert(retVal == 0 || retVal == WAIT_IO_COMPLETION);
		if (retVal == 0)
		{
			Chess::TaskYield();
		}
		else{
			//assert(retVal == WAIT_IO_COMPLETION);
			WrappersOnAlertableState(Chess::GetCurrentTid());
		}
		return retVal;
	}
	assert(false);
	return retVal;
}
#endif

BOOL WINAPI __wrapper_SwitchToThread( void ){
	Chess::TaskYield();
	//replace this with a nondeterminstic choice(2)
	return true;

}

DWORD WINAPI __wrapper_WaitForSingleObjectEx( HANDLE hHandle, DWORD dwMilliseconds, BOOL bAlertable ){
	DWORD retVal;

	SyncVar vars[2];
	int n = 1;
	vars[0] = GetWin32SyncManager()->GetSyncVarFromHandle(hHandle);
#ifndef UNDER_CE
	if(bAlertable){
		vars[1] = AsyncQueueSyncVar(Chess::GetCurrentTid());
		n = 2;
	}
#endif
	while(true){
		Chess::AggregateSyncVarAccess(vars, n, SVOP::WAIT_ANY);
#ifndef UNDER_CE
		retVal = WaitForSingleObjectEx(hHandle, 0, bAlertable);
#else
        retVal = WaitForSingleObject(hHandle, 0);
#endif
		ChessErrorSentry sentry;
		switch(retVal){
			case WAIT_OBJECT_0 : 
				Chess::CommitSyncVarAccess();
				//Chess::ThreadReadWriteSyncVar(GetWin32SyncManager()->GetSyncVarFromHandle(hHandle));
				return retVal;
			
			case WAIT_TIMEOUT :
				if(!IsInfiniteTimeout(dwMilliseconds))
				{
					Chess::MarkTimeout();
					Chess::CommitSyncVarAccess();
					// we are treating finite blocks as blocks with zero timeout
					Chess::TaskYield();
					return retVal;
				}
				Chess::LocalBacktrack();
				break;	
#ifndef UNDER_CE			
			case WAIT_IO_COMPLETION :
				Chess::CommitSyncVarAccess();
				assert(bAlertable);
				WrappersOnAlertableState(Chess::GetCurrentTid());
				return retVal;
#endif

			case WAIT_ABANDONED :
				Chess::CommitSyncVarAccess();
				fprintf(stderr, "WAIT_ABANDONED case not implemented in CHESS"); //really dont know what we should do here
				return retVal;

			default:
				Chess::CommitSyncVarAccess();
				// error condition
				return retVal;
		}
	}
	assert(false);
	return retVal;
}

DWORD WINAPI __wrapper_WaitForSingleObject(
	HANDLE hHandle,
	DWORD dwMilliseconds)
{
	return __wrapper_WaitForSingleObjectEx(hHandle, dwMilliseconds, false);
}


DWORD WINAPI __wrapper_WaitForMultipleObjectsEx(
	DWORD nCount,
	const HANDLE* lpHandles,
	BOOL fWaitAll,
	DWORD dwMilliseconds,
	BOOL bAlertable
	)
{
	DWORD retVal;

	int n = bAlertable ? nCount+1 : nCount;
	SyncVar* vars = new SyncVar[n];
	for(DWORD i=0; i<nCount; i++){
		vars[i] = GetWin32SyncManager()->GetSyncVarFromHandle(lpHandles[i]);
	}
#ifndef UNDER_CE
	if(bAlertable){
		vars[nCount] = AsyncQueueSyncVar(Chess::GetCurrentTid());
	}
#endif
	//SyncVar agg(vars, n);
	//delete[] vars;


	while(true){
		Chess::AggregateSyncVarAccess(vars, n, fWaitAll ? SVOP::WAIT_ALL : SVOP::WAIT_ANY);
#ifndef UNDER_CE
		retVal = WaitForMultipleObjectsEx(nCount, lpHandles, fWaitAll, 0, bAlertable);
#else
        retVal = WaitForMultipleObjects(nCount, lpHandles, fWaitAll, 0);
#endif
		ChessErrorSentry sentry;
		
		if(WAIT_OBJECT_0 <= retVal && retVal < WAIT_OBJECT_0+nCount){

			/// FIX: to only commit certain syncvars
			Chess::CommitSyncVarAccess();
			// At least one handle was signaled
			//*GetChessErrorStream()<< "WaitForMultipleObjects: " << retVal - WAIT_OBJECT_0 << std::endl;
			DWORD index = retVal - WAIT_OBJECT_0;
			break;
		}

		if(WAIT_ABANDONED_0 <= retVal && retVal < WAIT_ABANDONED_0+nCount){
			Chess::CommitSyncVarAccess();
			fprintf(stderr, "WAIT_ABANDONED case not implemented in CHESS"); //really dont know what we should do here
			break;
		}

		if(retVal == WAIT_TIMEOUT){
			if (!IsInfiniteTimeout(dwMilliseconds))
			{
				Chess::MarkTimeout();
				Chess::CommitSyncVarAccess();
				Chess::TaskYield();
				break;
			}
			Chess::LocalBacktrack();
			continue;
		}
		
#ifndef UNDER_CE
		if(retVal == WAIT_IO_COMPLETION){
			Chess::CommitSyncVarAccess();
			assert(bAlertable);
			WrappersOnAlertableState(Chess::GetCurrentTid());
		}
#endif
		break;
	}
	delete[] vars;
	return retVal;
}

DWORD WINAPI __wrapper_WaitForMultipleObjects(
	DWORD nCount, 
	CONST HANDLE* lpHandles, 
	BOOL fWaitAll, 
	DWORD dwMilliseconds) 
{
	return __wrapper_WaitForMultipleObjectsEx(nCount, lpHandles, fWaitAll, dwMilliseconds, false);
}

#ifndef UNDER_CE
DWORD WINAPI __wrapper_SignalObjectAndWait( 
	HANDLE hObjectToSignal, 
	HANDLE hObjectToWaitOn, 
	DWORD dwMilliseconds, 
	BOOL bAlertable )
{
	// SignalObjectAndWait(h1,h2, x, y) is equivalent to
	//     r = SignalObjectAndWait(h1, h2, 0, y);
	//     if(r == WAIT_TIMEOUT && x == INFINITE) 
	//         WaitForSingleObject(h2, x, y);
	//

	SyncVar vars[3];
	int n = 2;
	vars[0] = GetWin32SyncManager()->GetSyncVarFromHandle(hObjectToSignal);
	vars[1] = GetWin32SyncManager()->GetSyncVarFromHandle(hObjectToWaitOn);
	if(bAlertable){
		n = 3;
		vars[2] = AsyncQueueSyncVar(Chess::GetCurrentTid());
	}

//	SyncVar agg(vars, n);
	Chess::AggregateSyncVarAccess(vars, n, SVOP::RWVAR_READWRITE);
	DWORD retVal;
	retVal = SignalObjectAndWait(hObjectToSignal, hObjectToWaitOn, 0, bAlertable);
	ChessErrorSentry sentry;
	Chess::CommitSyncVarAccess();
	if(retVal != WAIT_TIMEOUT){
		if(retVal == WAIT_IO_COMPLETION){
			WrappersOnAlertableState(Chess::GetCurrentTid());
		}
		return retVal;
	}

	if(!IsInfiniteTimeout(dwMilliseconds)){
		Chess::TaskYield();
		return retVal;
	}
	
	sentry.Clear();
	return __wrapper_WaitForSingleObjectEx(hObjectToWaitOn, dwMilliseconds, bAlertable);
}
#endif

BOOL WINAPI __wrapper_DuplicateHandle( 
									  HANDLE hSourceProcessHandle, 
									  HANDLE hSourceHandle, 
									  HANDLE hTargetProcessHandle, 
									  LPHANDLE lpTargetHandle, 
									  DWORD dwDesiredAccess, 
									  BOOL bInheritHandle, 
									  DWORD dwOptions )
{
	BOOL res = DuplicateHandle(hSourceProcessHandle, hSourceHandle, hTargetProcessHandle, lpTargetHandle, dwDesiredAccess, bInheritHandle, dwOptions);
	ChessErrorSentry sentry;
	GetWin32SyncManager()->DuplicateHandle(hSourceHandle, *lpTargetHandle);
	return res;
}



extern "C"
__declspec(dllexport) void Win32ChessSyncVarAccess(void* addr, int size, bool isWrite, bool isRead, int pcId){
	Chess::EnterChess();
	if(size <= 4){
		if(size < 4){
			// approximate for now
			size = 4;
			addr = (void*)(((int)addr) & (~(0x3)));
		}
		SyncVar var = GetWin32SyncManager()->GetSyncVarFromAddress(addr);
		SyncVarOp op = SVOP::UNKNOWN;
		if(isRead){
			op = isWrite? SVOP::RWVAR_READWRITE : SVOP::RWVAR_READ;
		}
		else if(isWrite){
			op = SVOP::RWVAR_WRITE;
		}
		Chess::SyncVarAccess(var, op);
		Chess::CommitSyncVarAccess();
	}
	else{
		assert(size % 4 == 0);
		for(int i=0; i<size; i+=4){
			Win32ChessSyncVarAccess(((int*)addr)+i, 4, isWrite, isRead, pcId);
		}
	}
	Chess::LeaveChess();
}