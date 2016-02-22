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

int x = 0;
int deleted;
CRITICAL_SECTION cs;

using namespace std;

DWORD WINAPI foo(LPVOID param) {

  //	cout << "Worker thread signalling event..." << endl;
  //	SetEvent((HANDLE)param);
  int t;
  EnterCriticalSection(&cs);
  x --;
  t = x;
  LeaveCriticalSection(&cs);
  if(t == 0){
    DeleteCriticalSection(&cs);
    deleted = true;
  }
  return 0;
}

extern "C" __declspec(dllexport) void ModelStartup(){
  // int i = 0;
	//	cout << "Model Startup " << i+1 << std::endl;
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){

	HANDLE hEvents[NR_THREADS];
	InitializeCriticalSection(&cs);
	x = NR_THREADS;
        deleted = 0;

	for (int i = 0; i < NR_THREADS; i++) {
		if (!QueueUserWorkItem(foo, hEvents[i], WT_EXECUTELONGFUNCTION))
		{
			DebugBreak();
		}
	}

	while(!deleted)
	  Sleep(0);
	return 0;
}
