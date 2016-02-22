/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

/*
Testing FreeLibraryAndExitThread.
*/

#define _WIN32_WINNT 0x0520
#include <windows.h>
#include <iostream>

typedef void (*FOO_TYPE)();

DWORD WINAPI ThreadRoutine(PVOID lpParam)
{
  UNREFERENCED_PARAMETER(lpParam);
  
  HMODULE hModule = LoadLibraryA("flet-dll.dll");
  if(hModule == NULL){
    std::cout << "Cannot find flet-dll.dll" << std::endl; 
    exit(-1);
  }
  FOO_TYPE foo = (FOO_TYPE)GetProcAddress(hModule, "foo");
  if(foo == NULL){
    std::cout << "Cannot find foo function in flet-dll.dll " << std::endl; 
    exit(-1);
  }
  foo();
  FreeLibraryAndExitThread(hModule, 0);
}


extern "C" 
__declspec(dllexport) int ChessTestRun(){
  UNREFERENCED_PARAMETER(args);

  DWORD tid;
  HANDLE h = CreateThread(NULL, 0, ThreadRoutine, 0, NULL, &tid);
  WaitForSingleObject(h, INFINITE);
  return 0;
}
