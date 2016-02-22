/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

/*
    Testing events: CHESS should perform one execution only.
*/
#define _WIN32_WINNT        0x0501
#include <windows.h>
#include <iostream>
#include <assert.h>
HANDLE hWorkDone; 
HANDLE hMoreWork; 

volatile int done;
volatile int numWork;

DWORD WINAPI foo(LPVOID param) {

  UNREFERENCED_PARAMETER(param);

  while(!done){
	  DWORD retVal = SignalObjectAndWait(hWorkDone, hMoreWork, INFINITE, false);
	  assert(retVal == WAIT_OBJECT_0);
	  numWork--;
  }
  return 0;
}
using namespace std;

extern "C" 
__declspec(dllexport) int ChessTestRun(){

  DWORD tid;
  HANDLE hThread;

  done = 0;

    hWorkDone = CreateEvent( 
        NULL,         // default security attributes
        FALSE,         // auto-reset event
        FALSE,        // initial state is unsignaled
        NULL          // object name
        ); 

    hMoreWork = CreateEvent( 
        NULL,         // default security attributes
        FALSE,        // auto-reset event
        FALSE,        // initial state is unsignaled
        NULL          // object name
        ); 

  hThread = CreateThread(NULL, 0, foo, NULL, 0, &tid);

  const int NUM_ITER = 5;
  for(int i=0; i<NUM_ITER; i++){
	 DWORD retVal = WaitForSingleObject(hWorkDone, INFINITE);
	 assert(retVal == WAIT_OBJECT_0);
	 numWork++;
	 if(i == NUM_ITER-1)
		 done = 1;
	 SetEvent(hMoreWork);
  }

  WaitForSingleObject(hThread, INFINITE);
  assert(numWork == 0);

  CloseHandle(hThread);
  CloseHandle(hWorkDone);
  CloseHandle(hMoreWork);
  return 0;
}
