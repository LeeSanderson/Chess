/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

/*
   Frangipani FS synchronization idiom, from the Flanagan-Godefroid paper on Dynamic POR 
*/

#define _WIN32_WINNT 0x0520
#include <windows.h>
#include <iostream>

#define NR_THREADS 3

using namespace std;

const int NUMBLOCKS = 4;
const int NUMINODE = 4;

int inode[NUMINODE];

CRITICAL_SECTION locki[NUMINODE];
CRITICAL_SECTION lockb[NUMBLOCKS];

volatile bool busy[NUMBLOCKS];

#define acquire(lock) EnterCriticalSection(&lock);
#define release(lock) LeaveCriticalSection(&lock);

#define FREE_INODE (-1)

DWORD WINAPI foo(LPVOID param) {
	int tid = PtrToLong(param);
	int i, b;

	i = tid % NUMINODE;
	acquire(locki[i]);
	if (inode[i] == FREE_INODE) { 
		b = (i*2) % NUMBLOCKS;
		while (true) { 
			acquire(lockb[b]);
			if (!busy[b]) { 
				busy[b] = true;
				inode[i] = b+1;
				release(lockb[b]);
				break;
			}
			release(lockb[b]);
			b = (b+1)%NUMBLOCKS;
		}
	}
	release(locki[i]);
	return 0;
}

void initDaStuff() {
	for (int i = 0; i < NUMINODE; i++) {
		InitializeCriticalSection(&locki[i]);
	}

	for (int i = 0; i < NUMBLOCKS; i++) {
		InitializeCriticalSection(&lockb[i]);
	}

	for (int i = 0; i < NUMINODE; i++) {
		inode[i] = FREE_INODE;
	}

	for (int i = 0; i < NUMBLOCKS; i++) {
		busy[i] = false;
	}
}

void deleteDaStuff() {
	for (int i = 0; i < NUMINODE; i++) {
		DeleteCriticalSection(&locki[i]);
	}

	for (int i = 0; i < NUMBLOCKS; i++) {
		DeleteCriticalSection(&lockb[i]);
	}

}

extern "C" 
__declspec(dllexport) int ChessTestRun(){

	HANDLE hThreads[NR_THREADS];
	DWORD tid;

	initDaStuff();

	for (int i = 0; i < NR_THREADS; i++) {
		hThreads[i] = CreateThread(NULL, 0, foo, LongToPtr(i), 0, &tid);
	}

	for (int i = 0; i < NR_THREADS; i++) {
		WaitForSingleObject(hThreads[i], INFINITE);
		CloseHandle(hThreads[i]);
	}

	deleteDaStuff();

	return 0;
}
