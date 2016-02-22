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
#include <assert.h>
#include <process.h>

extern "C" 
__declspec(dllexport) int ChessTestRun(){
  int x = rand();
  
  assert(x < 42);

  return 0;
}

