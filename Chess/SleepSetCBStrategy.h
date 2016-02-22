/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once
#include "ChessBase.h"

#include "SyncVar.h"
#include "IChessStrategy.h"
#include "RepVector.h"
#include "HashFunction.h"
#include "IQueryEnabled.h"
#include "CBStrategy.h"
#include <hash_map>
#include <map>

#ifdef DEBUG_SLEEPSETS
#include "ChessTransition.h"
#endif

// Context Bounded Strategy
// Perform DFS, pruning an execution when number of context
// switches exceeds the bound

class SleepSetCBStrategy : public IChessStrategy, public IQueryEnabled{
public:
	SleepSetCBStrategy(int cb)
		: cbStrategy(cb), 
		depthBoundAtNextExecution(0),
		startStateAtCompletedExecution(0), execCnt(0){}

	ChessExecution* InitialExecution();
	void CompletedExecution(ChessExecution* curr);

	ChessExecution* NextExecution(ChessExecution* prev, IQueryEnabled* qEnabled, size_t depthBound);

private:
	CBStrategy cbStrategy;
//	stdext::hash_map<HashValue, int> minCBForState;
	struct SleepSetInfo{
		int cb;
		bool isVisitorThreadDisabled;
#ifdef DEBUG_SLEEPSETS
		std::vector<ChessTransition> stack;
		int depth;
		int execCnt;
#endif
	};
	std::map<HashValue, SleepSetInfo> minCBForState;
	int execCnt;

	void UpdateSleepSets(ChessExecution* currExecution);
	size_t depthBoundAtNextExecution;
	size_t startStateAtCompletedExecution;
};