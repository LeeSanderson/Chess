/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>


CRITICAL_SECTION lock;
CRITICAL_SECTION lock2;


DWORD WINAPI foo(LPVOID a){
  return (DWORD)a;
}

int x = 0;


extern "C" 
__declspec(dllexport) int ChessTestRun(){

	InitializeCriticalSection(&lock);
	InitializeCriticalSection(&lock2);

	EnterCriticalSection(&lock);
	HANDLE hThread = CreateThread(NULL, 0, foo, NULL, 0, NULL);

	if(x == 0){
	  EnterCriticalSection(&lock2);
	  LeaveCriticalSection(&lock2);
	}
	x++;

	WaitForSingleObject(hThread, INFINITE);
	LeaveCriticalSection(&lock);

	DeleteCriticalSection(&lock);
	DeleteCriticalSection(&lock2);
	return 0;
}
