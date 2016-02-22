/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>
#include <stdio.h>
#include <..\Chess\chessApi.h>

CRITICAL_SECTION lock1;
CRITICAL_SECTION lock2;


DWORD WINAPI foo(LPVOID param) {

  EnterCriticalSection(&lock1);
  EnterCriticalSection(&lock2);

  LeaveCriticalSection(&lock2);
  LeaveCriticalSection(&lock1);

  return 0;
}

extern "C" 
__declspec(dllexport) bool __stdcall ChessOnErrorCallback(int exitCode, char* details){
	printf("CHESS returned with errror code %d (%s)\n", exitCode, GetChessExitCodeString(exitCode));
	printf("%s\n", details);
	return false;
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){

  DWORD tid;
  HANDLE hThread;

  InitializeCriticalSection(&lock1);
  InitializeCriticalSection(&lock2);

  hThread = CreateThread(NULL, 0, foo, NULL, 0, &tid);
  
  EnterCriticalSection(&lock2);
  EnterCriticalSection(&lock1);

  LeaveCriticalSection(&lock1);
  LeaveCriticalSection(&lock2);

  WaitForSingleObject(hThread, INFINITE);
  CloseHandle(hThread);

  DeleteCriticalSection(&lock1);
  DeleteCriticalSection(&lock2);
  return 0;
}
