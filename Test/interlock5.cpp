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
  HANDLE ha[2];


  ha[0] = CreateThread(NULL, 0, foo, NULL, 0, &tid1);
  ha[1] = CreateThread(NULL, 0, foo, NULL, 0, &tid2);
  
  InterlockedIncrement(&counter);


  WaitForMultipleObjects(2, ha, true, INFINITE);
  
  CloseHandle(ha[0]);
  CloseHandle(ha[1]);
  return 0;
}
