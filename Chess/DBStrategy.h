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
#include "VBPruning.h"

// Delay Bounded Strategy

class DBStrategy : public IChessStrategy{
public:
	DBStrategy(size_t db)
		: delayBound(db){}

	ChessExecution* InitialExecution();
	void CompletedExecution(ChessExecution* curr);

	ChessExecution* NextExecution(ChessExecution* prev, IQueryEnabled* qEnabled, size_t depthBound);

private:
	const size_t delayBound;

	// delays.size <= delayBound
	// delays[i] is the sid of the i-th delay
	// repetition is possible since multiple delays are allowed at the same state
	std::vector<int> delays;
};
