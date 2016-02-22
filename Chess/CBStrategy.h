/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"

#include "SyncVar.h"
#include "IChessStrategy.h"
#include "DfsStrategy.h"
#include "RepVector.h"

// Context Bounded Strategy
// Perform DFS, pruning an execution when number of context
// switches exceeds the bound

class CBStrategy : public IChessStrategy{
public:
	CBStrategy(int cb)
		: contextBound(cb){}

	ChessExecution* InitialExecution();
	void CompletedExecution(ChessExecution* curr);

	ChessExecution* NextExecution(ChessExecution* prev, IQueryEnabled* qEnabled, size_t depthBound);


	// interfaces to CBQueryEnabled
	bool IsCBRestricted(ChessExecution* exec, size_t step);
	Task CBRestrictedTask(ChessExecution* exec, size_t step);

	int GetContextBoundAtStep(size_t step){
		assert(numContextSwitches.size() > step);
		return numContextSwitches[step];
	}

private:
	int contextBound;
	DfsStrategy dfs;

	void UpdateCB(ChessExecution* prev);
	std::vector<int> numContextSwitches;
};