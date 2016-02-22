/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>

CRITICAL_SECTION lock;

int x = 0;


DWORD WINAPI ChildThread (LPVOID param) {
  EnterCriticalSection(&lock);
  x = x + 1;
  LeaveCriticalSection(&lock);
  return 0;
}


const int NUM_CHILDREN = 4;

extern "C"
__declspec(dllexport) int ChessTestRun() {
  DWORD tid;
  HANDLE hThread[NUM_CHILDREN];

  InitializeCriticalSection(&lock);

  for(int i=0; i<NUM_CHILDREN; i++){
    hThread[i] = CreateThread(NULL, 0, ChildThread, NULL, 0, &tid);
  }
  
  WaitForMultipleObjects(NUM_CHILDREN, hThread, true, INFINITE);

  bool testSuccess = (x == NUM_CHILDREN);

  // Reset global state and free/cleanup any resources used
  x = 0;

  for(int i=0; i<NUM_CHILDREN; i++){
    CloseHandle(hThread[i]);
  }
  DeleteCriticalSection(&lock);

  // The test successfully brings the final state to be logically
  // equivalent to the initial state. This allows CHESS to
  // repeatedly run ChessTestRun() in a loop

  return testSuccess ? 0 : -1;
}

