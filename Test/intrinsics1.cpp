/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>
#include <intrin.h>
volatile long counter = 0;

DWORD WINAPI foo(LPVOID param) {

  _InterlockedIncrement(&counter);

  return 0;
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){

  DWORD tid;
  HANDLE hThread;

  hThread = CreateThread(NULL, 0, foo, NULL, 0, &tid);
  
  _InterlockedIncrement(&counter);


  WaitForSingleObject(hThread, INFINITE);
  CloseHandle(hThread);
  return 0;
}
