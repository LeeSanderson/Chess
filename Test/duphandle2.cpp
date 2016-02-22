/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>


CRITICAL_SECTION lock;

int x;
HANDLE hChildDup;

DWORD WINAPI foo(LPVOID param) {

  DuplicateHandle(GetCurrentProcess(), GetCurrentThread(), GetCurrentProcess(), &hChildDup, 0, FALSE, DUPLICATE_SAME_ACCESS);
  EnterCriticalSection(&lock);
  x = 1;
  LeaveCriticalSection(&lock);

  return 0;
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){
  DWORD tid;
  HANDLE hThread;

  InitializeCriticalSection(&lock);
  x = 0;

  hThread = CreateThread(NULL, 0, foo, NULL, 0, &tid);
  
  while(true){
    int t ;
  EnterCriticalSection(&lock);
    t = x;
  LeaveCriticalSection(&lock);
   if(t == 1) break;
   Sleep(1);
  }

  WaitForSingleObject(hChildDup, INFINITE);
  CloseHandle(hThread);
  CloseHandle(hChildDup);

  DeleteCriticalSection(&lock);
  return 0;
}

extern "C" __declspec(dllexport) DWORD WINAPI RunTest2(LPVOID p) {
  return ChessTestRun();
}
