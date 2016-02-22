/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessStl.h"

#include "IChessMonitor.h"
#include "ChessExecution.h"

// A sample monitor
// Also checks the invariants that hold on various callback

class ChessMonitorTest : public IChessMonitor{
public:
	ChessMonitorTest();

	// The argument is the partial execution that CHESS is about to execute
	virtual void OnExecutionBegin(IChessExecution* exec);

	// Called right after completing a new execution
	virtual void OnExecutionEnd(IChessExecution* exec);
	
	virtual void OnShutdown();

	// Called right before a Data Var access
	virtual void OnDataVarAccess(EventId id, void* loc, int size, bool isWrite, size_t pcId);
	
	// Called right *after* a Sync Var access - Calling it after ensures that there is no danger of LocalBacktrack
	virtual void OnSyncVarAccess(EventId id, Task tid, SyncVar var, SyncVarOp op, size_t sid);
	virtual void OnAggregateSyncVarAccess(EventId id, Task tid, SyncVar* var, int n, SyncVarOp op, size_t sid);
	
	enum SchedulePointType { SVACCESS, CHOOSE };
	// Called right before a Sync Var access / choose
	virtual void OnSchedulePoint(SchedulePointType type, Task tid, SyncVar var, SyncVarOp op, size_t sid);


private:
	std::vector<ChessTransition> currExecution;
	size_t currStep;

	enum FSMState {INIT, REPLAY, RECORD, SHUTDOWN};
	FSMState state;
};