/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

/*
    Testing mutexes: CHESS should perform three executions.
*/
#include <windows.h>
#include <iostream>

HANDLE hMutex1; 
HANDLE hMutex2; 

DWORD WINAPI foo(LPVOID param) {

  UNREFERENCED_PARAMETER(param);

  ReleaseMutex(hMutex1);
  WaitForSingleObject(hMutex2, INFINITE);

  return 0;
}
using namespace std;

extern "C" 
__declspec(dllexport) int ChessTestRun(){

  DWORD tid;
  HANDLE hThread;

  hMutex1 = CreateMutex( 
	  NULL,                       // default security attributes
	  TRUE,                      // initially owned
	  NULL);                      // unnamed mutex

  hMutex2 = CreateMutex( 
	  NULL,                       // default security attributes
	  TRUE,                      // initially owned
	  NULL);                      // unnamed mutex

  hThread = CreateThread(NULL, 0, foo, NULL, 0, &tid);

  WaitForSingleObject(hMutex1, INFINITE);

  ReleaseMutex(hMutex2);

  WaitForSingleObject(hThread, INFINITE);

  CloseHandle(hThread);
  CloseHandle(hMutex1);
  CloseHandle(hMutex2);
  return 0;
}
