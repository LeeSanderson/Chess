/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "StateMonitor.h"
#include "ChessImpl.h"
#include "HashFunction.h"

void StateMonitor::OnShutdown(){
	if(stateBuf)
		delete stateBuf;
	stateBuf = 0;
}

void StateMonitor::OnSchedulePoint(SchedulePointType type, Task tid, SyncVar var, SyncVarOp op, size_t sid){
	if(!getStateCallback) return;
	//if(Chess::GetOptions().depth_bound && sid > Chess::GetOptions().depth_bound) return;
	if(!stateBuf){
		stateLen = 1024;
		stateBuf = new char[stateLen];
	}
	assert(stateLen > 0);
	int len = getStateCallback(stateBuf, stateLen);
	if(len > stateLen){
		while(len > stateLen){
			stateLen *= 2; 
		}
		delete stateBuf;
		stateBuf = new char[stateLen];
		len = getStateCallback(stateBuf, stateLen);
		assert(len <= stateLen);
	}

	HashValue hash = ComputeHash(stateBuf, len);
	stdext::hash_set<HashValue>::iterator f = stateTable.find(hash);
	if(f == stateTable.end()){
		ChessImpl::GetStats()->OnNewState();
		stateTable.insert(hash);
	}
}

// Non IChessMonitor interfaces

GetStateCallback StateMonitor::RegisterGetStateCallback(GetStateCallback cb){
	GetStateCallback ret = getStateCallback;
	getStateCallback = cb;
	return ret;
}
