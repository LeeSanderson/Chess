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

DWORD WINAPI foo1(LPVOID param) {

  UNREFERENCED_PARAMETER(param);

  SetEvent(hEvent1);

  return 0;
}

DWORD WINAPI foo2(LPVOID param) {

  UNREFERENCED_PARAMETER(param);

  SetEvent(hEvent2);

  return 0;
}

DWORD WINAPI foo3(LPVOID param) {

  UNREFERENCED_PARAMETER(param);

  HANDLE h[2];
  h[0] = hEvent1;
  h[1] = hEvent2;
  WaitForMultipleObjects(2, h, FALSE, INFINITE);

  return 0;
}

using namespace std;

extern "C" 
__declspec(dllexport) int ChessTestRun(){

  DWORD tid;
  HANDLE hThread[3];

    hEvent1 = CreateEvent( 
        NULL,         // default security attributes
        TRUE,         // manual-reset event
        FALSE,        // initial state is unsignaled
        NULL          // object name
        ); 

    hEvent2 = CreateEvent( 
        NULL,         // default security attributes
        TRUE,        // auto-reset event
        FALSE,        // initial state is unsignaled
        NULL          // object name
        ); 

  hThread[0] = CreateThread(NULL, 0, foo1, NULL, 0, &tid);
  hThread[1] = CreateThread(NULL, 0, foo2, NULL, 0, &tid);
  hThread[2] = CreateThread(NULL, 0, foo3, NULL, 0, &tid);

  WaitForMultipleObjects(3, hThread, TRUE, INFINITE);

  CloseHandle(hThread[0]);
  CloseHandle(hThread[1]);
  CloseHandle(hThread[2]);
  CloseHandle(hEvent1);
  CloseHandle(hEvent2);
  return 0;
}
