/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>


CRITICAL_SECTION xlock;
CRITICAL_SECTION ylock;
int x = 0;
int y = 0;

DWORD WINAPI foo1(LPVOID param) {
	EnterCriticalSection(&ylock);
	y = 1;
	LeaveCriticalSection(&ylock);
	return 0;
}

DWORD WINAPI foo2(LPVOID param) {
	EnterCriticalSection(&ylock);
	while(y == 0){
		Sleep(1);
		LeaveCriticalSection(&ylock);
		EnterCriticalSection(&ylock);
	}
	LeaveCriticalSection(&ylock);

	EnterCriticalSection(&xlock);
	x = 1;
	LeaveCriticalSection(&xlock);
	return 0;
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){


	InitializeCriticalSection(&xlock);
	InitializeCriticalSection(&ylock);
	x = 0;
	y = 0;

	DWORD tid1;
	HANDLE hThread1 = CreateThread(NULL, 0, foo1, NULL, 0, &tid1);
	DWORD tid2;
	HANDLE hThread2 = CreateThread(NULL, 0, foo2, NULL, 0, &tid2);

	EnterCriticalSection(&xlock);
	while(x == 0){
		LeaveCriticalSection(&xlock);
		Sleep(1);
		EnterCriticalSection(&xlock);
	}
	LeaveCriticalSection(&xlock);

	WaitForSingleObject(hThread1, INFINITE);
	WaitForSingleObject(hThread2, INFINITE);
	CloseHandle(hThread1);
	CloseHandle(hThread2);

	DeleteCriticalSection(&xlock);
	DeleteCriticalSection(&ylock);
	return 0;
}
