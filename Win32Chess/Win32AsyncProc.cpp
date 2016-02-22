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
#include "SyncVarVector.h"

class AsyncProcedure{
private:
	bool enabledFlag;
protected:
	void Enable(){ enabledFlag = true; }

public:
	AsyncProcedure()
		: enabledFlag(false){}
	virtual ~AsyncProcedure(){}
	bool IsEnabled() const {return enabledFlag;}
	virtual void Execute(){}
};

struct AsyncQueue{
	AsyncQueue(){
		syncVarId = 0;
	}
	std::queue<AsyncProcedure*> asyncQueue;
	SyncVar syncVarId;
};

TaskVector<AsyncQueue> ThreadToAsyncQueue;

SyncVar AsyncQueueSyncVar(Task tid){
	//if (ThreadToAsyncQueue.find(tid) == ThreadToAsyncQueue.end()){
	//	ThreadToAsyncQueue[tid].syncVarId = GetWin32SyncManager()->GetNewSyncVar();
	//}
	if(ThreadToAsyncQueue[tid].syncVarId == 0)
		ThreadToAsyncQueue[tid].syncVarId = GetWin32SyncManager()->GetNewSyncVar();
	return ThreadToAsyncQueue[tid].syncVarId;
}

void FlushAsyncQueue(Task tid){
	while(!ThreadToAsyncQueue[tid].asyncQueue.empty()){
		AsyncProcedure* async = ThreadToAsyncQueue[tid].asyncQueue.front();
		delete async;
		ThreadToAsyncQueue[tid].asyncQueue.pop();
	}
}

//void Win32AsyncProcReset(){
//	for(size_t i=0; i<ThreadToAsyncQueue.size();i++){
//		if(ThreadToAsyncQueue[i].syncVarId != 0)
//			FlushAsyncQueue(i);
//	}
//	//stdext::hash_map<Task, AsyncQueue>::iterator i;
//	//for(i = ThreadToAsyncQueue.begin(); i!= ThreadToAsyncQueue.end(); i++){
//	//	FlushAsyncQueue(i->first);
//	//}
//	ThreadToAsyncQueue.clear();
//}

//void Win32AsyncProcThreadEnd(Task tid){
//	if(ThreadToAsyncQueue[tid].syncVarId != 0)
//		FlushAsyncQueue(tid);
//	//if(ThreadToAsyncQueue.find(tid) != ThreadToAsyncQueue.end())
//	//	FlushAsyncQueue(tid);
//}


void ExecuteAsyncProcedures(Task tid){
	AsyncQueue& tidQ = ThreadToAsyncQueue[tid];
	if(tidQ.syncVarId == 0){
		return;
	}
	if(tidQ.asyncQueue.empty())
		return;

	/*stdext::hash_map<Task, AsyncQueue>::iterator i = ThreadToAsyncQueue.find(tid);
	if(i == ThreadToAsyncQueue.end())
		return;

	AsyncQueue& tidQ = i->second;
	if(tidQ.asyncQueue.empty())
		return;*/

//	Chess::ThreadReadWriteSyncVar(tidQ.syncVarId);

	while(!tidQ.asyncQueue.empty()){
		AsyncProcedure* async = tidQ.asyncQueue.front();
		while(!async->IsEnabled()){
			// async procedures need to be executed in FIFO order to guarantee determinism
			// thus we need to wait for this async proc to get enabled
			SleepEx(INFINITE, true);
		}
		tidQ.asyncQueue.pop();

		ChessWrapperSentry sentry;
		async->Execute();

		delete async;
	}	
}

class APC : public AsyncProcedure{
private:
	PAPCFUNC pfnAPC;
	ULONG_PTR dwData;

public:
	APC(PAPCFUNC f, ULONG_PTR d)
		: pfnAPC(f), dwData(d){}

	void Complete(){
		Enable();
	}
	void Execute(){
		pfnAPC(dwData);
	}
};

VOID APIENTRY APCWrapperFunction(__in ULONG_PTR dwParam)
{
	APC* apc = (APC*) dwParam;
	apc->Complete();
}

DWORD WINAPI __wrapper_QueueUserAPC( PAPCFUNC pfnAPC, HANDLE hThread, ULONG_PTR dwData ) 
{
	if(!ChessWrapperSentry::Wrap("QueueUserAPC")){
		return QueueUserAPC(pfnAPC, hThread, dwData);
	}
	ChessWrapperSentry sentry;

	Task tid = GetWin32SyncManager()->GetTid(hThread);

	Chess::SyncVarAccess(AsyncQueueSyncVar(tid), SVOP::RWAPCQ);
	APC* apc = new APC(pfnAPC, dwData);
	DWORD retVal = QueueUserAPC(APCWrapperFunction, hThread, (ULONG_PTR)apc);
	ChessErrorSentry errsentry;

	Chess::CommitSyncVarAccess();

	if (retVal == 0){
		delete apc;
		return retVal;
	}

	ThreadToAsyncQueue[tid].asyncQueue.push(apc);
	//Chess::ThreadReadWriteSyncVar(ThreadToAsyncQueue[tid].syncVarId);
	//Chess::EnableThread(tid);
	return retVal; // success
}

class AsyncIO : public AsyncProcedure{
private:
	DWORD dwErrorCode;
	DWORD dwNumberOfBytesTransfered;
	LPOVERLAPPED lpOverlapped;
	LPOVERLAPPED_COMPLETION_ROUTINE completionRoutine;

	bool encoded;
	HANDLE hEvent;

public:
	AsyncIO(LPOVERLAPPED_COMPLETION_ROUTINE fn)
		: completionRoutine(fn), encoded(false) {}

	void Complete(DWORD err, DWORD numBytes, LPOVERLAPPED ovlp){
		assert(!encoded);
		dwErrorCode = err;
		dwNumberOfBytesTransfered = numBytes;
		lpOverlapped = ovlp;
		this->Enable();
	}

	void Execute(){
		completionRoutine(dwErrorCode, dwNumberOfBytesTransfered, lpOverlapped);
	}

	static void EncodePtr(LPOVERLAPPED lpOverlapped, AsyncIO* aio){
		aio->encoded = true;
		aio->lpOverlapped = lpOverlapped;
		aio->hEvent = lpOverlapped->hEvent;
		lpOverlapped->hEvent = (HANDLE)aio;
	}

	static AsyncIO* DecodePtr(LPOVERLAPPED lpOverlapped){
		AsyncIO* aio = (AsyncIO*)lpOverlapped->hEvent;
		assert(aio->encoded);
		assert(aio->lpOverlapped == lpOverlapped);
		lpOverlapped->hEvent = aio->hEvent;
		aio->encoded = false;
		return aio;
	}
};

VOID CALLBACK AsyncIOCompletionWrapper(
									 DWORD dwErrorCode,
									 DWORD dwNumberOfBytesTransfered,
									 LPOVERLAPPED lpOverlapped
									 )
{
	AsyncIO* aio = AsyncIO::DecodePtr(lpOverlapped);
	aio->Complete(dwErrorCode, dwNumberOfBytesTransfered, lpOverlapped);
}

BOOL WINAPI __wrapper_ReadFileEx( 
								 HANDLE hFile, 
								 LPVOID lpBuffer, 
								 DWORD nNumberOfBytesToRead, 
								 LPOVERLAPPED lpOverlapped, 
								 LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine 
								 )
{
	if(!ChessWrapperSentry::Wrap("ReadFileEx")){
		return ReadFileEx(hFile, lpBuffer, nNumberOfBytesToRead, lpOverlapped, lpCompletionRoutine);
	}
	ChessWrapperSentry sentry;

	assert(lpCompletionRoutine != NULL);

	Task tid = Chess::GetCurrentTid();

	//Chess::ThreadSchedule();
	Chess::SyncVarAccess(AsyncQueueSyncVar(tid), SVOP::RWAPCQ);

	AsyncIO* asyncIO = new AsyncIO(lpCompletionRoutine);
	AsyncIO::EncodePtr(lpOverlapped, asyncIO);

	BOOL retVal = ReadFileEx(hFile, lpBuffer, nNumberOfBytesToRead, lpOverlapped, AsyncIOCompletionWrapper);
	ChessErrorSentry errsentry;

	Chess::CommitSyncVarAccess();
	
	if (retVal == 0){
		//failure
		const AsyncIO* ret = AsyncIO::DecodePtr(lpOverlapped);
		assert(ret == asyncIO);
		delete asyncIO;
		return retVal;
	}

	// success
	ThreadToAsyncQueue[tid].asyncQueue.push(asyncIO);
	//	Chess::ThreadReadWriteSyncVar(ThreadToAsyncQueue[tid].syncVarId);
	//Chess::EnableThread(tid);
	return retVal; 
}

BOOL WINAPI __wrapper_WriteFileEx( 
								  HANDLE hFile, 
								  LPCVOID lpBuffer, 
								  DWORD nNumberOfBytesToWrite, 
								  LPOVERLAPPED lpOverlapped, 
								  LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine 
								  )
{
	if(!ChessWrapperSentry::Wrap("WriteFileEx")){
		return WriteFileEx(hFile, lpBuffer, nNumberOfBytesToWrite, lpOverlapped, lpCompletionRoutine);
	}
	ChessWrapperSentry sentry;

	assert(lpCompletionRoutine != NULL);

	Task tid = Chess::GetCurrentTid();

//	Chess::ThreadSchedule();
	Chess::SyncVarAccess(AsyncQueueSyncVar(tid), SVOP::RWAPCQ);

	AsyncIO* asyncIO = new AsyncIO(lpCompletionRoutine);
	AsyncIO::EncodePtr(lpOverlapped, asyncIO);

	BOOL retVal = WriteFileEx(hFile, lpBuffer, nNumberOfBytesToWrite, lpOverlapped, AsyncIOCompletionWrapper);
	ChessErrorSentry errsentry;

	Chess::CommitSyncVarAccess();

	if (retVal == 0){
		//failure
		const AsyncIO* ret = AsyncIO::DecodePtr(lpOverlapped);
		assert(ret == asyncIO);
		delete asyncIO;
		return retVal;
	}

	// success
	ThreadToAsyncQueue[tid].asyncQueue.push(asyncIO);
//	Chess::ThreadReadWriteSyncVar(ThreadToAsyncQueue[tid].syncVarId);
	//Chess::EnableThread(tid);
	return retVal; 
}

WrapperFunctionInfo AsyncWrapperFunctions[] = {
	WrapperFunctionInfo(QueueUserAPC, __wrapper_QueueUserAPC, "kernel32::QueueUserAPC"),
	WrapperFunctionInfo(ReadFileEx, __wrapper_ReadFileEx, "kernel32::ReadFileEx"),
	WrapperFunctionInfo(WriteFileEx, __wrapper_WriteFileEx, "kernel32::WriteFileEx"),
	WrapperFunctionInfo()
};

class AsyncProcWrapper : public IChessWrapper{
public:
	virtual WrapperFunctionInfo* GetWrapperFunctions(){return AsyncWrapperFunctions;}

	virtual void SetInitState(){
		for(size_t i=0; i< ThreadToAsyncQueue.size(); i++){
			if(ThreadToAsyncQueue[i].syncVarId != 0 && ThreadToAsyncQueue[i].asyncQueue.size() != 0){
				Chess::AbnormalExit(-1, "CHESS does not allow unflushed APCs at the initial state");
			}
		}
	}

	virtual void Reset(){
		for(size_t i=0; i<ThreadToAsyncQueue.size();i++){
			if(ThreadToAsyncQueue[i].syncVarId != 0)
				FlushAsyncQueue(i);
		}
		ThreadToAsyncQueue.clear();
	}	

	virtual void TaskEnd(Task tid){
		if(ThreadToAsyncQueue[tid].syncVarId != 0)
			FlushAsyncQueue(tid);
	}
};

IChessWrapper* GetAsyncProcWrapper(){
	return new AsyncProcWrapper();
}
