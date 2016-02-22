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

#define NR_THREADS 3


using namespace std;

VOID CALLBACK TimerRoutine(PVOID lpParam, BOOLEAN TimerOrWaitFired)
{
	UNREFERENCED_PARAMETER(TimerOrWaitFired);

	HANDLE * he = (HANDLE *)lpParam;

	//	cout << "TimerRoutine: releasing event at " << lpParam << "...";
	SetEvent(*he);
	//	cout << "done" << endl;
}


extern "C" 
__declspec(dllexport) int ChessTestRun(){

	HANDLE hEvent[NR_THREADS];
	HANDLE hTimer[NR_THREADS];

	HANDLE hTimerQueue;
	

	hTimerQueue = CreateTimerQueue();

	for (int i = 0; i < NR_THREADS; i++) {

		hEvent[i] = CreateEvent( 
			NULL,         // default security attributes
			TRUE,         // manual-reset event
			FALSE,        // initial state is unsignaled
			NULL); 

		//cout << "Creating timer " << i << " with event at " << &hEvent[i] << endl;

		CreateTimerQueueTimer( &hTimer[i], hTimerQueue, 
			(WAITORTIMERCALLBACK)TimerRoutine, (PVOID)&hEvent[i] , 5000 * i, 0, WT_EXECUTELONGFUNCTION);
	}

	for (int i = 0; i < NR_THREADS; i++) {
	  //	cout << "Main thread: waiting for event " << i << endl;
		WaitForSingleObject(hEvent[i], INFINITE);
	}

	for (int i = 0; i < NR_THREADS; i++) {
		CloseHandle(hEvent[i]);
	}

	if (!DeleteTimerQueueEx(hTimerQueue, INVALID_HANDLE_VALUE)) 
	{
		cout << "DeleteTimerQueueEx failed: " << GetLastError() << endl;
		//assert(false);
		//DebugBreak();
	}
	//else
	//cout << "DeleteTimerQueueEx OK" << endl;
	
	//for (int i = 0; i < NR_THREADS; i++) {
	//	CloseHandle(hTimer[i]);
	//}
	//CloseHandle(hTimerQueue);
	//cout << "RunTest end." << endl;
	return 0;
}
