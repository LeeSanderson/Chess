/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

/*
Testing repeatability in the presence of many threads.
*/

#define _WIN32_WINNT 0x0520
#include <windows.h>
#include <iostream>

#define NR_THREADS 4

using namespace std;

DWORD WINAPI foo(LPVOID param) {
	UNREFERENCED_PARAMETER(param);
	return 0;
}


extern "C" 
__declspec(dllexport) int ChessTestRun(){

	HANDLE hThreads[NR_THREADS];
	DWORD tid;

	for (int i = 0; i < NR_THREADS; i++) {
		hThreads[i] = CreateThread(NULL, 0, foo, NULL, 0, &tid);
	}

	for (int i = 0; i < NR_THREADS; i++) {
		WaitForSingleObject(hThreads[i], INFINITE);
		CloseHandle(hThreads[i]);
	}
	return 0;
}
