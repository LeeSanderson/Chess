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

  DWORD tid;
  HANDLE hThread;

  hThread = CreateThread(NULL, 0, foo, NULL, 0, &tid);
  
  InterlockedIncrement(&counter);


  WaitForSingleObject(hThread, INFINITE);
  CloseHandle(hThread);
  return 0;
}
