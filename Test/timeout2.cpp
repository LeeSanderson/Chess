/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>


CRITICAL_SECTION lock;

int x = 0;

DWORD WINAPI foo(LPVOID param) {
	EnterCriticalSection(&lock);
	while(x == 0){
		LeaveCriticalSection(&lock);
		Sleep(1);
		EnterCriticalSection(&lock);
	}
	LeaveCriticalSection(&lock);
	return 0;
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){

	DWORD tid;
	HANDLE hThread;

	InitializeCriticalSection(&lock);
	x = 0;
	hThread = CreateThread(NULL, 0, foo, NULL, 0, &tid);

	EnterCriticalSection(&lock);
	x = 1;
	LeaveCriticalSection(&lock);

	WaitForSingleObject(hThread, INFINITE);
	CloseHandle(hThread);

	DeleteCriticalSection(&lock);
	return 0;
}
