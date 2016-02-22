/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#define _WIN32_WINNT 0x0520
#include <windows.h>

const int NUM_PHIL = 2;

CRITICAL_SECTION lock[NUM_PHIL];

DWORD WINAPI LivePhil(LPVOID p) {
  int id = (int)p;
  int left = id;
  int right = (id+1)%NUM_PHIL;
  
  while(true){
    if(TryEnterCriticalSection(&lock[left])){
      if(TryEnterCriticalSection(&lock[right])){
	break;
      }
      else{
	LeaveCriticalSection(&lock[left]);
      }
    }
    // failed to get both forks
    Sleep(1);
  }

  //eat
  LeaveCriticalSection(&lock[left]);
  LeaveCriticalSection(&lock[right]);

  return 0;
}



extern "C" 
__declspec(dllexport) int ChessTestRun(){

  DWORD tid[NUM_PHIL];
  HANDLE hThread[NUM_PHIL];

  for(int i=0; i<NUM_PHIL; i++){
	  InitializeCriticalSection(&lock[i]);
  }

  for(int i=0; i<NUM_PHIL; i++){
    hThread[i] = CreateThread(NULL, 0, LivePhil, (LPVOID)i, 0, &tid[i]);
  }
  
  WaitForMultipleObjects(NUM_PHIL, hThread, true, INFINITE);

  for(int i=0; i<NUM_PHIL; i++){
    CloseHandle(hThread[i]);
  }
  for(int i=0; i<NUM_PHIL; i++){
	  DeleteCriticalSection(&lock[i]);
  }

  return 0;
}
