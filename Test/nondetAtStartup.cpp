/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>
#include <iostream>
#include <assert.h>

CRITICAL_SECTION lock;
int y;

HANDLE hBgthread = NULL;
volatile int poolDone = 0;

ULONG ioKey;
OVERLAPPED doneOverlapped;
OVERLAPPED jobOverlapped;

DWORD WINAPI poolthread(LPVOID p){
  HANDLE hIOCPort = (HANDLE)p;
  while(true){
    DWORD nrBytes;
    ULONG_PTR pKey;
    LPOVERLAPPED got;

    GetQueuedCompletionStatus(hIOCPort, &nrBytes, &pKey, &got, INFINITE);
    if(got == & doneOverlapped)
      break;

    EnterCriticalSection(&lock);
    y++;
    LeaveCriticalSection(&lock);
  }
  return 0;
}

const int NUM_POOL_THREADS = 2;
HANDLE hpool[NUM_POOL_THREADS];
HANDLE hevent[NUM_POOL_THREADS];
HANDLE hIOCPort = NULL;

extern "C" 
__declspec(dllexport) int ChessTestStartup(int argc, char** argv){
  InitializeCriticalSection(&lock);
  srand(GetTickCount());
  if(rand()%2){
    EnterCriticalSection(&lock);
    LeaveCriticalSection(&lock);
  }
    
  hIOCPort = CreateIoCompletionPort(
		INVALID_HANDLE_VALUE,  // FileHandle - not associated with a file handler
		NULL,                  // ExistingCompletionPort - must be NULL if not assoc'd with a file handler
		(ULONG_PTR)&ioKey,                  // CompletionKey
		NUM_POOL_THREADS             // NumberOfConcurrentThreads
		);

  for(int i=0; i<NUM_POOL_THREADS; i++){
    hpool[i] = CreateThread(NULL, 0, poolthread, hIOCPort, 0, NULL);
  }
  return 1;
}

const int NUM_JOBS_PER_THREAD = 1;

extern "C" 
__declspec(dllexport) int ChessTestRun(){
  for(int i=0; i<NUM_JOBS_PER_THREAD; i++){
    for(int j=0; j<NUM_POOL_THREADS; j++){
      PostQueuedCompletionStatus(hIOCPort, 0, (ULONG_PTR)&ioKey, &jobOverlapped); 
    }
  }
  return 0;
}

extern "C" 
__declspec(dllexport) void ChessTestShutdown(){
  poolDone = 1;
    for(int j=0; j<NUM_POOL_THREADS; j++){
      PostQueuedCompletionStatus(hIOCPort, 0, (ULONG_PTR)&ioKey, &doneOverlapped); 
    }
  WaitForMultipleObjects(NUM_POOL_THREADS, hpool, true, INFINITE);
  DeleteCriticalSection(&lock);
}
