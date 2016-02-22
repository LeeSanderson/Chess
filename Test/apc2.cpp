/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#define _WIN32_WINNT 0x520
#include <windows.h>
#include <stdio.h>

// This function tests the QueueUserAPC functionality
// In particular, it checks if CHESS implementation of APCs handles the case in which
// APCs are queued to a thread before it starts running
//  This program should not deadlock

HANDLE signalEvent;

ULONG x;
ULONG y;

DWORD WINAPI ChildThreadFunction(LPVOID param) {
	UNREFERENCED_PARAMETER(param);

	WaitForSingleObject(signalEvent, INFINITE);
	// will not come here unless the APC is executed

	return 0;
}

void CALLBACK APCProc(ULONG_PTR dwParam){
	UNREFERENCED_PARAMETER(dwParam);
	x++;
	y++;

	SetEvent(signalEvent);
}	

	

extern "C" 
__declspec(dllexport) int ChessTestRun(){

  DWORD tid;
  HANDLE hThread;

  x = 0;
  y = 0;

  signalEvent = CreateEvent(NULL, FALSE, FALSE, NULL);

  hThread = CreateThread(NULL, 0, ChildThreadFunction, NULL, CREATE_SUSPENDED, &tid);

  QueueUserAPC(APCProc, hThread, NULL);

  ResumeThread(hThread); // which should execute the APC before executing the child function

  WaitForSingleObject(hThread, INFINITE);
  CloseHandle(hThread);
  CloseHandle(signalEvent);
  return 0;
}

int main(){
	ChessTestRun();
}
