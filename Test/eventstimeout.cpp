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
#include <assert.h>

HANDLE hEvent1; 
char* p = 0;
char ch = 'a';

DWORD WINAPI foo(LPVOID param) {

  UNREFERENCED_PARAMETER(param);

  p = &ch;
  SetEvent(hEvent1);

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

  hThread = CreateThread(NULL, 0, foo, NULL, 0, &tid);

  WaitForSingleObject(hEvent1, 1000);
  assert(p);
  *p = 'b';

  CloseHandle(hThread);
  CloseHandle(hEvent1);
  return 0;
}
