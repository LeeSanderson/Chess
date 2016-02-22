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

DWORD WINAPI foo(LPVOID param) {

	ULONG_PTR pKey = (ULONG_PTR)malloc(sizeof(ULONG));
	LPOVERLAPPED pOverlapped = (LPOVERLAPPED)malloc(sizeof(OVERLAPPED));

	//cout << "Worker thread posting completion packet..." << endl;
	if (!PostQueuedCompletionStatus(
		(HANDLE)param,                     // CompletionPort
		0,                                 // dwNumberOfBytesTransferred
		pKey,                              // dwCompletionKey
		pOverlapped                        // lpOverlapped
		))
	{
		//cout << "PostQueuedCompletionStatus failed: " << GetLastError() << endl;
		DebugBreak();
	}
	else
		;//cout << "PostQueuedCompletionStatus OK" << endl;


  return 0;
}

int globalCounter = 0;
extern "C" 
__declspec(dllexport) int ChessTestRun(){

	HANDLE hThreads[NR_THREADS];
	HANDLE hIOCPort;
	DWORD tid;
    LPOVERLAPPED pOverlapped = (LPOVERLAPPED)malloc(sizeof(OVERLAPPED));
	ULONG_PTR pKey = (ULONG_PTR)malloc(sizeof(ULONG));
	DWORD nrBytesTransferred;


	globalCounter ++;

	hIOCPort = CreateIoCompletionPort(
		INVALID_HANDLE_VALUE,  // FileHandle - not associated with a file handler
		NULL,                  // ExistingCompletionPort - must be NULL if not assoc'd with a file handler
		pKey,                  // CompletionKey
		NR_THREADS             // NumberOfConcurrentThreads
		);

	if (!hIOCPort) 
	{
		//cout << "CreateIoCompletionPort failed: " << GetLastError() << endl;
		DebugBreak();
	}
	else
		//cout << "CreateIoCompletionPort OK" << endl;


	for (int i = 0; i < NR_THREADS; i++) {

		hThreads[i] = CreateThread(NULL, 0, foo, hIOCPort, 0, &tid);
		//cout << "Created thread " << i << endl;
	}

	for (int i = 0; i < NR_THREADS; i++) {
		//cout << "Main thread: waiting for event " << i << endl;
		if (!GetQueuedCompletionStatus(
			hIOCPort,                         // CompletionPort
			&nrBytesTransferred,              // lpNumberOfBytes
			&pKey,                            // lpCompletionKey
			&pOverlapped,                     // lpOverlapped
			INFINITE                          // dwMilliseconds
			))
		{
			//cout << "GetQueuedCompletionStatus failed: " << GetLastError() << endl;
			DebugBreak();
		}
		else
			//cout << "GetQueuedCompletionStatus OK" << endl;
;
	}

	for (int i = 0; i < NR_THREADS; i++) {
		//cout << "Waiting for thread " << i+1 << " to end..." << endl;
		WaitForSingleObject(hThreads[i], INFINITE);
		CloseHandle(hThreads[i]);
	}

	if (!CloseHandle(hIOCPort))
	{
		//cout << "Close IO completion failed: " << GetLastError() << endl;
		DebugBreak();
	}
	else
		//cout << "Close IO completion OK" << endl;

//	free(pOverlapped);

	//cout << "RunTest end." << endl;
	return 0;
}
