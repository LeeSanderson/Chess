/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>
#include <stdio.h>

extern "C" 
__declspec(dllimport) 
  DWORD WINAPI RunTest2(LPVOID p);

extern "C" 
__declspec(dllexport) int ChessTestRun(){
  return (int) RunTest2(0);

}
