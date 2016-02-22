/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include <hash_map>
#include <vector>
#include <assert.h>
#include <iostream>
#include "VarTable.h"

class FishNetChecker;

class SingleHappensBefore{
public:
	typedef int TID;
	typedef void* MLOC;

private:
	class VarInfo{
	public:
		TID owner;
		int acc_time;
		MLOC lock_owner;
		std::vector<TID> read_owners;
	};

	class LockInfo{
	public:
		TID owner;
		int lock_time;
		hash_map<TID, int> unlock_time;
	};

	class ThreadInfo{
	public:
		int time;
		vector<MLOC> locks_held;
	};

	stdext::hash_map<MLOC, LockInfo> lock_info;
//	var_table<VarInfo> var_info;
	stdext::hash_map<MLOC, VarInfo> var_info;
	stdext::hash_map<TID, ThreadInfo> thread_info;

	FishNetChecker* fishNetChecker;

public:
	SingleHappensBefore(FishNetChecker* fnc){
		fishNetChecker = fnc;
	}

	~SingleHappensBefore(void){
	}

	void Reset(){
		lock_info.clear();
		var_info.clear();
		thread_info.clear();
	}

	void Lock(MLOC lock, TID tid){
		tid++;
		//std::cout << lock << " " << tid << " " << std::endl;
		//return;
		LockInfo& linfo = lock_info[lock];
		ThreadInfo& tinfo = thread_info[tid];

		linfo.owner = tid;
		linfo.lock_time = ++tinfo.time;
		tinfo.locks_held.push_back(lock);
	}

	void Unlock(MLOC lock, TID tid){
		tid++;
		LockInfo& linfo = lock_info[lock];
		ThreadInfo& tinfo = thread_info[tid];

		assert(linfo.owner == tid);
		assert(tinfo.locks_held[tinfo.locks_held.size()-1] == lock);

		linfo.unlock_time[tid] = ++tinfo.time;
		tinfo.locks_held.pop_back();
	}

    void Fork(TID forked_tid, TID tid) {
    }
    
    void Join(TID joined_tid, TID tid) {
    }

	void Access(MLOC var, TID tid, bool isWrite){
		tid++;
		//std::cout << var << " " << tid << " " << isWrite << std::endl;
		//return;
		VarInfo& vinfo = var_info[var];
		ThreadInfo& tinfo = thread_info[tid];

		if(vinfo.owner == tid)
			goto set_time_and_ret;
		if(vinfo.owner == 0){
			if(isWrite)
				goto set_owner_and_ret;
			goto set_time_and_ret;
		}
		if(vinfo.lock_owner != 0 &&
			lock_info[vinfo.lock_owner].owner == tid)
			goto set_owner_and_ret;

		// slow case
		for(size_t i=tinfo.locks_held.size()-1; i<=0; i--){
			LockInfo& linfo = lock_info[tinfo.locks_held[i]];
			if(linfo.unlock_time[vinfo.owner] > vinfo.acc_time){
				vinfo.lock_owner = tinfo.locks_held[i];
				goto set_owner_and_ret;
			}
		}

		// report race
		fishNetChecker->OnRace(var);

set_owner_and_ret:
		vinfo.owner = tid;
set_time_and_ret:
		vinfo.acc_time = tinfo.time;
	}
};
