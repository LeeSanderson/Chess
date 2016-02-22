/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "ChessProgressTracker.h"
#include "ChessImpl.h"

void ChessProgressTracker::OnExecutionBegin(IChessExecution *exec){
	numExecutions++;
	inExec = true;
}

void ChessProgressTracker::OnSyncVarAccess(EventId id, Task tid, SyncVar var, SyncVarOp op, size_t sid){
	numTransitions++;
}

void ChessProgressTracker::OnAggregateSyncVarAccess(EventId id, Task tid, SyncVar* var, int n, SyncVarOp op, size_t sid){
	numTransitions++;
}

void ChessProgressTracker::OnExecutionEnd(IChessExecution* exec){
	inExec = false;
}

void ChessProgressTracker::OnTimeout(){
	if(!inExec || !active)
		return;

	int currNumExecutions = numExecutions;
	int currNumTransitions = numTransitions;

	if(lastNumExecutions == currNumExecutions && lastNumTransitions == currNumTransitions){
		ChessImpl::OnMaxExecTimeout();
	}

	lastNumExecutions = currNumExecutions;
	lastNumTransitions = currNumTransitions;
}