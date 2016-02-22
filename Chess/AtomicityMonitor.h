/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/


#pragma once
#include "ChessBase.h"
#include<vector>
#include<map>

#include "..\Chess\IChessMonitor.h"
#include "VClock.h"

#include "CacheRaceMonitor.h"

class Observation;

class AtomicityMonitor : public IChessMonitor {

 public:
	 AtomicityMonitor(CacheRaceMonitor *cacheracemonitor, Observation *curobservation);
	 virtual ~AtomicityMonitor() { };
	 void clear(); // clear state

	 // Called right before a new execution of CHESS
	 // The argument is the partial execution that CHESS is about to execute
	 virtual void OnExecutionBegin(IChessExecution* exec){ clear(); }

	 // Called right after completing a new execution
	 virtual void OnExecutionEnd(IChessExecution* exec);
	
	 virtual void OnShutdown();

	 // Called right before a Data Var access
	 virtual void OnDataVarAccess(EventId id, void* loc, int size, bool isWrite, size_t pcId);

	 // Called right *after* a Sync Var access - Calling it after ensures that there is no danger of LocalBacktrack
	 virtual void OnSyncVarAccess(EventId id, Task tid, SyncVar var, SyncVarOp op, size_t sid);
	 virtual void OnAggregateSyncVarAccess(EventId id, Task tid, SyncVar* var, int n, SyncVarOp op, size_t sid);

	 // called on operation call/return
	 void opcall(EventId id, void *object, const char *name);
	 void opreturn(EventId id);

protected:
    CacheRaceMonitor *crmonitor;
    Observation *curobs;

	// record and check memory accesses
	void record_load(EventId id, SyncVar var, CacheRaceMonitor::Location *inLocation, int inPcId, int nr);
	void record_store(EventId id, SyncVar var,CacheRaceMonitor:: Location *inLocation, int inPcId, int nr);
	void record_interlocked(EventId id, SyncVar var, CacheRaceMonitor::Location *inLocation, int nr);

	class ConflictGraph;
	ConflictGraph *cg;
	bool atomicityviolation_found;

	void reportAtomicityViolation(EventId id, std::set<EventId> events);
	std::string errorstring;
};
