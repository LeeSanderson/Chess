/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#define _WIN32_WINNT 0x520
#include <windows.h>
#include <stdio.h>

// This function tests the QueueUserAPC functionalit7

HANDLE signalEvent;

bool apcCalled = false;
ULONG x;
ULONG y;

DWORD WINAPI ChildThreadFunction(LPVOID param) {
	UNREFERENCED_PARAMETER(param);

	if(!apcCalled){
		//wait for APC
		SleepEx(INFINITE, true);
	}
	// done executing APC

	WaitForSingleObject(signalEvent, INFINITE);
	// will not come here unless the APC is executed

	return 0;
}

void CALLBACK APCProc(ULONG_PTR dwParam){
	UNREFERENCED_PARAMETER(dwParam);
	apcCalled = true;
	x++;
	y++;

	SetEvent(signalEvent);
}	

DWORD WINAPI ParentThreadFunction(LPVOID param){
	HANDLE hThread = (HANDLE)param;

	DWORD retVal;

	x++;
	retVal = QueueUserAPC(APCProc, hThread, NULL);
	
	y++; // a race

	return 0;
}


extern "C" 
__declspec(dllexport) int ChessTestRun(){

  DWORD tid;
  HANDLE hThread;

  x = 0;
  y = 0;
  apcCalled = false;	

  signalEvent = CreateEvent(NULL, FALSE, FALSE, NULL);

  hThread = CreateThread(NULL, 0, ChildThreadFunction, NULL, 0, &tid);

  ParentThreadFunction(hThread);


  WaitForSingleObject(hThread, INFINITE);
  CloseHandle(hThread);
  CloseHandle(signalEvent);
  return 0;
}

__declspec(dllexport) DWORD WINAPI RunTestOrig(LPVOID args) {
	return ChessTestRun();
}

int main(){
	ChessTestRun();
}
