/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

/*
   The Indexer program, from the Flanagan-Godefroid paper on Dynamic POR 
*/

#define _WIN32_WINNT 0x0520
#include <windows.h>
#include <iostream>

#define NR_THREADS 2

using namespace std;

const long TABLE_SIZE = 128;
const long MAX = 4;
volatile long table[TABLE_SIZE];

#define THREAD_EXIT (-1)
#define ENTRY_UNUSED (-1)

long getmsg(long tid, long & m) { 
	if (m < MAX ) 
		return (++m) * 11 + tid;
	else 
		return THREAD_EXIT; // terminate
}

long hash(long w) { 
	return (w * 7) % TABLE_SIZE;
}

DWORD WINAPI foo(LPVOID param) {
	long tid = PtrToLong(param) + 1;
	long m = 0, w, h;

	while (true) { 
		w = getmsg(tid, m);

		if (w == THREAD_EXIT)
			break;

		h = hash(w); // cas(table[h],0,w
		
		while (InterlockedCompareExchange(&table[h], w, ENTRY_UNUSED) != ENTRY_UNUSED) {
			//printf("h = %d, w = %d\n", h, w);
			h = (h+1) % TABLE_SIZE;
		}
	}

	return 0;
}

void initDaStuff() {
	for (int i = 0; i < TABLE_SIZE; i++) {
		table[i] = ENTRY_UNUSED;
	}
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){

	HANDLE hThreads[NR_THREADS];
	DWORD tid;

	initDaStuff();

	for (int i = 0; i < NR_THREADS; i++) {
		hThreads[i] = CreateThread(NULL, 0, foo, LongToPtr((long)i), 0, &tid);
	}

	for (int i = 0; i < NR_THREADS; i++) {
		WaitForSingleObject(hThreads[i], INFINITE);
		CloseHandle(hThreads[i]);
	}
	return 0;
}
