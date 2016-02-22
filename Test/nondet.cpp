/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>


CRITICAL_SECTION lock;
CRITICAL_SECTION lock2;

int x = 0;

DWORD WINAPI foo(LPVOID param) {
	EnterCriticalSection(&lock);
	x = 1;
	LeaveCriticalSection(&lock);
	return 0;
}

int nondcounter = 0;
const int nondetFrequency = 2;
const int numIterations = nondetFrequency*3-1;

void AccessNondetLock(){
	LPCRITICAL_SECTION lpCs = &lock;
	if(((nondcounter++)%nondetFrequency) == 0){
		lpCs = &lock2;
	}
	EnterCriticalSection(lpCs);
	x ++;
	LeaveCriticalSection(lpCs);
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){

	DWORD tid;
	HANDLE hThread;

	InitializeCriticalSection(&lock);
	InitializeCriticalSection(&lock2);

	x = 0;
	hThread = CreateThread(NULL, 0, foo, NULL, 0, &tid);
	for(int i=0; i<numIterations; i++)
		AccessNondetLock();

	WaitForSingleObject(hThread, INFINITE);
	CloseHandle(hThread);

	DeleteCriticalSection(&lock);
	DeleteCriticalSection(&lock2);
	return 0;
}
