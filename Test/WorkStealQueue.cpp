/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once;
#include <assert.h>
#include <windows.h>
#include "WorkStealQueue.h"
#include <iostream>

using namespace THE;

#define INITQSIZE 2 // must be power of 2

int nStealers = 1;
int nItems = 4;
int nStealAttempts = 2;

extern "C" 
__declspec(dllexport) int ChessTestStartup(int argc, char** argv){
	if(argc > 1){
		int arg1 = atoi(argv[1]);
		if(arg1 > 0){
			nStealers = arg1;
		}
	}
	if(argc > 2){
		int arg2 = atoi(argv[2]);
		if(arg2 > 0){
			nItems = arg2;
		}
	}
	if(argc > 3){
		int arg3 = atoi(argv[3]);
		if(arg3 > 0){
			nStealAttempts = arg3;
		}
	}
	std::cout << "\nWorkStealQueue Test: " 
		<< nStealers << " stealers, " 
		<< nItems << " items, "
		<< "and "
		<< nStealAttempts << " stealAttempts" 
		<< std::endl;
	return 1;
}

class ObjType {
public:
	ObjType() { field = 0; }
	void Operation() { field++; }
	void Check() { assert(field == 1); }

private:
	int field;
};

DWORD WINAPI Stealer(LPVOID param){
	WorkStealQueue<ObjType*> *q = (WorkStealQueue<ObjType*> *)param;

	ObjType* r; 
	for(int i=0; i<nStealAttempts; i++){
		if (q->Steal(r)) 
			r->Operation(); 
	}
	return 0;
}


extern "C" 
__declspec(dllexport) int ChessTestRun(){
	HANDLE*  handles = new HANDLE[nStealers];
	ObjType* items = new ObjType[nItems];

	WorkStealQueue<ObjType*> *q = new WorkStealQueue<ObjType*>(INITQSIZE);

	for (int i = 0; i < nStealers; i++) {
		DWORD tid;
		handles[i] = CreateThread(NULL, 0, Stealer, q, 0, &tid);
	}

	
	for (int i = 0; i < nItems/2; i++) {
		q->Push(&items[2*i]);
		q->Push(&items[2*i+1]);

		ObjType* r; 
		if (q->Pop(r)) 
			r->Operation(); 
	}

	for (int i = 0; i < nItems/2; i++) {
		ObjType* r; 
		if (q->Pop(r)) 
			r->Operation(); 
	}


	WaitForMultipleObjects(nStealers, handles, TRUE, INFINITE);

	for (int i = 0; i < nItems; i++) {
		items[i].Check();
	}

	delete[] items;
	delete[] handles;
	delete q;
	return 0;
}

