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

	//      std::cout << "Timer Fire" << std::endl;
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

		//		cout << "Creating timer " << i << " with event at " << &hEvent[i] << endl;

		CreateTimerQueueTimer( &hTimer[i], hTimerQueue, 
			(WAITORTIMERCALLBACK)TimerRoutine, (PVOID)&hEvent[i] , 5000 * i, 0, WT_EXECUTELONGFUNCTION);
	}

	HANDLE event = CreateEvent( 
			NULL,         // default security attributes
			TRUE,         // manual-reset event
			FALSE,        // initial state is unsignaled
			NULL); 
	
	BOOL retVal = DeleteTimerQueueTimer(hTimerQueue, hTimer[0], event);
	if(!retVal){
	  //std::cout << "Delete Fail" << std::endl;
		DWORD error = GetLastError();
		if(error != ERROR_IO_PENDING){
			assert(false);
			return -1;
		}
		WaitForSingleObject(event, INFINITE);
		WaitForSingleObject(hEvent[0], INFINITE);
	}	
	else{
	  //std::cout << "Delete Succ" << std::endl;
	}
	CloseHandle(event);

	for (int i = 1; i < NR_THREADS; i++) {
	  //		cout << "Main thread: waiting for event " << i << endl;
		WaitForSingleObject(hEvent[i], INFINITE);
	}

	for (int i = 0; i < NR_THREADS; i++) {
		CloseHandle(hEvent[i]);
		//CloseHandle(hTimer[i]);	
	}

	if (!DeleteTimerQueueEx(hTimerQueue, INVALID_HANDLE_VALUE)) 
	{
	  //		cout << "DeleteTimerQueueEx failed: " << GetLastError() << endl;
		DebugBreak();
	}
	//	else
	//		cout << "DeleteTimerQueueEx OK" << endl;
	
	//CloseHandle(hTimerQueue);
	//	cout << "RunTest end." << endl;
	return 0;
}
