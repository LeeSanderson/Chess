/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>

SRWLOCK srwLock;
int x;

DWORD WINAPI writer(void* p){
  UNREFERENCED_PARAMETER(p);

  AcquireSRWLockExclusive(&srwLock);
  x++;
  ReleaseSRWLockExclusive(&srwLock);

  AcquireSRWLockExclusive(&srwLock);
  x++;
  ReleaseSRWLockExclusive(&srwLock);

  return 0;
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){

  HANDLE h;
  InitializeSRWLock(&srwLock);
  AcquireSRWLockShared(&srwLock);
  h = CreateThread(NULL, 0, writer, 0, 0, NULL);
  WaitForSingleObject(h, INFINITE);
  CloseHandle(h);
  ReleaseSRWLockShared(&srwLock);
  
  return 0;
}
