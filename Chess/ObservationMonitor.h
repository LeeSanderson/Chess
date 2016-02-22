/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "IChessMonitor.h"
#include "ChessEvent.h"
#include <iostream>
#include <fstream>
#include <string>
#include <set>

class ChessOptions;
class ChessExecution;
class CacheRaceMonitor;
class AtomicityMonitor;
class Observation;
class ObservationSet;
class IQueryEnabled;

class ObservationMonitor : public IChessMonitor
{

friend class Observation;
friend class ObservationSet;

public:
	ObservationMonitor(const ChessOptions *options, CacheRaceMonitor *cacheracemonitor);
    virtual ~ObservationMonitor();

	// Called right before a new execution of CHESS
	// The argument is the partial execution that CHESS is about to execute
	// Monitors should expect a replay of the execution followed by a record phase
	virtual void OnExecutionBegin(IChessExecution* exec);

	// Called right after completing a new execution
	virtual void OnExecutionEnd(IChessExecution* exec);
	
	virtual void OnShutdown();

    void Call(Task tid, void *object, const char *opname, bool iscallback);
    void Return(Task tid, bool iscallback);

    void Deadlock(Task tid);

    void IntValue(const char *label, long long value);
    void PointerValue(const char *label, void *value);
    void StringValue(const char *label, const char *value);

	void MarkTimeout(Task tid);
	void SyncVarAccessCommitted(Task tid);
    void CheckBlock(IQueryEnabled *queryenabled, int transition);

	void ReportInvalidObservation(Observation &obs, bool some_ops_suppressed);
	void ReportFileError(const std::string &action, const std::string &filename);

private:
	const ChessOptions *options;
	CacheRaceMonitor *cacheracemonitor;
    AtomicityMonitor *atomicitymonitor;

	enum e_obsmode {   // enumerateobservations   checkobservations
                     // ---------------------   -----------------
		om_atom,        // n/a                     n/a                                             check conflict-serializability
		om_coarse,      // coarse (default)        n/a  
		om_serial,      // serial & block          n/a  
		om_all,         // fine                    n/a
		om_SC,          // n/a                     match call history & observed values to observations (default)
		om_SC_s,        // n/a                     match call history & observed values to serial observations
        om_lin,         // n/a                     match call history & observed values & return-to-call order to observations
        om_lin_s,       // n/a                     match call history & observed values & return-to-call order to serial observations

	} obsmode;

    bool specmining;


    Observation *curobs;
    ObservationSet *curset;


};