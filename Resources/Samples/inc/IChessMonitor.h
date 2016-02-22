/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "ChessEvent.h"
#include "SyncVar.h"
#include "IChessExecution.h"

/// Interface for monitoring Chess Events

class CHESS_API IChessMonitor {
public:
	virtual ~IChessMonitor(){}

	// Called right before a new execution of CHESS
	// The argument is the partial execution that CHESS is about to execute
	// Monitors should expect a replay of the execution followed by a record phase
	virtual void OnExecutionBegin(IChessExecution* exec){}

	// Called right after completing a new execution
	virtual void OnExecutionEnd(IChessExecution* exec){}
	
	virtual void OnShutdown(){}

	// Called right before a Data Var access
	virtual void OnDataVarAccess(EventId id, void* loc, int size, bool isWrite, size_t pcId){}
    // Generic event
	virtual void OnTraceEvent(EventId id, const std::string &info) {}
	
	// Called right *after* a Sync Var access - Calling it after ensures that there is no danger of LocalBacktrack
	virtual void OnSyncVarAccess(EventId id, Task tid, SyncVar var, SyncVarOp op, size_t sid){}
	virtual void OnAggregateSyncVarAccess(EventId id, Task tid, SyncVar* var, int n, SyncVarOp op, size_t sid){}
	
	enum SchedulePointType { SVACCESS, CHOOSE };
	// Called right before a Sync Var access / choose
	virtual void OnSchedulePoint(SchedulePointType type, EventId id, SyncVar var, SyncVarOp op, size_t sid){}

	virtual void OnEventAttributeUpdate(EventId id, EventAttribute attr, const char *val) {}

};