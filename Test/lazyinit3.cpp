/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>


CRITICAL_SECTION lock;

int x = 0;

DWORD WINAPI foo(LPVOID param) {
  for(int i=0; i<10; i++){
	EnterCriticalSection(&lock);
	LeaveCriticalSection(&lock);
  }
  return 0;
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){

	DWORD tid;
	HANDLE hThread;

	InitializeCriticalSection(&lock);

	if(x != 0 && x % 5 < 3){
	  // attempting to fail lazy initialization recovery in CHESS
	  for(int i=0; i<x%5; i++){
	    EnterCriticalSection(&lock);
	    LeaveCriticalSection(&lock);
	  }
	}


	hThread = CreateThread(NULL, 0, foo, NULL, 0, &tid);
	EnterCriticalSection(&lock);
	    x++;
	LeaveCriticalSection(&lock);

	WaitForSingleObject(hThread, INFINITE);
	CloseHandle(hThread);

	DeleteCriticalSection(&lock);
	return 0;
}
