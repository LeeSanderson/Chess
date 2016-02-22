/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "Win32Base.h"
#include "SyncVar.h"

#include <windows.h>
#include <hash_map>
#include <vector>
#include "ChessAssert.h"
#include "ChessProfilerTimer.h"
#include "SyncVarVector.h"

class Win32SyncVarManager{
public:
	Win32SyncVarManager()
	{
//		nextSyncVar = 512;
		SetInitState();
	}
	static void Init(){ // static to make Prefix happy
		//InitializeCriticalSection(&semcs);
	}

	void AddThreadHandleMapping(Task tid, HANDLE h){
		//if(threadHandles.size() <= tid){
		//	threadHandles.resize(tid+1, NULL);
		//}
		//assert(threadHandles[tid] == NULL);
		//threadHandles[tid] = h;
		threadHandles[tid] = h;
		handleThreadIdMap[h] = tid;
	}

	void SetInitState(){
		initState.handleSyncVarMap = handleSyncVarMap;
		initState.addressSyncVarMap = addressSyncVarMap;
		initState.anonymousSyncVars = anonymousSyncVars;
//		initState.nextSyncVar = nextSyncVar;
		initState.handleThreadIdMap = handleThreadIdMap;
	}

	void Reset(){
		//EnterCriticalSection(&semcs);
		//stdext::hash_map<Task, Semaphore>::iterator i;
		////std::vector<Semaphore*>::iterator i;
		//for(i=threadSemaphore.begin(); i!=threadSemaphore.end(); i++){
		//	(i->second).Clear();
		//}
		//threadSemaphore.clear();
		//LeaveCriticalSection(&semcs);

		//nextSyncVar = initState.nextSyncVar;
		//handleSyncVarMap.clear();
		//addressSyncVarMap.clear();
		//anonymousSyncVars.clear();
		handleSyncVarMap = initState.handleSyncVarMap;
		addressSyncVarMap = initState.addressSyncVarMap;
		anonymousSyncVars = initState.anonymousSyncVars;
		handleThreadIdMap = initState.handleThreadIdMap;
	}

	SyncVar GetNewSyncVar(){
//		SyncVar ret = nextSyncVar++;
		SyncVar ret = Chess::GetNextSyncVar();
		anonymousSyncVars.push_back(ret);
		return ret;
	}

	Task GetTid(HANDLE h){
		assert(handleThreadIdMap.find(h) != handleThreadIdMap.end());
		return handleThreadIdMap[h];
	}

	HANDLE GetCurrentHandle(Task tid){
		return threadHandles[tid];
		//if(threadHandles.size() > tid){
		//	return threadHandles[tid];
		//}
		//return NULL;
	}

	SyncVar GetSyncVarFromHandle(HANDLE h){
		if(handleThreadIdMap.find(h) != handleThreadIdMap.end()){
			return handleThreadIdMap[h];
		}
		// Assume it is non-thread HANDLE
		if(handleSyncVarMap.find(h) == handleSyncVarMap.end()){
			if(!Chess::GetOptions().unify_nonthreadhandles){
				// normal behavior: assign unique sync var ids to different, non-duplicate HANDLES
				handleSyncVarMap[h] = Chess::GetNextSyncVar();
			}else{
				// when CHESS misses to detect some duplicate HANDLES, it deadlocks
				// the work around is to assign all non-thread handles to the same syncvar id
				if(handleSyncVarMap.begin() == handleSyncVarMap.end()){
					handleSyncVarMap[h] = Chess::GetNextSyncVar();
				}
				else{
					SyncVar r = handleSyncVarMap.begin()->second;
					handleSyncVarMap[h] = r;
				}
			}
		}
		return handleSyncVarMap[h];
	}

	void DuplicateHandle(HANDLE orig, HANDLE copy){
		if(handleThreadIdMap.find(orig) != handleThreadIdMap.end()){
			handleThreadIdMap[copy] = handleThreadIdMap[orig];
		}
		else{
			SyncVar origVar = GetSyncVarFromHandle(orig); // define if necessary
			handleSyncVarMap[copy] = origVar;
		}
	}
	HANDLE AssociateNamedHandle(std::wstring& name, HANDLE handle){
		if(namedHandleMap.find(name) != namedHandleMap.end()){
			return namedHandleMap[name];
		}
		namedHandleMap[name] = handle;
		return handle;
	}

	SyncVar GetSyncVarFromAddress(void* addr){
//		ChessProfilerSentry sentry("Win32SyncVarManager::GetSyncVarFromAddress");
		if(addressSyncVarMap.find(addr) == addressSyncVarMap.end()){
//			addressSyncVarMap[addr] = nextSyncVar++;
			addressSyncVarMap[addr] = Chess::GetNextSyncVar();
		}
		return addressSyncVarMap[addr];
	}


	Semaphore* GetTaskSemaphore(Task tid){
		assert(InternalStateValid());
//		assert(threadSemaphore.find(tid) != threadSemaphore.end());
		assert(threadSemaphore[tid].sem != NULL);
		return &(threadSemaphore[tid]);
//		return ProtectedGetTaskSemaphore(tid);
	}
	
	//Semaphore* ProtectedGetTaskSemaphore(Task tid){
	//	Semaphore* ret;
	//	EnterCriticalSection(&semcs);
	//    ret = GetTaskSemaphore(tid);
	//	LeaveCriticalSection(&semcs);
	//	return ret;
	//}
	
	void RegisterThreadSemaphore(Task child, Semaphore sem){
		assert(threadSemaphore[child].sem == NULL);
		threadSemaphore[child] = sem;
	}

	void TaskEnd(Task tid){
		Semaphore* sem = GetTaskSemaphore(tid);
		sem->Clear();
		threadSemaphore[tid] = Semaphore();
		threadHandles[tid] = NULL;
		//if(threadHandles.size() > tid && threadHandles[tid]){
		//	//handleThreadIdMap.erase(threadHandles[tid]);
		//	threadHandles[tid] = NULL;
		//}
	}

	bool InternalStateValid(){
		for(size_t i=1; i<threadSemaphore.size(); i++){
			assert(threadSemaphore[i].InternalStateValid());
		}
		return true;
		//stdext::hash_map<Task, Semaphore>::iterator i;
		//for(i = threadSemaphore.begin(); i!= threadSemaphore.end(); i++){
		//	assert((i->second).InternalStateValid());
		//}
		//return true;
	}

	bool RenameSymmetricTasks(stdext::hash_map<Task, Task>& map){
		// map should be a bijection between symmetric tasks
		assert(IsBijection(map));
		TaskVector<Semaphore> old = threadSemaphore;
//		stdext::hash_map<Task, Semaphore> old = threadSemaphore;
		threadSemaphore.clear();
		for(size_t i=0; i<old.size(); i++){
			if(map.find(i) == map.end())
				threadSemaphore[i] = old[i];
			else
				threadSemaphore[map[i]] = old[i];
		}

		//stdext::hash_map<Task, Semaphore>::iterator si;
		//for(si = old.begin(); si != old.end(); si++){
		//	if(map.find(si->first) == map.end()){
		//		threadSemaphore[si->first] = si->second;
		//	}
		//	else{
		//		threadSemaphore[map[si->first]] = si->second;
		//	}
		//}
	
		stdext::hash_map<HANDLE, Task>::iterator hi;
		for(hi = handleThreadIdMap.begin(); hi != handleThreadIdMap.end(); hi++){
			if(map.find(hi->second) != map.end()){
				hi->second = map[hi->second];
			}
		}
		return true;
	}
private:
	struct InternalState{
		stdext::hash_map<HANDLE, SyncVar> handleSyncVarMap;
		//stdext::hash_map<void*, SyncVar> addressSyncVarMap;
		std::map<void*, SyncVar> addressSyncVarMap;
		std::vector<SyncVar> anonymousSyncVars;
		stdext::hash_map<HANDLE, Task> handleThreadIdMap;
		//int nextSyncVar;
		InternalState(){}
	};
	InternalState initState;

//	stdext::hash_map<Task, Semaphore> threadSemaphore;
	TaskVector<Semaphore> threadSemaphore;
	//std::vector<Semaphore> threadSemaphore;

	stdext::hash_map<HANDLE, SyncVar> handleSyncVarMap;
//	stdext::hash_map<void*, SyncVar> addressSyncVarMap;
	std::map<void*, SyncVar> addressSyncVarMap;
	std::vector<SyncVar> anonymousSyncVars;

	stdext::hash_map<HANDLE, Task> handleThreadIdMap;
	stdext::hash_map<std::wstring, HANDLE> namedHandleMap;

	// handleThreadIdMap is already defined above in SyncVar Management
//	std::vector<HANDLE> threadHandles;
	TaskVector<HANDLE> threadHandles;

	//// SyncVar Management State
	//int nextSyncVar;
	
//	CRITICAL_SECTION semcs;

	static bool IsBijection(stdext::hash_map<Task, Task>& map){
		stdext::hash_map<Task, bool> seen;
		stdext::hash_map<Task, Task>::iterator i;
		for(i=map.begin(); i!=map.end(); i++){
			if(map.find(i->second) == map.end()){
				return false;
			}
			if(seen.find(i->second) != seen.end()){
				return false;
			}
			seen[i->second] = true;
		}
		return true;
	}
};