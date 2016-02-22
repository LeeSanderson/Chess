/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

/*
Testing _beginthreadex.
*/

#define _WIN32_WINNT 0x0520
#include <windows.h>
#include <iostream>
#include <assert.h>
#include <process.h>

unsigned Counter; 
unsigned __stdcall SecondThreadFunc( void* pArguments )
{
  UNREFERENCED_PARAMETER(pArguments);
  while ( Counter < 10 )
    Counter++;
  
  _endthreadex(0);
  return 0;
} 

extern "C" 
__declspec(dllexport) int ChessTestRun(){
  HANDLE hThread;
  unsigned threadID;

  // Create the second thread.
  hThread = (HANDLE)_beginthreadex( NULL, 0, &SecondThreadFunc, NULL, 0, &threadID );

  WaitForSingleObject( hThread, INFINITE );
  assert(Counter == 10);

  // Destroy the thread object.
  CloseHandle( hThread );

  return 0;
}

