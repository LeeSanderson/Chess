/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

/*
    Testing events: CHESS should perform one execution only.
*/
#include <windows.h>
#include <iostream>

HANDLE hEvent1; 
HANDLE hEvent2; 

DWORD WINAPI foo(LPVOID param) {

  UNREFERENCED_PARAMETER(param);

  SetEvent(hEvent1);
  WaitForSingleObject(hEvent2, INFINITE);

  return 0;
}
using namespace std;

extern "C" 
__declspec(dllexport) int ChessTestRun(){

  DWORD tid;
  HANDLE hThread;

    hEvent1 = CreateEvent( 
        NULL,         // default security attributes
        TRUE,         // manual-reset event
        FALSE,        // initial state is unsignaled
        NULL          // object name
        ); 

    hEvent2 = CreateEvent( 
        NULL,         // default security attributes
        FALSE,        // auto-reset event
        FALSE,        // initial state is unsignaled
        NULL          // object name
        ); 

  hThread = CreateThread(NULL, 0, foo, NULL, 0, &tid);

  WaitForSingleObject(hEvent1, INFINITE);

  SetEvent(hEvent2);

  WaitForSingleObject(hThread, INFINITE);

  /* Now test the events themselves	*/
  /* Event 1 is manual-reset, so it should stay signaled. */
  
  if (WaitForSingleObject(hEvent1, 0) != WAIT_OBJECT_0)
	  cout << "Test Event 1 failed" << endl;
  else
	  cout << "Test Event 1 passed" << endl;

  /* Event 2 is auto-reset, so it should flip to unsignaled. */
  if (WaitForSingleObject(hEvent2, 0) != WAIT_TIMEOUT)
	  cout << "Test Event 2 failed" << endl;
  else
	  cout << "Test Event 2 passed" << endl;

  /* Event 1 is manual-reset; reset it and check again, it should be unsignaled. */
  ResetEvent(hEvent1);
  if (WaitForSingleObject(hEvent1, 0) != WAIT_TIMEOUT)
	  cout << "Test(2) Event 1 failed" << endl;
  else
	  cout << "Test(2) Event 1 passed" << endl;

  CloseHandle(hThread);
  CloseHandle(hEvent1);
  CloseHandle(hEvent2);
  return 0;
}
