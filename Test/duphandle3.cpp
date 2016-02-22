/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>


CRITICAL_SECTION lock;

int x;
HANDLE dup;

DWORD WINAPI foo(LPVOID param) {
  param = 0;
  int t = 0;
  EnterCriticalSection(&lock);
  t = x++;
  LeaveCriticalSection(&lock);
  
  if(t == 0){
     DuplicateHandle(GetCurrentProcess(), GetCurrentThread(), GetCurrentProcess(), &dup, 0, false, DUPLICATE_SAME_ACCESS);
  } 
  else{
     while(dup == 0){
        Sleep(0);
     }
    WaitForSingleObject(dup, INFINITE);
    CloseHandle(dup);
    dup = 0;
  }
  return 0;
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){
  HANDLE hThread1;
 HANDLE hThread2;

  InitializeCriticalSection(&lock);
  x = 0;
  dup = 0;

  hThread1 = CreateThread(NULL, 0, foo, NULL, 0, NULL);
  hThread2 = CreateThread(NULL, 0, foo, NULL, 0, NULL);
  

  WaitForSingleObject(hThread1, INFINITE);
  WaitForSingleObject(hThread2, INFINITE);

  CloseHandle(hThread1);
  CloseHandle(hThread2);

  DeleteCriticalSection(&lock);
  return 0;
}

extern "C" __declspec(dllexport) DWORD WINAPI RunTest2(LPVOID p) {
  return ChessTestRun();
}
