/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>


CRITICAL_SECTION lock;

extern "C" 
__declspec(dllexport) int ChessTestRun(){
  InitializeCriticalSection(&lock);
  
  EnterCriticalSection(&lock);
  LeaveCriticalSection(&lock);

  DeleteCriticalSection(&lock);
  return 0;
}
