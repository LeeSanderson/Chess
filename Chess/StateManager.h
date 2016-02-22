/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#ifdef OLD_STUFF
#include <windows.h>
#include <string>
#include <hash_map>
#include <map>
#include "SyncVar.h"

class StateManager{
public:
	StateManager();
	~StateManager();
	void RegisterDllModule(HMODULE dll);
	std::string GetStateSignature();
	void ThreadEnd(Task tid){
		stackFrames[tid].clear();
	}
private:
	CRITICAL_SECTION stateManagerCs;

	stdext::hash_map<PBYTE, SIZE_T> globals;
	stdext::hash_map<PBYTE, SIZE_T> chessDllCodeRegions;

	void RegisterGlobalRegion(LPVOID addr, SIZE_T len);
	
	// Heap Related Data Structures
	int numHeaps;
	HANDLE* heapHandles;
	struct PerHeapInfo{
		std::vector<PBYTE> heapRegions;
	};
	std::vector<PerHeapInfo> heapInfo;

	void GatherHeapRegions();
	void GatherHeapObjects(int i);
	void AddHeapRegion(PBYTE addr, SIZE_T len);
	bool IsHeapAddr(PBYTE addr, PBYTE& region, SIZE_T& len);

	void HeapCanonicalize(stdext::hash_map<PBYTE, SIZE_T>& roots);

	void InitCanonicalStream();
	void AddToCanonicalStream(SIZE_T val);
	std::string GetCanonicalStreamSignature();

	char canonicalStreamBuffer[64];
	char* canonicalStreamPtr;


	// Stack Related Data Structures
	stdext::hash_map<Task, stdext::hash_map<PBYTE, SIZE_T> > stackFrames;
	void GatherStackFrames();
	bool IsChessDllPC(DWORD64 pc);
	void AddStackFrame(PBYTE addr, SIZE_T len);
};
#endif