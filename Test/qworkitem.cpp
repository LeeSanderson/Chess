/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

/*
	Testing QueueUserWorkItem.
*/

#define _WIN32_WINNT 0x0520
#include <windows.h>
#include <iostream>

#define NR_THREADS 3


using namespace std;

DWORD WINAPI foo(LPVOID param) {

  //	cout << "Worker thread signalling event..." << endl;
	SetEvent((HANDLE)param);

	return 0;
}

extern "C" __declspec(dllexport) void ModelStartup(){
  // int i = 0;
	//	cout << "Model Startup " << i+1 << std::endl;
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){

	HANDLE hEvents[NR_THREADS];

	for (int i = 0; i < NR_THREADS; i++) {

		hEvents[i] = CreateEvent(NULL,         // default security attributes
        TRUE,         // manual-reset event
        FALSE,        // initial state is unsignaled
        NULL          // object name
		);

		//cout << "CreateEvent " << i+1 << " OK" << endl;
	}

	for (int i = 0; i < NR_THREADS; i++) {
		if (!QueueUserWorkItem(foo, hEvents[i], WT_EXECUTELONGFUNCTION))
		{
		  //cout << "QueueUserWorkItem " << i+1 << " failed: " << GetLastError() << endl;
			DebugBreak();
		}
		//else
		//cout << "QueueUserWorkItem " << i+1 << " OK" << endl;
	}

	for (int i = 0; i < NR_THREADS; i++) {
	  //cout << "Waiting for event " << i+1 << " ..." << endl;
		WaitForSingleObject(hEvents[i], INFINITE);
		CloseHandle(hEvents[i]);
	}

	//cout << "RunTest end." << endl;
	return 0;
}
