/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>


CRITICAL_SECTION lock;

int x;

DWORD WINAPI foo(LPVOID param) {

  EnterCriticalSection(&lock);
  x = 1;
  LeaveCriticalSection(&lock);

  return 0;
}

bool first = true;

const int NUM_THREADS = 3;

extern "C" 
__declspec(dllexport) int ChessTestRun(){
  HANDLE hThread[NUM_THREADS];

  if(!first)
    DeleteCriticalSection(&lock); // deleting cs from previous iteration
  first = false;
  InitializeCriticalSection(&lock);

  for(int i=0; i<NUM_THREADS; i++){
    hThread[i] = CreateThread(NULL, 0, foo, NULL, 0, NULL);
  }

  // Wait for everyone except the first
  WaitForMultipleObjects(NUM_THREADS-1, hThread+1, TRUE, INFINITE);

  for(int i=0; i<NUM_THREADS; i++){
    CloseHandle(hThread[i]);
  }

  return 0;
}
