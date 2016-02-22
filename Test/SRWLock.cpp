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

DWORD WINAPI reader(void* p){
  UNREFERENCED_PARAMETER(p);
  int i;

  AcquireSRWLockShared(&srwLock);
  i = x;
  ReleaseSRWLockShared(&srwLock);

  i++;

  AcquireSRWLockShared(&srwLock);
  i = x + 1;
  ReleaseSRWLockShared(&srwLock);

  return 0;
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){

  HANDLE h1, h2;
  InitializeSRWLock(&srwLock);
  h1 = CreateThread(NULL, 0, writer, 0, 0, NULL);
  h2 = CreateThread(NULL, 0, reader, 0, 0, NULL);
  AcquireSRWLockExclusive(&srwLock);
  x--;
  ReleaseSRWLockExclusive(&srwLock);

  AcquireSRWLockExclusive(&srwLock);
  x--;
  ReleaseSRWLockExclusive(&srwLock);

  WaitForSingleObject(h1, INFINITE);
  WaitForSingleObject(h2, INFINITE);
  CloseHandle(h1);
  CloseHandle(h2);
  
  return 0;
}
