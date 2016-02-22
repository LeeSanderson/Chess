/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>


volatile LONG x;
volatile LONG y;

DWORD WINAPI foo(LPVOID p) {
  p = 0;
  InterlockedIncrement(&y);
  return 0;
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){

  DWORD tid;
  HANDLE hThread;

  hThread = CreateThread(NULL, 0, foo, NULL, 0, &tid);
  
  InterlockedIncrement(&x);

  WaitForSingleObject(hThread, INFINITE);
  CloseHandle(hThread);
  return 0;
}
