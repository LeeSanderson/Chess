/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#define _WIN32_WINNT 0x520
#include <windows.h>
#include <stdio.h>

HANDLE signalEvent;

VOID CALLBACK FileIoCompletionRoutine(DWORD dwErrorCode, DWORD numBytes, LPOVERLAPPED lpOverlapped){
	UNREFERENCED_PARAMETER(dwErrorCode);
	UNREFERENCED_PARAMETER(numBytes);
	UNREFERENCED_PARAMETER(lpOverlapped);
	SetEvent(signalEvent);
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){

	signalEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
	if(signalEvent == NULL){
		printf("CreateEvent Failed: %d\n", GetLastError());
		return  -1;
	}

	HANDLE hFile = CreateFile("testReadFileEx", GENERIC_READ, 0, NULL, OPEN_ALWAYS, FILE_FLAG_OVERLAPPED, NULL);

	if(hFile == INVALID_HANDLE_VALUE){
		printf("CreateFile failed: %d\n", GetLastError());
		goto clean_and_die;
	}

	char buffer[256];
	
	OVERLAPPED overlapped;
	overlapped.Offset = 0;
	overlapped.OffsetHigh = 0;

	BOOL ret = ReadFileEx(hFile, buffer, 256, &overlapped, FileIoCompletionRoutine);
	if(!ret){
		printf("ReadFileEx failed with %d\n", GetLastError());
		goto clean_and_die;
	}
	
	//WaitForSingleObject(signalEvent, INFINITE, TRUE); //this should deadlock
	WaitForSingleObjectEx(signalEvent, INFINITE, TRUE);
	

clean_and_die:
	CloseHandle(signalEvent);
	return 0;
}

int main(){
	ChessTestRun();
}
