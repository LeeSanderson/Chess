/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>
#include <assert.h>

CRITICAL_SECTION gLock;
int x;
int done;

static DWORD WINAPI ChessT1(void * data) {
  EnterCriticalSection(&gLock); 
  done=1;
  LeaveCriticalSection(&gLock);
  
  return 0;
}

static DWORD WINAPI ChessT2(void * data) {
  EnterCriticalSection(&gLock);
  while (!done) {
	  LeaveCriticalSection(&gLock);
	  Sleep(1);
	  EnterCriticalSection(&gLock);
  }
  LeaveCriticalSection(&gLock);
  x = 1;
  
  return 0;
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){
  HANDLE hThreads[2];
  DWORD tid[2];

  x = 0;
  done = 0;
  InitializeCriticalSection(&gLock);
  
  hThreads[0] = CreateThread(NULL, 0, ChessT1, NULL, 0, &(tid[0]));
  hThreads[1] = CreateThread(NULL, 0, ChessT2, NULL, 0, &(tid[1]));
  WaitForMultipleObjects(2, hThreads, true, INFINITE);
  assert(x == 1);
  CloseHandle(hThreads[0]);
  CloseHandle(hThreads[1]);
  DeleteCriticalSection(&gLock);
  return 0;
}

