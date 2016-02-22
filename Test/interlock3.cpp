/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>

volatile long counter = 0;

DWORD WINAPI foo(LPVOID param) {

  InterlockedIncrement(&counter);

  return 0;
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){

  DWORD tid1, tid2;
  HANDLE hThread1, hThread2;

  hThread1 = CreateThread(NULL, 0, foo, NULL, 0, &tid1);
  hThread2 = CreateThread(NULL, 0, foo, NULL, 0, &tid2);
  
  InterlockedIncrement(&counter);


  WaitForSingleObject(hThread1, INFINITE);
  WaitForSingleObject(hThread2, INFINITE);

  CloseHandle(hThread1);
  CloseHandle(hThread2);
  return 0;
}
