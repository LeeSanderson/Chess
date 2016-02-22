/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "IChessMonitor.h"

class ChessProgressTracker : public IChessMonitor{
public:
	ChessProgressTracker()
		: inExec(false), numExecutions(0), numTransitions(0), 
		lastNumExecutions(-1), lastNumTransitions(-1), active(true){}

	void OnExecutionBegin(IChessExecution* exec);
	void OnSyncVarAccess(EventId id, Task tid, SyncVar var, SyncVarOp op, size_t sid);
	void OnAggregateSyncVarAccess(EventId id, Task tid, SyncVar* var, int n, SyncVarOp op, size_t sid);
	void OnExecutionEnd(IChessExecution* exec);
	void OnTimeout();

	void Cancel(){active = false;}
	void Enable(){numTransitions++;active = true;}
	void Disable(){active = false;}

private:
	volatile bool inExec;
	volatile int numExecutions;
	volatile int numTransitions;

	int lastNumExecutions;
	int lastNumTransitions;
	bool active;
};