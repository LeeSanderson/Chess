/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>


CRITICAL_SECTION lock;

int x;

DWORD WINAPI foo(LPVOID param) {

  x = 1;
  EnterCriticalSection(&lock);
  LeaveCriticalSection(&lock);
  x = 1;

  return 0;
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){
  DWORD tid;
  HANDLE hThread;
  HANDLE dup;

  InitializeCriticalSection(&lock);

  hThread = CreateThread(NULL, 0, foo, NULL, 0, &tid);
  DuplicateHandle(GetCurrentProcess(), hThread, GetCurrentProcess(), &dup, 0, FALSE, DUPLICATE_SAME_ACCESS);
  
  x = 1;
  EnterCriticalSection(&lock);
  LeaveCriticalSection(&lock);
  x = 1;

  WaitForSingleObject(dup, INFINITE);
  CloseHandle(hThread);
  CloseHandle(dup);

  DeleteCriticalSection(&lock);
  return 0;
}

extern "C" __declspec(dllexport) DWORD WINAPI RunTest2(LPVOID p) {
  return ChessTestRun();
}
