/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#define _WIN32_WINNT 0x0520
#include <windows.h>
#include <assert.h>
#include <stdio.h>
#include <string>
#define NR_THREADS 3

HANDLE hFile;

std::string GetLastErrorString(){
	char errorstr[256];
	DWORD error = GetLastError();
	FormatMessageA(FORMAT_MESSAGE_FROM_SYSTEM, NULL, error, 0, errorstr, 256, NULL);
	return errorstr;
}

DWORD WINAPI foo(LPVOID param) {
  char buffer[256];
  DWORD numberOfBytesWritten;
  LPOVERLAPPED x = (LPOVERLAPPED) param;

  BOOL ret = ReadFile(hFile, buffer, 1, &numberOfBytesWritten, x);
  if (!ret){
    assert(GetLastError() != ERROR_IO_PENDING);
    printf("ReadFile failed with %s\n", GetLastErrorString().c_str());
  }
	
  return 0;
}


extern "C" 
__declspec(dllexport) int ChessTestRun(){

  HANDLE hThreads[NR_THREADS];
  HANDLE hIOCPort;
  DWORD tid;

  hFile = CreateFile("testWriteFile", GENERIC_READ | GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, FILE_FLAG_OVERLAPPED, NULL);
  if (hFile == INVALID_HANDLE_VALUE) {
    printf("CreateFile failed: %d\n", GetLastError());
    return -1;
  }

  char buffer[256];
  DWORD numberOfBytesWritten;
  OVERLAPPED x;
  memset(&x, 0, sizeof(OVERLAPPED));
  
  BOOL ret = WriteFile(hFile, buffer, NR_THREADS*10, &numberOfBytesWritten, &x);
  if (!ret){
    assert(GetLastError() != ERROR_IO_PENDING);
    printf("WriteFile failed with %s\n", GetLastErrorString().c_str());
  }


  ULONG_PTR key1;
  key1 = 42;
  hIOCPort = CreateIoCompletionPort(
				    hFile,                 // FileHandle - not associated with a file handler
				    NULL,                  // ExistingCompletionPort - must be NULL if not assoc'd with a file handler
				    key1,                  // CompletionKey
				    NR_THREADS             // NumberOfConcurrentThreads
				    );

  if (!hIOCPort) 
    {
      printf("CreateIoCompletionPort failed: %d\n", GetLastError());
      CloseHandle(hFile);
      return -1;
    }

  OVERLAPPED overlapped[NR_THREADS];
  ULONG_PTR key2;
  DWORD nrBytesTransferred;

  for (int i = 0; i < NR_THREADS; i++) {
	memset(&overlapped[i], 0, sizeof(OVERLAPPED));
    overlapped[i].Offset = i*10;
    overlapped[i].OffsetHigh = 0;
    overlapped[i].hEvent = NULL;

    hThreads[i] = CreateThread(NULL, 0, foo, &overlapped[i], 0, &tid);
  }

  LPOVERLAPPED pOverlapped[NR_THREADS];
 
  for (int i = 0; i < NR_THREADS; i++) {
    if (!GetQueuedCompletionStatus(
				   hIOCPort,                         // CompletionPort
				   &nrBytesTransferred,              // lpNumberOfBytes
				   &key2,                            // lpCompletionKey
				   &pOverlapped[i],                  // lpOverlapped
				   INFINITE                          // dwMilliseconds
				   ))
      {
	assert(false);
      }
  }
  
  for (int i = 0; i < NR_THREADS; i++) {
    WaitForSingleObject(hThreads[i], INFINITE);
    CloseHandle(hThreads[i]);
  }
  
  CloseHandle(hFile);
  CloseHandle(hIOCPort);
  return 0;
}

