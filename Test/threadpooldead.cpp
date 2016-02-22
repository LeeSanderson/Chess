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


DWORD WINAPI poolthread(LPVOID p){
  HANDLE event = (HANDLE)p;
  while(!poolDone){
    DWORD ret = WaitForSingleObject(event, INFINITE);
    assert(ret == WAIT_OBJECT_0);

    EnterCriticalSection(&lock);
    y++;
    LeaveCriticalSection(&lock);
  }
  return 0;
}

const int NUM_POOL_THREADS = 2;
HANDLE hpool[NUM_POOL_THREADS];
HANDLE hevent[NUM_POOL_THREADS];

extern "C" 
__declspec(dllexport) int ChessTestStartup(int argc, char** argv){
  p = 0;
  InitializeCriticalSection(&lock);
  for(int i=0; i<NUM_POOL_THREADS; i++){
    hevent[i] = CreateEvent(NULL, false, false, NULL);
    DWORD tid;
    hpool[i] = CreateThread(NULL, 0, poolthread, hevent[i], 0, &tid);
  }
  return 0;
}

const int NUM_JOBS_PER_THREAD = 1;

extern "C" 
__declspec(dllexport) int ChessTestRun(){
  p = 0;
  for(int i=0; i<NUM_JOBS_PER_THREAD; i++){
    for(int j=0; j<NUM_POOL_THREADS; j++){
      SetEvent(hevent[j]);
    }
  }
  return 0;
}

extern "C" 
__declspec(dllexport) void ChessTestShutdown(){
  poolDone = 1;
  WaitForMultipleObjects(NUM_POOL_THREADS, hpool, true, INFINITE);
  DeleteCriticalSection(&lock);
  return 0;
}
