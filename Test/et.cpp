/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

/*
Testing ExitThread.
*/

#define _WIN32_WINNT 0x0520
#include <windows.h>
#include <iostream>

DWORD WINAPI ThreadRoutine(PVOID lpParam)
{
  UNREFERENCED_PARAMETER(lpParam);
  
  ExitThread(0);
}


extern "C" 
__declspec(dllexport) int ChessTestRun(){
  DWORD tid;
  HANDLE h = CreateThread(NULL, 0, ThreadRoutine, 0, NULL, &tid);
  WaitForSingleObject(h, INFINITE);
  return 0;
}
