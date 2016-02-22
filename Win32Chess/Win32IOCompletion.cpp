/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "Win32Base.h"
#include "ChessAssert.h"
#include "Chess.h"
#include "IChessWrapper.h"
#include "Win32Wrappers.h"
#include "Win32WrapperAPI.h"
#include "Win32SyncManager.h"

typedef struct {
	DWORD lpNumberOfBytes;
	ULONG_PTR lpCompletionKey;
	LPOVERLAPPED lpOverlapped;
	BOOL retVal;
} IoCompletionPortPacket;
stdext::hash_map<HANDLE, HANDLE> FileToIoCompletionPort;
stdext::hash_map<HANDLE, std::queue<IoCompletionPortPacket>> IoCompletionPortToQueue;

void Win32IOCompletionReset(){
	IoCompletionPortToQueue.clear();
	FileToIoCompletionPort.clear();
}


HANDLE WINAPI __wrapper_CreateIoCompletionPort( 
	HANDLE FileHandle, 
	HANDLE ExistingCompletionPort, 
	ULONG_PTR CompletionKey, 
	DWORD NumberOfConcurrentThreads 
	)
{
	HANDLE h = CreateIoCompletionPort(FileHandle, ExistingCompletionPort, CompletionKey, NumberOfConcurrentThreads);
	ChessErrorSentry sentry;
	if (h != NULL)
	{
		if (ExistingCompletionPort == NULL)
		{
			stdext::hash_map<HANDLE, std::queue<IoCompletionPortPacket>>::iterator t = 
				IoCompletionPortToQueue.find(h);
			if (t != IoCompletionPortToQueue.end()) {
				IoCompletionPortToQueue.erase(t);
			}
		}
		if (FileHandle != INVALID_HANDLE_VALUE)
		{
			// I assume that if FileHandle is already associated with another CompletionPort, 
			// then the call will fail and consequently return NULL.  Therefore, if we get here
			// it is because FileHandle was closed and reused.
			FileToIoCompletionPort[FileHandle] = h;
		}
	}

	return h;
}

BOOL WINAPI __wrapper_ReadFile( 
							   HANDLE hFile, 
							   LPVOID lpBuffer, 
							   DWORD nNumberOfBytesToRead, 
							   LPDWORD lpNumberOfBytesRead, 
							   LPOVERLAPPED lpOverlapped
							   )
{
	if (lpOverlapped == NULL)
		return ReadFile(hFile, lpBuffer, nNumberOfBytesToRead, lpNumberOfBytesRead, lpOverlapped);

	// Async Read File
	//   To control the nondeterminism, CHESS converts async read file into a synchronous read file
	//   XXX: This will not work if the test is using ReadFile/WriteFile to synchronize between threads

	HANDLE oldEvent = lpOverlapped->hEvent;
	HANDLE newEvent = CreateEventW(NULL, FALSE, FALSE, NULL);
	if(newEvent == NULL){
		Chess::AbnormalExit(-1, "Cannot create event in ReadFile wrapper");
		return false;
	}
	lpOverlapped->hEvent = (HANDLE) ((size_t)newEvent | 1);
	DWORD prevError = GetLastError();
	BOOL res = ReadFile(hFile, lpBuffer, nNumberOfBytesToRead, lpNumberOfBytesRead, lpOverlapped);
	DWORD errorCode = GetLastError();
	if (!res && errorCode == ERROR_IO_PENDING) 
	{
		// Since we convert an async operation to a synchronous one, 
		// call GetOverlappedResult() with bWait == TRUE
		DWORD junk;
		if(lpNumberOfBytesRead == 0){
			lpNumberOfBytesRead = &junk;
		}
		BOOL ret = GetOverlappedResult(hFile, lpOverlapped, lpNumberOfBytesRead, TRUE);
		if(ret == 0){
			errorCode = GetLastError();
			// since bWait is TRUE, we dont expect ERROR_IO_PENDING here
			assert(errorCode != ERROR_IO_PENDING);
			if(errorCode == ERROR_HANDLE_EOF){
				// Now, emulate the case of ReadFile returning EOF when the OS does the async call synchronously
				CloseHandle(newEvent);
				lpOverlapped->hEvent = oldEvent;
				SetLastError(errorCode);
				return FALSE;
			}
			else{
				*GetChessErrorStream()<< "CHESS tried making ReadFile asynchrnous, but GetOverLappedResult failed with " << errorCode << std::endl;
				assert(false);
			}
		}
//		WaitForSingleObject(newEvent, INFINITE);
		CloseHandle(newEvent);
		lpOverlapped->hEvent = oldEvent;
		SetLastError(prevError);
		return TRUE;
	}

	CloseHandle(newEvent);
	lpOverlapped->hEvent = oldEvent;
	SetLastError(errorCode);
	return res;
}

BOOL WINAPI __wrapper_WriteFile( 
								HANDLE hFile, 
								LPCVOID lpBuffer, 
								DWORD nNumberOfBytesToWrite, 
								LPDWORD lpNumberOfBytesWritten, 
								LPOVERLAPPED lpOverlapped 
								)
{
	if (lpOverlapped == NULL)
		return WriteFile(hFile, lpBuffer, nNumberOfBytesToWrite, lpNumberOfBytesWritten, lpOverlapped);

	// Async Write File
	//   To control the nondeterminism, CHESS converts async write file into a synchronous write file
	//   This will not work if the test is using ReadFile/WriteFile to synchronize between threads

	HANDLE oldEvent = lpOverlapped->hEvent;
	HANDLE newEvent = CreateEventW(NULL, FALSE, FALSE, NULL);
	if(newEvent == NULL){
		Chess::AbnormalExit(-1, "Cannot create event in ReadFile wrapper");
		return false;
	}
	lpOverlapped->hEvent = (HANDLE) ((size_t)newEvent | 1);
	DWORD prevError = GetLastError();
	BOOL res = WriteFile(hFile, lpBuffer, nNumberOfBytesToWrite, lpNumberOfBytesWritten, lpOverlapped);
	DWORD errorCode = GetLastError();
	if (!res && errorCode == ERROR_IO_PENDING) 
	{
		// Since we convert an async operation to a synchronous one, 
		// call GetOverlappedResult() with bWait == TRUE
		DWORD junk;
		if(lpNumberOfBytesWritten == 0){
			lpNumberOfBytesWritten = &junk;
		}
		BOOL ret = GetOverlappedResult(hFile, lpOverlapped, lpNumberOfBytesWritten, TRUE);
		if(ret == 0){
				*GetChessErrorStream()<< "CHESS tried making WriteFile asynchrnous, but GetOverLappedResult failed with " << GetLastError() << std::endl;
				assert(false);
		}
//		WaitForSingleObject(newEvent, INFINITE);
		CloseHandle(newEvent);
		lpOverlapped->hEvent = oldEvent;
		SetLastError(prevError);
		return TRUE;
	}

	CloseHandle(newEvent);
	lpOverlapped->hEvent = oldEvent;
	SetLastError(errorCode);
	return res;
}


BOOL WINAPI __wrapper_PostQueuedCompletionStatus( 
	HANDLE CompletionPort, 
	DWORD dwNumberOfBytesTransferred, 
	ULONG_PTR dwCompletionKey, 
	LPOVERLAPPED lpOverlapped )
{
	Chess::SyncVarAccess(GetWin32SyncManager()->GetSyncVarFromHandle(CompletionPort), SVOP::RWIOCP);

	BOOL res;
	res = PostQueuedCompletionStatus(CompletionPort, dwNumberOfBytesTransferred, dwCompletionKey, lpOverlapped);
	if (res)
	{
		IoCompletionPortPacket packet;
		BOOL retVal = GetQueuedCompletionStatus(CompletionPort, &packet.lpNumberOfBytes, 
			&packet.lpCompletionKey, &packet.lpOverlapped, INFINITE);
		assert(retVal);
		IoCompletionPortToQueue[CompletionPort].push(packet);
		SyncVar completionPortId = GetWin32SyncManager()->GetSyncVarFromHandle(CompletionPort);
	}
	Chess::CommitSyncVarAccess();
	return res;
}

BOOL WINAPI __wrapper_GetQueuedCompletionStatus( 
	HANDLE CompletionPort, 
	LPDWORD lpNumberOfBytesTransferred, 
	PULONG_PTR lpCompletionKey, 
	LPOVERLAPPED *lpOverlapped, 
	DWORD dwMilliseconds )
{
	assert(CompletionPort != NULL);
	BOOL retVal;
	while (true)
	{
		Chess::SyncVarAccess(GetWin32SyncManager()->GetSyncVarFromHandle(CompletionPort), SVOP::RWIOCP);
		bool flag = IoCompletionPortToQueue.find(CompletionPort) != IoCompletionPortToQueue.end() &&
			!IoCompletionPortToQueue[CompletionPort].empty();
		if (flag)
		{
			IoCompletionPortPacket packet = IoCompletionPortToQueue[CompletionPort].front();
			IoCompletionPortToQueue[CompletionPort].pop();
			BOOL res = PostQueuedCompletionStatus(CompletionPort, packet.lpNumberOfBytes, 
				packet.lpCompletionKey, packet.lpOverlapped);
			assert(res);
			retVal = GetQueuedCompletionStatus(CompletionPort, lpNumberOfBytesTransferred, 
				lpCompletionKey, lpOverlapped, INFINITE);
			assert(retVal);
			Chess::CommitSyncVarAccess();
			return retVal;
		}
		if (dwMilliseconds == INFINITE)
		{
			Chess::LocalBacktrack();
			continue;
		}

		// This call is guaranteed to timeout
		retVal = GetQueuedCompletionStatus(CompletionPort, lpNumberOfBytesTransferred, 
				lpCompletionKey, lpOverlapped, 0);
		Chess::CommitSyncVarAccess();
		Chess::TaskYield();
		return retVal;
	}
	assert(false);
	return FALSE;
}
