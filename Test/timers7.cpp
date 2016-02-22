/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

/*
Testing timers.
*/

#define _WIN32_WINNT 0x0520
#include <windows.h>
#include <iostream>  
#include <assert.h> 

#define NR_THREADS 2 

 
using namespace std;

VOID CALLBACK TimerRoutine(PVOID lpParam, BOOLEAN TimerOrWaitFired)
{
	UNREFERENCED_PARAMETER(TimerOrWaitFired);

	HANDLE * he = (HANDLE *)lpParam;

	SetEvent(*he);
}


extern "C" 
__declspec(dllexport) int ChessTestRun(){

	HANDLE hEvent[NR_THREADS];
	HANDLE hTimer[NR_THREADS];

	HANDLE hTimerQueue;
	

	hTimerQueue = CreateTimerQueue();

	for (int i = 0; i < NR_THREADS; i++) {

		hEvent[i] = CreateEvent(NULL, FALSE, FALSE, NULL);

		CreateTimerQueueTimer( &hTimer[i], hTimerQueue, 
			(WAITORTIMERCALLBACK)TimerRoutine, (PVOID)&hEvent[i] , 5000 * i, 0, WT_EXECUTELONGFUNCTION);
	}
	
	
	if (!DeleteTimerQueueEx(hTimerQueue, INVALID_HANDLE_VALUE)) 
	{
		assert(false);
	}


	for (int i = 0; i < NR_THREADS; i++) {
		CloseHandle(hEvent[i]);
	}

	return 0;
}
