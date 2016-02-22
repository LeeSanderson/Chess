/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>


CRITICAL_SECTION lock;

int x = 0;
int y = 0;
DWORD WINAPI foo(LPVOID param) {
  EnterCriticalSection(&lock);
  if(x == 1){
    if(y == 0){
      y++;
      EnterCriticalSection(&lock);
      LeaveCriticalSection(&lock);
    }
  }
  LeaveCriticalSection(&lock);
  return 0;
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){

	DWORD tid;
	HANDLE hThread;
	HANDLE hThread2;

	InitializeCriticalSection(&lock);
	x = 0;

	hThread = CreateThread(NULL, 0, foo, NULL, 0, &tid);
	hThread2 = CreateThread(NULL, 0, foo, NULL, 0, &tid);
	EnterCriticalSection(&lock);
	x++;
	LeaveCriticalSection(&lock);

	EnterCriticalSection(&lock);
	x++;
	LeaveCriticalSection(&lock);

	WaitForSingleObject(hThread, INFINITE);
	CloseHandle(hThread);
	WaitForSingleObject(hThread2, INFINITE);
	CloseHandle(hThread2);

	DeleteCriticalSection(&lock);
	return 0;
}
