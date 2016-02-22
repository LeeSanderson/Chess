/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"
#include "ChessStl.h"
#include "IChessStrategy.h"
#include "BitVector.h"
#include <map>

// Implements DFS strategy
// Includes trivial partial-order-reduction: 
//         Does not schedule right before a TaskFork, ThreadResume, TaskEnd
class DfsStrategy : public IChessStrategy{
public:
	DfsStrategy()
		: lastBacktrackSid(-1){}	
	ChessExecution* InitialExecution();
	ChessExecution* NextExecution(ChessExecution* prev, IQueryEnabled* qEnabled, size_t depthBound);	
	// Made these public so the BestFirstStrategy could use them as well (Katie)
	static bool DoneSchedulingAllThreads(Task first, Task curr, Task next);
	static Task FirstScheduledTaskAtStep(size_t step, ChessExecution* exec);

private:
	// if destructive is true the backtracking point will be removed, if it is false the backtracking point will be left in place (Katie)
	bool FindBacktrackingPoint(ChessExecution* exec, IQueryEnabled* qEnabled, size_t startStep, Task startTask, size_t& endStep, Task& endTask, std::set<SyncVar>& skipVars, bool destructive);
	SyncVarOp lastBacktrackOp;
	size_t lastBacktrackSid;
	// So we can skip a backtracking point that turned out not to be enabled (Katie)
	SyncVar lastBacktrackTask;
	// To keep track of backtracking points when DPOR is being used (Katie)
	std::map<size_t, BitVector> backtrackingPoints;
};