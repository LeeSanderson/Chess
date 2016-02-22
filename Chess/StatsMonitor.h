/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"
#include "ChessStl.h"

#include "IChessMonitor.h"
#include "IChessStats.h"

class SSEstimator;


class StatsMonitor : public IChessMonitor, public IChessStats {
public:
	StatsMonitor(int freq = 0);
	void OnExecutionEnd(IChessExecution* exec);
	void OnShutdown();	
	void OnExecutionBegin(IChessExecution* exec);

	// IChessStats interfaces
	void OnNonlocalBacktrack(SyncVarOp op);
	void OnNewHBExecution();
	void OnNewState();

	int GetNumExecutions(){return numExecs;}
	int GetMaxThreads(){return maxNumThreads;}
	int GetMaxSteps(){return maxStackDepth;}
	int GetTotalSteps(){return totalStackDepth;}
	int GetMaxContextSwitches(){return maxContextSwitches;}
	int GetTotalContextSwitches(){return totalContextSwitches;}
	int GetNumHBExecutions() const {return numHbExecs;}
	int GetElapsedTimeMS();

	~StatsMonitor();
private:
	int numExecs;
	int numNonterminatingExecs;
	int maxNumThreads;
	int totalContextSwitches;
	int maxContextSwitches;
	int numNlb;
	stdext::hash_map<SyncVarOp, int> nlbOps;
	int displayFreq;
	size_t maxStackDepth;
	size_t totalStackDepth;
	int numStates;

	int numHbExecs;

	bool startTimeValid;
#if SINGULARITY
	uint64 startTime;
#else
	clock_t startTime;
#endif
	void PrintStats(IChessExecution* exec, bool isFinal = false);
	SSEstimator* ssEstimator;
};