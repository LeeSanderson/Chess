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

    hEvent2 = CreateEvent( 
        NULL,         // default security attributes
        TRUE,         // manual-reset event
        FALSE,        // initial state is unsignaled
        "ChessTestNamedEvent" // object name
        ); 
  WaitForSingleObject(hEvent2, INFINITE);
  CloseHandle(hEvent2);

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
        "ChessTestNamedEvent" // object name
        ); 


  hThread = CreateThread(NULL, 0, foo, NULL, 0, &tid);

  SetEvent(hEvent1);

  WaitForSingleObject(hThread, INFINITE);

  CloseHandle(hThread);
  CloseHandle(hEvent1);
  return 0;
}
