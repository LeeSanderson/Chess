/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"

#include "IChessMonitor.h"
#include "Chess.h"
#include <hash_map>
#include "HashFunction.h"
#include <hash_set>

class StateMonitor : public IChessMonitor {
public:
	StateMonitor()
		: getStateCallback(0), stateLen(0), stateBuf(0) {}
	void OnShutdown();	
	
	void OnSchedulePoint(SchedulePointType type, Task tid, SyncVar var, SyncVarOp op, size_t sid);

	// Non IChessMonitor interfaces

	GetStateCallback RegisterGetStateCallback(GetStateCallback cb);


private:
	char* stateBuf;
	int stateLen;
	GetStateCallback getStateCallback;
	stdext::hash_set<HashValue> stateTable;
};