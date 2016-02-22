/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#define _WIN32_WINNT 0x0520
#include <windows.h>
#include <vector>
#include <strstream>

struct ActivationRecord {
public:
	int fname;
	int pc;
	std::vector<void *> lvars;
	ActivationRecord()
	{
		fname = 0;
		pc = 0;
	}

public: 
	void Update(int pc0)
	{
		pc = pc0;
	}

	void Update(int pc0, int id, void *val0)
	{
		pc = pc0;
		if (lvars.size() <= id)
			lvars.resize(id + 1);
		lvars[id] = val0;
	}

	void Clear()
	{
		fname = 0;
		pc = 0;
		lvars.clear();
	}
};

int numPhils = 2;
const int MAX_NUM_PHIL = 20; //max

CRITICAL_SECTION lock[MAX_NUM_PHIL];

bool lock_shadow[MAX_NUM_PHIL];
ActivationRecord phils[MAX_NUM_PHIL];
ActivationRecord test;

DWORD WINAPI LivePhil(LPVOID p) {
	int id = (int)p;
	int left = id;
	int right = (id+1)%numPhils;

	ActivationRecord& ar = phils[id];

	while(true){
		// pc == 0
		ar.Update(0);
		if(TryEnterCriticalSection(&lock[left])){
			lock_shadow[left] = TRUE;
			// pc == 1
			ar.Update(1);
			if(TryEnterCriticalSection(&lock[right])){
				lock_shadow[right] = TRUE;
				break;
			}
			else{
				// pc == 2
				ar.Update(2);
				LeaveCriticalSection(&lock[left]);
				lock_shadow[left] = FALSE;
			}
		}
		// failed to get both forks
		// pc == 3
		ar.Update(3);
		Sleep(1);
	}

	//eat
	// pc == 4
	ar.Update(4);
	LeaveCriticalSection(&lock[left]);
	lock_shadow[left] = FALSE;

	// pc == 5
	ar.Update(5);
	LeaveCriticalSection(&lock[right]);
	lock_shadow[right] = FALSE;

	return 0;
}

#include <iostream>
extern "C" 
__declspec(dllexport) int ChessTestStartup(int argc, char** argv){
	if(argc > 1){
		int arg1 = atoi(argv[1]);
		if(arg1 > 0){
			numPhils = arg1;
		}
	}
	std::cout << "Dining Phils with " 
		<< numPhils << " phils" 
		<< std::endl;
	return 1;
}


extern "C" 
__declspec(dllexport) int ChessTestRun(){

	DWORD tid[MAX_NUM_PHIL];
	HANDLE hThread[MAX_NUM_PHIL];

	for(int i=0; i<numPhils; i++){
		InitializeCriticalSection(&lock[i]);
		lock_shadow[i] = FALSE;
	}

	for(int i=0; i<numPhils; i++){
		// pc == 0
		test.Update(0, 0, (void *) i);
		hThread[i] = CreateThread(NULL, 0, LivePhil, (LPVOID)i, 0, &tid[i]);
	}

	// pc == 1
	test.Update(1);
	WaitForMultipleObjects(numPhils, hThread, true, INFINITE);

	for(int i=0; i<numPhils; i++){
		CloseHandle(hThread[i]);
		phils[i].Clear();
		DeleteCriticalSection(&lock[i]);
	}
	test.Clear();
	return 0;
}

__declspec(dllexport) 
int GetState(char* buf, int len)
{
	std::ostrstream o;
	o << "D";

	for (int i = 0; i < MAX_NUM_PHIL; i++)
	{
		o << lock_shadow[i];
	}

	for (int i = 0; i < MAX_NUM_PHIL; i++)
	{
		ActivationRecord ar = phils[i];
		o << ar.pc;
		for (int j = 0; j < ar.lvars.size(); j++)
		{
			o << ar.lvars[j];
		}
	}

	o << test.pc;
	for (int j = 0; j < test.lvars.size(); j++)
	{
		o << test.lvars[j];
	}
	o << std::endl;
	
	int ret = o.pcount();
	if(len > o.pcount()){
		len = o.pcount();
	}
	memcpy(buf, o.str(), len);
	o.freeze(0);
	return ret;
}
