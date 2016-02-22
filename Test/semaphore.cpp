/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

/*
    Testing semaphores: CHESS should perform X executions.
*/
#include <windows.h>
#include <iostream>

HANDLE hSem1; 
HANDLE hSem2; 

DWORD WINAPI foo(LPVOID param) {

  UNREFERENCED_PARAMETER(param);

  ReleaseSemaphore(hSem1, 1, NULL);
  WaitForSingleObject(hSem2, INFINITE);

  return 0;
}
using namespace std;

extern "C" 
__declspec(dllexport) int ChessTestRun(){

  DWORD tid;
  HANDLE hThread;

  hSem1 = CreateSemaphore(
	  NULL,   // default security attributes
	  0,   // initial count
	  100,   // maximum count
	  NULL);  // unnamed semaphore

  hSem2 = CreateSemaphore(
	  NULL,   // default security attributes
	  0,   // initial count
	  100,   // maximum count
	  NULL);  // unnamed semaphore


  hThread = CreateThread(NULL, 0, foo, NULL, 0, &tid);

  WaitForSingleObject(hSem1, INFINITE);

  ReleaseSemaphore(hSem2, 1, NULL);

  WaitForSingleObject(hThread, INFINITE);

  CloseHandle(hThread);
  CloseHandle(hSem1);
  CloseHandle(hSem2);
  return 0;
}
