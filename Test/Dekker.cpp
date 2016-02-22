/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <windows.h>
#include <assert.h>
// #include <ChessApi.h>

volatile LONG f0 = 0;
volatile LONG f1 = 0;
volatile LONG turn = 0;

volatile LONG x;

//#define USE_INTERLOCKS
//#define USE_INTRINSICS

inline void Write(volatile LONG* p, LONG val){
#ifdef USE_INTERLOCKS
	InterlockedExchange(p, val);
#else
	//ChessSyncVarAccess((int)p);
	//ChessDataVarAccess((void*)p, 4, true, 1);
	(*p) = val;
#endif
}

inline LONG Read(volatile LONG* p){
#ifdef USE_INTERLOCKS
	return InterlockedCompareExchange(p, 0, 0);
#else
	//ChessSyncVarAccess((int)p);
	//ChessDataVarAccess((void*)p, 4, false, 2);
	return (*p);
#endif
}

DWORD WINAPI Dekker0(void* p){
	p = 0;

	Write(&f0, 1);
	while(Read(&f1) == 1){
		if(Read(&turn) == 1){
			Write(&f0,0);
			while(Read(&turn) == 1){
				Sleep(0);
			}
			Write(&f0,1);
		}
		Sleep(0);
	}
	x = 0;
	assert(InterlockedCompareExchange(&x, 0, 0) == 0);

	turn = 1;
	Write(&f0, 0);
	return 0;
}

DWORD WINAPI Dekker1(void* p){
	p = 0;

	Write(&f1, 1);
	while(Read(&f0) == 1){
		if(Read(&turn) == 0){
			Write(&f1, 0);
			while(Read(&turn) == 0){
				Sleep(0);
			}
			Write(&f1, 1);
		}
		Sleep(0);
	}
	x = 1;
	assert(InterlockedCompareExchange(&x, 1, 1) == 1);

	turn = 0;
	Write(&f1, 0);
	return 0;
}

extern "C" 
__declspec(dllexport) int ChessTestRun(){
	HANDLE h[2];

	f0 = 0;
	f1 = 0;
	turn = 0;

	h[0] = CreateThread(NULL, 0, Dekker0, 0, 9, NULL);
	h[1] = CreateThread(NULL, 0, Dekker1, 0, 9, NULL);

	WaitForMultipleObjects(2, h, true, INFINITE);

	CloseHandle(h[0]);
	CloseHandle(h[1]);

	return 0;
}

int main(){
	for(int i=0; i<100; i++){
		ChessTestRun();
	}
	return 0;
}